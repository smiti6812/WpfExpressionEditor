using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace WpfExpressionEditor.CustomCommands
{
    public class CustomCommands
    {
        public static readonly DependencyProperty DataGridSelectionChangedCommandProperty = DependencyProperty.RegisterAttached(
                                                                        "DataGridSelectionChangedCommand",
                                                                        typeof(ICommand),
                                                                        typeof(CustomCommands),
                                                                        new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridSelectionChanged)));

        public static void SetDataGridSelectionChangedCommand(DependencyObject target, ICommand value) => target.SetValue(DataGridSelectionChangedCommandProperty, value);

        public static ICommand GetDataGridSelectionChangedCommand(DependencyObject target) => (ICommand)target.GetValue(DataGridSelectionChangedCommandProperty);

        private static void DataGridSelectionChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is DataGrid control)
            {
                if ((e.NewValue != null) && (e.OldValue == null)) // If we're putting in a new command and there wasn't one already hook the event
                {
                    control.SelectionChanged += Datagrid_SelectionChanged;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))// If we're clearing the command and it wasn't already null unhook the event
                {
                    control.SelectionChanged -= Datagrid_SelectionChanged;
                }
            }
        }
        private static void Datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = (DataGrid)sender;
            System.Collections.IList selectedItems = control.SelectedItems;
            var command = (ICommand)control.GetValue(DataGridSelectionChangedCommandProperty);
            command.Execute(selectedItems);
        }
        public static readonly DependencyProperty IsTextSelectedProperty = DependencyProperty.RegisterAttached(
                                                                       "IsTextSelected",
                                                                       typeof(bool),
                                                                       typeof(CustomCommands),
                                                                       new FrameworkPropertyMetadata(IsTextSelectedChanged));

        public static void SetIsTextSelected(DependencyObject target, bool value) => target.SetValue(IsTextSelectedProperty, value);

        public static bool GetIsTextSelected(DependencyObject target) => (bool)target.GetValue(IsTextSelectedProperty);

        private static void IsTextSelectedChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = target as TextBox;
            FocusManager.SetFocusedElement(target, element);
            FocusManager.SetIsFocusScope(target, true);
            Keyboard.Focus(element);
        }
    }
}
