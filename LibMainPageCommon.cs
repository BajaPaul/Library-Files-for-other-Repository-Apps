using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.Storage;
using Windows.System;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

/* 
 * File: \Libraries\LibMainPageCommon.  This library provides common public variables and methods previously defined in MainPage.cs
 * and used in various Applications. Consolidated code to this library simplify maintenance.
 * 
 * This file will need to be copied into project or added via 'Add as Link'. Normally the namespace below would be proceeded by the
 * project name but is ommitted so files can be shared between projects easily. Common namespace LibraryCoder is used instead.
 */

/// <summary>
/// Enum separator to use when joining strings used in JoinListString().
/// </summary>
public enum EnumStringSeparator { None, OneSpace, TwoSpaces, OneNewline, TwoNewlines };

namespace LibraryCoder.MainPageCommon
{
    /// <summary>
    /// LibMPC = Shorthand for LibMainPageCommon.
    /// </summary>
    public static class LibMPC
    {
        /************************************* Public Variables *************************************/

        // Send all application TextBlock output messages using colors and methods defined below. 
        // Currently set colors intended for Dark Theme.  Dark theme is set in App.xaml file.

        /// <summary>
        /// Bright color to use throughout application.
        /// White appears good with SystemControlAcrylicWindowBrush background.
        /// </summary>
        public static SolidColorBrush colorBright = new SolidColorBrush(Colors.White);

        /// <summary>
        /// Normal color to use throughout application.
        /// Yellow appears good with SystemControlAcrylicWindowBrush background.
        /// </summary>
        public static SolidColorBrush colorNormal = new SolidColorBrush(Colors.Yellow);

        /// <summary>
        /// Success color to use throughout application.
        /// LightGreen appears good with SystemControlAcrylicWindowBrush background.
        /// </summary>
        public static SolidColorBrush colorSuccess = new SolidColorBrush(Colors.LightGreen);

        /// <summary>
        /// Error color to use throughout application.
        /// LightSalmon appears good with SystemControlAcrylicWindowBrush background.
        /// </summary>
        public static SolidColorBrush colorError = new SolidColorBrush(Colors.LightSalmon);

        /************************************* Public Methods *************************************/

        /// <summary>
        /// Output stringMsgBright to textBlockOutput using bright color.
        /// </summary>
        /// <param name="textBlockOutput">Output message to this TextBlock.</param>
        /// <param name="stringMsgBright">Message to output using bright color.</param>
        public static void OutputMsgBright(TextBlock textBlockOutput, string stringMsgBright)
        {
            textBlockOutput.Foreground = colorBright;
            textBlockOutput.Text = stringMsgBright;
        }

        /// <summary>
        /// Output stringMsgNormal to textBlockOutput using normal color.
        /// </summary>
        /// <param name="textBlockOutput">Output message to this TextBlock.</param>
        /// <param name="stringMsgNormal">Message to output using normal color.</param>
        public static void OutputMsgNormal(TextBlock textBlockOutput, string stringMsgNormal)
        {
            textBlockOutput.Foreground = colorNormal;
            textBlockOutput.Text = stringMsgNormal;
        }

        /// <summary>
        /// Output stringMsgSuccess to textBlockOutput using success color.
        /// </summary>
        /// <param name="textBlockOutput">Output message to this TextBlock.</param>
        /// <param name="stringMsgSuccess">Message to output using success color.</param>
        public static void OutputMsgSuccess(TextBlock textBlockOutput, string stringMsgSuccess)
        {
            textBlockOutput.Foreground = colorSuccess;
            textBlockOutput.Text = stringMsgSuccess;
        }

        /// <summary>
        /// Output stringMsgError to textBlockOutput using error color.
        /// </summary>
        /// <param name="textBlockOutput">Output message to this TextBlock.</param>
        /// <param name="stringMsgError">Message to output using error color.</param>
        public static void OutputMsgError(TextBlock textBlockOutput, string stringMsgError)
        {
            textBlockOutput.Foreground = colorError;
            textBlockOutput.Text = stringMsgError;
        }

