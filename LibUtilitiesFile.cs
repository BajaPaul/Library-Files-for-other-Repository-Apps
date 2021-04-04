using LibraryCoder.AesEncryption;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

/*
 * File: \Libraries\LibUtilitiesFile.cs.  This library provides various file and folder utilities.
 * 
 * May be difficult to create a useful library!!!
 * Async methods cannot use 'ref' or 'out' variables but messages could be saved to a global variable and access by calling method from there.
 * If any exception occur and cannot be handled locally, then generally best to bubble them up via a 'throw' to calling methods up to UI level to be dealt with there.
 *
 * This file will need to be copied into project or added via 'Add as Link'.
 * 
 * Normally the namespace below would be proceeded by the project name but is ommitted so files can be shared between projects easily.
 */

namespace LibraryCoder.UtilitiesFile
{

    /// <summary>
    /// LibUF = Shorthand for LibraryUtilitiesFile.
    /// </summary>
    public static class LibUF
    {
        /// <summary>
        /// Check if folder path or file path is a network location.  Returns true if so, false otherwise.
        /// </summary>
        /// <param name="stringItemPath">Folder or file path to check.</param>
        /// <returns></returns>
        public static bool PathIsNetworkLocation(string stringItemPath)
        {
            // Network path sample: \\C1\Users\xxxx\Documents\SampleFolder\SampleFile.txt
            // Local path sample:     C:\Users\xxxx\Documents\SampleFolder\SampleFile.txt
            string stringDoubleBackSlash = $"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}";     // "\\" string
            string stringVolumeSeparator = Path.VolumeSeparatorChar.ToString();     // ":" string.
            // Debug.WriteLine($"LibUF.PathIsNetworkLocation(): stringItemPath='{stringItemPath}'");
            // Debug.WriteLine($"LibUF.PathIsNetworkLocation(): stringDoubleBackSlash='{stringDoubleBackSlash}', stringVolumeSeparator='{stringVolumeSeparator}'");
            if (stringItemPath.StartsWith(stringDoubleBackSlash) && !stringItemPath.Contains(stringVolumeSeparator))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if foldername entered by user is valid.  Return foldername if valid, null otherwise.  Does not check if path is valid. 
        /// This method should be called directly from UI so UI can deal with any invalid results.  Returned foldername will have any leading 
        /// and trailing white-space characters removed, so best to use returned value versus input parameter.
        /// </summary>
        /// <param name="stringFoldername">Foldername to check (no path).</param>
        /// <returns></returns>
        public static string FoldernameCheck(string stringFoldername)
        {
            // More about naming files, paths, and namespaces: https://msdn.microsoft.com/en-us/library/aa365247.aspx
            if (!string.IsNullOrWhiteSpace(stringFoldername))
            {
                stringFoldername = stringFoldername.Trim();     // Remove all leading and trailing white-space characters.
                if (stringFoldername != null)
                {
                    // Original code from: https://stackoverflow.com/questions/62771/how-do-i-check-if-a-given-string-is-a-legal-valid-file-name-under-windows
                    // string original = @"^(?!^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\..*)(\..+)?$)[^\x00-\x1f\\?*:\"";|/]+$";
                    // Modified original by adding check for '<' and '>' characters.
                    string stringPattern = @"^(?!^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\..*)(\..+)?$)[^\x00-\x1f\\?*:\"";|/<>]+$";
                    // Regular Expression Language (RegEx): https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference
                    // Debug.WriteLine($"LibUF.FolderNameCheck(): stringPattern={stringPattern}");
                    try
                    {
                        if (!Regex.IsMatch(stringFoldername, stringPattern, RegexOptions.IgnoreCase))
                        {
                            // Debug.WriteLine($"LibUF.FolderNameCheck(): Regex match found, {stringFoldername} is invalid.");
                            return null;
                        }
                    }
                    catch (Exception)
                    {
                        // Catch any RegEx exceptions since not related to folder name validity.
                        // More at: https://msdn.microsoft.com/en-us/library/ktzf2d23(v=vs.110).aspx
                        throw;  // Pass exception to calling method to be handled there.
                    }
                    return stringFoldername;
                }
            }
            return null;    // Foldername check failed.  return null;
        }

