using LibraryCoder.LibZipArchive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;

/*
 * File: \Libraries\LibAesEncryption.cs.  This library provides various methods related to AES encryption and decryption.  This file will need to be 
 * copied into project or added via 'Add as Link'.
 * 
 * Normally the namespace below would be proceeded by the project name. This is ommitted so library can be shared between projects easily.
 * 
 * Following methods use these procedures to encrypt and decrypt files:  Read unencrypted content of source file to IBuffer. Then encrypt IBuffer.
 * Then write encrypted IBuffer to temporary file. If this completes successfully, delete original unencrypted source file. Then rename temporary file 
 * to name of original source file with extension appended indicating file is now encrypted. To decrypt file, read encrypted content of source file to 
 * IBuffer. Then decrypt IBuffer. Then write decrypted IBuffer to temporary file. If this completes successfully, delete encrypted source file and then 
 * rename temporary file to name of original unencrypted file. In either case, source file is not deleted until process on temporary file completes successfully.
 * 
 * For security reasons, do not save plaintext passwords or hashed passwords in apps.  Secure work around is use a known string of text placed in app and 
 * encrypt it with User password using methods below.  Then save encrypted string to App DataStore.  Later, to confirm if User has entered correct password, 
 * attempt to decrypt saved DataStore string that was encrypted. If decryption fails, password entered was incorrect. If decryption succeeds, then password 
 * entered is likely correct. But to make sure, compare if decrypted string is equal to the known string. Following methods IBufferFromString() and 
 * StringFromIBuffer() can simplify this check.
 * 
 * Important: For performance and efficiency, generally no parameter checking is done by methods in this library. It is responsibility of calling methods to insure 
 * parameter values are valid and that new items created by this library do not already exist. These methods generally return valid value or true if success. 
 * Null or false otherwise. Methods attempt to cleanup and restore original data state if error or exception occurs. These methods attempt to catch any exceptions 
 * and return null or false instead. Calling methods should check results returned by this library are not null or false before proceeding.
 * 
 * Note about maximum IBuffer size:  IBuffers automatically scale up to size needed.  IBuffer maximum size is uint MaxValue which is 4 GiB or 4,294,967,295 bytes.
 * That said, the encryption and decryption process cause intensive memory and disk operations.  Testing shows a 4 GiB folder uses ALL available memory of a 12 GiB PC.
 * Testing also shows a 2 GiB Windows Mobile ARM phone can only process a 250 MB folder.  Files that can be processed will be limited to device used.
 */

namespace LibraryCoder.AesEncryption
{
    /// <summary>
    /// Enum to select Initialization Vector (IV) mode used to process object.
    /// OmitIV does NOT embed or extract IV to or from begining of object. IV will need to be retained independent of object to decrypt object.
    /// EmbedIV will embed or extract IV to to or from begining of object.
    /// </summary>
    public enum EnumModeIV { OmitIV, EmbedIV };

    /// <summary>
    /// Enum to select delete mode used to delete objects. Deleted objects not placed into Recycle Bin.
    /// DeleteNormal option (fast) deletes object from Windows database.
    /// DeleteSecure option (slow) overwrites object with random password and IV then deletes object from Windows database.
    /// </summary>
    public enum EnumModeDelete { DeleteNormal, DeleteSecure };

    /// <summary>
    /// LibAES = Shorthand for LibraryAesEncryption.
    /// </summary>
    public static class LibAES
    {
        /// <summary>
        /// BlockLength is 16 bytes. This is value returned from GetBlockLength(). Set value here once so do not retrieve over and over again.
        /// This value is also required length of the initialization vector (IV) byte array.
        /// </summary>
        public static readonly uint uintBlockLength = 16;

        /// <summary>
        /// Public counter of files found during process. Some items may not have been processed due to error.
        /// Used in recursive methods to eliminate parameter passing cascade.
        /// </summary>
        public static int intCounterItemsRecursive;

        /// <summary>
        /// Public list of file paths that were not encrypted or decrypted due to error. Used in recursive methods to eliminate parameter passing cascade.
        /// </summary>
        public static List<string> listFilePathErrorsRecursive = new List<string>();

        /// <summary>
        /// StorageFile used in recursive methods to eliminate parameter passing cascade.
        /// </summary>
        private static StorageFile storageFileRecrusive;

        /// <summary>
        /// File extension used in recursive methods to eliminate parameter passing cascade.
        /// </summary>
        private static string stringFileExtensionRecursive;

        /// <summary>
        /// Password used in recursive methods to eliminate parameter passing cascade.
        /// </summary>
        private static CryptographicKey cryptographicKeyPasswordRecursive;

        /// <summary>
        /// Initialization vector used in recursive methods to eliminate parameter passing cascade.
        /// </summary>
        private static IBuffer iBufferIVRecursive;

        /// <summary>
        /// Delete mode used in recursive methods to delete objects to eliminate parameter passing cascade.
        /// </summary>
        private static EnumModeDelete enumModeDeleteRecursive;

        /// <summary>
        /// IV mode used in recursive methods to omit or embed IV in items to eliminate parameter passing cascade.
        /// </summary>
        private static EnumModeIV enumModeIVRecursive;

        /// <summary>
        /// True if no errors. Changes to false if error processing any files. 
        /// List of filenames causing an error are saved in listFilePathErrorsRecursive.
        /// Used in recursive methods to eliminate parameter passing cascade.
        /// </summary>
        private static bool boolProcessSuccessfulRecursive;

        /// <summary>
        /// Hash algorithm type used in methods below. Hashing will convert user entered password into a 64 byte long password.
        /// Sha512 is very secure and fast on 64-bit computers since usually supported in hardware.
        /// </summary>
        private static readonly string hashAlgorithmType = HashAlgorithmNames.Sha512;   // The '512' means 512 bits, so this yields 512/8 = 64 byte long password.

        /// <summary>
        /// Symmetric algorithm type used in methods below. AesCbcPkcs7 provides block padding.
        /// </summary>
        private static readonly string symmetricAlgorithmType = SymmetricAlgorithmNames.AesCbcPkcs7;    // BlockLength=16.

        /// <summary>
        /// Binary string encoding type for text strings used in methods below. Same encoding used to encrypt must be used to decrypt. Value is BinaryStringEncoding.Utf8
        /// </summary>
        private const BinaryStringEncoding binaryStringEncodingType = BinaryStringEncoding.Utf8;

        // More about initialization vectors at: https://docs.microsoft.com/en-us/windows/uwp/security/cryptographic-keys
        // Comment out next line since not used.
        /// <summary>
        /// Sample below is a valid initialization vector (IV). Same IV used to encrypt data must be used to decrypt data. IV can be null for symmetric algorithms 
        /// but is not good practice. But, IV should always be null for asymmetric algorithms since they use private/public key pairs. Byte length of IV must match 
        /// symmetricKeyAlgorithmProvider.BlockLength or exception will be thrown. IV does not need to consist of only digits since any valid byte value is valid.
        /// Valid IV for this library can be obtained by via method ByteArrayInitializationVectorRandom(). Or you can set your own IV by initializing a 
        /// byte array similar to this sample.
        /// </summary>
        // private static readonly byte[] byteArrayIV_Sample = { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5, 6 };      // BlockLength=16.

        // This is sample code showing how to retrieve Blocklength. For efficiency, once this value has been determined, 
        // place value in uintBlockLength variable above and use that value instead of calling this method over and over again.
        // Comment out since not used.
        /// <summary>
        /// Return BlockLength used by symmetricAlgorithmType, 0 otherwise.
        /// </summary>
        /// <returns></returns>
        public static uint GetBlockLength()
        {
            try
            {
                return (SymmetricKeyAlgorithmProvider.OpenAlgorithm(symmetricAlgorithmType)).BlockLength;
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.GetBlockLength(): Returned 0 since exception occurred. Type={ex.GetType()}");
                return 0;   // Uint32 is not nullable so return zero instead.
                throw;
            }
        }

