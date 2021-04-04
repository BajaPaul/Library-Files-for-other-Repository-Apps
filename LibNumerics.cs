using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Controls;

/*
 * File: \Libraries\LibNumerics.cs.  This library provides various methods related to using numeric values.  This file will need to be 
 * copied into project or added via 'Add as Link'.  This file will not compile to a Windows Runtime Component library that can be used by adding 
 * a Reference since uses decimal types.  Decimals do not have an equivalent underlying Windows Runtime type.
 * 
 * Normally the namespace below would be proceeded by the project name but is ommitted so files can be shared between projects easily.
 */

namespace LibraryCoder.Numerics
{
    // For more information on these types see: https://msdn.microsoft.com/en-us/library/s1ax56ch.aspx
    /// <summary>
    /// Enumeration of supported TextBox input numeric types. Leading underline characters are necessary to prevent conflict.
    /// </summary>
    public enum EnumNumericType { _decimal, _double, _float, _long, _ulong, _int, _uint, _short, _ushort, _sbyte, _byte };
    
    /// <summary>
    /// Update the content of TextBox on Enter key pressed or lost focus if Yes.  Content has been verified by NumericTextBoxTextChanged() 
    /// and resulting TryParse().  Cursor position in updated TextBox will be retained to extent possible.  If no, do not update content of TextBox.
    /// </summary>
    public enum EnumTextBoxUpdate { No, Yes };

    /// <summary>
    /// Enum used in FormatAsPowerOfTen().  If 'Yes', then remove any trailing zeros.  If 'No', the leave trailing zeros to limit of showDigits parameter.
    /// </summary>
    public enum EnumRemoveTrailingZeros { No, Yes };

    /// <summary>
    /// LibNum = Shorthand for LibraryNumerics.
    /// </summary>
    public static class LibNum
    {
        // Set Unicode exponential characters used in methods below.  These values from Seqoe UI font character map.
        // These characters may not be supported in all fonts.
        public const char expCharN = '\u207F';          // ⁿ    // Generic exponent power.
        public const char expChar0 = '\u2070';          // ⁰
        public const char expChar1 = '\u00B9';          // ¹
        public const char expChar2 = '\u00B2';          // ²
        public const char expChar3 = '\u00B3';          // ³
        public const char expChar4 = '\u2074';          // ⁴
        public const char expChar5 = '\u2075';          // ⁵
        public const char expChar6 = '\u2076';          // ⁶
        public const char expChar7 = '\u2077';          // ⁷
        public const char expChar8 = '\u2078';          // ⁸
        public const char expChar9 = '\u2079';          // ⁹
        public const char expCharPlus = '\u207A';       // ⁺
        public const char expCharMinus = '\u207B';      // ⁻
        public const char expCharTimes = '\u00D7';      // ×
        public const char expCharDivide = '\u00F7';     // ÷
        public const char expCharEqual = '\u207C';      // ⁼
        
        /// <summary>
        /// Get four digit hexidecimal Unicode value of character called.
        /// </summary>
        /// <param name="character">Get hexidecimal Unicode for this character.</param>
        /// <returns>Four digit string of hexidecimal Unicode value.</returns>
        public static string GetCharUnicodeValue(char character)
        {
            return ((int)character).ToString("x4").ToUpper();
        }
        
        // Floating-point numeric format output strings used in floating-point methods below.  Can use for decimal, double, or float.
        // Decimal has up to 28 significant digits of precision.  Need at least 28 "#'s" after decimal to always show everything.
        //
        // Sample: TextBlock.Text = fpValue.ToString(LibNum.fpNumericFormatNone);
        //
        // Output floating-point value in "raw" format.
        public const string fpNumericFormatNone = "0.############################";
        // Output floating-point value in scientific format.
        public const string fpNumericFormatScientific = "0.############################E+0";
        // Output floating-point in comma separator formart.
        public const string fpNumericFormatSeparator = "#,0.############################";
        
        /****************************** Numerical Input Methods - Begin ********************************/
        
