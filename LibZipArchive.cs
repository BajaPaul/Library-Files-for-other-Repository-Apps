using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Windows.Storage;

/*
 * File: \Libraries\LibZipArchive.cs. Note edit: 2019-02-15. This library provides UWP Apps methods that compress and extract folders and files 
 * using ZipArchive Class. This file will need to be copied into project or added via 'Add as Link'.
 * 
 * Normally the namespace below would be proceeded by the project name. This is ommitted so library can be shared between projects easily.
 *  
 * Important: Generally, no parameter checking is done by methods in this library to eliminate redundant checks and to increase performance.
 * It is responsibility of calling methods to insure parameters are valid and that new items created by methods in this library do not already exist.
 * These methods generally return true if success, false otherwise. Methods attempt to cleanup and restore original data state if error or exception occurs.
 * These methods catch exception and return false if they occur. Calling methods should check values returned by methods in this library are not false 
 * before proceeding.
 * 
 * This library was created as alternative to using ZipFile class methods. Microsoft broke the use of those methods in UWP Apps if if minimum build >= 16299
 * since they currently throw System.UnauthorizedAccessException exception on parameter paths.
 * 
 * This library is not limited by Microsoft's 260 character path length limit since does not use paths as parameters. After considerable benchmarking,
 * pleasently suprised to find methods in this library are generally quicker than the ZipFile methods they replace. Maybe in the range of 10 to 30 percent.
 * Conclusion: Use of this library will not result in performance loss and might have a slight performance gain.
 * 
 */

namespace LibraryCoder.LibZipArchive
{
    /// <summary>
    /// LibZA = Shorthand for LibZipArchive.
    /// </summary>
    public static class LibZA
    {
        /// <summary>
        /// Public list of file paths not processed if public methods ZipArchiveCompressAsync() or ZipArchiveExtractAsync() return false.
        /// Use of global variable eliminates parameter passing cascade in recursive methods.
        /// </summary>
        public static List<string> listItemPathErrors = new List<string>();

        // Following private global variables eliminate parameter passing cascade in recursive method.

        /// <summary>
        /// ZipArchive value that is calculated once and used in recursive method.
        /// </summary>
        private static ZipArchive zipArchiveRecursive;

        /// <summary>
        /// Initialized to true before recursive method call. Changes to false if error processing a file.
        /// </summary>
        private static bool boolSuccessRecursive;

        /// <summary>
        /// Path of folder to compress. This path is prefix that is removed from paths of found subfolders to create relative path.
        /// </summary>
        private static string stringPathPrefixRecursive;

        /// <summary>
        /// Relative path prepended to filename when creating ZipArchiveEntry to retain hierarchy of file in ZipArchive.
        /// </summary>
        private static string stringPathRecursive;

        /// <summary>
        /// Compression level to use when compressing files in recursive method.
        /// </summary>
        private static CompressionLevel compressionLevelRecursive;

        // Optimal intChunkSizeBytes value: Benchmarked 3GB file on typical hard drive using values from 16,384 bytes to 31,457,280 bytes.
        // Each run compressed file and extracted file. Conclusion was processing 3GB file with very large chuncks ended up being slightly slower 
        // than using smaller chuncks. Sweet spot for test computer was 1,048,576 bytes.
        /// <summary>
        /// Default chunk size used to process items when adding to or removing from a ZipArchive. Value should be divisible by 1024.
        /// Files smaller than this chunk size use size of file as chunk size and will be processed on first pass. Value is 1048576 bytes.
        /// </summary>
        private const int intChunkSizeBytes = 1048576;     // Value is bytes that is divisible by 1024.

        /************ Public Methods Follow ************/

