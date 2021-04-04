using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

/*
 * File: \Libraries\LibUMitiesMisc.cs.  This library provides various miscellaneous general utility methods.  This file will need to be 
 * copied into project or added via 'Add as Link'.  This file will not compile to a Windows Runtime Component library that can be used by adding 
 * a Reference since it uses decimal types.  Decimals do not have an equivalent underlying Windows Runtime type.
 * 
 * Normally the namespace below would be proceeded by the project name but is ommitted so files can be shared between projects easily.
 * 
 * C# Compiler Options: Compiler symbol 'MOBILE_EXTENSION' is set in project's Properties page. On the Build tab, type the symbol 'MOBILE_EXTENSION'
 * in the Conditional compilation symbols box.  Then the symbol will be in effect for all files in the project.
 * If compiler symbol 'MOBILE_EXTENSION' is not defined, then code using 'Windows Mobile Extensions for the UWP' will be omitted.
 * More at: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/define-compiler-option
 */

namespace LibraryCoder.UtilitiesMisc
{
    // More about DeviceFamily at: https://msdn.microsoft.com/en-us/library/windows/apps/dn986903.aspx
    /// <summary>
    /// Enumeration of UWP device families with associated 'Description' string.
    /// To get the 'Description' string use method LibUM.GetEnumDescription().
    /// </summary>
    public enum DeviceFamily
    {
        [Display(Description = "Windows.Unknown")] Unknown,
        [Display(Description = "Windows.Desktop")] Desktop,
        [Display(Description = "Windows.Team")] Team,
        [Display(Description = "Windows.IoT")] IoT,
        [Display(Description = "Windows.Xbox")] Xbox,
        [Display(Description = "Windows.Holographic")] Holographic,
    };

    /// <summary>
    /// TextBlock output action. Clear TextBlock or add additional text to it.  Used in TextBlockDisplay() method below.
    /// </summary>
    public enum EnumTextBlock { Reset, Add };

    /// <summary>
    /// LibUM = Shorthand for LibraryUtilitiesMisc.
    /// </summary>
    public static class LibUM
    {

        /// <summary>
        /// Get DeviceFamily enumeration of current device. Samples = Desktop, Xbox, IoT, ...
        /// </summary>
        /// <returns></returns>
        public static DeviceFamily GetDeviceFamily()
        {
            string stringDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            Array devices = Enum.GetValues(typeof(DeviceFamily));
            foreach (DeviceFamily deviceFamily in devices)
            {
                string stringDevice = GetEnumDescription(deviceFamily);
                // Debug.WriteLine($"LibUM.GetDeviceFamily(): stringDeviceFamily={stringDeviceFamily}, stringDevice={stringDevice}, deviceFamily={deviceFamily}");
                if (stringDeviceFamily.Equals(stringDevice))
                    return deviceFamily;    // Return the DeviceFamily; Desktop, Xbox, IoT, ...
            }
            // Throw exception so error can be discovered and corrected.
            throw new ArgumentOutOfRangeException("LibUM.GetDeviceFamily(): Current DeviceFamily does not match any enumeration in DeviceFamily.");
        }

        /// <summary>
        /// Reverse the order of a simple string.  This method is only for strings consisting of 16-bit Unicode characters.
        /// Otherwise expect unpredictable results.  Appears to work with "\n" type characters, but not with "\uE700" type characters.
        /// </summary>
        /// <param name="stringToReverse">String to reverse.</param>
        /// <returns></returns>
        public static string ReverseString(string stringToReverse)
        {
            if (stringToReverse == null)
                return null;
            return new string(stringToReverse.ToCharArray().Reverse().ToArray());
        }

        // Found similar code at: http://stackoverflow.com/questions/1167361/how-do-i-convert-an-enum-to-a-list-in-c
        /// <summary>
        /// This takes an enumeration of values and converts it into a list.
        /// </summary>
        /// <typeparam name="T">Enumeration</typeparam>
        /// <returns></returns>
        public static List<T> GetEnumAsList<T>() where T : IComparable, IFormattable, IConvertible
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();    // Convert enum to array and then cast it to a list.
        }

