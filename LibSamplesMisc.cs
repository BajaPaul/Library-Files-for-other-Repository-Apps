using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

/*
 * File: \Libraries\LibSamplesMisc.cs.  This library contains a collection of various miscellaneous sample code methods.  
 * This file will need to be copied into project or added via 'Add as Link'.  This file likely will not compile to a Windows 
 * Runtime Component library.
 * 
 * Primary purpose of this library is a archive location to copy code samples too for learning purposes.  Most of libaray will 
 * not be applicable for day-to-day use.  But since effort was made to get it working it is shameful just to discard it.  Also,
 * it is a lost cause to have this content scattered in dozens of various project files never to be found again when needed.
 * 
 * Normally the namespace below would be proceeded by the project name but is ommitted so files can be shared between projects easily.
 */

namespace LibraryCoder.SamplesMisc
{
    /// <summary>
    /// LibUM = Shorthand for LibrarySamplesMisc.
    /// </summary>
    public static class LibSM
    {

        // Code to show a circular prgress ring indication App busy.
        // https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.ProgressRing
        //
        // XAML:
        //<ProgressRing x:Name="ProgressRingSI" Margin="16,8" RelativePanel.Below="TblkSISetupMsg2" RelativePanel.AlignHorizontalCenterWith="RectSILayoutCenter" MinWidth="31" MinHeight="30" FocusVisualPrimaryBrush="Red"/>
        //<ToggleSwitch Header = "Toggle Switch Example" OffContent="Do work" OnContent="Working" Toggled="ToggleSwitch_Toggled" RelativePanel.Below="ButSINotUsed" RelativePanel.AlignHorizontalCenterWith="RectSILayoutCenter" Margin="16,8,16,24" HorizontalContentAlignment="Center" HorizontalAlignment="Center"/>
        //
        /// <summary>
        /// This example shows how to set the IsActive property of a ProgressRing in code. 
        /// A ToggleSwitch is used to turn ProgressRing control on or off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
                ProgressRingShow(toggleSwitch.IsOn);
        }
        // ......
        private static void ProgressRingShow(bool boolShow)
        {
            if (boolShow)
            {
                //ProgressRingSI.IsActive = true;
                //ProgressRingSI.Visibility = Visibility.Visible;
            }
            else
            {
                //ProgressRingSI.IsActive = false;
                //ProgressRingSI.Visibility = Visibility.Collapsed;
            }
        }


        // Timer code using Stopwatch!  Output to TextBlock if want to use 'timeSpanElapsed' in release code.
        //
        // Global timer variable.
        // private Stopwatch stopWatchTimer = new Stopwatch();
        //
        // Global elapsed time variable.
        // private TimeSpan timeSpanElapsed;
        //
        // Timer start code.
        // stopWatchTimer.Reset();
        // stopWatchTimer.Start();             // Start the timer.
        // timeSpanElapsed = TimeSpan.Zero;    // Zero in case any access to value before timer stops.
        //
        // Timer stop code.
        // stopWatchTimer.Stop();
        // timeSpanElapsed = stopWatchTimer.Elapsed;
        // Debug.WriteLine(String.Format("ProgressBar ran for {0:N2} seconds.", timeSpanElapsed.TotalSeconds));