        /// <summary>
        /// Return true if parameter iStorageItemToCheckIfLocked is locked, false otherwise. No parameter checking done.
        /// </summary>
        /// <param name="iStorageItemCheckIfLocked">Check if parameter is locked.</param>
        /// <returns></returns>
        public static async Task<bool> IStorageItemLockCheckAsync(IStorageItem iStorageItemToCheckIfLocked)
        {
            try
            {
                // Microsoft does not provide API to check if object is locked. Efficient work-a-around is try to rename object.
                // If rename does not throw System.UnauthorizedAccessException, or any other exceptions, then object is not locked.
                string stringIStorageItemNameOrig = iStorageItemToCheckIfLocked.Name;
                string stringIStorageItemNameGuid = $"TempNameLockCheck_{Guid.NewGuid():N}";
                // Debug.WriteLine($"LibZA.IStorageItemLockCheckAsync(): stringIStorageItemNameOrig={stringIStorageItemNameOrig}");
                // Debug.WriteLine($"LibZA.IStorageItemLockCheckAsync(): stringIStorageItemNameGuid={stringIStorageItemNameGuid}");
                // Try to rename iStorageItemToCheckIfLocked. If it is locked then rename will throw System.UnauthorizedAccessException.
                await iStorageItemToCheckIfLocked.RenameAsync(stringIStorageItemNameGuid);
                await iStorageItemToCheckIfLocked.RenameAsync(stringIStorageItemNameOrig);    // Restore original name if no excpetion.
                // Debug.WriteLine($"LibZA.IStorageItemLockCheckAsync((): {iStorageItemToCheckIfLocked.Name} not locked since rename sequence did not throw exception");
                return false;   // Return false since no exception. This indicates iStorageItemToCheckIfLocked not locked.
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibZA.IStorageItemLockCheckAsync(): Returned true since exception occurred. Type={ex.GetType()}");
                return true;    // Return true since exception. This indicates iStorageItemToCheckIfLocked is locked.
                throw;
            }
        }

