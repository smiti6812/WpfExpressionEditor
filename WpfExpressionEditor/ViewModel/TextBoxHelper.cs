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
        private const string SelectedTextPropertyDefault = " And ";

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

            //element.SelectionChanged += SelectionChangedForSelectedText;

            var newValue = eventArgs.NewValue as string;

            if (dependencyObject is not TextBox textBox)
            {
                return;
            }
            if (newValue is not null)
            {
                int length = newValue.Length;
                if (newValue.StartsWith("Or "))
                {
                    newValue = newValue.Substring(3, newValue.Length - 3);
                    length = newValue.Length;
                    int i = 0;
                    while (newValue[i] == '(')
                    {
                        i++;
                    }
                    length = length - i;
                    newValue = newValue.Substring(i, newValue.IndexOf("Or",1) - i);
                }
                else if (newValue.StartsWith("And "))
                {
                    newValue = newValue.Substring(4, newValue.Length - 4);
                }
                int start = textBox.Text.ToString().IndexOf(newValue.TrimStart(' '));
                textBox.Select(start, length);
            }
            /*
            var oldValue = eventArgs.OldValue as string;
            var newValue = eventArgs.NewValue as string;

            if (oldValue == SelectedTextPropertyDefault && newValue != SelectedTextPropertyDefault)
            {
                textBox.SelectionChanged += SelectionChangedForSelectedText;
            }
            else if (oldValue != SelectedTextPropertyDefault && newValue == SelectedTextPropertyDefault)
            {
                textBox.SelectionChanged -= SelectionChangedForSelectedText;
            }

            if (newValue is not null && newValue != textBox.SelectedText)
            {
                textBox.SelectedText = newValue;
            }
            */
        }

        private static void SelectionChangedForSelectedText(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is TextBox textBox)
            {
                //SetSelectedText(textBox, textBox.SelectedText);
                if (!string.IsNullOrEmpty(textBox.SelectedText))
                {
                    textBox.SelectionChanged -= SelectionChangedForSelectedText;
                    int start = textBox.Text.ToString().IndexOf(textBox.SelectedText);
                }
            }
        }
    }
}