        /* Comment Consolidation for following methods.  Goal: Use time to edit code versus endlessly editing mutiple comments.
         * 
         * See file TestNumericInputTypes.xaml.cs for method samples of each EnumNumericType that can be copied and pasted.
         * 
         * Numeric Entry: Check if TextBox entry is a valid numeric.  It is necessary to monitor three events for each EnumNumericType TextBox:
         *      #1: Check TextBox if KeyDown=Enter.
         *      #2: Check TextBox if LostFocus.
         *      #3: Check TextBox if TextChanged still results in valid numeric entry of type desired.
         * 
         * #1: private void NumericType_TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
         *          Invoked when user presses any key while in EnumNumericType TextBox.  It ignores everything until ENTER key is pressed.
         *          When ENTER key is pressed then get string from TextBox and to convert to matching numeric.
         *          Call: LibNum.TextBoxGetNumericType(TextBox, EnumTextBoxUpdate);
         *              TextBox = Get numeric string from TextBox and convert to matching numeric.  Update of TextBox is optional.
         *              EnumTextBoxUpdate = Yes/No.  If Yes, update the TextBox with new TryParse value.  Cursor position will be maintained to extent possible.
         *              
         * #2: private void NumericType_TextBox_LostFocus(object sender, RoutedEventArgs e)
         *          Invoked when EnumNumericType TextBox loses focus.  On focus lost, get string from TextBox and to convert to matching numeric.
         *          Call: LibNum.TextBoxGetNumericType(TextBox, EnumTextBoxUpdate);
         *              TextBox = Get numeric string from TextBox and convert to matching numeric.  Update of TextBox is optional.
         *              EnumTextBoxUpdate = Yes/No.  If Yes, update the TextBox with new TryParse value.  Cursor position will be maintained to extent possible.
         *              
         * #3: private void NumericType_TextBox_TextChanged(object sender, TextChangedEventArgs e)
         *          Invoked when any character is entered in the TextBox.  This method insures that character entered continues to provide a string that will 
         *          convert to a valid numeric of type specified.  Any character that does not match specified numeric type is deleted.  Also does other formatting.
         *          Call: LibNum.NumericTextBoxTextChanged(TextBox, EnumNumericType);
         *              TextBox = Get last character inputed from TextBox and check if it is valid for specified EnumNumericType.  Delete character otherwise.
         *              EnumNumericType = One of the supported numeric types defined in the Numerictype enumeration.
         */
         
        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events.
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static decimal TextBoxGetDecimal(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0m;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            text = StripTrailingZeros(text, ref cursorPosition);        // Strip any trailing zeros.
            // decimal: Signed 128-bit (16 byte) floating-point with 29 significant digit precision.  Valid decimal range: -7.9228162514264337593543950335E+28 to 7.9228162514264337593543950335E+28
            // Valid decimal range: -79,228,162,514,264,337,593,543,950,335.00 to 79,228,162,514,264,337,593,543,950,335.00
            // decimal min = -79228162514264337593543950335
            // decimal max =  79228162514264337593543950335
            if (decimal.TryParse(text, out decimal userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString(fpNumericFormatNone);
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;        // Clean up textBox.Text by setting to new userInput string.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0m;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox. Call this method from TextBox_KeyDown and TextBox_LostFocus events.
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static double TextBoxGetDouble(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0d;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            text = StripTrailingZeros(text, ref cursorPosition);        // Strip any trailing zeros.
            // double: Signed 64-bit (8 byte) floating-point with 15 significant digit precision.  Valid double range: -1.79769313486232E+308 to 1.79769313486232E+308.
            // Valid double range: -179,769,313,486,232,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000.00
            //                   to 179,769,313,486,232,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000.00
            // Reduced last significant digit below so values will Round-Trip (digit 15 changed, 2 to 1).  Values can now can be copied and pasted for testing.
            // double min = -179769313486231000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
            // double max =  179769313486231000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
            if (double.TryParse(text, out double userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString(fpNumericFormatNone);
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;        // Clean up textBox.Text by setting to new userInput string.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0d;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events.
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static float TextBoxGetFloat(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0f;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            text = StripTrailingZeros(text, ref cursorPosition);        // Strip any trailing zeros.
            // float: Signed 32-bit (4 byte) floating-point with 7 significant digit precision.  Valid float range: -3.402823E+38 to 3.402823E+38
            // Valid float range: -340,282,300,000,000,000,000,000,000,000,000,000,000.00 to 340,282,300,000,000,000,000,000,000,000,000,000,000.00
            // float min = -340282300000000000000000000000000000000
            // float max =  340282300000000000000000000000000000000
            if (float.TryParse(text, out float userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString(fpNumericFormatNone);
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;        // Clean up textBox.Text by setting to new userInput string.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0f;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static long TextBoxGetLong(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            // long: Signed 64-bit (8 byte) integer.  Valid long range: –9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
            // long min = -9223372036854775808
            // long max =  9223372036854775807
            if (long.TryParse(text, out long userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString();
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;    // Clean up textBox.Text by setting to new userInput string.  Will trigger NumericTextBoxTextChanged() event.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static ulong TextBoxGetUlong(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            // ulong: Unsigned 64-bit (8 byte) integer.  Valid ulong range: 0 to 18,466,744,073,709,551,615.
            // ulong min = 0
            // ulong max = 18446744073709551615
            if (ulong.TryParse(text, out ulong userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString();
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;    // Clean up textBox.Text by setting to new userInput string.  Will trigger NumericTextBoxTextChanged() event.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static int TextBoxGetInt(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            // int: Signed 32-bit (4 byte) integer.  Valid int range: -2,147,483,648 to 2,147,483,647.
            // int min = -2147483648
            // int max =  2147483647
            if (int.TryParse(text, out int userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString();
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;    // Clean up textBox.Text by setting to new userInput string.  Will trigger NumericTextBoxTextChanged() event.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static uint TextBoxGetUint(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            // uint: Unsigned 32-bit (4 byte) integer.  Valid uint range: 0 to 4,294,967,295.
            // uint min = 0
            // uint max = 4294967295
            if (uint.TryParse(text, out uint userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString();
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;    // Clean up textBox.Text by setting to new userInput string.  Will trigger NumericTextBoxTextChanged() event.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static short TextBoxGetShort(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            // short: Signed 16-bit (2 byte) integer.  Valid short range: -32,768 to 32,767.
            // short min = -32768
            // short max =  32767
            if (short.TryParse(text, out short userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString();
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;    // Clean up textBox.Text by setting to new userInput string.  Will trigger NumericTextBoxTextChanged() event.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static ushort TextBoxGetUshort(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            // ushort: Unsigned 16-bit (2 byte) integer.  Valid ushort range: 0 to 65,535.
            // ushort min = 0
            // ushort max = 65535
            if (ushort.TryParse(text, out ushort userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString();
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;    // Clean up textBox.Text by setting to new userInput string.  Will trigger NumericTextBoxTextChanged() event.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static sbyte TextBoxGetSbyte(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            // sbyte: Signed 8-bit (1 byte) integer.  Valid sbyte range (1 byte): -128 to 127.
            // sbyte min = -128
            // sbyte max =  127
            if (sbyte.TryParse(text, out sbyte userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString();
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;    // Clean up textBox.Text by setting to new userInput string.  Will trigger NumericTextBoxTextChanged() event.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
        }

        /// <summary>
        /// Get numeric input from a TextBox.  Call this method from TextBox_KeyDown and TextBox_LostFocus events
        /// </summary>
        /// <param name="textBox">TextBox to get numeric string from and convert to numeric.</param>
        /// <param name="textBoxUpdate">If yes, then update the TextBox with the converted value.</param>
        /// <returns></returns>
        public static byte TextBoxGetByte(TextBox textBox, EnumTextBoxUpdate textBoxUpdate)
        {
            int cursorPosition = textBox.SelectionStart;    // Save cursor position.
            bool firstCharIsPlus = false;                   // If first character is '+' then TryParse() will remove it.
            if (textBox.Text.Length <= 0)
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
            string text = textBox.Text;
            if (text[0] == '+')
            {
                firstCharIsPlus = true;     // Need to adjust cursor position back one space to keep in same relative position.
            }
            // byte: Unsigned 8-bit (1 byte) integer. Valid byte range (1 byte): 0 to 255.
            // byte min = 0
            // byte max = 255
            if (byte.TryParse(text, out byte userInput))
            {
                // text converts to a valid number.
                text = userInput.ToString();
                int len = text.Length;
                if (len < cursorPosition)       // Overflow check!  Do not always know exactly what TryParse will return.
                    cursorPosition = len;       // Place the cursor at end of number as default if overflow.
                else
                {
                    if (firstCharIsPlus)        // TryParse will remove the leading '+' sign so move cursor back one space.
                        cursorPosition--;
                    if (cursorPosition < 0)     // Underflow check!
                        cursorPosition = 0;     // Place the cursor at begining of number as default if underflow.
                }
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = text;    // Clean up textBox.Text by setting to new userInput string.  Will trigger NumericTextBoxTextChanged() event.
                    textBox.SelectionStart = cursorPosition;    // Set cursor position.
                }
                return userInput;
            }
            else    // text did not convert to a valid number.  Should never get here if using NumericTextBoxTextChanged() to verify input as new characters are entered.
            {
                if (textBoxUpdate == EnumTextBoxUpdate.Yes)
                {
                    textBox.Text = "0";                 // Clean up textBox.Text by setting to new userInput value.
                    textBox.SelectionStart = 1;         // Place cursor behind the zero.
                }
                return 0;
            }
        }

        // Following bool skipNextTextChangedEvent is used in NumericTextBoxTextChanged() method below to skip duplicate event triggered when
        // any change is made to parameter textBox.Text.
        private static bool skipNextTextChangedEvent = false;

        /// <summary>
        /// This method prevents non-numeric entry from a TextBox.  Call this method from a TextBox_TextChanged event.
        /// Supported numeric types are defined in the EnumNumericType enumeration.
        /// </summary>
        /// <param name="textBox">TextBox to get user input from.</param>
        /// <param name="numericType">Specified numeric type from TextBox used to limit valid input too.</param>
        public static void NumericTextBoxTextChanged(TextBox textBox, EnumNumericType numericType)
        {
            // To isolate valid numeric entry is quite difficult to achieve since so many ways to edit input stream.  Cursor can be moved around 
            // and any character can be added or deleted at any random locations.  Each change needs to be validated requiring following mutiple tests.
            // In addition, TryParse in not necessarily consistient in what it returns as valid for all the different numeric types.
            //
            if (!skipNextTextChangedEvent)      // Skip to end of method if duplicate event is triggered.
            {
                //
                // Phase 1: Check if beginning character is '+', '-', or '.' and is valid for the specified EnumNumericType.
                //
                if (textBox.Text.Length == 0)   // Do nothing if no input.  Also needed to prevent throwing exceptions below.
                    return;
                // textBox.Text should not change unless method 'returns' immediately below since any change will trigger this event again.
                string text = textBox.Text;
                if (numericType == EnumNumericType._decimal || numericType == EnumNumericType._double || numericType == EnumNumericType._float)
                {
                    if (text == "+" || text == "-" || text == ".")      // Valid first characters for decimal, double, and float.
                        return;
                }
                else
                {
                    if (numericType == EnumNumericType._long || numericType == EnumNumericType._int || numericType == EnumNumericType._short || numericType == EnumNumericType._sbyte)
                    {
                        if (text == "+" || text == "-")         // Valid first characters for long, int, short, or sbyte.
                            return;
                    }
                    else    // Default case:
                    {
                        if (numericType == EnumNumericType._ulong || numericType == EnumNumericType._uint || numericType == EnumNumericType._ushort || numericType == EnumNumericType._byte)
                        {
                            if (text == "+")                    // Valid first character for ulong, uint, ushort, or byte.
                                return;
                        }
                    }
                }
                //
                // Phase 2: Simple conditions eliminated above.  Prevent spaces at front of string and '+' or '-' or space at end of string.  
                // TryParse will return valid numeric in these instances but prevent it.
                //
                if (text == " ")    // First character is a space.  Space is valid for TryParse() but do not allow it.  Delete it.
                {
                    skipNextTextChangedEvent = true;    // Set to true to skip directly to end on next triggered event.
                    textBox.Text = string.Empty;        // This change to textBox.Text will trigger another event leading back to this method.
                    return;
                }
                if (text.Length > 1 && (text.EndsWith(" ") || text.EndsWith("+") || text.EndsWith("-")))   // Prevent +, -, or space end of number. Override since TryParse() will accept these as valid.
                {
                    text = text.Remove(text.Length - 1);
                    skipNextTextChangedEvent = true;                // Set to true to skip directly to end on next triggered event.
                    textBox.Text = text;                            // This change to textBox.Text will trigger another event leading back to this method.
                    textBox.SelectionStart = text.Length;           // Place the cursor at the end of number.
                    return;
                }
                //
                // Phase 3: Simple conditions above eliminated.  Prevent any spaces from being entered.
                //
                int cursorPosition = textBox.SelectionStart;    // Save current cursor position.  This value should not change.  Use selectionStart for edits.
                int selectionStart = cursorPosition - 1;        // Manipulate cursor position with this variable.  Get starting cursor position of last character entered. 
                if (selectionStart < 0)                         // Check to prevent underflow.
                    selectionStart = 0;
                // Test to prevent any spaces from being entered in number after eliminating above possibilities.  TryParse() will allow spaces before and after number.
                if (textBox.Text[selectionStart] == ' ')
                {
                    text = text.Remove(selectionStart, 1);      // Remove the space character.
                    skipNextTextChangedEvent = true;            // Set to true to skip directly to end on next triggered event.
                    textBox.Text = text;                        // This change to textBox.Text will trigger another event leading back to this method.
                    textBox.SelectionStart = selectionStart;
                    return;
                }
                //
                // Phase 4: Valid character entered thus far.  Check if input character is valid for the specified EnumNumericType.  Delete if not.
                //
                bool notNumber = false;     // Set back to false on each pass so no conflict if using mutiple TextBoxes.
                switch (numericType)        // Test if input string entered thus far matches numeric type assigned by parameter.
                {
                    case EnumNumericType._decimal:
                        // decimal: Signed 128-bit (16 byte) floating-point with 29 significant digit precision.  Valid decimal range: -7.9228162514264337593543950335E+28 to 7.9228162514264337593543950335E+28
                        // Valid decimal range: -79,228,162,514,264,337,593,543,950,335.00 to 79,228,162,514,264,337,593,543,950,335.00
                        // decimal min = -79228162514264337593543950335
                        // decimal max =  79228162514264337593543950335
                        if (!decimal.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._double:
                        // double: Signed 64-bit (8 byte) floating-point with 15 significant digit precision.  Valid double range: -1.79769313486232E+308 to 1.79769313486232E+308.
                        // Valid double range: -179,769,313,486,232,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000.00
                        //                   to 179,769,313,486,232,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000.00
                        // Reduced last significant digit below so values will Round-Trip (digit 15 changed, 2 to 1).  Values can now can be copied and pasted for testing.
                        // double min = -179769313486231000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
                        // double max =  179769313486231000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
                        if (!double.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._float:
                        // float: Signed 32-bit (4 byte) floating-point with 7 significant digit precision.  Valid float range: -3.402823E+38 to 3.402823E+38
                        // Valid float range: -340,282,300,000,000,000,000,000,000,000,000,000,000.00 to 340,282,300,000,000,000,000,000,000,000,000,000,000.00
                        // float min = -340282300000000000000000000000000000000
                        // float max =  340282300000000000000000000000000000000
                        if (!float.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._long:
                        // long: Signed 64-bit (8 byte) integer.  Valid long range: –9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
                        // long min = -9223372036854775808
                        // long max =  9223372036854775807
                        if (!long.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._ulong:
                        // ulong: Unsigned 64-bit (8 byte) integer.  Valid ulong range: 0 to 18,466,744,073,709,551,615.
                        // ulong min = 0
                        // ulong max = 18446744073709551615
                        if (!ulong.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._int:
                        // int: Signed 32-bit (4 byte) integer.  Valid int range: -2,147,483,648 to 2,147,483,647.
                        // int min = -2147483648
                        // int max =  2147483647
                        if (!int.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._uint:
                        // uint: Unsigned 32-bit (4 byte) integer.  Valid uint range: 0 to 4,294,967,295.
                        // uint min = 0
                        // uint max = 4294967295
                        if (!uint.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._short:
                        // short: Signed 16-bit (2 byte) integer.  Valid short range: -32,768 to 32,767.
                        // short min = -32768
                        // short max =  32767
                        if (!short.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._ushort:
                        // ushort: Unsigned 16-bit (2 byte) integer.  Valid ushort range: 0 to 65,535.
                        // ushort min = 0
                        // ushort max = 65535
                        if (!ushort.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._sbyte:
                        // sbyte: Signed 8-bit (1 byte) integer.  Valid sbyte range (1 byte): -128 to 127.
                        // sbyte min = -128
                        // sbyte max =  127
                        if (!sbyte.TryParse(text, out _))
                            notNumber = true;
                        break;
                    case EnumNumericType._byte:
                        // byte: Unsigned 8-bit (1 byte) integer. Valid byte range (1 byte): 0 to 255.
                        // byte min = 0
                        // byte max = 255
                        if (!byte.TryParse(text, out _))
                            notNumber = true;
                        break;
                    default:
                        // Do not throw exception here.  Default condition likely handled below.
                        break;
                }
                if (notNumber)      // Last character entered did not result in valid TryParse number so remove it.
                {
                    selectionStart = cursorPosition - 1;            // Variable cursorPosition never changes.
                    // Following switch statement tests for rare case where type MinValue entered and then minus sign is deleted.
                    // Deleting last character will not result in valid number and will throw exception.  Two changes are needed versus one. 
                    // Solution is to set to type MaxValue so input is still valid after the minus sign is deleted.
                    bool minValue = false;      // Set back to false on each pass so no conflict if using mutiple TextBoxes.
                    switch (numericType)        // Test if input string entered thus far matches numeric type assigned by parameter.
                    {
                        case EnumNumericType._long:
                            if (text == "9223372036854775808")  // Type MinValue less minus sign.
                            {
                                text = "9223372036854775807";   // Type MaxValue.
                                selectionStart++;
                                minValue = true;
                            }
                            break;
                        case EnumNumericType._int:
                            if (text == "2147483648")
                            {
                                text = "2147483647";
                                selectionStart++;
                                minValue = true;
                            }
                            break;
                        case EnumNumericType._short:
                            if (text == "32768")
                            {
                                text = "32767";
                                selectionStart++;
                                minValue = true;
                            }
                            break;
                        case EnumNumericType._sbyte:
                            if (text == "128")
                            {
                                text = "127";
                                selectionStart++;
                                minValue = true;
                            }
                            break;
                        default:
                            // Do not throw exception here.  Default condition likely handled below.
                            break;
                    }
                    if (selectionStart < 0)                         // Check to prevent underflow.
                        selectionStart = 0;
                    if (!minValue)                                  // If minValue=true, do not remove character since it has been handled in switch statement above.
                        text = text.Remove(selectionStart, 1);      // Remove invalid character.
                    skipNextTextChangedEvent = true;                // Set to true to skip directly to end on next triggered event.
                    textBox.Text = text;                            // This change to textBox.Text will trigger another event leading back to this method.
                    textBox.SelectionStart = selectionStart;
                    return;
                }
                //
                // Phase 5: Postprocessing, if made this far, then valid character for specified EnumNumericType was entered.  Strip leading zeros.
                //
                if (text[0] == '+' || text[0] == '-' || text[0] == '0')     // Skip all postprocessing unless one of these conditions met.
                {
                    bool madeChange = false;
                    switch (text)
                    {
                        case "0":           // Only character is '0' and is valid for all NumericTypes so return;
                            return;
                        case "+0":          // First character is '+' and second character is '0'.  Valid for all NumericTypes so return.
                            return;
                        case "-0":          // It is possible to enter "-0" which is wrong for ulong, uint, ushort, and byte types.
                            if (numericType == EnumNumericType._ulong || numericType == EnumNumericType._uint || numericType == EnumNumericType._ushort || numericType == EnumNumericType._byte)
                            {
                                text = "0";                 // Remove '-' sign.
                                if (cursorPosition == 2)
                                    selectionStart = 1;
                                else
                                    selectionStart = 0;
                                madeChange = true;
                            }
                            else                            // Valid for all other NumericTypes so return.
                                return;
                            break;
                        case "00":
                            text = "0";                 // Remove extra zero.
                            if (cursorPosition == 2)
                                selectionStart = 1;
                            madeChange = true;
                            break;
                        case "+00":
                            text = "+0";                // Remove extra zero.
                            if (cursorPosition == 3)
                                selectionStart = 2;
                            else
                            {
                                if (cursorPosition == 2)
                                    selectionStart = 1;
                            }
                            madeChange = true;
                            break;
                        case "-00":
                            text = "-0";                // Remove extra zero.
                            if (cursorPosition == 3)
                                selectionStart = 2;
                            else
                            {
                                if (cursorPosition == 2)
                                    selectionStart = 1;
                            }
                            madeChange = true;
                            break;
                        default:
                            // madeChange still false.  Nothing done in this switch statement.  Run the following code that strips any leading zeros away.
                            break;
                    }
                    if (madeChange)     // Process changes made in switch statement above and return.  Skip the following code that strips any leading zeros.
                    {
                        skipNextTextChangedEvent = true;    // Set to true to skip directly to end on next triggered event.
                        textBox.Text = text;                // This change to textBox.Text will trigger another event leading back to this method.
                        textBox.SelectionStart = selectionStart;
                        return;
                    }
                    selectionStart = cursorPosition;    // Reset selectionStart to cursorPosition.  cursorPosition never changes after initial loading.
                    //
                    // Phase 6: Valid character entered thus far.  Strip leading zeros.  Can be mutiple zeros if number before zeros is deleted.
                    //
                    bool plusOrMinus = false;           // True is first character is '+' or '-', false otherwise.
                    int i = 0;                          // Default index location is start of 'text' string.
                    if (text[0] == '+' || text[0] == '-')
                    {
                        plusOrMinus = true;
                        if (text[1] == '0')
                            i = 1;              // Set the index past the '+' or '-' sign.
                        else                    // No leading zeros to strip so return.
                            return;             // Character following '+' or '-' is not '0'.  Valid number so return.
                    }
                    int len = text.Length;
                    bool removedZero = false;       // True if any leading zeros were removed, false otherwise.
                    while (len > (i + 1) && text[i] == '0' && text[i + 1] != '.')       // Do not strip leading zero if next character is '.', a decimal.
                    {
                        text = text.Remove(i, 1);   // Remove the leading zero.
                        removedZero = true;
                        len--;
                    }
                    if (removedZero)    // Skip if no leading zeros were removed.
                    {
                        selectionStart -= 2;           // Move curor back one space if any leading zeros removed.  Seems redundant but cursor can be before or after character removed.
                        if (selectionStart < 0)     // Necessary Underflow check to prevent exception.
                            selectionStart = 0;
                        if (plusOrMinus)            // Advance cursor one position if first character is '+' or '-'.
                            selectionStart++;
                        skipNextTextChangedEvent = true;                    // Set to true to skip directly to end on next triggered event.
                        textBox.Text = text;                                // This change to textBox.Text will trigger another event leading back to this method.
                        textBox.SelectionStart = selectionStart;
                        return;     // This 'return' is redundant here but safe and consistent with code above always returning after a change to textBox.Text.
                    }
                    // If did not remove any zeros then 'return' natuarlly.
                }
            }
            else
            {
                skipNextTextChangedEvent = false;
            }
        }

        /****************************** Numerical Input Methods - End ********************************/
        
        /// <summary>
        /// From a numeric string, strip any trailing zeros after decimal.  Also strip decimal if it is last item.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="cursorPosition">Position of cursor before entering this method</param>
        /// <returns>Modified string with no trailing zeros or trailing decimal.</returns>
        public static string StripTrailingZeros(string number, ref int cursorPosition)
        {
            if (number.Contains("."))   // Do nothing if no decimal in number.  Do not want to delete trailing zeros from integal types.
            {
                if (number[0] == '.')
                {
                    number = "0" + number;      // Add leading zero if missing.
                    cursorPosition++;           // Need to adjust the cursor position to allow for the added '0' in front.
                }
                while (number.Length > 1)
                {
                    if (number.EndsWith("0"))
                    {
                        number = number.Remove(number.Length - 1);       // Remove trailing zero.
                    }
                    else
                    {
                        if (number.EndsWith("."))
                        {
                            number = number.Remove(number.Length - 1);   // Remove trailing decimal.
                        }
                        break;  // Last character was not zero or decimal so end loop.
                    }
                }
            }
            return number;
        }

        /// <summary>
        /// Convert a double into a string with alternate scientific format.  Example: 123.456 becomes "1.23456 × 10²" versus system output of "1.2346E+2".
        /// This solution will not work with all fonts.  Segoe UI font works without issues.
        /// Found this solution at:  http://stackoverflow.com/questions/9442243/draw-string-with-normalized-scientific-notation-superscripted
        /// 
        /// </summary>
        /// <param name="number">Number to reformat.</param>
        /// <param name="showDigits">Set maximum number of digits to show after decimal.  Not used if EnumRemoveTrailingZeros=Yes.</param>
        /// <param name="EnumRemoveTrailingZeros">Option to remove any trailing zeros.</param>
        /// <returns></returns>
        public static string FormatAsPowerOfTen(double number, int showDigits, EnumRemoveTrailingZeros rtZeros)
        {
            bool numberIsNegative = false;
            if (number == 0d)
                return number.ToString();       // Return "0".  No formatting required.
            if (number < 0d)
            {
                numberIsNegative = true;        // number is negative.  Change sign for now.
                number = -number;
            }
            showDigits = Math.Abs(showDigits);  // Make sure showDigits is positive.
            if (showDigits > 15)                // Double precision is 15 digits maximum.
                showDigits = 15;
            // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). number={number}, showDigits={showDigits}, RTZeros={rtZeros}, numberIsNegative={numberIsNegative}");
            // Use Math.Floor to determine exponent that leaves one digit before decimal separator.
            // More info at: https://msdn.microsoft.com/en-us/library/e0b5f0xb(v=vs.110).aspx
            //
            int exp = Convert.ToInt32(Math.Floor(Math.Log10(number)));
            number /= Math.Pow(10, exp);    // Calculate significand and save back to number.
            // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). number={number}, exp={exp}");
            string fmt;
            if (rtZeros == EnumRemoveTrailingZeros.Yes)
            {
                fmt = "{0}";    // Build a format string.  Later fill it in with values.
                // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). First fmt={fmt}");
                if (exp != 0)   // If exp == 0, do not show scientific portion, " x 10^0", since not commonly used.
                {
                    fmt = fmt + " " + expCharTimes + " 10{1}";    // Use Unicode multiplication sign.  Yields "{0} × 10{1}".
                    // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). Second fmt={fmt}");
                }
                fmt = string.Format(fmt, number, ToSuperscript(exp));
                // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). Third fmt={fmt}, number={number}, ToSuperscript(exp)={ToSuperscript(exp)}");
            }
            else
            {
                fmt = string.Format("{{0:F{0}}}", showDigits);   // Yields "{0:F15}" when showDigits=15.
                // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). First fmt={fmt}");
                if (exp != 0)   // If exp == 0, do not show scientific portion, " x 10^0", since not commonly used.
                {
                    fmt = string.Concat(fmt, " " + expCharTimes + " 10{1}");    // Use Unicode multiplication sign.  Yields "{0:F15} × 10{1}" when showDigits=15.
                    // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). Second fmt={fmt}");
                }
                fmt = string.Format(fmt, number, ToSuperscript(exp));
                // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). Third fmt={fmt}, number={number}, ToSuperscript(exp)={ToSuperscript(exp)}");
            }
            if (numberIsNegative)   // Add negative sign back if removed above.
            {
                fmt = "-" + fmt;    // Convert string output of number back to negative again.
            }
            // Test ToSuperscript() method:
            // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). ToSuperscript(1234)={ToSuperscript(1234)}");
            // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). ToSuperscript(-1234)={ToSuperscript(-1234)}");
            // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). ToSuperscript(0)={ToSuperscript(0)}");
            // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). ToSuperscript(-0)={ToSuperscript(-0)}");
            // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). ToSuperscript(int.MaxValue)={ToSuperscript(int.MaxValue)}");
            // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). ToSuperscript(int.MinValue)={ToSuperscript(int.MinValue)}");
            // Debug.WriteLine($"LibNum.FormatAsPowerOfTen(). Final fmt={fmt}");
            return fmt;
        }

        /// <summary>
        /// Convert integer parameter superscript into matching string of Superscripts.  This method will convert all valid integers.
        /// </summary>
        /// <param name="superscript">Convert into matching string of superscripts.</param>
        /// <returns>String of superscripts.</returns>
        public static string ToSuperscript(int superscript)
        {
            if (superscript == 0)               // superscript = 0.  Special case so handle directly.
            {
                return "" + expChar0;
            }
            if (superscript == -2147483648)     // superscript = int.MinValue.  Special case so handle directly.  Removing minus sign will cause overflow.
            {
                return "" + expCharMinus + expChar2 + expChar1 + expChar4 + expChar7 + expChar4 + expChar8 + expChar3 + expChar6 + expChar4 + expChar8; // -2147483648
            }
            bool isNegative = false;
            if (superscript < 0)
            {
                isNegative = true;
                superscript = -superscript;     // Remove negative sign for now.
            }
            string result = string.Empty;
            string lastDigit;
            while (superscript != 0)
            {
                lastDigit = GetExponentDigit(superscript % 10).ToString();   // Break integer down one digit at a time and get matching superscript.
                superscript /= 10;              // Integer division drops fraction.  7 / 2 = 3.  Not 3.5 or 4.  No rounding occurs.
                result = lastDigit + result;    // Insert lastDigit before present string.  Digits are added in reverse.
            }
            // Add negative sign back if removed above.  Insert unicode SUPERSCRIPT minus at beginning of exponent string.
            if (isNegative)
            {
                result = expCharMinus + result;
            }
            return result;
        }

        /// <summary>
        /// Used by FormatExponentWithSuperscript() to format exponential string.  Uses Seqoe UI font character map.  Other fonts may or may not work.
        /// </summary>
        /// <param name="digit">Get exponential characer that matches digit value. Valid values equal 0 through 9.</param>
        /// <returns>Return exponential characer that matches digit value.</returns>
        public static char GetExponentDigit(int digit)
        {
            switch (digit)
            {
                case 0:
                    return expChar0;    // ⁰
                case 1:
                    return expChar1;    // ¹
                case 2:
                    return expChar2;    // ²
                case 3:
                    return expChar3;    // ³
                case 4:
                    return expChar4;    // ⁴
                case 5:
                    return expChar5;    // ⁵
                case 6:
                    return expChar6;    // ⁶
                case 7:
                    return expChar7;    // ⁷
                case 8:
                    return expChar8;    // ⁸
                case 9:
                    return expChar9;    // ⁹
                default:    // Throw exception so error can be discovered and corrected.
                    throw new NotSupportedException($"LibNum.GetExponentDigit(): digit={digit} not found in switch statement.");
            }
        }

        /*********************************************************************************************************************************************************
         * The following methods check the equality of floating point types, decimal, double, and float.  These methods check equality that allow for small 
         * differences between the numbers.  EqualByValue() methods are superior but are not supported by decimal types.  EqualByRounding() methods could 
         * potentially have some issues regarding midpoint rounding.  MidpointRounding AwayFromZero is used. Rounding AwayFromZero is the most widely known form 
         * of rounding but is not default.   Rounding AwayFromZero example, 3.75 rounds to 3.8, 3.85 rounds to 3.9, -3.75 rounds to -3.8, and -3.85 rounds to -3.9. 
         * For more information see https://msdn.microsoft.com/en-us/library/system.math.round(v=vs.110).aspx
         */

        // Following code found at https://msdn.microsoft.com/en-us/library/system.double(v=vs.110).aspx
        /// <summary>
        /// Double override.  Check the equality of two doubles.  If the relative difference between them is less than or equal to the 
        /// difference parameter then returns true.  False otherwise.  Doubles have 15 digits of precision.
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="difference">Maximum relative difference between the two numbers. Example=0.000000001</param>
        /// <returns></returns>
        public static bool EqualWithinDifference(double num1, double num2, double difference)
        {
            if (num1.Equals(num2))      // If the doubles are equal just return true.
                return true;
            if (double.IsInfinity(num1) | double.IsNaN(num1))       // Handle NaN, Infinity.
                return num1.Equals(num2);
            else if (double.IsInfinity(num2) | double.IsNaN(num2))
                return num1.Equals(num2);
            double divisor = Math.Max(num1, num2);      // Handle zero to avoid division by zero
            if (divisor.Equals(0))
                divisor = Math.Min(num1, num2);
            return Math.Abs(num1 - num2) / divisor <= difference;  // Return true or false.
        }

        // Following code found at https://msdn.microsoft.com/en-us/library/system.single(v=vs.110).aspx
        /// <summary>
        /// Float override.  Check the equality of two floats.  If the relative difference between them is less than or equal to the 
        /// difference parameter then returns true.  False otherwise.  Floats have 7 digits of precision.
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="difference">Maximum relative difference between the two numbers. Example=0.000001f</param>
        /// <returns></returns>
        public static bool EqualWithinDifference(float num1, float num2, float difference)
        {
            if (num1.Equals(num2))      // If the floats are equal just return true.
                return true;
            if (double.IsInfinity(num1) | double.IsNaN(num1))       // Handle NaN, Infinity.
                return num1.Equals(num2);
            else if (double.IsInfinity(num2) | double.IsNaN(num2))
                return num1.Equals(num2);
            double divisor = Math.Max(num1, num2);      // Handle zero to avoid division by zero
            if (divisor.Equals(0))
                divisor = Math.Min(num1, num2);
            return Math.Abs(num1 - num2) / divisor <= difference;  // Return true or false.
        }

        // For more information see: https://msdn.microsoft.com/en-us/library/zy06z30k(v=vs.110).aspx
        /// <summary>
        /// Decimal override.  Check the equality of two decimals.  Round both values to the number of decimals specified in the 
        /// rounding parameter before the equality comparision.  Decimals have 28 digits of precision.   
        /// This method could potentially have some issues regarding midpoint rounding.  MidpointRounding.AwayFromZero is used.
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="rounding">Number of digits to round too before equality comparison.</param>
        /// <returns></returns>
        public static bool EqualByRounding(decimal num1, decimal num2, int rounding)
        {
            if (rounding < 0)   // Check rounding range to prevent exception.
                rounding = 0;
            if (rounding > 28)
                rounding = 28;
            num1 = Math.Round(num1, rounding, MidpointRounding.AwayFromZero);
            num2 = Math.Round(num2, rounding, MidpointRounding.AwayFromZero);
            return num1.Equals(num2);           // Return true or false.
        }

        // For more information see: https://msdn.microsoft.com/en-us/library/75ks3aby(v=vs.110).aspx
        /// <summary>
        /// Double override.  Check the equality of two doubles.  Round both values to the number of decimals specified in the 
        /// rounding parameter before the equality comparision.  Doubles have 15 digits of precision.   
        /// This method could potentially have some issues regarding midpoint rounding.  MidpointRounding.AwayFromZero is used.
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="rounding">Number of digits to round too before equality comparison.</param>
        /// <returns></returns>
        public static bool EqualByRounding(double num1, double num2, int rounding)
        {
            if (rounding < 0)   // Check rounding range to prevent exception.
                rounding = 0;
            if (rounding > 15)
                rounding = 15;
            num1 = Math.Round(num1, rounding, MidpointRounding.AwayFromZero);
            num2 = Math.Round(num2, rounding, MidpointRounding.AwayFromZero);
            return num1.Equals(num2);           // Return true or false.
        }

        /// <summary>
        /// Float Override.  Check the equality of two floats.  Round both values to the number of decimals specified in the 
        /// rounding parameter before the equality comparision.  Floats have 7 digits of precision.   
        /// This method could potentially have some issues regarding midpoint rounding.  MidpointRounding.AwayFromZero is used.
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="rounding">Number of digits to round too before equality comparison.</param>
        /// <returns></returns>
        public static bool EqualByRounding(float num1, float num2, int rounding)
        {
            if (rounding < 0)   // Check rounding range to prevent exception.
                rounding = 0;
            if (rounding > 7)
                rounding = 7;
            //Note: Casting float to double does not result in new number with just more zeros as expected!  Must cast float to decimal first to get that result.
            decimal num1Decimal = (decimal)num1;
            decimal num2Decimal = (decimal)num2;
            // Debug.WriteLine($"LibNum.EqualByRounding(float): num1={num1}, num2={num2}, Cast to decimal. num1Decimal={num1Decimal}, num2Decimal={num2Decimal}");
            num1 = (float)Math.Round(num1Decimal, rounding, MidpointRounding.AwayFromZero);
            num2 = (float)Math.Round(num2Decimal, rounding, MidpointRounding.AwayFromZero);
            // Debug.WriteLine($"LibNum.EqualByRounding(float): After rounding to {rounding} digits, num1={num1} and num2={num2}.  Equal={num1.Equals(num2)}");
            return num1.Equals(num2);           // Return true or false.
        }

        /*********************************************************************************************************************************************************
         * Following methods truncate floating-point numbers to specified number of digits.  These methods do no rounding.
         * Methods verified by: Sample08_Test_Truncate_Methods
         */

        /// <summary>
        /// Truncate floating-point number to specified number of digits.  Method does no rounding.
        /// Method will return truncated value or original value if truncation did not occur.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static decimal TruncateFloatingPoint(decimal input, int digits)
        {
            if (digits < 0)
                return input;
            string num = input.ToString(fpNumericFormatNone);
            int numLen = num.Length;
            if (num.Contains("."))
            {
                int retainLen = num.IndexOf('.') + 1 + digits;  // Get location of decimal and calc length of string to retain.
                if (retainLen < numLen)                         // retainLen must be less than numLen or exception thrown.
                {
                    num = num.Remove(retainLen);                // Remove end of string.
                    if (decimal.TryParse(num, out input))       // Try to convert the modified string back into a decimal.
                        return input;                           // Return the truncated value.
                }
            }
            return input;
        }
        
        /// <summary>
        /// Truncate floating-point number to specified number of digits.  Method does no rounding.
        /// Method will return truncated value or original value if truncation did not occur.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double TruncateFloatingPoint(double input, int digits)
        {
            if (digits < 0)
                return input;
            string num = input.ToString(fpNumericFormatNone);
            int numLen = num.Length;
            if (num.Contains("."))
            {
                int retainLen = num.IndexOf('.') + 1 + digits;  // Get location of decimal and calc length of string to retain.
                if (retainLen < numLen)                         // retainLen must be less than numLen or exception thrown.
                {
                    num = num.Remove(retainLen);                // Remove end of string.
                    if (double.TryParse(num, out input))        // Try to convert the modified string back into a decimal.
                        return input;                           // Return the truncated value.
                }
            }
            return input;
        }

        /// <summary>
        /// Truncate floating-point number to specified number of digits.  Method does no rounding.
        /// Method will return truncated value or original value if truncation did not occur.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static float TruncateFloatingPoint(float input, int digits)
        {
            if (digits < 0)
                return input;
            string num = input.ToString(fpNumericFormatNone);
            int numLen = num.Length;
            if (num.Contains("."))
            {
                int retainLen = num.IndexOf('.') + 1 + digits;  // Get location of decimal and calc length of string to retain.
                if (retainLen < numLen)                         // retainLen must be less than numLen or exception thrown.
                {
                    num = num.Remove(retainLen);                // Remove end of string.
                    if (float.TryParse(num, out input))         // Try to convert the modified string back into a decimal.
                        return input;                           // Return the truncated value.
                }
            }
            return input;
        }

        /*********************************************************************************************************************************************************
         * The following two methods attempt to do selective rounding to decimals to correct inherent errors from floating-point operations.
         * Methods verified by RoundingTester project via many samples
         */

        /// <summary>
        /// Apply selective rounding to decimals to correct inherent errors from floating-point operations.
        /// If rounding can not be completed, then original value is returned.
        /// </summary>
        /// <param name="valueToRound">Value to attempt correction on.</param>
        /// <returns></returns>
        public static decimal RoundOutput(decimal valueToRound)
        {
            // See RoundingTester project for various samples.
            // Debug.WriteLine($"LibNum.RoundOutput(): Begin method. valueToRound={valueToRound.ToString(fpNumericFormatNone)}");
            // CASE Zero: Return if valueToRound is zero.
            if (valueToRound == 0)
            {
                // Debug.WriteLine($"LibNum.RoundOutput(): CASE Zero: valueToRound={valueToRound.ToString(fpNumericFormatNone)}");
                return valueToRound;    // Return zero.
            }
            // CASE Integer: Return if valueToRound is integer.
            if (valueToRound == Math.Round(valueToRound))
            {
                // Debug.WriteLine($"LibNum.RoundOutput(): CASE Integer: valueToRound={valueToRound.ToString(fpNumericFormatNone)}");
                return valueToRound;    // Return integer.
            }
            // CASE Decimal: checkLength=18, trashDigits=9, minPatternLen=3.  Best values to use determined via considerable trial and error.
            int checkLength = 18;
            int trashDigits = 9;
            int minPatternLen = 3;
            if (FloatingPointCorrection(valueToRound, out decimal roundResult1, checkLength, trashDigits, minPatternLen))
            {
                // Call FloatingPointCorrection() again for occasional double-pattern cases similar to: 1 second to nanosecond = 999999999.99999992000000008, which should round to 1000000000.
                if (FloatingPointCorrection(roundResult1, out decimal roundResult2, checkLength, trashDigits, minPatternLen))
                {
                    // Debug.WriteLine($"LibNum.RoundOutput(): CASE Decimal P2: roundResult2={roundResult2}, valueToRound={valueToRound.ToString(fpNumericFormatNone)}, checkLength={checkLength}, trashDigits={trashDigits}, minPatternLen={minPatternLen}");
                    return roundResult2;
                }
                else
                {
                    // Debug.WriteLine($"LibNum.RoundOutput(): CASE Decimal P1: roundResult1={roundResult1}, valueToRound={valueToRound.ToString(fpNumericFormatNone)}, checkLength={checkLength}, trashDigits={trashDigits}, minPatternLen={minPatternLen}");
                    return roundResult1;
                }
            }
            // CASE None: Code above did not return so no rounding occurred. Return unchanged parameter valueToRound.
            // Debug.WriteLine($"LibNum.RoundOutput(): CASE None: No rounding done and end of method. Returned valueToRound={valueToRound.ToString(fpNumericFormatNone)}");
            return valueToRound;    // Never make situation worse so return original value.
        }

        /// <summary>
        /// Attempt to correct round-off error of a floating-point calculation to show true result.  Integers are ignored.  
        /// Method behaves similar to TryParse methods, returns true if correction made, false otherwise.
        /// This lower level method can be called directly but RoundOutput() above is higher level and easier to use and understand.
        /// </summary>
        /// <param name="resultToCorrect">Value to attempt correction on.</param>
        /// <param name="correctedResult">Out parameter. Corrected value placed here. Zero otherwise.</param>
        /// <param name="lenCheck">Attempt rounding on decimals that have string length >= this value.  Ignore otherwise. Valid range 4 to 30.</param>
        /// <param name="maxTrashDigits">Digits at end of decimal string that are not part of pattern plus 1. Valid range 2 to 13.</param>
        /// <param name="lenMinimumPattern">Minimum pattern of matching digits, default is 2. 3 matching digits is rare. Valid range 2 to 6.</param>
        /// <returns>True if correction made.  False otherwise.</returns>
        public static bool FloatingPointCorrection(decimal resultToCorrect, out decimal correctedResult, int lenCheck, int maxTrashDigits, int lenMinimumPattern = 2)
        {
            // This method attempts to correct decimal results similar to following cases.  See RoundingTester project for many samples.  precision = lenPatternOffset + lenPattern.
            // [359.99999999999999999999999969],  lenResultToCorrect=[30], precision=[24], lenPatternOffset=[0], lenPattern=[24]. 1 revolution to degree corrects to 360.
            // [-359.99999999999999999999999969], lenResultToCorrect=[31], precision=[24], lenPatternOffset=[0], lenPattern=[24]. 1 revolution to degree corrects to 360.
            // [3.5999999999999999999999999997],  lenResultToCorrect=[30], precision=[27], lenPatternOffset=[1], lenPattern=[26]. 1 meter/sec to kilometer/hour corrects to 3.6.
            // [0.0249999999999999999999996133],  lenResultToCorrect=[30], precision=[24], lenPatternOffset=[3], lenPattern=[21]. 0.15 arcsecond to arcminute corrects to 0.025.
            // [0.1499999999999999999999999978],  lenResultToCorrect=[30], precision=[26], lenPatternOffset=[2], lenPattern=[24]. 0.0015 quadrant to gradian corrects to 0.15.
            // [100.0000000000000000000000001],   lenResultToCorrect=[29], precision=[23], lenPatternOffset=[0], lenPattern=[23]. 1 quadrant to gradian corrects to 100.
            // [11.999999999999999999999999999],  lenResultToCorrect=[30], precision=[26], lenPatternOffset=[0], lenPattern=[26]. 1 revolution to sign corrects to 12.
            // [8000000000000000000000.000008],   lenResultToCorrect=[29], precision=[5],  lenPatternOffset=[0], lenPattern=[5].  2000000... revolution to gradian corrects to 8000000...

            // Set some reasonable limits on parameters.
            // 4 is minimum value that will be rounded and has form of x.yy. 30 is maximum digits a decimal will return.  lenCheck is increased by 1 if resultToCorrect negative.
            if (lenCheck < 4 || lenCheck > 30)
                throw new ArgumentOutOfRangeException("LibNum.FloatingPointCorrection(): Invalid lenCheck parameter: Value < 4 or > 30.");
            // 2 is minimum trash digits need for rounding process to occur.  9 is maximum seen so do not see need for more than 13.  13 can be adjusted if needed.
            if (maxTrashDigits < 2 || maxTrashDigits > 13)
                throw new ArgumentOutOfRangeException("LibNum.FloatingPointCorrection(): Invalid maxTrashDigits parameter: Value < 2 or > 13.");
            // 2 is minimum pattern of matching digits to reduce chance of false negatives if any trash digits repeat.  3 is very rare so do not see need for more than 6.  6 can be adjusted if needed.
            if (lenMinimumPattern < 2 || lenMinimumPattern > 6)
                throw new ArgumentOutOfRangeException("LibNum.FloatingPointCorrection(): Invalid lenMinimumPattern parameter: Value < 2 or > 6.");
            correctedResult = 0;    // Initialize 'out' parameter to zero here assuming correction fails.  Do not change unless otherwise.
            // Need to check if resultToCorrect is negative.  If not, rare negative cases where 'lenCheck - 1' will not be corrected when they should be.
            if (resultToCorrect < 0)
                lenCheck++;         // Compensate for '-' sign if resultToCorrect is negative
            string fraction = resultToCorrect.ToString(fpNumericFormatNone);
            // Negative numbers will increase lenResultToCorrect by one character since '-' is part of result.
            int lenResultToCorrect = fraction.Length;
            // Debug.WriteLine($"LibNum.FloatingPointCorrection(): fraction={fraction}, lenResultToCorrect={lenResultToCorrect}, lenCheck={lenCheck}");
            // Skip following if lenResultToCorrect < lenCheck and fraction does not contain a period.
            // In these case resultToCorrect does not require any rounding.
            if (lenResultToCorrect >= lenCheck && fraction.Contains("."))
            {
                // Get fraction digits and remove negative sign if present.
                fraction = Math.Abs(resultToCorrect - Math.Truncate(resultToCorrect)).ToString(fpNumericFormatNone);
                fraction = fraction.Remove(0, 2);   // Remove the leading "0." so all left are the digits of the fraction.
                // Debug.WriteLine($"LibNum.FloatingPointCorrection(): integer={integer}, fraction={fraction}");
                int count = fraction.Length;
                // Reverse the digits to work string backwards. Easier to dispose of bad trailing digits this way.
                if (count > 1)
                {
                    // Debug.WriteLine($"LibNum.FloatingPointCorrection(): Before reverse, fraction={fraction}");
                    // Reverse string fraction.
                    // At this point, reversed fraction should look similar to following samples.
                    // Sample: 359.99999999999999999999999969 becomes 96999999999999999999999999
                    // Sample: 0.0249999999999999999999996133 becomes 3316999999999999999999999420
                    // Sample: 1296000.0000000000000000109664 becomes 4669010000000000000000
                    // Sample: 59.999999999999999999999992093 becomes 390299999999999999999999999
                    fraction = new string(fraction.ToCharArray().Reverse().ToArray());
                    // Debug.WriteLine($"LibNum.FloatingPointCorrection(): After reverse, fraction={fraction}");
                }
                // Debug.WriteLine($"LibNum.FloatingPointCorrection(): Reversed digits of fraction={fraction}, count={count}, lenMinimumPattern={lenMinimumPattern}");
                char c = fraction[0];        // Set inital comparison value.  lenPattern now equals 1.
                char test = c;
                int lenPattern = 1;                 // Set to 1 before beginning since value was set above.
                int lenPatternOffset = 0;           // Set to zero before beginning.
                int trashDigits = 1;                // Initialize trashDigits counter.
                int trashDigitsIntermediate = 0;    // intermediate trash counter.
                int precision;                      // precision = lenPatternOffset + lenPattern;
                bool patternEstablished = false;    // Do not reset pattern if one has been established.
                // string fractionUsed = test.ToString();      // This is debug code. Comment out when working. Set initial value to test value.
                // Debug.WriteLine($"LibNum.FloatingPointCorrection(): Debug #01: i=[00], c={c}, test={test}, lenPatternOffset={lenPatternOffset}, lenPattern={lenPattern}, trashDigits={trashDigits}, maxTrashDigits={maxTrashDigits}, fractionUsed={fractionUsed}");
                for (int i = 1; i < count; i++)   // Run thru string after decimal seeking a pattern.  First value of string, i=0, was set above.
                {
                    c = fraction[i];
                    if (!patternEstablished && c == test)   // Build the pattern while c=test.
                    {
                        lenPattern++;
                        trashDigitsIntermediate++;
                        // fractionUsed += c;      // This is debug code. Comment out when working correct.
                        // Debug.WriteLine($"LibNum.FloatingPointCorrection(): Debug #02: i={i.ToString("D2")}, c={c}, test={test}, patternEstablished={patternEstablished}, lenPattern={lenPattern}, lenPatternOffset={lenPatternOffset}, trashDigitsIntermediate={trashDigitsIntermediate}, fractionUsed={fractionUsed}");
                    }
                    else
                    {
                        if (lenPattern <= lenMinimumPattern)    // Test length of pattern.
                        {
                            test = c;
                            lenPattern = 1;                 // Reset to 1 since not a valid pattern.
                            trashDigits = trashDigits + 1 + trashDigitsIntermediate;
                            trashDigitsIntermediate = 0;    // Reset counter.
                            // fractionUsed = c.ToString();    // This is debug code. Comment out when working. Reset debug string to new pattern test value.
                            // Debug.WriteLine($"LibNum.FloatingPointCorrection() Debug #03: i={i.ToString("D2")}, c={c}, test={test}, patternEstablished={patternEstablished}, lenPattern={lenPattern}, trashDigits={trashDigits}, maxTrashDigits={maxTrashDigits}, fractionUsed={fractionUsed}");
                            if (trashDigits > maxTrashDigits)
                                break;      // Exit loop and return false.  This is intended to prevent second pattern from developing on long fractions.
                        }
                        else    // lenPattern >= lenMinimumPattern
                        {
                            lenPatternOffset++;
                            patternEstablished = true;          // Found valid pattern.
                            // fractionUsed += c;    // This is debug code. Comment out when working correct.
                            // Debug.WriteLine($"LibNum.FloatingPointCorrection() Debug #04: i={i.ToString("D2")}, c={c}, test={test}, patternEstablished={patternEstablished}, lenPattern={lenPattern}, lenPatternOffset={lenPatternOffset}, fractionUsed={fractionUsed}");
                        }
                    }
                }
                // Debug.WriteLine($"LibNum.FloatingPointCorrection(): Loop results: lenMinimumPattern={lenMinimumPattern}, lenPatternOffset={lenPatternOffset}, lenPattern={lenPattern}");
                if (lenPattern > 1)
                {
                    precision = lenPatternOffset + lenPattern;
                    // Debug.WriteLine($"LibNum.FloatingPointCorrection(): precision={precision}, lenPatternOffset={lenPatternOffset}, lenPattern={lenPattern}");
                    decimal truncated = TruncateFloatingPoint(resultToCorrect, precision);  // Chop off trash digits.
                    // On extreamly small numbers like 0.0000000000000000000000359989 (potentially should round to 0.000000000000000000000036 but "99" following '5' does 
                    // not meet lenMinimumPattern = 3 requirement), above loop calculates all trailing digits as trash and returns nothing but a pattern of leading 0's.
                    // Therefore truncated = 0.
                    if (truncated != 0)
                    {
                        // Debug.WriteLine($"LibNum.FloatingPointCorrection() Chopped trailing digits off of resultToCorrect, result is truncated={truncated}");
                        precision--;    // Before rounding, need to step precision back by one digit so last digit can bes used for rounding.  Otherwise value will not round.
                        decimal round1 = Math.Round(resultToCorrect, lenPatternOffset, MidpointRounding.AwayFromZero);
                        // Debug.WriteLine($"LibNum.FloatingPointCorrection(): round1={round1} using length lenPatternOffset={lenPatternOffset}");
                        decimal round2 = Math.Round(truncated, precision, MidpointRounding.AwayFromZero);
                        // Debug.WriteLine($"LibNum.FloatingPointCorrection(): round2={round2} using length precision={precision}");
                        if (round1.CompareTo(round2) == 0)
                        {
                            correctedResult = round1;
                            // Debug.WriteLine($"LibNum.FloatingPointCorrection(): Correction made and returned true. correctedResult={correctedResult}");
                            return true;
                        }
                    }
                    else    // Compiler will likely eliminate this else statement in release mode since nothing will be in it.
                    {
                        // Debug.WriteLine($"LibNum.FloatingPointCorrection(): Returned without correction since truncated={truncated} was zero");
                    }
                }
            }
            // Out parameter 'correctedResult' was initialized to zero above.  If still zero then correction failed.
            // Debug.WriteLine($"LibNum.FloatingPointCorrection(): End method. Corrections NOT made and returned false. correctedResult={correctedResult}");
            return false;
        }

    }
}