        /// <summary>
        /// Change visibility of parameter button. Make button visible if boolVisible is true, otherwise collapse button.
        /// </summary>
        /// <param name="button">Button to make visible or collapse.</param>
        /// <param name="boolVisible">Make button visible if true, collpase button otherwise.</param>
        public static void ButtonVisibility(Button button, bool boolVisible)
        {
            if (boolVisible)
                button.Visibility = Visibility.Visible;
            else
                button.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Enable or disable parameter button. Enable button if boolIsEnabled is true, otherwise disable button.
        /// </summary>
        /// <param name="button">Button to make visible or collapse.</param>
        /// <param name="boolIsEnabled">Enable button if true, otherwise disable button.</param>
        public static void ButtonIsEnabled(Button button, bool boolIsEnabled)
        {
            button.IsEnabled = boolIsEnabled;
        }

        /// <summary>
        /// If device is Xbox then disable parameter buttonEmail since email is not supported on Xbox.
        /// </summary>
        /// <param name="buttonEmail">Button that opens email from User to developer.</param>
        /// <returns></returns>
        public static void ButtonEmailXboxDisable(Button buttonEmail)
        {
            // More info at: https://docs.microsoft.com/en-us/uwp/schemas/appxpackage/uapmanifestschema/element-targetdevicefamily
            string stringDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            // Debug.WriteLine($"LibMPC.ButtonEmailXboxDisable(): stringDeviceFamily={stringDeviceFamily}");
            if (stringDeviceFamily.Equals("Windows.Xbox"))
                ButtonIsEnabled(buttonEmail, false);
        }

        /// <summary>
        /// Open User web browser to hyperlink defined in button tag.
        /// </summary>
        /// <param name="buttonHyperlinkTag">Button with tag containing hyperlink.</param>
        /// <returns></returns>
        public static async Task ButtonHyperlinkLaunchAsync(Button buttonHyperlinkTag)
        {
            string stringTag = buttonHyperlinkTag.Tag.ToString();
            // Debug.WriteLine($"LibMPC.HyperlinkLaunch(): stringTag={stringTag}");
            if (stringTag.Length > 0)
                await Launcher.LaunchUriAsync(new Uri(stringTag));
        }

        // More about FrameworkElement.Width Property: https://msdn.microsoft.com/en-us/library/system.windows.frameworkelement.width(v=vs.110).aspx
        /// <summary>
        /// Set MinWidth and MinHeight values of Buttons on page to same value based on largest rendered width and height found.
        /// Call from Page_Loaded event of a rendered page or sizes will not set properly.
        /// </summary>
        /// <param name="listButtonsOnPage">List of Buttons on page to size.</param>
        public static void SizePageButtons(List<Button> listButtonsOnPage)
        {
            // Set a minimum sizes so Buttons do not disappear if retrieved value is 0.
            double doubleMinWidth = 10d;
            double doubleMinHeight = 10d;
            foreach (Button button in listButtonsOnPage)
            {
                // Debug.WriteLine($"LibMPC.SizePageButtons(): Found {button.Name}, ActualWidth={button.ActualWidth}, ActualHeight={button.ActualHeight}");
                if (button.ActualWidth > doubleMinWidth)
                    doubleMinWidth = button.ActualWidth;
                if (button.ActualHeight > doubleMinHeight)
                    doubleMinHeight = button.ActualHeight;
            }
            // Debug.WriteLine($"LibMPC.SizePageButtons(): Max width found={doubleMinWidth}, max height found={doubleMinHeight}");
            foreach (Button button in listButtonsOnPage)
            {
                button.MinWidth = doubleMinWidth;
                button.MinHeight = doubleMinHeight;
                // Debug.WriteLine($LibMPC.SizePageButtons(): Set {button.Name} MinWidth={button.MinWidth}, MinHeight={button.MinHeight}");
            }
        }

        /// <summary>
        /// Set MinWidth and MinHeight values of TextBoxes on page to same value based on largest rendered width and height found.
        /// Call from Page_Loaded event of a rendered page or sizes will not set properly.
        /// </summary>
        /// <param name="listTextBoxesOnPage">List of TextBoxes on page to size.</param>
        public static void SizePageTextBoxes(List<TextBox> listTextBoxesOnPage)
        {
            // Set a minimum size so TextBoxes do not disappear if retrieved value is 0.
            double doubleMinWidth = 10d;
            double doubleMinHeight = 10d;
            foreach (TextBox textBox in listTextBoxesOnPage)
            {
                // Debug.WriteLine($"LibMPC.SizePageTextBoxes(): Found {textBox.Name}, ActualWidth={textBox.ActualWidth}, ActualHeight={textBox.ActualHeight}");
                if (textBox.ActualWidth > doubleMinWidth)
                    doubleMinWidth = textBox.ActualWidth;
                if (textBox.ActualHeight > doubleMinHeight)
                    doubleMinHeight = textBox.ActualHeight;
            }
            //Debug.WriteLine($"LibMPC.SizePageTextBoxes(): Max width found={doubleMinWidth}, max height found={doubleMinHeight}");
            foreach (TextBox textBox in listTextBoxesOnPage)
            {
                textBox.MinWidth = doubleMinWidth;
                textBox.MinHeight = doubleMinHeight;
                //Debug.WriteLine($"LibMPCSizePageTextBoxes(): Set {textBox.Name} MinWidth={textBox.MinWidth}, MinHeight={textBox.MinHeight}");
            }
        }

        /// <summary>
        /// Set MinWidth and MinHeight values of ToggleSwitches on page to same value based on largest rendered width and height found.
        /// Call from Page_Loaded event of a rendered page or sizes will not set properly.
        /// </summary>
        /// <param name="listToggleSwitchesOnPage">List of ToggleSwitches on page to size.</param>
        public static void SizePageToggleSwitches(List<ToggleSwitch> listToggleSwitchesOnPage)
        {
            // Set a minimum sizes so Buttons do not disappear if retrieved value is 0.
            double doubleMinWidth = 10d;
            double doubleMinHeight = 10d;
            foreach (ToggleSwitch toggleSwitch in listToggleSwitchesOnPage)
            {
                // Debug.WriteLine($"LibMPC.SizePageToggleSwitches(): Found {toggleSwitch.Name}, ActualWidth={toggleSwitch.ActualWidth}, ActualHeight={toggleSwitch.ActualHeight}");
                if (toggleSwitch.ActualWidth > doubleMinWidth)
                    doubleMinWidth = toggleSwitch.ActualWidth;
                if (toggleSwitch.ActualHeight > doubleMinHeight)
                    doubleMinHeight = toggleSwitch.ActualHeight;
            }
            // Debug.WriteLine($"LibMPC.SizePageToggleSwitches(): Max width found={doubleMinWidth}, max height found={doubleMinHeight}");
            foreach (ToggleSwitch toggleSwitch in listToggleSwitchesOnPage)
            {
                toggleSwitch.MinWidth = doubleMinWidth;
                toggleSwitch.MinHeight = doubleMinHeight;
                // Debug.WriteLine($"LibMPC.SizePageToggleSwitches(): Set {toggleSwitch.Name} MinWidth={toggleSwitch.MinWidth}, MinHeight={toggleSwitch.MinHeight}");
            }
        }

        // More info about following method at: https://docs.microsoft.com/en-us/windows/uwp/design/style/acrylic
        /// <summary>
        /// Replace default UWP title bar with Grid.Row="0" and set background of caption buttons to transparent.
        /// Items in Grid.Row="0" will no longer be accessible via mouse clicks without considerable trickery.
        /// Background of Grid.Row="0" must be transparent to allow Page Background to show through.
        /// </summary>
        public static void CustomizeAppTitleBar()
        {
            // Note: True/false setting in next line will persist on following App starts.  May need to toggle back and forth to see changes.
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar applicationViewTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            applicationViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            applicationViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        /// <summary>
        /// Turn parameter scrollViewer on and initialize and set settings accordingly.
        /// Only parameters that need to be changed need to be entered.
        /// </summary>
        /// <param name="scrollViewer">ScrollViewer to turn on.</param>
        /// <param name="horz"></param>
        /// <param name="vert"></param>
        /// <param name="horzVis"></param>
        /// <param name="vertVis"></param>
        /// <param name="zoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="minZoom"></param>
        public static void ScrollViewerOn(ScrollViewer scrollViewer, ScrollMode horz = ScrollMode.Auto, ScrollMode vert = ScrollMode.Auto, ScrollBarVisibility horzVis = ScrollBarVisibility.Disabled,
            ScrollBarVisibility vertVis = ScrollBarVisibility.Visible, ZoomMode zoom = ZoomMode.Disabled, float maxZoom = 2.0f, float minZoom = 0.5f)
        {
            scrollViewer.HorizontalScrollMode = horz;
            scrollViewer.VerticalScrollMode = vert;
            scrollViewer.HorizontalScrollBarVisibility = horzVis;
            scrollViewer.VerticalScrollBarVisibility = vertVis;
            scrollViewer.ZoomMode = zoom;
            scrollViewer.MaxZoomFactor = maxZoom;
            scrollViewer.MinZoomFactor = minZoom;
        }

        /// <summary>
        /// Turn parameter scrollViewer off and reset settings accordingly.
        /// </summary>
        /// <param name="scrollViewer">ScrollViewer to turn off.</param>
        public static void ScrollViewerOff(ScrollViewer scrollViewer)
        {
            scrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
            scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.ChangeView(0d, 0d, 1.0f);      // Reset zoom factor before navigating to new page.
            scrollViewer.MaxZoomFactor = 2.0f;
            scrollViewer.MinZoomFactor = 0.5f;
            scrollViewer.ZoomMode = ZoomMode.Disabled;
        }

        /// <summary>
        /// Launch FileExplorer from App. Optionally, select specific folder or file when FileExplorer opens.
        /// </summary>
        /// <param name="storageFolderLocation">Folder location where FileExplorer opens at.</param>
        /// <param name="stringStorageItemName">Optional folder or file to select when FileExplorer opens. Default is null.</param>
        /// <returns></returns>
        public static async Task LaunchFileExplorerAsync(StorageFolder storageFolderLocation, string stringStorageItemName = null)
        {
            if (storageFolderLocation != null)
            {
                if (stringStorageItemName != null)
                {
                    FolderLauncherOptions folderLauncherOptions = new FolderLauncherOptions();
                    IStorageItem iStorageItem = await storageFolderLocation.TryGetItemAsync(stringStorageItemName);
                    if (iStorageItem != null)   // Item found.
                    {
                        folderLauncherOptions.ItemsToSelect.Add(iStorageItem);
                        await Launcher.LaunchFolderAsync(storageFolderLocation, folderLauncherOptions);
                        return;
                    }
                }
                await Launcher.LaunchFolderAsync(storageFolderLocation);
            }
        }

        /// <summary>
        /// Check if applicationDataContainer contains stringDataStoreBool. Return true if stringDataStoreBool is true, return false otherwise.
        /// If stringDataStoreBool not found in applicationDataContainer then create entry and set it to false and return false.
        /// </summary>
        /// <param name="applicationDataContainer">Container of application settings.</param>
        /// <param name="stringDataStoreBool">Data store string that represents a bool.</param>
        /// <returns></returns>
        public static bool DataStoreStringToBool(ApplicationDataContainer applicationDataContainer, string stringDataStoreBool)
        {
            if (applicationDataContainer.Values.ContainsKey(stringDataStoreBool))
            {
                object objectBool = applicationDataContainer.Values[stringDataStoreBool];
                if (objectBool.Equals(true))
                {
                    // Debug.WriteLine($"LibMPC.DataStoreStringToBool(): Found data store value {stringDataStoreBool}={objectBool}, so returned true");
                    return true;    // Data store value found and was true so return true.
                }
                else
                {
                    // Debug.WriteLine($"LibMPC.DataStoreStringToBool(): Found data store value {stringDataStoreBool}={objectBool}, so returned false");
                    return false;   // Data store value found and was false so return false.
                }
            }
            else
            {
                // Data store value was not found so save false to data store and return false.
                applicationDataContainer.Values[stringDataStoreBool] = false;
                // object objectBoolDebug = applicationDataContainer.Values[stringDataStoreBool];  // Comment out this debug code used in next Debug.WriteLine().
                // Debug.WriteLine($"LibMPC.DataStoreStringToBool(): Did not find data store value {stringDataStoreBool} so it was set to {objectBoolDebug}, so returned false");
                return false;
            }
        }

        /// <summary>
        /// Check if applicationDataContainer contains stringDataStoreString. Return string if stringDataStoreString is valid string.
        /// If stringDataStoreString not found in applicationDataContainer then create entry and set it to "" and return string.Empty.
        /// </summary>
        /// <param name="applicationDataContainer">Container of application settings.</param>
        /// <param name="stringDataStoreString">Data store string that represents a string.</param>
        /// <returns></returns>
        public static string DataStoreStringToString(ApplicationDataContainer applicationDataContainer, string stringDataStoreString)
        {
            if (applicationDataContainer.Values.ContainsKey(stringDataStoreString))
            {
                object objectString = applicationDataContainer.Values[stringDataStoreString];
                // Debug.WriteLine($"LibMPC.DataStoreStringToString(): Found data store value so returned {stringDataStoreString}={objectString}");
                return objectString.ToString();
            }
            else
            {
                // Data store value was not found so save 0 to data store and return 0.
                // Debug.WriteLine($"LibMPC.DataStoreStringToString(): Did not find data store value {stringDataStoreString} so returned string.Empty");
                applicationDataContainer.Values[stringDataStoreString] = string.Empty;
                return string.Empty;
            }
        }

        /// <summary>
        /// Check if applicationDataContainer contains stringDataStoreInt. Return int if stringDataStoreInt is valid int, otherwise throw exception.
        /// If stringDataStoreInt not found in applicationDataContainer then create entry and set it to "0" and return 0.
        /// </summary>
        /// <param name="applicationDataContainer">Container of application settings.</param>
        /// <param name="stringDataStoreInt">Data store string that represents a int.</param>
        /// <returns></returns>
        public static int DataStoreStringToInt(ApplicationDataContainer applicationDataContainer, string stringDataStoreInt)
        {
            if (applicationDataContainer.Values.ContainsKey(stringDataStoreInt))
            {
                object objectInt = applicationDataContainer.Values[stringDataStoreInt];
                if (!int.TryParse(objectInt.ToString(), out int intDataStore))
                {
                    // Debug.WriteLine($"LibMPC.DataStoreStringToInt(): Found data store value {objectInt} but not valid int.");
                    throw new InvalidOperationException($"LibMPC.DataStoreStringToInt(): Found data store value {objectInt} but not valid int.");
                }
                // Debug.WriteLine($"LibMPC.DataStoreStringToInt(): Found data store value and it was valid int so returned {stringDataStoreInt}={intDataStore}");
                return intDataStore;
            }
            else
            {
                // Data store value was not found so save 0 to data store and return 0.
                // Debug.WriteLine($"LibMPC.DataStoreStringToInt(): Did not find data store value {stringDataStoreInt} so returned 0");
                applicationDataContainer.Values[stringDataStoreInt] = "0";
                return 0;
            }
        }

        /// <summary>
        /// Check if applicationDataContainer contains stringDataStoreDouble. Return double if stringDataStoreDouble is valid double, otherwise throw exception.
        /// If stringDataStoreDouble not found in applicationDataContainer then create entry and set it to "0" and return 0.
        /// </summary>
        /// <param name="applicationDataContainer">Container of application settings.</param>
        /// <param name="stringDataStoreDouble">Data store string that represents a double.</param>
        /// <returns></returns>
        public static double DataStoreStringToDouble(ApplicationDataContainer applicationDataContainer, string stringDataStoreDouble)
        {
            if (applicationDataContainer.Values.ContainsKey(stringDataStoreDouble))
            {
                object objectDouble = applicationDataContainer.Values[stringDataStoreDouble];
                if (!double.TryParse(objectDouble.ToString(), out double doubleDataStore))
                {
                    // Debug.WriteLine($"LibMPC.DataStoreStringToDouble(): Found data store value {objectDouble} but not valid double.");
                    throw new InvalidOperationException($"LibMPC. DataStoreStringToDouble(): Found data store value {objectDouble} but not valid double.");
                }
                // Debug.WriteLine($"LibMPC.DataStoreStringToDouble(): Found data store value and it was valid double so returned {stringDataStoreDouble}={doubleDataStore}");
                return doubleDataStore;
            }
            else
            {
                // Data store value was not found so save 0 to data store and return 0.
                // Debug.WriteLine($"LibMPC.DataStoreStringToDouble(): Did not find data store value {stringDataStoreDouble} so returned 0");
                applicationDataContainer.Values[stringDataStoreDouble] = "0";
                return 0d;
            }
        }

        /// <summary>
        /// Show User W10 App review dialog box. Return true if dialog box appeared, false otherwise.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ShowRatingReviewDialogAsync()
        {
            // More info at: https://docs.microsoft.com/en-us/windows/uwp/monetize/request-ratings-and-reviews
            // and: https://stackoverflow.com/questions/18784697/how-to-import-jsonconvert-in-c-sharp-application
            // This code is simple and good enough and is not reliant upon Json.NET library from Newtonsoft. Json.NET was thowing compile errors
            // after a Windows 10 update so deleted it. Json.NET can be installed via Visual Studio Project tab, Manage NuGet packages,
            // then search for "newtonsoft json", and install package".
            // Microsoft has updated review API's but new code targets Windows 10 October 2018 Update (10.0; Build 17763) so not going to use at this time.

            StoreSendRequestResult storeSendRequestResult = await StoreRequestHelper.SendRequestAsync(StoreContext.GetDefault(), 16, string.Empty);
            if (storeSendRequestResult.ExtendedError == null)
            {
                // Returned true since User opened review dialog box. Do not know if User rated or reviewed application or just closed review dialog box.
                // Debug.WriteLine($"LibMPC.ShowRatingReviewDialog(): Returned true since User opened review dialog box. Do not know if User rated or reviewed application or just closed review dialog box.");
                return true;
            }
            // Returned false since error with request to open review dialog box.
            // Debug.WriteLine($"LibMPC.ShowRatingReviewDialog(): Returned false since error with request to open review dialog box.");
            return false;
        }

        /// <summary>
        /// Show popup box requestiing User to make choice. Return true if User selects button with content stringButtonYes, otherwise return false.
        /// </summary>
        /// <param name="stringTitle">Popup box title. Sample="Reset application?"</param>
        /// <param name="stringContent">Popup box message. Sample="This will revert application to default settings."</param>
        /// <param name="stringButtonYes">Content for button that returns true if selected. Sample="Yes".</param>
        /// <param name="stringButtonNo">Content for button that returns false if selected. Sample="No".</param>
        /// <returns></returns>
        public static async Task<bool> ShowPopupBoxAsync(string stringTitle, string stringContent, string stringButtonYes, string stringButtonNo)
        {
            // https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/buttons
            // https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.ContentDialog
            ContentDialog contentDialog = new ContentDialog
            {
                // 3 buttons max.
                Title = stringTitle,
                Content = stringContent,
                // CloseButtonText = "No",          // Do not use.
                PrimaryButtonText = stringButtonYes,
                SecondaryButtonText = stringButtonNo,     
                BorderBrush = LibMPC.colorError     // Set popup box border color.
            };
            ContentDialogResult contentDialogResult = await contentDialog.ShowAsync();
            if (contentDialogResult.Equals(ContentDialogResult.Primary))
                return true;    // User wants reset.
            else
                return false;   // User does not want reset.
        }

        // More info at: https://msdn.microsoft.com/windows/uwp/app-settings/store-and-retrieve-app-data#local-app-data
        /// <summary>
        /// Show data store values currently used by application. This is debug code and output is sent to Debug.
        /// </summary>
        /// <param name="applicationDataContainer">Container of application settings.</param>
        public static void ListDataStoreItems(ApplicationDataContainer applicationDataContainer)
        {
            if (applicationDataContainer != null)
            {
                IPropertySet iPropertySet = applicationDataContainer.Values;   // Get all values under localSettings but no containers.
                if (iPropertySet.Count > 0)
                {
                    Debug.WriteLine($"\nLibMPC.ListDataStoreItems(): Found {iPropertySet.Count} items in App data store.");
                    List<string> listStringValueNames = new List<string>();
                    foreach (KeyValuePair<string, object> keyValuePair in iPropertySet)
                    {
                        listStringValueNames.Add($"{keyValuePair.Key}={keyValuePair.Value}");
                    }
                    listStringValueNames.Sort();      // Sort list by name.
                    foreach (string stringFound in listStringValueNames)
                    {
                        Debug.WriteLine($"\t{stringFound}");
                    }
                    Debug.WriteLine("LibMPC.ListDataStoreItems(): End of method.\n");
                }
                else
                {
                    Debug.WriteLine("\nLibMPC.ListDataStoreItems(): Did not find any items in App data store.\n");
                }
            }
        }

        /// <summary>
        /// Return device culture if parameter boolDeviceCulture is true, otherwise return culture currently used by application.
        ///  Device culture is top user-preferred language that is set in Sample/Time & Language/Language.
        /// </summary>
        /// <param name="boolDeviceCulture">Get device culture if true, otherise get application culture.</param>
        /// <returns></returns>
        public static string GetCulture(bool boolDeviceCulture)
        {
            string stringCulture;
            if (boolDeviceCulture)
            {
                // More at: https://docs.microsoft.com/en-us/uwp/api/windows.globalization.language
                // Get top user-preferred language that is set in Sample/Time & Language/Language. This is device culture.
                stringCulture = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];  // Get normalized BCP-47 language tag used by device. Sample is "en-US".
                // Language language = new Language(stringCulture);
                ///string stringDisplayName = language.DisplayName;
                // Debug.WriteLine($"LibMPC.GetCulture(): Device culture={stringCulture}, Device displayName={stringDisplayName}");
            }
            else
            {
                // More at: https://msdn.microsoft.com/en-us/library/windows/apps/hh694557.aspx?f=255&MSPPError=-2147217396
                stringCulture = CultureInfo.CurrentCulture.Name;    // Get normalized BCP-47 language tag currently used by application. Sample is "en".
                ///Language language = new Language(stringCulture);
                ///string stringDisplayName = language.DisplayName;
                // Debug.WriteLine($"LibMPC.GetCulture(): App culture={stringCulture}, App language name={stringDisplayName}");
            }
            return stringCulture;
        }

        /// <summary>
        /// Return string built from list of strings using specified separator. Separator not added to last string.
        /// </summary>
        /// <param name="listOfStrings"></param>
        /// <param name="enumStringSeparator"></param>
        /// <returns></returns>
        public static string JoinListString(List<string> listOfStrings, EnumStringSeparator enumStringSeparator)
        {
            string stringSeparator = string.Empty;
            switch (enumStringSeparator)
            {
                case EnumStringSeparator.OneSpace:
                    stringSeparator = " ";      // One space.
                    break;
                case EnumStringSeparator.TwoSpaces:
                    stringSeparator = "  ";     // Two spaces.
                    break;
                case EnumStringSeparator.OneNewline:
                    stringSeparator = Environment.NewLine;
                    break;
                case EnumStringSeparator.TwoNewlines:
                    stringSeparator = $"{Environment.NewLine}{Environment.NewLine}";
                    break;
                default:    // Case EnumStringSeparator.None:  Use empty separator set above.
                    break;
            }
            int intCount = 1;
            int intItems = listOfStrings.Count;
            string stringResult = string.Empty;
            foreach (string stringItem in listOfStrings)
            {
                stringResult += stringItem;
                // Add separator after string if not last string in list.
                if (intCount < intItems)
                    stringResult += stringSeparator;
                intCount++;
            }
            return stringResult;
        }

        /*** Public application purchase methods and strings *******************************************************************/

        // More at: https://docs.microsoft.com/en-us/windows/uwp/monetize/in-app-purchases-and-trials#testing
        // See ColorBars application as sample how to implement following purchase methods in an application.

        /// <summary>
        /// Public result string from application purchase methods.
        /// Calling methods can use this result string to display success or error messages to User.
        /// </summary>
        public static string stringAppPurchaseResult;

        // Following strings are used in following application purchase methods. These strings can be changed by calling methods if desired.
        // Calling methods can translate these string to another language if needed. Most of the strings will not require translations since 
        // will never be seen by user. Obviously, if want to change any of following strings, then this needs to be done before calling following methods.
        // Following strings sorted by method used in.

        /// <summary>
        /// Default value is "Application not purchased since purchase may have been canceled."
        /// </summary>
        public static string stringAppPurchaseBuyMsg1 = "Application not purchased since purchase may have been canceled.";

        /// <summary>
        /// Default value is "Application not purchased since network error."
        /// </summary>
        public static string stringAppPurchaseBuyMsg2 = "Application not purchased since network error.";

        /// <summary>
        /// Default value is "Application not purchased since server error."
        /// </summary>
        public static string stringAppPurchaseBuyMsg3 = "Application not purchased since server error.";

        /// <summary>
        /// Default value is "Application not purchased since unknown error."
        /// </summary>
        public static string stringAppPurchaseBuyMsg4 = "Application not purchased since unknown error.";

        /// <summary>
        /// Default value is "Application not purchased since storePurchaseResult.ExtendedError:"
        /// </summary>
        public static string stringAppPurchaseBuyMsg5 = "Application not purchased since storePurchaseResult.ExtendedError:";

        /// <summary>
        /// Default value is "Application not purchased since storePurchaseResult was null."
        /// </summary>
        public static string stringAppPurchaseBuyMsg6 = "Application not purchased since storePurchaseResult was null.";

        /// <summary>
        /// Default value is "Application not purchased since storeProductResult.ExtendedError:"
        /// </summary>
        public static string stringAppPurchaseBuyMsg7 = "Application not purchased since storeProductResult.ExtendedError:";

        /// <summary>
        /// Default value is "Application not purchased since storeProductResult was null."
        /// </summary>
        public static string stringAppPurchaseBuyMsg8 = "Application not purchased since storeProductResult was null.";

        /// <summary>
        /// Default value is "Application not purchased since storeContext was null."
        /// </summary>
        public static string stringAppPurchaseBuyMsg9 = "Application not purchased since storeContext was null.";

        /// <summary>
        /// Default value is "Application has been purchased. Thank you!"
        /// </summary>
        public static string stringAppPurchaseTrueMsg1 = "Application has been purchased. Thank you!";

        /// <summary>
        /// Default value is "Application price is: "
        /// </summary>
        public static string stringAppPurchaseFalseMsg1 = "Application price is:";

        /// <summary>
        /// Default value is "Application has not been purchased."
        /// </summary>
        public static string stringAppPurchaseFalseMsg2 = "Application has not been purchased.";

        /// <summary>
        /// Default value is "Trial period is unlimited. Purchase check will occur on every start of application."
        /// </summary>
        public static string stringAppPurchaseFalseMsg3 = "Trial period is unlimited. Purchase check will occur on every start of application.";

        /// <summary>
        /// Default value is "Trial period ends at"
        /// </summary>
        public static string stringAppPurchaseFalseMsg4 = "Trial period ends at";

        /// <summary>
        /// Default value is "Trial period remaining cannot be determined at this time."
        /// </summary>
        public static string stringAppPurchaseFalseMsg5 = "Trial period remaining cannot be determined at this time.";

        /// <summary>
        /// Default value is "Aborted application purchase check since storeContext was null."
        /// </summary>
        public static string stringAppPurchaseFalseMsg6 = "Aborted application purchase check since storeContext was null.";

        /// <summary>
        /// Return true if application has been purchased, otherwise return false.
        /// Output message sent to public string stringAppPurchaseMessage which can be used by calling methods to display success or error messages.
        /// </summary>
        /// <param name="applicationDataContainer">Container of application settings.</param>
        /// <param name="stringDataStoreBoolAppPurchased">Data store string that is set to true or false.</param>
        /// <returns></returns>
        public static async Task<bool> AppPurchaseStatusAsync(ApplicationDataContainer applicationDataContainer, string stringDataStoreBoolAppPurchased)
        {
            if (DataStoreStringToBool(applicationDataContainer, stringDataStoreBoolAppPurchased))
            {
                return AppPurchaseTrue(applicationDataContainer, stringDataStoreBoolAppPurchased);
            }
            return await AppPurchaseFalseAsync(applicationDataContainer, stringDataStoreBoolAppPurchased);
        }

        /// <summary>
        /// Return true if application has been purchased, otherwise return false.
        /// Output message sent to public string stringAppPurchaseMessage which can be used by calling methods to display success or error messages.
        /// </summary>
        /// <param name="applicationDataContainer">Container of application settings.</param>
        /// <param name="stringDataStoreBoolAppPurchased">Data store string that is set to true or false.</param>
        /// <returns></returns>
        public static async Task<bool> AppPurchaseBuyAsync(ApplicationDataContainer applicationDataContainer, string stringDataStoreBoolAppPurchased)
        {
            StoreContext storeContext = StoreContext.GetDefault();
            if (storeContext != null)
            {
                // Get application store product details
                StoreProductResult storeProductResult = await storeContext.GetStoreProductForCurrentAppAsync();
                if (storeProductResult != null)
                {
                    if (storeProductResult.ExtendedError == null)
                    {
                        StoreAppLicense storeAppLicense = await storeContext.GetAppLicenseAsync();
                        // TODO: Debug code, reverse before MS Store publish. Negate next if statement to see StorePurchaseStatus.AlreadyPurchased: message if App has been purchased.
                        if (storeAppLicense.IsTrial)
                        {
                            StorePurchaseResult storePurchaseResult = await storeProductResult.Product.RequestPurchaseAsync();
                            if (storePurchaseResult != null)
                            {
                                if (storePurchaseResult.ExtendedError == null)
                                {
                                    StorePurchaseStatus storePurchaseStatus = storePurchaseResult.Status;
                                    switch (storePurchaseStatus)
                                    {
                                        case StorePurchaseStatus.Succeeded:
                                            // Debug.WriteLine("LibMPC.AppPurchaseBuy(): case StorePurchaseStatus.Succeeded:");
                                            return AppPurchaseTrue(applicationDataContainer, stringDataStoreBoolAppPurchased);   // Purchase succeeded.
                                        case StorePurchaseStatus.AlreadyPurchased:
                                            // Debug.WriteLine("LibMPC.AppPurchaseBuy(): StorePurchaseStatus.AlreadyPurchased:");
                                            // Application should never get here since values set when purchased in case above, but handle it if so.
                                            return AppPurchaseTrue(applicationDataContainer, stringDataStoreBoolAppPurchased);   // User already bought application and has fully-licensed version.
                                        case StorePurchaseStatus.NotPurchased:
                                            stringAppPurchaseResult = stringAppPurchaseBuyMsg1;
                                            break;
                                        case StorePurchaseStatus.NetworkError:
                                            stringAppPurchaseResult = stringAppPurchaseBuyMsg2;
                                            break;
                                        case StorePurchaseStatus.ServerError:
                                            stringAppPurchaseResult = stringAppPurchaseBuyMsg3;
                                            break;
                                        default:
                                            stringAppPurchaseResult = stringAppPurchaseBuyMsg4;
                                            break;
                                    }
                                }
                                else
                                    stringAppPurchaseResult = $"{stringAppPurchaseBuyMsg5} {storePurchaseResult.ExtendedError.Message}";
                            }
                            else
                                stringAppPurchaseResult = stringAppPurchaseBuyMsg6;
                        }
                        else
                        {
                            // Application should never get here since values set when purchased in code above, but handle here if so.
                            // Debug.WriteLine($"LibMPC.AppPurchaseBuy(): storeAppLicense.IsTrial was false.");
                            return AppPurchaseTrue(applicationDataContainer, stringDataStoreBoolAppPurchased);     // Application not in trial mode so has been purchased.
                        }
                    }
                    else
                    {
                        // The user may be offline or there might be some other server failure.
                        stringAppPurchaseResult = $"{stringAppPurchaseBuyMsg7} {storeProductResult.ExtendedError.Message}";
                    }
                }
                else
                    stringAppPurchaseResult = stringAppPurchaseBuyMsg8;
            }
            else
            {
                stringAppPurchaseResult = stringAppPurchaseBuyMsg9;
            }
            return false;
        }

        /*** Private application purchase methods ******************************************************************************/

        /// <summary>
        /// Return true since application has been purchased and create or update data store value to true.
        /// Output message sent to public string stringAppPurchaseMessage which can be used by calling methods to display success or error messages.
        /// </summary>
        /// <param name="applicationDataContainer">Container of application settings.</param>
        /// <param name="stringDataStoreBoolAppPurchased">Data store string that is set to true or false.</param>
        /// <returns></returns>
        private static bool AppPurchaseTrue(ApplicationDataContainer applicationDataContainer, string stringDataStoreBoolAppPurchased)
        {
            applicationDataContainer.Values[stringDataStoreBoolAppPurchased] = true;     // Save setting to data store.
            stringAppPurchaseResult = stringAppPurchaseTrueMsg1;
            return true;
        }

        /// <summary>
        /// Update application purchase status since stringDataStoreBoolAppPurchased does not exist or is false.
        /// Return true if application has been purchased, otherwise return false.
        /// Output message sent to public string stringAppPurchaseMessage which can be used by calling methods to display success or error messages.
        /// </summary>
        /// <param name="applicationDataContainer">Container of application settings.</param>
        /// <param name="stringDataStoreBoolAppPurchased">Data store string that is set to true or false.</param>
        /// <returns></returns>
        private static async Task<bool> AppPurchaseFalseAsync(ApplicationDataContainer applicationDataContainer, string stringDataStoreBoolAppPurchased)
        {
            string stringAppPrice = string.Empty;
            StoreContext storeContext = StoreContext.GetDefault();
            if (storeContext != null)
            {
                // Get application price.
                StoreProductResult storeProductResult = await storeContext.GetStoreProductForCurrentAppAsync();
                if (storeProductResult.ExtendedError == null)
                {
                    stringAppPrice = $"{stringAppPurchaseFalseMsg1} {storeProductResult.Product.Price.FormattedPrice}";
                }
                StoreAppLicense storeAppLicense = await storeContext.GetAppLicenseAsync();
                if (storeAppLicense.IsActive)
                {
                    // TODO: Debug code, reverse before MS Store publish. Negate next if statement to see trial message if App has been purchased.
                    if (storeAppLicense.IsTrial)
                    {
                        // Note: If trial period has expired, W10 will close Application on start and show Store message "Your free trial is over. Hope you enjoyed it."
                        // and provides link to purchase Application. The folowing code will not run in this case since Application was closed.
                        // More at: https://docs.microsoft.com/en-us/windows/uwp/monetize/implement-a-trial-version-of-your-app
                        DateTimeOffset dateTimeExpirationLocal = storeAppLicense.ExpirationDate;
                        // Debug.WriteLine($"LibMPC.AppPurchaseFalse(): dateTimeExpirationLocal={dateTimeExpirationLocal}");
                        // Convert local time to UTC time.
                        DateTime dateTimeExpirationUTC = dateTimeExpirationLocal.UtcDateTime;
                        // Debug.WriteLine($"LibMPC.AppPurchaseFalse(): DateTime={dateTimeExpirationUTC}");
                        // Microsoft sets expiration date for unlimited trial to UTC time of "12/31/9999 12:00:00 AM". This date string is in "en-US" format.
                        // Date string format can change if a different language is in use as shown below:
                        // For English: "12/31/9999 12:00:00 AM"
                        // For Spanish: "31/12/9999 0:00:00"
                        // For Hindi: "31-12-9999 00:00:00"
                        // Need to compare dateTimeExpirationUTC to see if is unlimited trial date but "en-US" formatted string need to be converted
                        // to format of current language in use via DateTime.TryParseExact() line below.
                        // More at: https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tryparseexact?view=netframework-4.8
                        CultureInfo enUS = new CultureInfo("en-US");
                        string stringUnlimitedTrialPeriodUTC = "12/31/9999 12:00:00 AM";
                        if (!DateTime.TryParseExact(stringUnlimitedTrialPeriodUTC, "G", enUS, DateTimeStyles.None, out DateTime dateTimeUnlimitedTrial))
                        {
                            // This should never happen but throw exception here so error found and can be corrected.
                            throw new FormatException($"LibMPC.AppPurchaseFalse(): Could not parse stringUnlimitedTrialPeriodUTC={stringUnlimitedTrialPeriodUTC}");
                        }
                        if (dateTimeExpirationUTC.Equals(dateTimeUnlimitedTrial))
                        {
                            // Debug.WriteLine($"LibMPC.AppPurchaseFalse(): Trial time is unlimited. Trial period ends at {dateTimeExpirationUTC.ToString("G")}");
                            stringAppPurchaseResult = $"{stringAppPurchaseFalseMsg2}  {stringAppPrice}{Environment.NewLine}{stringAppPurchaseFalseMsg3}";
                        }
                        else
                        {
                            // Debug.WriteLine($"LibMPC.AppPurchaseFalse(): Trial time is limited. Trial period ends at {dateTimeExpirationLocal.ToString("G")}");
                            stringAppPurchaseResult = $"{stringAppPurchaseFalseMsg2}  {stringAppPrice}{Environment.NewLine}{stringAppPurchaseFalseMsg4} {dateTimeExpirationLocal:G}";
                        }
                    }
                    else
                    {
                        // Aplication has been purchased. Should not get here unless App was uninstalled and reinstalled.
                        return AppPurchaseTrue(applicationDataContainer, stringDataStoreBoolAppPurchased);
                    }
                }
                else
                    stringAppPurchaseResult = $"{stringAppPurchaseFalseMsg2}  {stringAppPrice}{Environment.NewLine}{stringAppPurchaseFalseMsg5}";
            }
            else
                stringAppPurchaseResult = stringAppPurchaseFalseMsg6;  // User should not ever see this.
            applicationDataContainer.Values.Remove(stringDataStoreBoolAppPurchased);     // Delete since not used by application purchase methods if false.
            return false;
        }

        /*** End of public and private application purchase methods and strings ************************************************/

    }
}