        /// <summary>
        /// Check if filename entered by user is valid. Return filename if valid, null otherwise. This method should be 
        /// called directly from UI so UI can deal with any invalid results.  Returned filename will have any leading and 
        /// trailing white-space characters removed, so best to use returned value versus input parameter.
        /// </summary>
        /// <param name="stringFilename">Filename with extension to check.</param>
        /// <returns></returns>
        public static string FilenameCheck(string stringFilename)
        {
            // More about naming files, paths, and namespaces: https://msdn.microsoft.com/en-us/library/aa365247.aspx
            if (!string.IsNullOrWhiteSpace(stringFilename))
            {
                stringFilename = stringFilename.Trim();     // Remove all leading and trailing white-space characters.
                if (stringFilename != null)
                {
                    // Original code from: https://stackoverflow.com/questions/62771/how-do-i-check-if-a-given-string-is-a-legal-valid-file-name-under-windows
                    // string original = @"^(?!^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\..*)(\..+)?$)[^\x00-\x1f\\?*:\"";|/]+$";
                    // Modified original by adding check for '<' and '>' characters.
                    string stringPattern = @"^(?!^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\..*)(\..+)?$)[^\x00-\x1f\\?*:\"";|/<>]+$";
                    // Regular Expression Language (RegEx): https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference
                    // Debug.WriteLine($"LibUF.FilenameCheck(): stringFilename={stringFilename}");
                    try
                    {
                        if (!Regex.IsMatch(stringFilename, stringPattern, RegexOptions.IgnoreCase))
                        {
                            // Debug.WriteLine($"LibUF.FilenameCheck(): Regex match found, {stringFilename} is invalid.");
                            return null;
                        }
                    }
                    catch (Exception)
                    {
                        // Catch any RegEx exceptions since not related to file name validity.
                        // More at: https://msdn.microsoft.com/en-us/library/ktzf2d23(v=vs.110).aspx
                        throw;  // Pass exception to calling method to be handled there.
                    }
                    // Attempt to catch any other filename errors by converting name to path and then snatching name back.
                    // Not best practice to use exceptions to trap errors but works here.
                    string stringFilenamePath = $"{Path.DirectorySeparatorChar}{stringFilename}";
                    if (stringFilenamePath != null)
                    {
                        // Debug.WriteLine($"LibUF.FilenameCheck(): stringFilenamePath={stringFilenamePath}");
                        // Path.GetFileName Method: https://msdn.microsoft.com/en-us/library/system.io.path.getfilename(v=vs.110).aspx
                        string stringFilenameResult = Path.GetFileName(stringFilenamePath);      // Get file name and extension from path.
                        if (stringFilenameResult != null)
                        {
                            // Debug.WriteLine($"LibUF.FilenameCheck(): stringFilenameResult={stringFilenameResult}");
                            return stringFilenameResult;
                        }
                    }
                }
            }
            return null;    // Filename check failed.  return null;
        }