        /// <summary>
        /// Return generic array concatenated from parameters array1 and array2, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="array1">First generic array.</param>
        /// <param name="array2">Second generic array to appended to array1.</param>
        /// <returns></returns>
        public static T[] ArrayConcat<T>(T[] array1, T[] array2)
        {
            try
            {
                T[] arrayConcat = new T[array1.Length + array2.Length];   // Initialize to required size.
                Array.Copy(array1, 0, arrayConcat, 0, array1.Length);
                Array.Copy(array2, 0, arrayConcat, array1.Length, array2.Length);
                return arrayConcat;
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.ArrayConcat(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return IBuffer concatenated from parameters iBuffer1 and iBuffer2, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="iBuffer1">First IBuffer.</param>
        /// <param name="iBuffer2">Second IBuffer to append to iBuffer1.</param>
        /// <returns></returns>
        public static IBuffer IBufferConcat(IBuffer iBuffer1, IBuffer iBuffer2)
        {
            try
            {
                // IBuffers can only be created by using return value from another method.
                return CryptographicBuffer.CreateFromByteArray(ArrayConcat(iBuffer1.ToArray(), iBuffer2.ToArray()));
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.IBufferConcat(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return IBuffer from parameter stringToIBuffer, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="stringToIBuffer">String to convert to IBuffer.</param>
        /// <returns></returns>
        public static IBuffer IBufferFromString(string stringToIBuffer)
        {
            try
            {
                return CryptographicBuffer.ConvertStringToBinary(stringToIBuffer, binaryStringEncodingType);
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.IBufferFromString(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return string from parameter iBufferToString, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="iBufferToString">IBuffer to convert to string.</param>
        /// <returns></returns>
        public static string StringFromIBuffer(IBuffer iBufferToString)
        {
            try
            {
                return CryptographicBuffer.ConvertBinaryToString(binaryStringEncodingType, iBufferToString);
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.StringFromIBuffer(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return unique temporary filename with extension of parameter stringFileType. No parameter checking done.
        /// Temporary filename format is $"TempFile_{unique Guid string}{parameter stringFileType}".
        /// </summary>
        /// <param name="stringFileType">File extension, including leading period, to add to end of temp filename. Common value is StorageFile.FileType.</param>
        /// <returns></returns>
        public static string StringGuidTempFilename(string stringFileType)
        {
            return $"TempFile_{Guid.NewGuid():N}{stringFileType}";
        }

        /// <summary>
        /// Return unique temporary foldername. 
        /// Temporary foldername format is $"TempFolder_{unique Guid string}".
        /// </summary>
        /// <returns></returns>
        public static string StringGuidTempFoldername()
        {
            return $"TempFolder_{Guid.NewGuid():N}";
        }

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
                // Debug.WriteLine($"LibAES.IStorageItemLockCheckAsync(): stringIStorageItemNameOrig={stringIStorageItemNameOrig}");
                // Debug.WriteLine($"LibAES.IStorageItemLockCheckAsync(): stringIStorageItemNameGuid={stringIStorageItemNameGuid}");
                // Try to rename iStorageItemToCheckIfLocked. If it is locked then rename will throw System.UnauthorizedAccessException.
                await iStorageItemToCheckIfLocked.RenameAsync(stringIStorageItemNameGuid);
                await iStorageItemToCheckIfLocked.RenameAsync(stringIStorageItemNameOrig);    // Restore original name if no excpetion.
                // Debug.WriteLine($"LibAES.IStorageItemLockCheckAsync((): {iStorageItemToCheckIfLocked.Name} not locked since rename sequence did not throw exception");
                return false;   // Return false since no exception. This indicates iStorageItemToCheckIfLocked not locked.
            }
            catch // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.IStorageItemLockCheckAsync(): Returned true since exception occurred. Type={ex.GetType()}");
                return true;    // Return true since exception. This indicates iStorageItemToCheckIfLocked is locked.
                throw;
            }
        }

        /// <summary>
        /// Delete parameter iStorageItemToDelete using specified enumModeDelete value. If EnumModeDelete.DeleteSecure fails, then use normal deletion.
        /// Lock check on iStorageItemToDelete must be done by calling method since not repeated here.
        /// </summary>
        /// <param name="iStorageItemToDelete">IStorageItem to delete.</param>
        /// <param name="enumModeDelete">Delete mode to use to delete objects.</param>
        /// <returns></returns>
        public static async Task IStorageItemDeleteAsync(IStorageItem iStorageItemToDelete, EnumModeDelete enumModeDelete)
        {
            if (enumModeDelete == EnumModeDelete.DeleteSecure)
            {
                if (iStorageItemToDelete.IsOfType(StorageItemTypes.File))
                {
                    if (await StorageFileDeleteSecureAsync((StorageFile)iStorageItemToDelete, false))      // Secure delete file.
                    {
                        // Debug.WriteLine($"LibAES.IStorageItemDeleteAsync(): Successful file secure delete of {iStorageItemToDelete.Name}");
                        return;
                    }
                }
                if (iStorageItemToDelete.IsOfType(StorageItemTypes.Folder))
                {
                    if (await StorageFolderDeleteSecureAsync((StorageFolder)iStorageItemToDelete, false))  // Secure delete folder.
                    {
                        // Debug.WriteLine($"LibAES.IStorageItemDeleteAsync(): Successful folder secure delete of {iStorageItemToDelete.Name}");
                        return;
                    }
                }
            }
            // Otherwise, use normal deletion.
            await iStorageItemToDelete.DeleteAsync(StorageDeleteOption.PermanentDelete);
            // Debug.WriteLine($"LibAES.IStorageItemDeleteAsync(): Normal delete of {iStorageItemToDelete.Name}");
        }

        /// <summary>
        /// Return CryptographicKey from parameter stringPassword, null otherwise. No parameter checking done.
        /// If return value is null then stringPassword likely was not a valid password. Therefore, this method 
        /// can be used to check that User entered a valid password.
        /// </summary>
        /// <param name="stringPassword">Password string entered by user. Get value directly from a PasswordBox for best security.</param>
        /// <returns></returns>
        public static CryptographicKey CryptographicKeyPassword(string stringPassword)
        {
            try
            {
                // Convert stringPassword to binary and then hash it. Result will be 64 bytes long.
                HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(hashAlgorithmType);
                IBuffer iBufferPassword = hashAlgorithmProvider.HashData(CryptographicBuffer.ConvertStringToBinary(stringPassword, binaryStringEncodingType));
                SymmetricKeyAlgorithmProvider symmetricKeyAlgorithmProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(symmetricAlgorithmType);
                return symmetricKeyAlgorithmProvider.CreateSymmetricKey(iBufferPassword);
            }
            catch   // (Exception ex)    // Parameter stringPassword was likely not a valid password.
            {
                // Debug.WriteLine($"LibAES.CryptographicKeyPassword(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        // More at: https://docs.microsoft.com/en-us/uwp/api/Windows.Security.Cryptography.Core.SymmetricKeyAlgorithmProvider
        /// <summary>
        /// Return CryptographicKey derived from random password, null otherwise.
        /// </summary>
        /// <returns></returns>
        public static CryptographicKey CryptographicKeyPasswordRandom()
        {
            try
            {   
                // Generate random 64 byte IBuffer. 64 byte length is same as hashed password length returned by CryptographicKeyPassword() method above.
                IBuffer iBufferPasswordRandom = CryptographicBuffer.GenerateRandom(64);   // Get 64 byte IBuffer of random bytes.
                SymmetricKeyAlgorithmProvider symmetricKeyAlgorithmProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(symmetricAlgorithmType);
                return symmetricKeyAlgorithmProvider.CreateSymmetricKey(iBufferPasswordRandom);
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.CryptographicKeyPasswordRandom(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return IBuffer from parameter byteArrayIV containing initialization vector (IV), null otherwise. Length of byteArrayIV is checked.
        /// </summary>
        /// <param name="byteArrayIV">Byte array similar to byteArrayIV_Sample declared above. Array length must equal uintBlockLength.</param>
        /// <returns></returns>
        public static IBuffer IBufferFromByteArraryIV(byte[] byteArrayIV)
        {
            try
            {
                if (byteArrayIV.Length == (int)uintBlockLength)
                {
                    return CryptographicBuffer.CreateFromByteArray(byteArrayIV);
                }
                return null;
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.IBufferFromByteArraryIV(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        // More at: https://docs.microsoft.com/en-us/uwp/api/Windows.Security.Cryptography.CryptographicBuffer#Windows_Security_Cryptography_CryptographicBuffer_GenerateRandom_System_UInt32_
        /// <summary>
        /// Return IBuffer containing random Initialization Vector (IV).
        /// </summary>
        /// <returns></returns>
        public static IBuffer IBufferIVRandom()
        {
            try
            {
                return CryptographicBuffer.GenerateRandom(uintBlockLength);
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.IBufferIVRandom(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return encrypted IBuffer derived from parameter values, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="iBufferToEncrypt">IBuffer to encrypt.</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object.</param>
        /// <returns></returns>
        public static IBuffer IBufferEncrypt(IBuffer iBufferToEncrypt, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV)
        {
            try
            {
                return CryptographicEngine.Encrypt(cryptographicKeyPassword, iBufferToEncrypt, iBufferIV);
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.IBufferEncrypt(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return decrypted IBuffer derived from parameter values, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="iBufferToDecrypt">IBuffer to decrypt.</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object.</param>
        /// <returns></returns>
        public static IBuffer IBufferDecrypt(IBuffer iBufferToDecrypt, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV)
        {
            try
            {
                return CryptographicEngine.Decrypt(cryptographicKeyPassword, iBufferToDecrypt, iBufferIV);
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.IBufferDecrypt(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return encrypted string derived from parameter values, null otherwise. No parameter checking done. 
        /// </summary>
        /// <param name="stringToEncrypt">String to encrypt.</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object. If enumModeIV=EnumModeIV.EmbedIV, then this value will be prepended to object.</param>
        /// <param name="enumModeIV">IV mode to omit or embed IV into string. Same mode used to encrypt string must be used to decrypt string.</param>
        /// <returns></returns>
        public static string StringEncrypt(string stringToEncrypt, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV, EnumModeIV enumModeIV)
        {
            try
            {
                // Create IBuffer that contains stringToEncrypt after converting it to binary.
                IBuffer iBufferBinary = CryptographicBuffer.ConvertStringToBinary(stringToEncrypt, binaryStringEncodingType);
                IBuffer iBufferEncrypted = CryptographicEngine.Encrypt(cryptographicKeyPassword, iBufferBinary, iBufferIV);
                if (enumModeIV.Equals(EnumModeIV.EmbedIV))  // Embed IV in string.
                {
                    IBuffer iBufferEncryptedEmbedIV = IBufferConcat(iBufferIV, iBufferEncrypted);
                    return CryptographicBuffer.EncodeToBase64String(iBufferEncryptedEmbedIV);     // Convert iBufferEncryptedEmbedIV to string.
                }
                else
                {
                    return CryptographicBuffer.EncodeToBase64String(iBufferEncrypted);     // Convert iBufferEncrypted to string.
                }
            }
            catch // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.StringEncrypt(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return decrypted string derived from parameter values, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="stringToDecrypt">String to decrypt.</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object. This value not used if enumModeIV=EnumModeIV.EmbedIV so can be null if so.</param>
        /// <param name="enumModeIV">IV mode to omit or embed IV into string. Same mode used to encrypt string must be used to decrypt string.</param>
        /// <returns></returns>
        public static string StringDecrypt(string stringToDecrypt, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV, EnumModeIV enumModeIV)
        {
            try
            {
                IBuffer iBufferDecrypt = null;
                IBuffer iBufferDecode = CryptographicBuffer.DecodeFromBase64String(stringToDecrypt);
                if (enumModeIV.Equals(EnumModeIV.EmbedIV))
                {
                    // Get embedded IV from beginning of iBufferDecode.
                    IBuffer iBufferIVRandom = CryptographicBuffer.CreateFromByteArray(new byte[uintBlockLength]);
                    iBufferDecode.CopyTo(0, iBufferIVRandom, 0, iBufferIVRandom.Length);   // Copy embedded IV in iBufferDecode to iBufferRandomIV.
                    // Get rest of encrypted data from iBufferDecode.
                    IBuffer iBufferEncryptedData = CryptographicBuffer.CreateFromByteArray(new byte[iBufferDecode.Length - iBufferIVRandom.Length]);
                    iBufferDecode.CopyTo(iBufferIVRandom.Length, iBufferEncryptedData, 0, iBufferEncryptedData.Length);

                    // Decrypt iBufferEncryptedData.
                    iBufferDecrypt = CryptographicEngine.Decrypt(cryptographicKeyPassword, iBufferEncryptedData, iBufferIVRandom);
                }
                else
                {
                    iBufferDecrypt = CryptographicEngine.Decrypt(cryptographicKeyPassword, iBufferDecode, iBufferIV);
                }
                return CryptographicBuffer.ConvertBinaryToString(binaryStringEncodingType, iBufferDecrypt);
            }
            catch   // (Exception ex)
            {
                //Debug.WriteLine($"LibAES.StringDecrypt(): Returned null since exception occurred. Type={ex.GetType()}");
                return null;
                throw;
            }
        }

        /// <summary>
        /// Return true if parameter storageFileToDelete was secure deleted, false otherwise. No parameter checking done.
        /// This method overwrites file with encrypted content of file using random password and random IV and then 
        /// deletes the file. Note: This is complex issue since Windows uses various caches including recycle bin, file history, hibernation 
        /// file, swap file, recovery files, and older erased versions of file, just to mention some. Therefore, file could 
        /// still exist somewhere on system.
        /// </summary>
        /// <param name="storageFileToDelete">StorageFile to secure delete.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<bool> StorageFileDeleteSecureAsync(StorageFile storageFileToDelete, bool boolCheckIfLocked = true)
        {
            try
            {
                if (boolCheckIfLocked)  // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(storageFileToDelete))
                    {
                        // Debug.WriteLine($"LibAES.StorageFileDeleteSecureAsync(): {storageFileToDelete.Name} is locked.");
                        return false;
                    }
                }
                IBuffer iBufferDecrypted = await FileIO.ReadBufferAsync(storageFileToDelete);           // Read content of file into buffer.
                CryptographicKey cryptographicKeyPasswordRandom = CryptographicKeyPasswordRandom();     // Get random key.
                IBuffer iBufferEncrypted = IBufferEncrypt(iBufferDecrypted, cryptographicKeyPasswordRandom, IBufferIVRandom());
                await FileIO.WriteBufferAsync(storageFileToDelete, iBufferEncrypted);           // Overwrite file with encrypted data.
                await storageFileToDelete.DeleteAsync(StorageDeleteOption.PermanentDelete);     // Data has been encrypted so delete file.
                // Debug.WriteLine("LibAES.StorageFileDeleteSecureAsync(): Secure file deletion succeeded so returned true.");
                return true;
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.StorageFileDeleteSecureAsync(): Returned false since exception occurred. Type={ex.GetType()}");
                return false;
                throw;
            }
        }

        /// <summary>
        /// Return true if parameter storageFolderToDelete was secure deleted, false otherwise. No parameter checking done.
        /// This method uses recursion to step through folder structure and overwrites any found file with encrypted content of file using random password 
        /// and random IV and then deletes the file. All folders found are also deleted. Note: This is complex issue since Windows uses various caches
        /// including recycle bin, file history, hibernation file, swap file, recovery files, and older erased versions of file, just to mention some. 
        /// Therefore, folder could still exist somewhere on system.
        /// </summary>
        /// <param name="storageFolderToDelete">StorageFolder to secure delete.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<bool> StorageFolderDeleteSecureAsync(StorageFolder storageFolderToDelete, bool boolCheckIfLocked = true)
        {
            try
            {
                if (boolCheckIfLocked)      // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(storageFolderToDelete))
                    {
                        // Debug.WriteLine($"LibAES.StorageFolderDeleteSecureAsync(): {storageFolderToDelete.Name} is locked.");
                        return false;
                    }
                }
                cryptographicKeyPasswordRecursive = CryptographicKeyPasswordRandom();   // Set global recursive key with random key,
                iBufferIVRecursive = IBufferIVRandom();     // Set global recursive IV with a random IV.
                await StorageFolderDeleteSecureRecursiveAsync(storageFolderToDelete);   // Recursion used here! Step through any file in any subfolders and then secure delete them.
                await storageFolderToDelete.DeleteAsync(StorageDeleteOption.PermanentDelete);   // Delete storageFolderToDelete since now empty.
                // Debug.WriteLine("LibAES.StorageFolderDeleteSecureAsync(): Secure folder deletion succeeded so returned true.");
                return true;
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.StorageFolderDeleteSecureAsync(): Returned false since exception occurred. Type={ex.GetType()}");
                return false;
                throw;
            }
        }

        /// <summary>
        /// Private recursive method called from StorageFolderDeleteSecureAsync() to secure delete parameter storageFolderDelete. No parameter checking done.
        /// Method steps through each subfolder secure deleting files found on way in and then deletes folders found on way out.
        /// </summary>
        /// <param name="storageFolderDelete">StorageFolder to secure delete.</param>
        /// <returns></returns>
        private static async Task StorageFolderDeleteSecureRecursiveAsync(StorageFolder storageFolderDelete)
        {
            /****** Be extremely careful editing this method since it uses recursion. ******/
            IReadOnlyList<StorageFile> listStorageFiles = await storageFolderDelete.GetFilesAsync();
            if (listStorageFiles.Count > 0)
            {
                foreach (StorageFile storageFileFound in listStorageFiles)
                {
                    // Debug.WriteLine($"LibAES.StorageFolderDeleteSecureRecursiveAsync(): Deleted file={storageFileFound.Path}");
                    await StorageFileDeleteSecureRecursiveAsync(storageFileFound);   // Secure delete found file.
                }
            }
            IReadOnlyList<StorageFolder> listStorageFolders = await storageFolderDelete.GetFoldersAsync();
            if (listStorageFolders.Count > 0)
            {
                foreach (StorageFolder storageFolderFound in listStorageFolders)
                {
                    // Next line is recursive method call.
                    await StorageFolderDeleteSecureRecursiveAsync(storageFolderFound);
                    // Debug.WriteLine($"LibAES.StorageFolderDeleteSecureRecursiveAsync(): Deleted Folder={storageFolderFound.Path}");
                    await storageFolderFound.DeleteAsync(StorageDeleteOption.PermanentDelete);  // Delete found folder.
                }
            }
        }

        /// <summary>
        /// Private method called from StorageFolderDeleteSecureRecursiveAsync() that secure deletes parameter storageFileToDelete. No parameter checking done.
        /// This method uses global recursive variables set in StorageFolderDeleteSecureAsync().
        /// </summary>
        /// <param name="storageFileToDelete">StorageFile to secure delete.</param>
        /// <returns></returns>
        private static async Task StorageFileDeleteSecureRecursiveAsync(StorageFile storageFileToDelete)
        {
            IBuffer iBufferDecrypted = await FileIO.ReadBufferAsync(storageFileToDelete);   // Read content of file into buffer.
            IBuffer iBufferEncrypted = IBufferEncrypt(iBufferDecrypted, cryptographicKeyPasswordRecursive, iBufferIVRecursive);   // Encrypt content of buffer.
            await FileIO.WriteBufferAsync(storageFileToDelete, iBufferEncrypted);           // Overwrite file with encrypted content.
            await storageFileToDelete.DeleteAsync(StorageDeleteOption.PermanentDelete);     // Delete file.
        }







        // Parameter storageFolderParent added as performance improvement. Easy enough to get parent folder from parameter storageFileToDecrypt 
        // but this is usually already known and saves a bunch of underlying methods calls and cleanup to recreate a known value.

        /// <summary>
        /// Encrypt StorageFile. If successful, return encrypted StorageFile, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFolderParent">Parent folder of storageFileToEncrypt that App has read/write access too.</param>
        /// <param name="storageFileToEncrypt">StorageFile to encrypt.</param>
        /// <param name="stringFileExtensionAdd">File extension string, including leading period, to append to name of encrypted file. Format is ".xxx".</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object. If enumModeIV=EnumModeIV.EmbedIV, then this value will be prepended to object.</param>
        /// <param name="enumModeDelete">Delete mode to use to delete objects.</param>
        /// <param name="enumModeIV">IV mode to omit or embed IV into file. Same mode used to encrypt file must be used to decrypt file.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<StorageFile> StorageFileEncryptAsync(StorageFolder storageFolderParent, StorageFile storageFileToEncrypt, string stringFileExtensionAdd, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV, EnumModeDelete enumModeDelete, EnumModeIV enumModeIV, bool boolCheckIfLocked = true)
        {
            string stringFilenameTemp = null;   // Initialize name of temp file here since may need to cleanup in catch block if exception.
            try
            {
                if (boolCheckIfLocked)      // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(storageFileToEncrypt))
                    {
                        // Debug.WriteLine($"LibAES.StorageFileEncryptAsync(): {storageFileToEncrypt.Name} is locked.");
                        return null;
                    }
                }
                IBuffer iBufferDecrypted = await FileIO.ReadBufferAsync(storageFileToEncrypt);      // Read content of file into buffer.
                if (iBufferDecrypted != null)
                {
                    IBuffer iBufferEncrypted = IBufferEncrypt(iBufferDecrypted, cryptographicKeyPassword, iBufferIV);  // Encrypt buffer.
                    if (iBufferEncrypted != null)
                    {
                        string stringFilenameEncrypted = $"{storageFileToEncrypt.Name}{stringFileExtensionAdd}";    // Build name of final encrypted file..
                        stringFilenameTemp = StringGuidTempFilename(storageFileToEncrypt.FileType);                 // Build name of temporary file that encrpted content will be saved too.
                        StorageFile StorageFileTemp = await storageFolderParent.CreateFileAsync(stringFilenameTemp);
                        if (StorageFileTemp != null)
                        {
                            bool boolSuccess = false;
                            // Embed IV at beginning of file.
                            using (IRandomAccessStream iRandomAccessStream = await StorageFileTemp.OpenAsync(FileAccessMode.ReadWrite))    // Must be ReadWrite since writing data.
                            {
                                IOutputStream iOutputStream = iRandomAccessStream.GetOutputStreamAt(0);     // Open stream to beginning of temp file.
                                if (iOutputStream != null)
                                {
                                    using (DataWriter dataWriter = new DataWriter(iOutputStream))
                                    {
                                        if (dataWriter != null)
                                        {
                                            if (enumModeIV.Equals(EnumModeIV.EmbedIV))
                                                dataWriter.WriteBuffer(iBufferIV);                                  // Add iBufferIV bytes to stream.
                                            dataWriter.WriteBuffer(iBufferEncrypted);                               // Add encrypted data to stream.
                                            await IStorageItemDeleteAsync(storageFileToEncrypt, enumModeDelete);    // Delete original file before writing stream to temp file.
                                            await dataWriter.StoreAsync();                                          // Write stream to temp file.
                                            await dataWriter.FlushAsync();                                          // Remove stream.
                                            boolSuccess = true;
                                            // Exit using blocks ASAP to release memory before doing anything else.
                                        }
                                    }
                                }
                            }
                            if (boolSuccess)    // Encryption of temp file succeeded and original file deleted so rename temp file.
                            {
                                await StorageFileTemp.RenameAsync(stringFilenameEncrypted);    // Rename temp file.
                                return StorageFileTemp;
                            }
                        }
                    }
                }
                return null;    // Something failed so return null.
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.StorageFileEncryptAsync(): Returned null since exception occurred. Type={ex.GetType()}");
                // Clean up by deleting temp file if still exists.
                if (stringFilenameTemp != null)
                {
                    IStorageItem iStorageItem = await storageFolderParent.TryGetItemAsync(stringFilenameTemp);
                    if (iStorageItem != null)   // Found something with name of temp file.
                    {
                        await IStorageItemDeleteAsync(iStorageItem, enumModeDelete);
                        // Debug.WriteLine($"LibAES.StorageFileEncryptAsync(): Exception occurred so deleted temp file: {iStorageItem.Name}");
                    }
                }
                return null;
                throw;
            }
        }

        /// <summary>
        /// Decrypt StorageFile. If successful, return decrypted StorageFile, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFolderParent">Parent folder of storageFileToDecrypt that App has read/write access too.</param>
        /// <param name="storageFileToDecrypt">StorageFile to decrypt.</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object. This value not used if enumModeIV=EnumModeIV.EmbedIV so can be null if so.</param>
        /// <param name="enumModeDelete">Delete mode to use to delete items.</param>
        /// <param name="enumModeIV">IV mode to omit or embed IV into file. Same mode used to encrypt file must be used to decrypt file.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<StorageFile> StorageFileDecryptAsync(StorageFolder storageFolderParent, StorageFile storageFileToDecrypt, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV, EnumModeDelete enumModeDelete, EnumModeIV enumModeIV, bool boolCheckIfLocked = true)
        {
            string stringFilenameTemp = null;   // Initialize name of temp file here since may need to cleanup in catch block if exception.
            try
            {
                if (boolCheckIfLocked)      // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(storageFileToDecrypt))
                    {
                        // Debug.WriteLine($"LibAES.StorageFileEncryptAsync(): {storageFileToDecrypt.Name} is locked.");
                        return null;
                    }
                }
                IBuffer iBufferUseIV = null;
                IBuffer iBufferEncrypted = null;
                using (IRandomAccessStream iRandomAccessStream = await storageFileToDecrypt.OpenAsync(FileAccessMode.Read))
                {
                    if (iRandomAccessStream != null)
                    {
                        using (DataReader dataReader = new DataReader(iRandomAccessStream))    // Pass file input stream to DataReader.
                        {
                            if (dataReader != null)
                            {
                                // Get file size so know how many bytes to read. Cast to uint from ulong should be safe since file was previously encrypted by library.
                                uint uintStorageFileToDecryptSize = (uint)iRandomAccessStream.Size; ;  // Return size of stream in bytes which is also size of storageFileToDecrypt.
                                // Debug.WriteLine($"LibAES.StorageFileDecryptAsync(): uintStorageFileToDecryptSize={uintStorageFileToDecryptSize}");
                                if (uintStorageFileToDecryptSize > 0)
                                {
                                    bool boolReadRestOfFile = false;
                                    uint uintBytesRead = 0;
                                    uint uintBytesBufferSize = 0;
                                    if (enumModeIV.Equals(EnumModeIV.EmbedIV))
                                    {
                                        // Get IV from beginning of file and use it to decrypt rest of file.
                                        // But first check if file length greater than uintBlockLength before attempting to get embedded IV.
                                        if (uintStorageFileToDecryptSize > uintBlockLength)
                                        {
                                            uint uintBytesToRead = uintBlockLength;     // Read uintBlockLength bytes from beginning of file.
                                            do
                                            {
                                                uintBytesRead = await dataReader.LoadAsync(uintBytesToRead);    // Experience shows that LoadAsync returns all bytes requested on first gobble but this is safe way if not!
                                                uintBytesToRead -= uintBytesRead;
                                            } while (uintBytesToRead > 0);
                                            iBufferUseIV = dataReader.ReadBuffer(uintBlockLength);   // Save first uintBlockLength bytes of file to buffer. This should be the embedded IV.
                                            if (iBufferUseIV != null)
                                            {
                                                boolReadRestOfFile = true;
                                            }
                                        }   // Error so fall through and return null.
                                    }
                                    else
                                    {
                                        // Do NOT get IV from beginning of file. Use parameter iBufferIV to decrypt file.
                                        iBufferUseIV = iBufferIV;
                                        boolReadRestOfFile = true;
                                    }
                                    if (boolReadRestOfFile)
                                    {
                                        // Get unread content of dataReader. This is encrypted content of original file.
                                        do
                                        {
                                            // uintStorageFileToDecryptSize is max number of bytes that can be read. Result read may be less if IV was embedded in file and previously extracted.
                                            uintBytesRead = await dataReader.LoadAsync(uintStorageFileToDecryptSize);
                                            // Debug.WriteLine($"LibAES.StorageFileDecryptAsync(): uintBytesRead={uintBytesRead}");
                                            uintBytesBufferSize += uintBytesRead;   // Accumulate total bytes read since need to create IBuffer.
                                        } while (uintBytesRead > 0);    // Keep reading until dataReader is consumed.
                                                                        // Debug.WriteLine($"LibAES.StorageFileDecryptAsync(): uintBytesBufferSize={uintBytesBufferSize}");
                                        iBufferEncrypted = dataReader.ReadBuffer(uintBytesBufferSize);  // Write content of dataReader to buffer.
                                                                                                        // Exit using blocks ASAP to release memory before doing anything else.
                                    }
                                }
                            }
                        }
                    }
                }
                if (iBufferEncrypted != null)
                {
                    // Debug.WriteLine($"LibAES.StorageFileDecryptAsync(): iBufferEncrypted != null");
                    IBuffer iBufferDecrypted = IBufferDecrypt(iBufferEncrypted, cryptographicKeyPassword, iBufferUseIV);
                    if (iBufferDecrypted != null)
                    {
                        stringFilenameTemp = StringGuidTempFilename(storageFileToDecrypt.FileType);    // Build filename of decrypted temp file.
                        // Create temp file to write decrypted buffer data too.
                        StorageFile storageFileTemp = await storageFolderParent.CreateFileAsync(stringFilenameTemp);
                        if (storageFileTemp != null)
                        {
                            // Remove extension from storageFileToDecrypt that was added when file was encrypted. No error checking!
                            string stringFilenameDecrypted = Path.GetFileNameWithoutExtension(storageFileToDecrypt.Name);
                            // Delete original file and then rename temp file to name of original file.
                            // Secure delete is redundant here since file is already encrypted, so just delete normally.
                            await storageFileToDecrypt.DeleteAsync(StorageDeleteOption.PermanentDelete);
                            // Write decrypted content to temp file.
                            await FileIO.WriteBufferAsync(storageFileTemp, iBufferDecrypted);
                            await storageFileTemp.RenameAsync(stringFilenameDecrypted);
                            return storageFileTemp;
                        }
                    }
                }
                return null;    // Something failed so return null.
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.StorageFileDecryptAsync(): Returned null since exception occurred. Type={ex.GetType()}");
                // Clean up by deleting temp file if still exists.
                if (stringFilenameTemp != null)
                {
                    IStorageItem iStorageItem = await storageFolderParent.TryGetItemAsync(stringFilenameTemp);
                    if (iStorageItem != null)   // Found temp file.
                    {
                        await IStorageItemDeleteAsync(iStorageItem, enumModeDelete);
                        // Debug.WriteLine($"LibAES.StorageFileDecryptAsync(): Exception occurred so deleted temp file: {iStorageItem.Name}");
                    }
                }
                return null;
                throw;
            }
        }

        /// <summary>
        /// Encrypt files in StorageFolder including subfolders. Returns true if all files were encrypted without error, false otherwise. 
        /// If false, path of files with error placed in public list listFilePathErrorsRecursive. Method uses recursion.
        /// </summary>
        /// <param name="storageFolderToEncrypt">StorageFolder to encrypt.</param>
        /// <param name="stringFileExtensionAdd">File extension string, including leading period, to append to name of encrypted file. Format is ".xxx".</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object. If enumModeIV=EnumModeIV.EmbedIV, then this value will be prepended to object.</param>
        /// <param name="enumModeDelete">Delete mode to use to delete objects.</param>
        /// <param name="enumModeIV">IV mode to omit or embed IV into file. Same mode used to encrypt file must be used to decrypt file.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<bool> StorageFolderEncryptAsync(StorageFolder storageFolderToEncrypt, string stringFileExtensionAdd, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV, EnumModeDelete enumModeDelete, EnumModeIV enumModeIV, bool boolCheckIfLocked = true)
        {
            try
            {
                if (boolCheckIfLocked)      // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(storageFolderToEncrypt))
                    {
                        // Debug.WriteLine($"LibAES.StorageFolderEncryptAsync(): {storageFolderToEncrypt.Name} is locked.");
                        return false;
                    }
                }
                // Folder lock check here redundant since each file found is checked before encryption. If file found is locked then encryption of it is skipped.
                // Set global variables used during recursion one time to eliminate a parameter passing cascade.
                boolProcessSuccessfulRecursive = true;  // Reset flag to true. Change to false if error while processing any file.
                intCounterItemsRecursive = 0;           // Reset counter to 0.
                listFilePathErrorsRecursive.Clear();    // Reset/Clear list of filenames that had errors on previous runs.
                stringFileExtensionRecursive = stringFileExtensionAdd;
                cryptographicKeyPasswordRecursive = cryptographicKeyPassword;
                iBufferIVRecursive = iBufferIV;
                enumModeDeleteRecursive = enumModeDelete;
                enumModeIVRecursive = enumModeIV;
                await StorageFolderEncryptRecursiveAsync(storageFolderToEncrypt);    // Step through folder structure and attempt to encrypt any files found.
                if (boolProcessSuccessfulRecursive)
                    return true;    // Success since bool still true. All files encrypted without error.
                return false;       // At least one file was not encrypted. List of file paths with errors placed in listFilePathErrorsRecursive.
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.StorageFolderEncryptAsync(): Returned false since exception occurred. Type={ex.GetType()}");
                return false;
                throw;
            }
        }

        /// <summary>
        /// Private recursive method that encrypts files in a StorageFolder. No parameter checking done. 
        /// Method steps through each subfolder and encrypts any files found.
        /// </summary>
        /// <param name="storageFolderEncrypt">StorageFolder to encrypt.</param>
        /// <returns></returns>
        private static async Task StorageFolderEncryptRecursiveAsync(StorageFolder storageFolderEncrypt)
        {
            /****** Be extremely careful editing this method since it uses recursion. Method needs to run out to end without returns since recursive. *******/
            IReadOnlyList<StorageFile> listStorageFiles = await storageFolderEncrypt.GetFilesAsync();
            if (listStorageFiles != null)
            {
                if (listStorageFiles.Count > 0)
                {
                    foreach (StorageFile storageFileFound in listStorageFiles)
                    {
                        // Debug.WriteLine($"LibAES:StorageFolderEncryptRecursiveAsync(): Found {storageFileFound.Path}");
                        if (!storageFileFound.FileType.Equals(stringFileExtensionRecursive, StringComparison.OrdinalIgnoreCase))    // Skip file if extension indicates it is already encryted.
                        {
                            bool boolError = true;          // Set to false when file successfully encrypted.
                            intCounterItemsRecursive++;     // Increase count of files processed.
                                                            // Check that encrypted file with result name does not already exist.
                            string stringFilenameEncrypted = $"{storageFileFound.Name}{stringFileExtensionRecursive}";
                            // Debug.WriteLine($"LibAES:StorageFolderEncryptRecursiveAsync(): stringFilenameEncrypted={stringFilenameEncrypted}");
                            if (await storageFolderEncrypt.TryGetItemAsync(stringFilenameEncrypted) == null)
                            {
                                // Skip file lock check on storageFileFound since lock check was done by calling method.
                                storageFileRecrusive = await StorageFileEncryptAsync(storageFolderEncrypt, storageFileFound, stringFileExtensionRecursive, cryptographicKeyPasswordRecursive, iBufferIVRecursive, enumModeDeleteRecursive, enumModeIVRecursive, false);
                                if (storageFileRecrusive != null)
                                {
                                    boolError = false;
                                    // Debug.WriteLine($"LibAES:StorageFolderEncryptRecursiveAsync(): Encrypted {storageFileRecrusive.Name}");
                                }
                            }
                            if (boolError)
                            {
                                boolProcessSuccessfulRecursive = false;
                                listFilePathErrorsRecursive.Add(storageFileFound.Path);  // Add file path to error list.
                                // Debug.WriteLine($"LibAES.StorageFolderEncryptRecursiveAsync(): boolError=true, boolProcessSuccessfulRecursive={boolProcessSuccessfulRecursive}, file={storageFileFound.Path}");
                            }
                        }
                    }
                }
            }
            IReadOnlyList<StorageFolder> listStorageFolders = await storageFolderEncrypt.GetFoldersAsync();
            if (listStorageFolders != null)
            {
                if (listStorageFolders.Count > 0)
                {
                    foreach (StorageFolder storageFolderFound in listStorageFolders)
                    {
                        // Debug.WriteLine($"FolderEncryptRecursiveAsync(): Found Folder={storageFolderFound.Path}");
                        // Next line is recursive method call.
                        await StorageFolderEncryptRecursiveAsync(storageFolderFound);
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt files in StorageFolder including subfolders. Returns true if all files were decrypted without error, false otherwise.
        /// If false, path of files with error placed in public list listFilePathErrorsRecursive. Method uses recursion.
        /// </summary>
        /// <param name="storageFolderToDecrypt">StorageFolder to decrypt.</param>
        /// <param name="stringFileExtensionRemove">File extension, including leading period, to remove from name of any files found that have same extension.</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object. This value not used if enumModeIV=EnumModeIV.EmbedIV so can be null if so.</param>
        /// <param name="enumModeDelete">Delete mode to use to delete objects.</param>
        /// <param name="enumModeIV">IV mode to omit or embed IV into file. Same mode used to encrypt file must be used to decrypt file.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<bool> StorageFolderDecryptAsync(StorageFolder storageFolderToDecrypt, string stringFileExtensionRemove, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV, EnumModeDelete enumModeDelete, EnumModeIV enumModeIV, bool boolCheckIfLocked = true)
        {
            try
            {
                if (boolCheckIfLocked)      // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(storageFolderToDecrypt))
                    {
                        // Debug.WriteLine($"LibAES.StorageFolderDecryptAsync(): {storageFolderToDecrypt.Name} is locked.");
                        return false;
                    }
                }
                // Set global variables used during recursion once to eliminate parameter passing cascade.
                boolProcessSuccessfulRecursive = true;  // Reset flag to true. Change to false if error while processing any file.
                intCounterItemsRecursive = 0;           // Reset counter to 0.
                listFilePathErrorsRecursive.Clear();    // Reset/Clear list of filenames that had errors.
                stringFileExtensionRecursive = stringFileExtensionRemove;
                cryptographicKeyPasswordRecursive = cryptographicKeyPassword;
                iBufferIVRecursive = iBufferIV;
                enumModeDeleteRecursive = enumModeDelete;
                enumModeIVRecursive = enumModeIV;
                await StorageFolderDecryptRecursiveAsync(storageFolderToDecrypt);    // Step through folder structure and attempt to decrypt any files found.
                if (boolProcessSuccessfulRecursive)
                    return true;    // Success since bool still true. All files decrypted without error.
                return false;       // At least one file was not decrypted. List of file paths with errors placed in listFilePathErrorsRecursive.
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.StorageFolderDecryptAsync(): Returned false since exception occurred. Type={ex.GetType()}");
                return false;
                throw;
            }
        }

        /// <summary>
        /// Private recursive method that decrypts files in a folder. No parameter checking done. 
        /// Method steps through each subfolder and decrypts any files found.
        /// </summary>
        /// <param name="storageFolderDecrypt">StorgeFolder to decrypt.</param>
        /// <returns></returns>
        private static async Task StorageFolderDecryptRecursiveAsync(StorageFolder storageFolderDecrypt)
        {
            /****** Be extremely careful editing this method since it uses recursion. Method needs to run out to end without returns since recursive. *******/
            IReadOnlyList<StorageFile> listStorageFiles = await storageFolderDecrypt.GetFilesAsync();
            if (listStorageFiles != null)
            {
                if (listStorageFiles.Count > 0)
                {
                    foreach (StorageFile storageFileFound in listStorageFiles)
                    {
                        // Debug.WriteLine($"LibAES.StorageFolderDecryptRecursiveAsync(): Found {storageFileFound.Path}");
                        if (storageFileFound.FileType.Equals(stringFileExtensionRecursive, StringComparison.OrdinalIgnoreCase))     // Skip file unless extension indicates it is encryted.
                        {
                            bool boolError = true;          // Set to false when file successfully decrypted.
                            intCounterItemsRecursive++;     // Increase count of files processed.
                                                            // Debug.WriteLine($"LibAES.StorageFolderDecryptRecursiveAsync(): Processing {storageFileFound.Path}");
                                                            // Check that decrypted file with result name does not already exist.
                            string stringFilenameDecrypted = Path.GetFileNameWithoutExtension(storageFileFound.Name);     // Remove extension stringFileExtensionRecursive.
                                                                                                                          // Debug.WriteLine($"LibAES.StorageFolderDecryptRecursiveAsync(): stringFilenameDecrypted={stringFilenameDecrypted}");
                            if (await storageFolderDecrypt.TryGetItemAsync(stringFilenameDecrypted) == null)
                            {
                                // Skip file lock check on storageFileFound since lock check was done by calling method.
                                storageFileRecrusive = await StorageFileDecryptAsync(storageFolderDecrypt, storageFileFound, cryptographicKeyPasswordRecursive, iBufferIVRecursive, enumModeDeleteRecursive, enumModeIVRecursive, false);
                                if (storageFileRecrusive != null)
                                {
                                    boolError = false;  // File was decrypted.
                                                        // Debug.WriteLine($"LibAES.StorageFolderDecryptRecursiveAsync(): Decrypted {storageFileRecrusive.Name}");
                                }
                            }
                            if (boolError)
                            {
                                boolProcessSuccessfulRecursive = false;
                                listFilePathErrorsRecursive.Add(storageFileFound.Path);  // Add file path to error list.
                                                                                         // Debug.WriteLine($"LibAES.StorageFolderDecryptRecursiveAsync(): boolError=true, boolProcessSuccessfulRecursive={boolProcessSuccessfulRecursive}, file={storageFileFound.Path}");
                            }
                        }
                    }
                }
            }
            IReadOnlyList<StorageFolder> listStorageFolders = await storageFolderDecrypt.GetFoldersAsync();
            if (listStorageFolders != null)
            {
                if (listStorageFolders.Count > 0)
                {
                    foreach (StorageFolder storageFolderFound in listStorageFolders)
                    {
                        // Debug.WriteLine($"LibAES.StorageFolderDecryptRecursiveAsync(): Found Folder={storageFolderFound.Path}");
                        // Next line is recursive method call.
                        await StorageFolderDecryptRecursiveAsync(storageFolderFound);
                    }
                }
            }
        }

        /// <summary>
        /// Compress storageFolderToEncypt into a file and then encrypt that file.
        /// If success return processed StorageFile, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFolderParent">Parent folder of storageFolderToEncypt that App has read/write access too.</param>
        /// <param name="storageFolderToEncypt">Compress this folder into a file and then encrypt that file.</param>
        /// <param name="stringFileExtensionAdd">File extension string, including leading period, to append to name of encrypted file. Format is ".xxx".</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object. If enumModeIV=EnumModeIV.EmbedIV, then this value will be prepended to object.</param>
        /// <param name="enumModeDelete">Delete mode to use to delete objects.</param>
        /// <param name="enumModeIV">IV mode to omit or embed IV into file. Same mode used to encrypt file must be used to decrypt file.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<StorageFile> StorageFolderCompressEncryptAsync(StorageFolder storageFolderParent, StorageFolder storageFolderToEncypt, string stringFileExtensionAdd, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV, EnumModeDelete enumModeDelete, EnumModeIV enumModeIV, bool boolCheckIfLocked = true)
        {
            string stringFilenameCompressed = null;   // Initialize name of compressed temp file here since may need to cleanup in catch block if exception.
            try
            {
                if (boolCheckIfLocked)      // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(storageFolderToEncypt))
                    {
                        // Debug.WriteLine($"LibAES.StorageFolderCompressEncryptAsync(): {storageFolderToEncypt.Name} is locked.");
                        return null;
                    }
                }
                // Debug.WriteLine($"LibAES.StorageFolderCompressEncryptAsync(): {storageFolderToEncypt.Name} not locked.");
                stringFilenameCompressed = StringGuidTempFilename(stringFileExtensionAdd);     // Build name of compressed temp file.
                // Debug.WriteLine($"LibAES.StorageFolderCompressEncryptAsync(): stringFilenameCompressed={stringFilenameCompressed}");
                StorageFile storageFileCompressed = await storageFolderParent.CreateFileAsync(stringFilenameCompressed);
                if (storageFileCompressed != null)
                {
                    if (await LibZA.ZipArchiveCompressAsync(storageFileCompressed, storageFolderToEncypt, CompressionLevel.NoCompression))
                    {
                        // Debug.WriteLine($"LibAES.StorageFolderCompressEncryptAsync(): Compressed {storageFolderToEncypt.Name} into {storageFileCompressed.Name}");
                        // Encrypt compressed temp file using parameter values. Do not add another extension to compressed temp file so use string.Empty for parameter stringFileExtensionAdd.
                        StorageFile storageFileEncrypted = await StorageFileEncryptAsync(storageFolderParent, storageFileCompressed, string.Empty, cryptographicKeyPassword, iBufferIV, enumModeDelete, enumModeIV, false);
                        if (storageFileEncrypted != null)    // Name of this file same as storageFileCompressed.Name since did not add another extension. storageFileCompressed was deleted so no conflict.
                        {
                            // Debug.WriteLine($"LibAES.StorageFolderCompressEncryptAsync(): Encrypted compressed temp file to {storageFileEncrypted.Name}");
                            string stringFilenameEncrypted = $"{storageFolderToEncypt.Name}{stringFileExtensionAdd}";
                            // Encryption of zip file was successful so delete original folder.
                            // Next line is where exception is thrown if file is opened/in-use/locked since cannot delete original folder.
                            await IStorageItemDeleteAsync(storageFolderToEncypt, enumModeDelete);
                            // Rename resulting compressed and encrypted file to stringFilenameEncrypted. Calling method should check that exising file does not exist.
                            await storageFileEncrypted.RenameAsync(stringFilenameEncrypted);
                            return storageFileEncrypted;     // Success so return encrypted file that contains content of storageFolderToEncypt.
                        }
                        else
                        {
                            // Encryption of compressed temp file failed so clean up by deleting compressed temp file.
                            await IStorageItemDeleteAsync(storageFileCompressed, enumModeDelete);
                        }
                    }
                }
                // Debug.WriteLine($"LibAES.StorageFolderCompressEncryptAsync(): Return null since did not compress or encrypt {storageFolderToEncypt.Path}.");
                return null;    // Return null since did not compress or encrypt storageFolderToEncypt.
            }
            catch   // (Exception ex)
            {
                // Debug.WriteLine($"LibAES.StorageFolderCompressEncryptAsync(): Returned null since exception occurred. Type={ex.GetType()}");
                // Clean up by deleting compressed temp file if exists.
                if (stringFilenameCompressed != null)
                {
                    IStorageItem iStorageItem = await storageFolderParent.TryGetItemAsync(stringFilenameCompressed);
                    if (iStorageItem != null)   // Found object that has same name as compressed temp file.
                    {
                        await IStorageItemDeleteAsync(iStorageItem, enumModeDelete);
                        // Debug.WriteLine($"LibAES.StorageFolderCompressEncryptAsync(): Exception occurred so deleted compressed temp object: {iStorageItem.Name}");
                    }
                }
                return null;
                throw;
            }
        }

        /// <summary>
        /// Decrypt storageFileToDecrypt and then extract compressed content into folder named stringFoldernameDestination.
        /// If success return processed StorageFolder, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="storageFolderParent">Parent folder of storageFileToDecrypt that App has read/write access too.</param>
        /// <param name="storageFileToDecrypt">Decrypt this file and then extract compressed content into folder named stringFoldernameDestination</param>
        /// <param name="stringFoldernameDestination">Name of destination folder. Destination folder will be placed in storageFolderParent.</param>
        /// <param name="cryptographicKeyPassword">Password key to use to encrypt object.</param>
        /// <param name="iBufferIV">Initialization vector (IV) used to encrypt object. This value not used if enumModeIV=EnumModeIV.EmbedIV so can be null if so.</param>
        /// <param name="enumModeDelete">Delete mode to use to delete objects.</param>
        /// <param name="enumModeIV">IV mode to omit or embed IV into file. Same mode used to encrypt file must be used to decrypt file.</param>
        /// <param name="boolCheckIfLocked">If true then check if input object is locked. Default is true. This check should generally be done by 
        /// calling method so it can be handled there. If done there then this value can be false so check is skipped here. A locked object can 
        /// not be deleted causing exception, garbled output, and possible data loss!</param>
        /// <returns></returns>
        public static async Task<StorageFolder> StorageFileDecryptExtractAsync(StorageFolder storageFolderParent, StorageFile storageFileToDecrypt, string stringFoldernameDestination, CryptographicKey cryptographicKeyPassword, IBuffer iBufferIV, EnumModeDelete enumModeDelete, EnumModeIV enumModeIV, bool boolCheckIfLocked = true)
        {
            string stringFilenameBackup = null;         // Initialize name of backup temp file here since may need to cleanup in catch block if exception.
            try
            {
                if (boolCheckIfLocked)      // A locked object can not be deleted causing exception, garbled output, and possible data loss!
                {
                    if (await IStorageItemLockCheckAsync(storageFileToDecrypt))
                    {
                        // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): {storageFileToDecrypt.Name} is locked.");
                        return null;
                    }
                }
                // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): {storageFileToDecrypt.Name} is not locked");
                stringFilenameBackup = StringGuidTempFilename(storageFileToDecrypt.FileType);  // Build name of backup temp file that has same extension as storageFileToDecrypt.
                // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): stringFilenameBackup={stringFilenameBackup}");
                // Copy storageFileToDecrypt to backup file so original content saved until decryption and extraction processes complete successfully.
                StorageFile storageFileBackup = await storageFileToDecrypt.CopyAsync(storageFolderParent, stringFilenameBackup);
                if (storageFileBackup != null)
                {
                    // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): storageFileBackup.Name={storageFileBackup.Name}");
                    // Decrypt storageFileBackup. This will delete storageFileBackup and remove ending extension.
                    StorageFile storageFileDecrypted = await StorageFileDecryptAsync(storageFolderParent, storageFileBackup, cryptographicKeyPassword, iBufferIV, enumModeDelete, enumModeIV, false);
                    if (storageFileDecrypted != null)
                    {
                        // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): Decrypted temp backup file to {storageFileDecrypted.Path}");
                        // Calling method needs to ensure parameter stringFoldernameDestination does not exist or next line will throw exception.
                        StorageFolder storageFolderDestination = await storageFolderParent.CreateFolderAsync(stringFoldernameDestination);
                        if (storageFolderDestination != null)
                        {
                            if (await LibZA.ZipArchiveExtractAsync(storageFileDecrypted, storageFolderDestination))
                            {
                                // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): Extracted compressed content to {storageFolderDestination.Name}, enumModeDelete={enumModeDelete}");
                                await IStorageItemDeleteAsync(storageFileDecrypted, enumModeDelete);     // Delete decrypted temp file which is still archived.
                                await storageFileToDecrypt.DeleteAsync(StorageDeleteOption.PermanentDelete); // Delete original file. Use normal deletion since file is encrypted.
                                // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): Success! Returned encrypted and compressed content to {storageFolderDestination.Path}");
                                return storageFolderDestination;   // Return restored folder.
                            }
                            else
                            {
                                // Clean up since extraction failed. Need to delete storageFileDecrypted. storageFileBackup deleted during prior decryption process.
                                await IStorageItemDeleteAsync(storageFileDecrypted, enumModeDelete);
                            }
                        }
                    }
                    else
                    {
                        // Clean up since could not decrypt storageFileBackup. File is encrypted so delete normally.
                        await storageFileBackup.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                }
                // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): Return null since did not decrypt or extract {storageFileToDecrypt.Path}");
                return null;    // Return null since did not decrypt or extract storageFileToDecrypt.
            }
            catch //(Exception ex)
            {
                // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): Returned null since exception occurred. Type={ex.GetType()}");
                // Clean up by deleting two temp files if they still exists.
                if (stringFilenameBackup != null)
                {
                    IStorageItem iStorageItem = await storageFolderParent.TryGetItemAsync(stringFilenameBackup);
                    if (iStorageItem != null)
                    {
                        await IStorageItemDeleteAsync(iStorageItem, enumModeDelete);
                        // Debug.WriteLine($"LibAES.StorageFileDecryptExtractAsync(): Exception occurred so deleted {iStorageItem.Path}");
                    }
                }
                return null;
                throw;
            }
        }

    }
}