        /// <summary>
        /// Compress items in iStorageItemToCompress into storageFileCompressed using zip archive format. Returns true if success, false otherwise.
        /// No parameter checking done. Method does not delete iStorageItemToCompress.
        /// </summary>
        /// <param name="storageFileCompressed">Existing file to place compressed items into. Items are added using zip archive format.</param>
        /// <param name="iStorageItemToCompress">Existing folder or file to compress.</param>
        /// <param name="compressionLevel">Compression level to use. Default is no compression.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<bool> ZipArchiveCompressAsync(StorageFile storageFileCompressed, IStorageItem iStorageItemToCompress, CompressionLevel compressionLevel = CompressionLevel.NoCompression, bool boolCheckIfLocked = true)
        {
            // Minimal error checking done to eliminate redundant checks and keep method fast. Exceptions return false.
            try
            {
                if (boolCheckIfLocked)      // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(iStorageItemToCompress))
                    {
                        // Debug.WriteLine($"LibZA.ZipArchiveCompressAsync(): {iStorageItemToCompress.Name} is locked.");
                        return false;
                    }
                }
                if (iStorageItemToCompress.IsOfType(StorageItemTypes.File))
                {
                    StorageFile storageFileToCompress = (StorageFile)iStorageItemToCompress;
                    using (Stream streamArchive = await storageFileCompressed.OpenStreamForWriteAsync())
                    {
                        using (ZipArchive zipArchive = new ZipArchive(streamArchive, ZipArchiveMode.Create))
                        {
                            return await ZipArchiveEntryCreateAsync(zipArchive, storageFileToCompress, storageFileToCompress.Name, compressionLevel);
                        }
                    }
                }
                else if (iStorageItemToCompress.IsOfType(StorageItemTypes.Folder))
                {
                    StorageFolder storageFolderToCompress = (StorageFolder)iStorageItemToCompress;
                    // Initialize global variables used in recursive method to eliminate parameter passing cascade.
                    listItemPathErrors.Clear();                                         // Clear entries from list.
                    boolSuccessRecursive = true;                                        // Reset flag to true. Change to false if error processing any file.
                    stringPathRecursive = string.Empty;                                 // Set initial path to empty string for use in recursive method.
                    stringPathPrefixRecursive = storageFolderToCompress.Path; // Set path of storageFolderEntry for use in recursive method.
                    compressionLevelRecursive = compressionLevel;                       // Set compression level for use in recursive method.
                    using (Stream streamArchive = await storageFileCompressed.OpenStreamForWriteAsync())
                    {
                        using (zipArchiveRecursive = new ZipArchive(streamArchive, ZipArchiveMode.Create, true))    // Set ZipArchive used in recursive method.
                        {
                            await ZipAchiveCompressRecursiveAsync(storageFolderToCompress);
                        }
                    }
                    if (boolSuccessRecursive)
                    {
                        return true;    // Success since bool still true.
                    }
                }
                // iStorageItemToCompress not file or folder, or, at least one file not compressed. Path of files not compressed placed in listItemPathErrors.
                return false;
            }
            catch
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Extract compressed items in zip archive format from storageFileCompressed to storageFolderDestination. Return true if success, false otherwise. 
        /// No parameter checking done. Method does not delete storageFileCompressed.
        /// </summary>
        /// <param name="storageFileCompressed">Existing file containing package of compressed items in zip archive format.</param>
        /// <param name="storageFolderDestination">Existing folder to extract compressed items into.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<bool> ZipArchiveExtractAsync(StorageFile storageFileCompressed, StorageFolder storageFolderDestination, bool boolCheckIfLocked = true)
        {
            // Minimal error checking done to eliminate redundant checks and keep method fast. Exceptions return false.
            listItemPathErrors.Clear();     // Clear entries from list.
            try
            {
                if (boolCheckIfLocked)      // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(storageFileCompressed))
                    {
                        // Debug.WriteLine($"LibZA.ZipArchiveExtractAsync(): {storageFileCompressed.Name} is locked.");
                        return false;
                    }
                }
                bool boolSuccess = false;
                using (Stream streamArchive = await storageFileCompressed.OpenStreamForReadAsync())
                {
                    using (ZipArchive zipArchive = new ZipArchive(streamArchive, ZipArchiveMode.Read))
                    {
                        IReadOnlyCollection<ZipArchiveEntry> zipArchiveEntries = zipArchive.Entries;
                        int intZipArchiveEntries = zipArchiveEntries.Count;
                        if (intZipArchiveEntries == 0)
                        {
                            // Already created empty extract folder but no entries to extract so return true.
                            return true;
                        }
                        else
                        {
                            bool boolAllItemsProcessed = true;
                            foreach (ZipArchiveEntry zipArchiveEntry in zipArchiveEntries)
                            {
                                if (!await ZipArchiveEntryExtractAsync(zipArchiveEntry, storageFolderDestination))
                                {
                                    // Entry not processed so add entry path to error list that can be accessed by calling method.
                                    boolAllItemsProcessed = false;
                                    // zipArchiveEntry.FullName will return paths using Path.DirectorySeparatorChar '\' or Path.AltDirectorySeparatorChar '/'.
                                    // So need to replace any '/' with '\' so always have consistent output to test.
                                    string stringPathRelative = zipArchiveEntry.FullName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                                    listItemPathErrors.Add(stringPathRelative);     // Add entry path to error list.
                                }
                            }
                            if (boolAllItemsProcessed)
                                boolSuccess = true;
                        }
                    }
                }
                return boolSuccess;
            }
            catch
            {
                return false;
                throw;
            }
        }

        /************ Private Methods Follow ************/

        /// <summary>
        /// Recursive Method: Compress items in storageFolderToCompress using zip archive format. No parameter checking done. Method steps through
        /// each subfolder in storageFolderToCompress and compresses any files found using relative path from stringPathPrefixRecursive.
        /// </summary>
        /// <param name="storageFolderToCompress">Existing folder to compress. This value will change as recursion occurs.</param>
        /// <returns></returns>
        private static async Task ZipAchiveCompressRecursiveAsync(StorageFolder storageFolderToCompress)
        {
            /****** Be extremely careful editing this method since it uses recursion. Method needs to run out to end without returns since recursive. *******/
            IReadOnlyList<StorageFile> listStorageFiles = await storageFolderToCompress.GetFilesAsync();
            if (listStorageFiles != null)
            {
                if (listStorageFiles.Count > 0)
                {
                    foreach (StorageFile storageFileFound in listStorageFiles)
                    {
                        string stringPathEntry = $"{stringPathRecursive}{storageFileFound.Name}";   // stringPathEntry is relative path of entry.
                        if (!await ZipArchiveEntryCreateAsync(zipArchiveRecursive, storageFileFound, stringPathEntry, compressionLevelRecursive))
                        {
                            boolSuccessRecursive = false;
                            listItemPathErrors.Add(storageFileFound.Path);  // Did not add entry from storageFileFound so add it's path to listItemPathErrors.
                        }
                    }
                }
            }
            IReadOnlyList<StorageFolder> listStorageFolders = await storageFolderToCompress.GetFoldersAsync();
            if (listStorageFolders != null)
            {
                if (listStorageFolders.Count > 0)
                {
                    foreach (StorageFolder storageFolderFound in listStorageFolders)
                    {
                        // Do not create folder entry from storageFolderFound since folders are created as needed when creating entry from storageFileFound.
                        // Calculate relative path of storageFolderFound by removing prefix path saved in stringPathPrefixRecursive from it's path.
                        stringPathRecursive = $"{storageFolderFound.Path.Substring(stringPathPrefixRecursive.Length + 1)}{Path.DirectorySeparatorChar}";
                        // Next line is recursive method call.
                        await ZipAchiveCompressRecursiveAsync(storageFolderFound);
                    }
                }
            }
        }