        /// <summary>
        /// Check if folder or file is under stringFolderPath hierarchy. If successful, return true, false otherwise. No parameter checking done.
        /// </summary>
        /// <param name="stringFolderPath">Path of folder above stringItemPath.</param>
        /// <param name="stringItemPath">Path of folder or file below stringFolderPath.</param>
        /// <returns></returns>
        public static bool ItemInFolderHierarchy(string stringFolderPath, string stringItemPath)
        {
            try
            {
                // Need to append a backslash to stringFolderPath so can filter out folders that start with same name.
                stringFolderPath += Path.DirectorySeparatorChar;
                //
                // Error condition sample: C:\Data\Users\Public\Documents\... or C:\Data\USERS\Public\Documents\...
                // Sometimes either format is returned causing comparision to fail and erratic behavior afterwards...
                // https://docs.microsoft.com/en-us/dotnet/api/system.stringcomparison?view=netframework-4.7.2
                // https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings?view=netframework-4.7.2
                //
                return stringItemPath.StartsWith(stringFolderPath, StringComparison.OrdinalIgnoreCase);     // Need to ignore case.
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if folder exists. If successful, return true, false otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFolderLocation">Location of folder to check.</param>
        /// <param name="stringFoldername">Name of folder to check if exists.</param>
        /// <returns></returns>
        public static async Task<bool> FolderCheckIfExistsAsync(StorageFolder storageFolderLocation, string stringFoldername)   // Overloaded.
        {
            try
            {
                IStorageItem iStorageItem = await storageFolderLocation.TryGetItemAsync(stringFoldername);
                if (iStorageItem != null)   // Item found but don't know if it was folder or file.
                {
                    if (iStorageItem.IsOfType(StorageItemTypes.Folder))
                        return true;    // Item found was a folder.
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if folder exists. If successful, return true, false otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFolder">Folder to check if exists.</param>
        /// <returns></returns>
        public static async Task<bool> FolderCheckIfExistsAsync(StorageFolder storageFolder)    // Overloaded.
        {
            try
            {
                StorageFolder storageFolderLocation = await storageFolder.GetParentAsync();
                if (storageFolderLocation != null)
                {
                    IStorageItem iStorageItem = await storageFolderLocation.TryGetItemAsync(storageFolder.Name);
                    if (iStorageItem != null)   // Item found but don't know if it was folder or file.
                    {
                        if (iStorageItem.IsOfType(StorageItemTypes.Folder))
                            return true;    // Item found was a folder.
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if file exists. If successful, return true, false otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFolderLocation">Location of file to check.</param>
        /// <param name="stringFilename">Name of file with extension to check if exists.</param>
        /// <returns></returns>
        public static async Task<bool> FileCheckIfExistsAsync(StorageFolder storageFolderLocation, string stringFilename)   // Overloaded.
        {
            try
            {
                IStorageItem iStorageItem = await storageFolderLocation.TryGetItemAsync(stringFilename);
                if (iStorageItem != null)   // Item found but don't know if it was folder or file.
                {
                    if (iStorageItem.IsOfType(StorageItemTypes.File))
                        return true;    // Item found was a file.
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if file exists. If successful, return true, false otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFileName">Name of file to check if exists.</param>
        /// <returns></returns>
        public static async Task<bool> FileCheckIfExistsAsync(StorageFile storageFileName)  // Overloaded.
        {
            try
            {
                StorageFolder storageFolderLocation = await storageFileName.GetParentAsync();
                if (storageFolderLocation != null)
                {
                    IStorageItem iStorageItem = await storageFolderLocation.TryGetItemAsync(storageFileName.Name);
                    if (iStorageItem != null)   // Item found but don't know if it was folder or file.
                    {
                        if (iStorageItem.IsOfType(StorageItemTypes.File))
                            return true;    // Item found was a file.
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete folder or file. If successful, return true, false otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFolderLocation">Location of folder or file to delete.</param>
        /// <param name="stringItemName">Foldername or filename of item to delete. Filenames must include extension.</param>
        public static async Task<bool> StorageItemDeleteAsync(StorageFolder storageFolderLocation, string stringItemName)
        {
            try
            {
                IStorageItem iStorageItem = await storageFolderLocation.TryGetItemAsync(stringItemName);
                if (iStorageItem != null)   // Item found but don't know if it was folder or file.
                {
                    if (iStorageItem.IsOfType(StorageItemTypes.File))
                    {
                        StorageFile storageFileDelete = (StorageFile)iStorageItem;          // Found a file.
                        await storageFileDelete.DeleteAsync();
                        return true;
                    }
                    if (iStorageItem.IsOfType(StorageItemTypes.Folder))
                    {
                        StorageFolder storageFolderDelete = (StorageFolder)iStorageItem;    // Found a folder.
                        await storageFolderDelete.DeleteAsync();
                        return true;
                    }
                }
                return false;   // Item not found.
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete folder or file. If successful, return true, false otherwise. No parameter checking done.
        /// Overload method that provides item lock check and secure delete option.
        /// </summary>
        /// <param name="storageFolderLocation">Location of folder or file to delete.</param>
        /// <param name="stringItemName">Foldername or filename  of item to delete. Filenames must include extension.</param>
        /// <param name="boolDeleteSecure">Secure delete item if true, otherwise use normal deletion.</param>
        /// <returns></returns>
        public static async Task<bool> StorageItemDeleteAsync(StorageFolder storageFolderLocation, string stringItemName, bool boolDeleteSecure)
        {
            try
            {
                IStorageItem iStorageItem = await storageFolderLocation.TryGetItemAsync(stringItemName);
                if (iStorageItem != null)
                {
                    if (iStorageItem.IsOfType(StorageItemTypes.Folder))
                    {
                        StorageFolder storageFolder = (StorageFolder)iStorageItem;
                        // Check if storageFolder is locked before attempting deletion.
                        if (!await IStorageItemLockCheckAsync(storageFolder))
                        {
                            if (boolDeleteSecure)
                            {
                                if (!await LibAES.StorageFolderDeleteSecureAsync(storageFolder, false))
                                    await storageFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                            }
                            else
                                await storageFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                            // Debug.WriteLine($"LibUF.StorageItemDeleteAsync(): Deleted {storageFolder.Path}");
                            return true;    // Success
                        }
                        return false;       // Error, item is locked
                    }
                    else if (iStorageItem.IsOfType(StorageItemTypes.File))
                    {
                        StorageFile storageFile = (StorageFile)iStorageItem;
                        // Check if storageFile is locked before attempting deletion.
                        if (!await IStorageItemLockCheckAsync(storageFile))
                        {
                            if (boolDeleteSecure)
                            {
                                if (!await LibAES.StorageFileDeleteSecureAsync(storageFile, false))
                                    await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                            }
                            else
                                await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                            // Debug.WriteLine($"LibUF.StorageItemDeleteAsync(): Deleted {storageFile.Path}");
                            return true;    // Success
                        }
                        return false;       // Error, item is locked
                    }
                    else
                    {
                        // iStorageItem is not a folder or file. No lock check done in this case.
                        await iStorageItem.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        // Debug.WriteLine($"LibUF.StorageItemDeleteAsync(): Deleted {iStorageItem.Path}");
                        return true;        // Success
                    }
                }
                return false;   // Item not found.
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Copy file. If successful, return true, false otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFileSource">Source file.</param>
        /// <param name="storageFolderLocation">Location to copy source file too.</param>
        /// <param name="stringFilenameDestination">Desired name of copied file.  May change depending on collision option setting.</param>
        /// <param name="nameCollisionOption">Default is ReplaceExisting. Other options are FailIfExist or GenerateUniqueName.</param>
        /// <returns></returns>
        public static async Task<bool> FileCopyAsync(StorageFile storageFileSource, StorageFolder storageFolderLocation, string stringFilenameDestination, NameCollisionOption nameCollisionOption = NameCollisionOption.ReplaceExisting )
        {
            try
            {
                StorageFile storageFileDestination = await storageFileSource.CopyAsync(storageFolderLocation, stringFilenameDestination, nameCollisionOption);
                if (storageFileDestination != null)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Global file extension filter used in recursive methods. Format ".xxx". Used to prevent parameter passing cascade.
        /// </summary>
        private static string stringFileExtensionFilterRecursive;

        /// <summary>
        /// Copy folder. If successful, return true, false otherwise. Will overwrite existing destination folders and/or files.
        /// No parameter checking done.
        /// </summary>
        /// <param name="storageFolderSource">Source folder.</param>
        /// <param name="storageFolderDestination">Destination folder to copy source folder too.</param>
        /// <param name="stringFoldernameDestination">Desired name of destination folder.</param>
        /// <param name="stringFileExtensionFilter">File extension filter. If not null, only copy files that have extension matching filter. Format ".xxx".</param>
        /// <returns></returns>
        public static async Task<bool> FolderCopyAsync(StorageFolder storageFolderSource, StorageFolder storageFolderDestination, string stringFoldernameDestination, string stringFileExtensionFilter = null)
        {
            // This method is a try-catch wrapper for recursive private method FolderCopyRecursiveAsync().
            // Do not wrap recursive methods in potentially endless try-catch blocks bogging them down.
            try
            {
                stringFileExtensionFilterRecursive = stringFileExtensionFilter;
                await FolderCopyRecursiveAsync(storageFolderSource, storageFolderDestination, stringFoldernameDestination);
                return true;    // No exceptions thrown.
            }
            catch
            { 
                return false;
            }
        }

        /// <summary>
        /// Private recursive method to copy folder. Will overwrite existing destination folders and/or files. 
        /// No parameter checking done.
        /// </summary>
        /// <param name="storageFolderSource">Source folder.</param>
        /// <param name="storageFolderDestination">Destination folder to copy source folder too.</param>
        /// <param name="stringFoldernameDestination">Desired name of destination folder.</param>
        /// <returns></returns>
        private static async Task FolderCopyRecursiveAsync(StorageFolder storageFolderSource, StorageFolder storageFolderDestination, string stringFoldernameDestination = null)
        {
            // Following code, before modifications, came from: https://stackoverflow.com/questions/18248287/copy-folder-on-winrt
            // Be extremely careful editing this method since it uses recursion.
            StorageFolder storageFolderDestinationRecursion = null;
            // The ?? operator is called the null-coalescing operator. It returns left-hand operand if the operand is not null, otherwise returns right hand operand.
            storageFolderDestinationRecursion = await storageFolderDestination.CreateFolderAsync(stringFoldernameDestination ?? storageFolderSource.Name, CreationCollisionOption.ReplaceExisting);
            foreach (StorageFile storageFileFound in await storageFolderSource.GetFilesAsync())
            {
                if (stringFileExtensionFilterRecursive == null)     // Copy file.
                    await storageFileFound.CopyAsync(storageFolderDestinationRecursion, storageFileFound.Name, NameCollisionOption.ReplaceExisting);
                else
                {
                    if (storageFileFound.FileType.Equals(stringFileExtensionFilterRecursive, StringComparison.OrdinalIgnoreCase))   // Only copy file if extension matches filter.
                        await storageFileFound.CopyAsync(storageFolderDestinationRecursion, storageFileFound.Name, NameCollisionOption.ReplaceExisting);
                }
            }
            foreach (StorageFolder storageFolderFound in await storageFolderSource.GetFoldersAsync())
            {
                await FolderCopyRecursiveAsync(storageFolderFound, storageFolderDestinationRecursion);      // Recursion happens here.
            }
        }

        /// <summary>
        /// Global list of StorageFiles found in recursive query. Used to prevent parameter passing cascade.
        /// </summary>
        private static List<StorageFile> listStorageFileFound;

        /// <summary>
        /// Query a folder and all subfolders and create a List<StorageFile> of matching files found. Return List<StorageFile> if successful, null otherwise.
        /// No parameter checking done.
        /// </summary>
        /// <param name="storageFolderSource">Source folder.</param>
        /// <param name="stringFileExtensionFilter">File extension filter. Add all files if null. Otherwise only add files that have extension matching filter. Format ".xxx".</param>
        /// <returns></returns>
        public static async Task<List<StorageFile>> FileQueryAsync(StorageFolder storageFolderSource, string stringFileExtensionFilter = null)
        {
            // This method is a try-catch wrapper for recursive private method FileQueryRecursiveAsync().
            // Do not wrap recursive methods in potentially endless try-catch blocks bogging them down.
            try
            {
                stringFileExtensionFilterRecursive = stringFileExtensionFilter;
                listStorageFileFound = new List<StorageFile>();
                await FileQueryRecursiveAsync(storageFolderSource);
                return listStorageFileFound;    // No exceptions thrown so must be OK.
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Private recursive method to query folder. No parameter checking done.
        /// </summary>
        /// <param name="storageFolderSource">Source folder.</param>
        /// <returns></returns>
        private static async Task FileQueryRecursiveAsync(StorageFolder storageFolderSource)
        {
            foreach (StorageFile storageFileFound in await storageFolderSource.GetFilesAsync())
            {
                // Debug.WriteLine($"LibUF.FileQueryRecursiveAsync(): Found file={storageFileFound.Name}");
                if (stringFileExtensionFilterRecursive == null)     // Add found file to list.
                    listStorageFileFound.Add(storageFileFound);
                else
                {
                    if (storageFileFound.FileType.Equals(stringFileExtensionFilterRecursive, StringComparison.OrdinalIgnoreCase))   // Only add found file to list if extension matches filter.
                        listStorageFileFound.Add(storageFileFound);
                }
            }
            foreach (StorageFolder storageFolderFound in await storageFolderSource.GetFoldersAsync())
            {
                // Debug.WriteLine($"LibUF.FileQueryRecursiveAsync(): Found folder={storageFolderFound.Name}");
                await FileQueryRecursiveAsync(storageFolderFound);      // Recursion happens here.
            }
        }

        /// <summary>
        /// Append IBuffer to end of existing file. Return true if success or false if exception. No parameter checking done.
        /// </summary>
        /// <param name="storageFileToAppend">File to append.</param>
        /// <param name="iBufferDataToAppend">IBuffer to append to end of storageFileToAppend.</param>
        /// <returns></returns>
        public static async Task AppendIBufferToFile(StorageFile storageFileToAppend, IBuffer iBufferDataToAppend)
        {
            using (IRandomAccessStream iRandomAccessStream = await storageFileToAppend.OpenAsync(FileAccessMode.ReadWrite))
            {
                ulong ulongStreamSize = iRandomAccessStream.Size;
                //Debug.WriteLine($"LibUF.AppendIBufferToFile(): Existing file size is ulongFileSize={ulongFileSize}");
                IOutputStream iOutputStream = iRandomAccessStream.GetOutputStreamAt(ulongStreamSize);
                using (DataWriter dataWriter = new DataWriter(iOutputStream))
                {
                    dataWriter.WriteBuffer(iBufferDataToAppend);
                    await dataWriter.StoreAsync();
                    await dataWriter.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Return StorageFile size in bytes if success, 0 otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFileGetSize">StorageFile to get size of.</param>
        /// <returns></returns>
        public static async Task<ulong> StorageFileSizeAsync(StorageFile storageFileGetSize)
        {
            BasicProperties basicProperties = await storageFileGetSize.GetBasicPropertiesAsync();
            if (basicProperties != null)
                return basicProperties.Size;    // Return ulong size of file in bytes.
            return 0;   // Return 0 otherwise.
        }

        /// <summary>
        /// Read content of storageFileToByteArray to byte[] and then return byte[]. No parameter checking done.
        /// </summary>
        /// <param name="storageFileToByteArray">Read content of this StorageFile to byte[].</param>
        /// <returns></returns>
        public static async Task<byte[]> StorageFileToByteArray(StorageFile storageFileToByteArray)
        {
            // Minimal error checking done to eliminate redundant checks and keep method fast. Exceptions return null.
            try
            {
                byte[] byteArray;
                using (IRandomAccessStreamWithContentType iRandomAccessStreamWithContentType = await storageFileToByteArray.OpenReadAsync())
                {
                    using (DataReader dataReader = new DataReader(iRandomAccessStreamWithContentType))
                    {
                        byteArray = new byte[iRandomAccessStreamWithContentType.Size];
                        await dataReader.LoadAsync((uint)iRandomAccessStreamWithContentType.Size);
                        // uint MaxValue = 4294967295 bytes or 4.294967295 GB so should be able to process large files.
                        // More at: https://docs.microsoft.com/en-us/dotnet/api/system.uint32.maxvalue?redirectedfrom=MSDN&view=netframework-4.7.2
                        dataReader.ReadBytes(byteArray);
                    }
                }
                return byteArray;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Return true if parameter iStorageItemTCheckIfLocked is locked, false otherwise. No parameter checking done.
        /// </summary>
        /// <param name="iStorageItemCheckIfLocked">Check if parameter is locked.</param>
        /// <returns></returns>
        public static async Task<bool> IStorageItemLockCheckAsync(IStorageItem iStorageItemTCheckIfLocked)
        {
            try
            {
                // Microsoft does not provide API to check if object is locked. Efficient work-a-around is try to rename object.
                // If rename does not throw System.UnauthorizedAccessException, or any other exceptions, then object is not locked.
                string stringIStorageItemNameOrig = iStorageItemTCheckIfLocked.Name;
                string stringIStorageItemNameGuid = $"TempLockCheck_{Guid.NewGuid():N}";
                // Debug.WriteLine($"LibUF.IStorageItemLockCheckAsync(): stringIStorageItemNameOrig={stringIStorageItemNameOrig}");
                // Debug.WriteLine($"LibUF.IStorageItemLockCheckAsync(): stringIStorageItemNameGuid={stringIStorageItemNameGuid}");
                // Try to rename iStorageItemTCheckIfLocked. If it is locked then rename will throw System.UnauthorizedAccessException.
                await iStorageItemTCheckIfLocked.RenameAsync(stringIStorageItemNameGuid);
                await iStorageItemTCheckIfLocked.RenameAsync(stringIStorageItemNameOrig);    // Restore original name if no excpetion.
                // Debug.WriteLine($"LibUF.IStorageItemLockCheckAsync((): {iStorageItemTCheckIfLocked.Name} not locked since rename sequence did not throw exception");
                return false;   // Return false since no exception. This indicates parameter not locked.
            }
            catch // (Exception ex)
            {
                // Debug.WriteLine($"LibUF.IStorageItemLockCheckAsync(): Returned true since exception occurred. Type={ex.GetType()}");
                return true;    // Return true since exception. This indicates parameter is locked.
            }
        }


    }
}
