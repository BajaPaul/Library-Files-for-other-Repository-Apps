This is a sample that shows how to update existing files to use new C# PatternMatching.
Code before C# 7.0 used Keyword 'as' that is now replaced in pattern matching situations with keyword 'is'.
This change requires some slight code juggling in order to update to new standard. This sample shows process.


using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SampleUWP.Common
{
    class PatternMatchingSample
    {

        public static void PatternMatchingSampleOld(Page page)
        {
            page.GotFocus += (object sender, RoutedEventArgs e) =>
            {
                // Old: Code before C# 7.0:  Keyword 'as' replaced in pattern matching situations with keyword 'is'.

                FrameworkElement focus = FocusManager.GetFocusedElement() as FrameworkElement;
                if (focus != null)
                {
                    Debug.WriteLine("Has focus: " + focus.Name + " (" + focus.GetType().ToString() + ")");
                }
                else
                {
                    // Throw exception so error can be discovered and corrected.
                    throw new ArgumentException("Method LibUM.PatterMatchingSampleOld: Exception occurred.");
                }
            };
        }


        public static void PatternMatchingSampleNew(Page page)
        {
            page.GotFocus += (object sender, RoutedEventArgs e) =>
            {
                // New: C# 7.0 using pattern matching and the keyword 'is' versus 'as' as shown above.

                if (FocusManager.GetFocusedElement() is FrameworkElement focus)
                {
                    Debug.WriteLine("Has focus: " + focus.Name + " (" + focus.GetType().ToString() + ")");
                }
                else
                {
                    // Throw exception so error can be discovered and corrected.
                    throw new ArgumentException("Method LibUM.PatterMatchingSampleNew: Exception occurred.");
                }
            };
        }

    }
}


