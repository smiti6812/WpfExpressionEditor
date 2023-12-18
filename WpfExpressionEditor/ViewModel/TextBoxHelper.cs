using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace WpfExpressionEditor.ViewModel
{
    public static class TextBoxHelper
    {
        private const string SelectedTextPropertyDefault = "And ";

        public static string GetSelectedText(DependencyObject obj) => (string)obj.GetValue(SelectedTextProperty);

        public static void SetSelectedText(DependencyObject obj, string value) => obj.SetValue(SelectedTextProperty, value);

        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.RegisterAttached(
                "SelectedText",
                typeof(string),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(
                    SelectedTextPropertyDefault,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    SelectedTextChanged));

        private static void SelectedTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            
            var element = dependencyObject as TextBox;
            FocusManager.SetFocusedElement(dependencyObject, element);
            FocusManager.SetIsFocusScope(dependencyObject, true);
            Keyboard.Focus(element);
           
            string newValue = eventArgs.NewValue as string;

            if (dependencyObject is not TextBox textBox)
            {
                return;
            }
            if (newValue is not null)
            {
                int length = newValue.Length;
                if (newValue.StartsWith("Or ") || newValue.StartsWith("And "))
                {
                    newValue = newValue.StartsWith("Or ") ? newValue.Substring(3, newValue.Length - 3) : newValue.Substring(4, newValue.Length - 4);
                    length = newValue.Length;
                    int i = 0;
                    while (newValue[i] == '(')
                    {
                        i++;
                    }
                    length = length - i;
                    int nextClosingBracket = newValue.IndexOf(")", 1);               
                    newValue = newValue.Substring(i, nextClosingBracket - i);
                }

                newValue = newValue.TrimStart(' ').TrimEnd(' ').Replace("==","Equal");
                int start = textBox.Text.ToString().IndexOf(newValue);
                if (start > -1)
                {
                    textBox.Select(start, length);
                }
            }            
        }       
    }
}