        // This method doesn't have much use other than for debugging.  SAVE since excellent example how to use generic enumeration in methods.
        /// <summary>
        /// Iterate through an enumeration and return string showing all values including 'DisplayAttribute'.  TEnum is generic.
        /// Example: string enumOutput = LibUM.EnumShowValues<ConversionType>(true); where 'ConversionType' is an enum.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration that may contain 'DisplayAttribute'.  Can be generic.</typeparam>
        /// <param name="sort">Sort enum list if true.  Default is false.</param>
        /// <returns></returns>
        public static string EnumShowValues<TEnum>(bool sort = false) where TEnum : IComparable, IFormattable, IConvertible
        {
            string returnVal = string.Empty;
            string[] enumNames = Enum.GetNames(typeof(TEnum));      // Get an arrary of name values from a enum.
            // Following debug code shows values saved in 'enumNames'.
            //Debug.WriteLine("");
            //foreach (string item in enumNames)
            //    Debug.WriteLine(item
            if (sort)
            {
                Array.Sort(enumNames);
                returnVal += string.Format("\nEnumShowValues: Sorted items of enum '{0}': \nOutput order: Integer, Value, DisplayAttribute.\n", typeof(TEnum).Name);      // Header line if sorted.
            }
            else
            {
                returnVal += string.Format("\nEnumShowValues: Unsorted items of enum '{0}': \nOutput order: Integer, Value, DisplayAttribute.", typeof(TEnum).Name);    // Header line if unsorted.
            }
            foreach (string item in enumNames)
            {
                TEnum enumName = (TEnum)Enum.Parse(typeof(TEnum), item);    // Convert the name back into enum.  This will throw exception if conversion fails!
                returnVal += string.Format("\n{0,2:D}, {0}, {1}.", enumName, GetEnumDescription(enumName));
            }
            return returnVal;
        }

        private static object GetEnumDescription<TEnum>(TEnum enumName) where TEnum : IComparable, IFormattable, IConvertible
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sample of GetInvalidFileNameChars() and GetInvalidPathChars() to retrieve array of invalid file and path characters.
        /// More info at: https://msdn.microsoft.com/en-us/library/system.io.path.getinvalidfilenamechars(v=vs.110).aspx
        /// </summary>
        public static void FileNameGetInvalidCharsSample()
        {
            // Get a list of invalid path characters.
            char[] invalidPathChars = Path.GetInvalidPathChars();

            Debug.WriteLine("The following characters are invalid in a path:");
            ShowChars(invalidPathChars);
            Debug.WriteLine("");

            // Get a list of invalid file characters.
            char[] invalidFileChars = Path.GetInvalidFileNameChars();

            Debug.WriteLine("The following characters are invalid in a filename:");
            ShowChars(invalidFileChars);
        }

        public static void ShowChars(char[] arrayChar)
        {
            Debug.WriteLine("Char\tHex Value");
            // Display each invalid character to the console.
            foreach (char someChar in arrayChar)
            {
                if (char.IsWhiteSpace(someChar))
                {
                    Debug.WriteLine(",\t{0:X4}", (int)someChar);
                }
                else
                {
                    Debug.WriteLine("{0:c},\t{1:X4}", someChar, (int)someChar);
                }
            }
        }

        /// <summary>
        /// Sample of IndexOfAny() use.  IndexOfAny() returns zero-based index position of the first occurrence in string that
        /// matches any character in array, or returns -1 if no match found.
        /// More at: https://msdn.microsoft.com/en-us/library/11w09h50(v=vs.110).aspx
        /// </summary>
        public static void IndexOfAnySample()
        {
            char[] chars = { 'a', 'e', 'i', 'o', 'u', 'y', 'A', 'E', 'I', 'O', 'U', 'Y' };
            string s = "The long and winding road...";
            Debug.WriteLine("The first vowel in \n   {0}\nis found at position {1}", s, s.IndexOfAny(chars) + 1);
        }

        /// <summary>
        /// Monitor changes in a folder via setting up a query and an event that fires when any changes in folder have been made.
        /// More at: https://docs.microsoft.com/en-us/uwp/api/windows.storage.search.storagefolderqueryresult#Windows_Storage_Search_StorageFolderQueryResult_ContentsChanged
        /// </summary>
        private static async void MonitorFolder()
        {
            // Sample that works! http://lunarfrog.com/blog/filesystem-change-notifications
            // var query = mainPage.storageFolderLocker.CreateFileQuery();      // CreateFileQuery() without options fires twice, could be a bug!

            QueryOptions queryOptions = new QueryOptions { FileTypeFilter = { "*" } };    // Filter type does not matter since fires on any change within folder.
            StorageFileQueryResult query = ApplicationData.Current.LocalFolder.CreateFileQueryWithOptions(queryOptions);

            query.ContentsChanged += QueryContentsChanged;      // subscribe to event.  ContentsChanged fires on any change within folder regardless of filter.

            IReadOnlyList<StorageFile> listStorageFileList = await query.GetFilesAsync();
        }