        /// <summary>
        /// Create ZipArchiveEntry from storageFileToCompress and place it in zipArchive. Return true if success, false otherwise. 
        /// No parameter checking done.
        /// </summary>
        /// <param name="zipArchive">Contains package of compressed files in zip archive format.</param>
        /// <param name="storageFileToCompress">Existing file to compress.</param>
        /// <param name="stringPathEntry">Relative path of entry placed in zipArchive.</param>
        /// <param name="enumCompressLevel">Compression level to use.</param>
        /// <returns></returns>
        private static async Task<bool> ZipArchiveEntryCreateAsync(ZipArchive zipArchive, StorageFile storageFileToCompress, string stringPathEntry, CompressionLevel compressionLevel)
        {
            // Minimal error checking done to eliminate redundant checks and keep method fast. Exceptions return false.
            // This method will process large files over 2 GB in size.
            try
            {
                bool boolSuccess = false;
                int intChunkSize = intChunkSizeBytes;
                ulong ulongSizeFileToCompress = (await storageFileToCompress.GetBasicPropertiesAsync()).Size;   // Get size of storageFileToCompress.
                if (ulongSizeFileToCompress < (ulong)intChunkSize)
                {
                    // File size less than chunk size, so set chunk size to file size and process in one pass.
                    intChunkSize = (int)ulongSizeFileToCompress;
                    // Debug.WriteLine($"LibZA.ZipArchiveEntryCreateAsync(): Reset intChunkSize={intChunkSize}");
                }
                // Debug.WriteLine($"LibZA.ZipArchiveEntryCreateAsync(): Chunk size used=={intChunkSize}");
                // Create empty entry in zipArchive.
                ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(stringPathEntry, compressionLevel);
                using (BinaryWriter binaryWriter = new BinaryWriter(zipArchiveEntry.Open()))
                {
                    using (Stream streamToCompress = (await storageFileToCompress.OpenSequentialReadAsync()).AsStreamForRead())
                    {
                        byte[] byteArray = new byte[intChunkSize];
                        // Debug.WriteLine($"LibZA.ZipArchiveEntryCreateAsync(): byteArray.Length={byteArray.Length}, intChunkSize={intChunkSize}. Values should be equal.");
                        using (BinaryReader binaryReader = new BinaryReader(streamToCompress))
                        {
                            int intBytesRead;
                            while ((intBytesRead = binaryReader.Read(byteArray, 0, intChunkSize)) > 0)
                            {
                                binaryWriter.Write(byteArray, 0, intBytesRead);
                                // Debug.WriteLine($"LibZA.ZipArchiveEntryCreateAsync(): intBytesRead={intBytesRead}");
                            }
                            boolSuccess = true;
                        }
                    }
                }
                return boolSuccess;
            }
            catch
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Extract zipArchiveEntry to StorageFile located in hierarchy of storageFolderDestination. Returns true if success, false otherwise.
        /// No parameter checking done. Subfolders embeded in path of ZipArchiveEntry are created as needed.
        /// </summary>
        /// <param name="zipArchiveEntry">Compressed entry from a ZipArchive.</param>
        /// <param name="storageFolderDestination">Existing folder to extract compressed items into.</param>
        /// <returns></returns>
        private static async Task<bool> ZipArchiveEntryExtractAsync(ZipArchiveEntry zipArchiveEntry, StorageFolder storageFolderDestination)
        {
            // Minimal error checking done to eliminate redundant checks and keep method fast. Exceptions return false.
            // zipArchiveEntry can be a folder or a file. Skip folder entries since file entries include subfolder paths, if used.
            // Folder paths in file entries are created if they do not exist.
            try
            {
                bool boolSuccess = false;
                // zipArchiveEntry.FullName will return paths using Path.DirectorySeparatorChar '\' or Path.AltDirectorySeparatorChar '/'.
                // So need to replace any '/' with '\' so always have consistent output to test.
                string stringPathRelative = zipArchiveEntry.FullName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                string stringDirectorySeparatorChar = Path.DirectorySeparatorChar.ToString();   // Builds < 16299 not able to convert this value to string where used.
                // Check if zipArchiveEntry is a folder or a file.
                // More at: https://github.com/dotnet/corefx/issues/19889
                if (stringPathRelative.EndsWith(stringDirectorySeparatorChar))  // This is proceedure Microsoft uses in ZipFile methods to test if entry is a folder.
                {
                    // zipArchiveEntry is a folder. Skip processing this entry and return true.
                    boolSuccess = true;
                }
                else    // zipArchiveEntry is a file.
                {
                    // Check if zipArchiveEntry is proceeded by a path. If so, need to change storageFolderDestination to end of path.
                    if (stringPathRelative.Contains(stringDirectorySeparatorChar))
                    {
                        string stringPathSubFolders = Path.GetDirectoryName(stringPathRelative);
                        // Check if SubFolders exists.
                        StorageFolder storageFolderSub = (StorageFolder)await storageFolderDestination.TryGetItemAsync(stringPathSubFolders);
                        if (storageFolderSub != null)
                        {
                            storageFolderDestination = storageFolderSub;    // Update destination folder to extract zipArchiveEntry into.
                        }
                        else
                        {
                            // Create SubFolders and save result to storageFolderDestination
                            storageFolderDestination = await storageFolderDestination.CreateFolderAsync(stringPathSubFolders);
                        }
                    }
                    int intChunkSize = intChunkSizeBytes;
                    long longZipArchiveEntryLength = zipArchiveEntry.Length;
                    // Debug.WriteLine($"LibZA.ZipArchiveEntryExtractAsync(): Found {zipArchiveEntry.Name}, longZipArchiveEntryLength={longZipArchiveEntryLength}");
                    if (longZipArchiveEntryLength < intChunkSize)
                    {
                        // zipArchiveEntry size less than chunk size, so set chunk size to zipArchiveEntry size and process in one pass.
                        intChunkSize = (int)longZipArchiveEntryLength;
                        // Debug.WriteLine($"LibZA.ZipArchiveEntryCreateAsync(): Reset intChunkSize to {intChunkSize}");
                    }
                    // Debug.WriteLine($"LibZA.ZipArchiveEntryCreateAsync(): Chunk size used is {intChunkSize}");
                    StorageFile storageFileExtract = await storageFolderDestination.CreateFileAsync(zipArchiveEntry.Name);   // Create empty file to extract zipArchiveEntry too.
                    // Debug.WriteLine($"LibZA.ZipArchiveEntryCreateAsync(): Created file {storageFileExtract.Path}");
                    using (Stream streamFileExtract = (await storageFileExtract.OpenAsync(FileAccessMode.ReadWrite)).AsStreamForWrite())
                    {
                        using (Stream streamZipArchiveEntry = zipArchiveEntry.Open())
                        {
                            byte[] byteArray = new byte[intChunkSize];
                            int intBytesRead;
                            while ((intBytesRead = streamZipArchiveEntry.Read(byteArray, 0, intChunkSize)) > 0)
                            {
                                streamFileExtract.Write(byteArray, 0, intBytesRead);
                                // Debug.WriteLine($"LibZA.ZipArchiveEntryCreateAsync(): intBytesRead={intBytesRead}");
                            }
                            boolSuccess = true;
                        }
                    }
                }
                return boolSuccess;
            }
            catch
            {
                return false;
                throw;
            }
        }

    }
}
 