        // Use DisplayAttribute in following method since they work with UWP apps.
        // More at: https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.displayattribute
        /// <summary>
        /// Return string value of enumeration DisplayAttribute if exists, otherwise return string value of enumeration.
        /// Sample: Enumeration containing value '[Display(Description = "Length/Distance")] Length' will return "Length/Distance" versus "Length" since DisplayAttribute exists.
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration type.</typeparam>
        /// <param name="enumeration">Any enumeration since generic method.</param>
        /// <returns></returns>
        public static string GetEnumDescription<TEnum>(TEnum enumeration) where TEnum : IComparable, IFormattable, IConvertible
        {
            return !(enumeration.GetType()
                .GetField(enumeration.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault() is DisplayAttribute attribute) ? enumeration.ToString() : attribute.Description;
        }

        /// <summary>
        /// Ouput to Debug which item currently has focus.  Place below "this.InitializeComponent();" and
        /// call via LibUM.FocusLogger(this);
        /// </summary>
        /// <param name="page"></param>
        public static void FocusLogger(Page page)
        {
            page.GotFocus += (object sender, RoutedEventArgs e) =>
            {
                if (FocusManager.GetFocusedElement() is FrameworkElement focus)
                    Debug.WriteLine("Has focus: " + focus.Name + " (" + focus.GetType().ToString() + ")");
                else
                    // Throw exception so error can be discovered and corrected.
                    throw new ArgumentException("Method LibUM.FocusLogger: Exception occurred.");
            };
        }

        /// <summary>
        /// Save current content of TextBlock in this string.  Used in TextBlockDisplay() below.
        /// </summary>
        private static string stringTextBlock;

        /// <summary>
        /// Accumulate TextBock output into one common string for consistent word-wrapped output versus a bunch of lines at different lengths.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="stringInput">Input string to add at end of current string. Result is padded with two leading spaces.</param>
        /// <param name="enumTextBlock">TextBlock output action. Reset TextBlock or add additional text to it.</param>
        public static void TextBlockDisplay(TextBlock textBlock, string stringInput, EnumTextBlock enumTextBlock = EnumTextBlock.Add)
        {
            if (enumTextBlock == EnumTextBlock.Reset)
            {
                stringTextBlock = stringInput;      // Reset saved string with new value.
                textBlock.Text = stringInput;
            }
            else
                textBlock.Text = stringTextBlock += "  " + stringInput;     // Pad with two spaces.
        }

        /// <summary>
        /// Return string built from list of strings with two spaces added between each string in list. Spaces not added after last item in list.
        /// </summary>
        /// <param name="listOfStrings">List of strings to combine.</param>
        /// <returns></returns>
        public static string ListToStringWithSpaces(List<string> listOfStrings)
        {
            string stringResult = string.Empty;
            int intItems = listOfStrings.Count;
            int intCount = 1;
            foreach (string stringItem in listOfStrings)
            {
                stringResult += stringItem;
                // Add two spaces after string if not last string in list.
                if (intCount < intItems)
                    stringResult += "  ";
                intCount++;
            }
            return stringResult;
        }

        /// <summary>
        /// Return string built from list of strings with two NewLines added between each string in list. Newlines not added after last item in list.
        /// </summary>
        /// <param name="listOfStrings">List of strings to combine.</param>
        /// <returns></returns>
        public static string ListToStringWithNewlines(List<string> listOfStrings)
        {
            string stringResult = string.Empty;
            int intItems = listOfStrings.Count;
            int intCount = 1;
            foreach (string stringItem in listOfStrings)
            {
                stringResult += stringItem;
                // Add two NewLines after string if not last string in list.
                if (intCount < intItems)
                    stringResult += Environment.NewLine + Environment.NewLine;
                intCount++;
            }
            return stringResult;
        }

        /// <summary>
        /// Combine two generic lists of same type into a new list.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="listFirst">First list.</param>
        /// <param name="listSecond">Second list.</param>
        /// <returns></returns>
        public static List<T> ListCombine<T>(List<T> listFirst, List<T> listSecond)
        {
            // More at: https://stackoverflow.com/questions/2002770/net-combining-two-generic-lists
            List<T> listCombined = new List<T>(listFirst.Count + listSecond.Count);     // Initialize to required size for best performance.
            listCombined.AddRange(listFirst);
            listCombined.AddRange(listSecond);
            return listCombined;
        }

        /// <summary>
        /// Return generic array concatenated from parameters array1 and array2, null otherwise. No parameter checking done.
        /// </summary>
        /// <param name="array1">First generic array.</param>
        /// <param name="array2">Second generic array to appended to array1.</param>
        /// <returns></returns>
        public static T[] ArrayConcat<T>(T[] array1, T[] array2)
        {
            T[] arrayConcat = new T[array1.Length + array2.Length];   // Initialize to required size.
            Array.Copy(array1, 0, arrayConcat, 0, array1.Length);
            Array.Copy(array2, 0, arrayConcat, array1.Length, array2.Length);
            if (arrayConcat != null)
                return arrayConcat;
            return null;
        }

        /// <summary>
        /// Compare if content of two arrays are same. Return true if so, false otherwise.
        /// LINQ's a1.SequenceEqual(a2) also works but is more than 10 times slower and returns exception if either array is null.
        /// </summary>
        /// <typeparam name="T">Generic to support any type of array.</typeparam>
        /// <param name="a1">First array.</param>
        /// <param name="a2">Second Array.</param>
        /// <returns></returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;
            if (a1 == null || a2 == null)
                return false;
            if (a1.Length != a2.Length)
                return false;
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Debug Output: Show byte content of IBuffer.
        /// </summary>
        /// <param name="stringIBufferName">IBuffer name.</param>
        /// <param name="iBufferInput">IBuffer to show byte content..</param>
        public static void IBufferContentShow(string stringIBufferName, IBuffer iBufferInput)
        {
            Debug.WriteLine("");
            var iBufferInputLength = iBufferInput.Length;
            for (int i = 0; i < iBufferInputLength; i++)
                Debug.WriteLine($"{stringIBufferName}[{i}]={iBufferInput.GetByte((uint)i)}");
            Debug.WriteLine("");
        }

    }   // End of class LibUM.
}