        /// <summary>
        /// Handle event fired by MonitorFolder(). This sample just show list of files in folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static async void QueryContentsChanged(IStorageQueryResultBase sender, object args)
        {
            // Debug code!
            Debug.WriteLine("");
            // Debug.WriteLine("sender={0}, {1}, {2}", sender.GetItemCountAsync(), sender.GetType(), sender.GetCurrentQueryOptions());
            // Debug.WriteLine("args={0}", args);
            IReadOnlyList<StorageFile> storageFileList = await ApplicationData.Current.LocalFolder.GetFilesAsync();   // Get list of files in folder.
            if (storageFileList != null)
            {
                foreach (StorageFile storageFile in storageFileList)
                    Debug.WriteLine("File: {0}", storageFile.Name);
            }

            // Useful real code below!
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //ButCreateFolderNoArchiveUpdateAsync();      // TODO: This will turn NoZip buttons back on after all have been disabled for a process.
            });
            Debug.WriteLine("QueryContentsChanged event fired.");
        }

        /// <summary>
        /// Monitor changes in a folder via setting up a query and an event that fires when any changes in folder have been made.
        /// </summary>
        private static async void MonitorFolderTwo()
        {
            // Web sample that works! http://lunarfrog.com/blog/filesystem-change-notifications
            QueryOptions queryOptions = new QueryOptions { FileTypeFilter = { "*" } };    // Filter type does not matter since fires on any change within folder.
            StorageFolderQueryResult query = ApplicationData.Current.LocalFolder.CreateFolderQueryWithOptions(queryOptions);
            query.ContentsChanged += QueryContentsChangedTwo;      // subscribe to event.  ContentsChanged fires on any change within folder regardless of filter.
            IReadOnlyList<StorageFolder> listStorageFolders = await query.GetFoldersAsync();
        }

        /// <summary>
        /// Handle event fired by MonitorFolderTwo(). This sample just shows list of files and folders in a folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static async void QueryContentsChangedTwo(IStorageQueryResultBase sender, object args)
        {
            Debug.WriteLine("");
            IReadOnlyList<StorageFile> storageFileList = await ApplicationData.Current.LocalFolder.GetFilesAsync();        // Get list of files in folder.
            if (storageFileList != null)
            {
                foreach (StorageFile storageFile in storageFileList)
                    Debug.WriteLine("File: " + storageFile.Name);
            }
            IReadOnlyList<StorageFolder> listStorageFolder = await ApplicationData.Current.LocalFolder.GetFoldersAsync();   // Get list of folders in folder.
            if (storageFileList != null)
            {
                foreach (StorageFolder storageFolder in listStorageFolder)
                    Debug.WriteLine("Folder: " + storageFolder.Name);
            }
        }


        /// <summary>
        /// This sample illustrates three different procedures to write and read text from files. No error checking is done.
        /// </summary>
        /// <returns></returns>
        private static async Task FileCreateWriteReadAsync()
        {
            // This sample code is from: https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-reading-and-writing-files

            // Creating File:

            // Create sample files; replace if exists.
            Windows.Storage.StorageFolder storageFolderApp = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFileWrite1 = await storageFolderApp.CreateFileAsync("Sample1.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            Windows.Storage.StorageFile sampleFileWrite2 = await storageFolderApp.CreateFileAsync("Sample2.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            Windows.Storage.StorageFile sampleFileWrite3 = await storageFolderApp.CreateFileAsync("Sample3.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);

            // Write to File:

            // Get the files.
            sampleFileWrite1 = await storageFolderApp.GetFileAsync("Sample1.txt");
            sampleFileWrite2 = await storageFolderApp.GetFileAsync("Sample2.txt");
            sampleFileWrite3 = await storageFolderApp.GetFileAsync("Sample3.txt");
            // Write text to file.
            await Windows.Storage.FileIO.WriteTextAsync(sampleFileWrite1, "He is swift as a shadow.");

            // Write bytes to file using buffer (2 steps). First, call ConvertStringToBinary to get buffer of the bytes from a string. Then write bytes from buffer to file.
            IBuffer buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary("What fools these mortals are.", Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
            await Windows.Storage.FileIO.WriteBufferAsync(sampleFileWrite2, buffer);

            // Write text to file using a stream (4 steps). First, open file by calling StorageFile.OpenAsync method. It returns stream of file's content.
            IRandomAccessStream streamWrite = await sampleFileWrite3.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            // Second, get output stream by calling the GetOutputStreamAt method from the stream. Put in using statement to manage the output stream's lifetime.
            using (IOutputStream outputStream = streamWrite.GetOutputStreamAt(0))
            {
                // Third, add this code within the existing using statement to write to the output stream by creating new DataWriter object and calling the DataWriter.WriteString method.
                using (DataWriter dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                {
                    dataWriter.WriteString("DataWriter has methods to write to various types.  ");
                    dataWriter.WriteString("One such type is DataTimeOffset.");
                    // Fourth, add this code (within the inner using statement) to save the text to your file with StoreAsync and close the stream with FlushAsync.
                    await dataWriter.StoreAsync();
                    await outputStream.FlushAsync();
                }
            }
            streamWrite.Dispose(); // Or use the stream variable (see previous code snippet) with a using statement as well.

            // Read from File:

            // // Get the files created above.
            Windows.Storage.StorageFile sampleFileRead1 = await storageFolderApp.GetFileAsync("Sample1.txt");
            Windows.Storage.StorageFile sampleFileRead2 = await storageFolderApp.GetFileAsync("Sample2.txt");
            Windows.Storage.StorageFile sampleFileRead3 = await storageFolderApp.GetFileAsync("Sample3.txt");

            // Read text from file by calling ReadTextAsync method.
            string stringFileText1 = await Windows.Storage.FileIO.ReadTextAsync(sampleFileRead1);
            Debug.WriteLine($"FileCreateWriteReadAsync(): ReadTextAsync(): stringFileText1={stringFileText1}");

            // Reading text from file using buffer (2 steps). First, call ReadBufferAsync method.
            IBuffer bufferRead = await Windows.Storage.FileIO.ReadBufferAsync(sampleFileRead2);
            // Second, use a DataReader object to read first the length of the buffer and then its contents.
            using (DataReader dataReader = Windows.Storage.Streams.DataReader.FromBuffer(bufferRead))
            {
                string stringFileText2 = dataReader.ReadString(bufferRead.Length);
                Debug.WriteLine($"FileCreateWriteReadAsync(): ReadBufferAsync() with DataReader: stringFileText2={stringFileText2}");
            }

            // Reading text from a file by using a stream (4 steps). First, open stream for file by calling StorageFile.OpenAsync method. It returns stream of file's content.
            IRandomAccessStream streamRead = await sampleFileRead3.OpenAsync(Windows.Storage.FileAccessMode.Read);
            // Second, get size of stream to use later.
            ulong size = streamRead.Size;
            // Third, get input stream by calling GetInputStreamAt method. Put this in using statement to manage stream's lifetime. Specify 0 when you call GetInputStreamAt to set position to beginning of stream.
            using (IInputStream inputStream = streamRead.GetInputStreamAt(0))
            {
                // Fourth, add this code within existing using statement to get DataReader object on stream then read text by calling DataReader.LoadAsync and DataReader.ReadString.
                using (DataReader dataReader = new Windows.Storage.Streams.DataReader(inputStream))
                {
                    uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                    string stringFileText3 = dataReader.ReadString(numBytesLoaded);
                    Debug.WriteLine($"FileCreateWriteReadAsync(): OpenAsync() with DataReader: stringFileText3={stringFileText3}");
                }
            }
            streamRead.Dispose(); // Or use the stream variable (see previous code snippet) with a using statement as well.
        }










    }   // End of class LibSM.


    /*
     * SafeFileEnumerator class methods prevents loosing the entire enumeration when you simply don't have access to a 
     * single file so you are still able to iterate through the files that you can access.
     * 
     * from: https://stackoverflow.com/questions/13954630/how-to-handle-unauthorizedaccessexception-when-attempting-to-add-files-from-loca
     * 
     *  Sample Usage:
     *  
     *  foreach(string fileName in SafeFileEnumerator.EnumerateFiles(folderPath, "*" + extension, SearchOption.AllDirectories))
     *  {
     *     //Do something with filename, store into an array or whatever you want to do.
     *  }
     * 
     */

    public static class SafeFileEnumerator
    {
        public static IEnumerable<string> EnumerateDirectories(string parentDirectory, string searchPattern, SearchOption searchOpt)
        {
            try
            {
                var directories = Enumerable.Empty<string>();
                if (searchOpt == SearchOption.AllDirectories)
                {
                    directories = Directory.EnumerateDirectories(parentDirectory).SelectMany(x => EnumerateDirectories(x, searchPattern, searchOpt));
                }
                return directories.Concat(Directory.EnumerateDirectories(parentDirectory, searchPattern));
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<string>();
            }
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOpt)
        {
            try
            {
                var dirFiles = Enumerable.Empty<string>();
                if (searchOpt == SearchOption.AllDirectories)
                {
                    dirFiles = Directory.EnumerateDirectories(path).SelectMany(x => EnumerateFiles(x, searchPattern, searchOpt));
                }
                return dirFiles.Concat(Directory.EnumerateFiles(path, searchPattern));
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<string>();
            }
        }




    }

    // This is saved sample XAML code showing VisualState trigers so not added as a class here!!!

    // Decided to use WindowsStateTriggers from GitHub but save following code anyway.  Decided this level of detail not necessary.
    // More at: https://github.com/dotMorten/WindowsStateTriggers

    // Following class creates XAML visual state trigger for device families, "Windows.Desktop", "Windows.Mobile", "Windows.Xbox",...
    // XAML code sample follows.  The class below is referenced by this XAML code.
    // More at: https://msdn.microsoft.com/windows/uwp/input-and-devices/designing-for-tv#custom-visual-state-trigger-for-xbox

    /*
    For this XAML code to work will need to add following code line to end of Page definition in XAML file
    xmlns:triggers="using:DeviceFamilyTrigger"
    //
    Then add following XAML code into Grid, StackPanel, or RelativePanel code in XAML file.
    //
    <VisualStateManager.VisualStateGroups>
        <VisualStateGroup>
            <VisualState>
                <VisualState.StateTriggers>
                    <triggers:DeviceFamilyTrigger DeviceFamily = "Windows.Xbox" />
                </ VisualState.StateTriggers >
                < VisualState.Setters >
                    < Setter Target="RootSplitView.OpenPaneLength" Value="368"/>
                    <Setter Target = "RootSplitView.CompactPaneLength" Value="96"/>
                    <Setter Target = "NavMenuList.Margin" Value="0,75,0,27"/>
                    <Setter Target = "Frame.Margin" Value="0,27,48,27"/>
                    <Setter Target = "NavMenuList.ItemContainerStyle" Value="{StaticResource NavMenuItemContainerXboxStyle}"/>
                </VisualState.Setters>
            </VisualState>
        </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>     

    /// <summary>
    /// This class creates XAML visual state trigger for device families such as "Windows.Desktop", "Windows.Mobile", "Windows.Xbox".
    /// </summary>
    class DeviceFamilyTrigger : StateTriggerBase
    {
        private string _currentDeviceFamily, _queriedDeviceFamily;

        public string DeviceFamily
        {
            get
            {
                return _queriedDeviceFamily;
            }

            set
            {
                _queriedDeviceFamily = value;
                _currentDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
                SetActive(_queriedDeviceFamily == _currentDeviceFamily);
            }
        }
    }*/

}
