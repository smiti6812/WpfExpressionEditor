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

using GalaSoft.MvvmLight.Messaging;
using WpfExpressionEditor.Model;

namespace WpfExpressionEditor.ViewModel
{
    public static class TextBoxHelper
    {
        public static string GetSelectedText(DependencyObject obj) => (string)obj.GetValue(SelectedTextProperty);
        public static void SetSelectedText(DependencyObject obj, string value) => obj.SetValue(SelectedTextProperty, value);
        public static int GetSelectionStart(DependencyObject obj) => int.Parse(obj.GetValue(SelectionStartProperty).ToString());
        public static void SetSelectionStart(DependencyObject obj, int value) => obj.SetValue(SelectionStartProperty, value);

        public static readonly DependencyProperty SelectionStartProperty =
           DependencyProperty.RegisterAttached(
               "SelectionStart",
               typeof(int),
               typeof(TextBoxHelper),
               new FrameworkPropertyMetadata(
                   0,
                   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                   SelectionStartChanged));

        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.RegisterAttached(
                "SelectedText",
                typeof(string),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(
                    "Default",
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    SelectedTextChanged));

        

        private static void SelectionStartChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var textBox = dependencyObject as TextBox;
            int startSelection = (int)eventArgs.NewValue;
            Messenger.Default.Send(new NotificationMessage<int>(startSelection, "MsgSendStartSelection"));
            Messenger.Default.Send(new NotificationMessage<int>(startSelection, "MsgSendSelectionLength"));
            //MappingRuleEditorViewModel.Instance.ExpressionEditorViewModel.SelectionStart = startSelection;
            //MappingRuleEditorViewModel.Instance.ExpressionEditorViewModel.SelectionLength = textBox.SelectionLength;
        }
        private static void SelectedTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var element = dependencyObject as TextBox;
            FocusManager.SetFocusedElement(dependencyObject, element);
            FocusManager.SetIsFocusScope(dependencyObject, true);
            _ = Keyboard.Focus(element);

            string newValue = eventArgs.NewValue as string;
            if (newValue == null)
            {
                element.SelectionChanged += TextSelectionChanged;
            }

            if (!string.IsNullOrEmpty(newValue))
            {
                string[] newValueArray = newValue.Split(',');
                if (dependencyObject is not TextBox textBox)
                {
                    return;
                }

                if (newValueArray[0] is not null && (!newValueArray[0].StartsWith("And") && !newValueArray[0].StartsWith("Or")))
                {
                    int start = int.Parse(newValueArray[1]);
                    int length = newValueArray[0].Length;
                    if (start > -1)
                    {
                        textBox.Select(start, length);
                    }
                }
                else
                {
                    textBox.Select(0, 0);
                }
            }
        }
        private static void TextSelectionChanged(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            Messenger.Default.Send(new NotificationMessage<int>(textBox.SelectionStart, "MsgSendStartSelection"));
            Messenger.Default.Send(new NotificationMessage<int>(textBox.SelectionLength, "MsgSendSelectionLength"));
            //MappingRuleEditorViewModel.Instance.ExpressionEditorViewModel.SelectionStart = textBox.SelectionStart;
            //MappingRuleEditorViewModel.Instance.ExpressionEditorViewModel.SelectionLength = textBox.SelectionLength;
        }
    }
}
