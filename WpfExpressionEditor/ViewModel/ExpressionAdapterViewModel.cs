using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using WpfExpressionEditor.Interfaces;
using WpfExpressionEditor.Model;

namespace WpfExpressionEditor.ViewModel
{
    public class ExpressionAdapterViewModel : ViewModelBase
    {
        private ExpressionAdapterViewModel selectedItem;
      
        public int Level
        {
            get => expressionAdapterTree.Level;
            set
            {
                if (expressionAdapterTree.Level != value)
                {
                    expressionAdapterTree.Level = value;
                    RaisePropertyChanged(nameof(expressionAdapterTree.Level));
                }
            }
        }

        private IExpressionAdapterTree expressionAdapterTree;
        public IExpressionAdapterTree ExpressionAdapterTree
        {
            get => expressionAdapterTree;
            set
            {
                expressionAdapterTree = value;
                RaisePropertyChanged(nameof(ExpressionAdapterTree));
            }
        }

        private bool isReadOnly;
        public bool IsReadOnly
        {
            get => isReadOnly;
            set
            {
                isReadOnly = value;
                RaisePropertyChanged(nameof(IsReadOnly));
            }
        }

        private bool isVisible;

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                RaisePropertyChanged(nameof(IsVisible));
            }
        }
        public ExpressionAdapterViewModel Parent { get; set; }

        private ObservableCollection<ExpressionAdapterViewModel> children;

        public ObservableCollection<ExpressionAdapterViewModel> Children
        {
            get => children;
            set
            {
                children = value;
                RaisePropertyChanged(nameof(Children));
            }
        }

        public string Text
        {
            get => expressionAdapterTree.Text;
            set
            {
                if (expressionAdapterTree.Text != value)
                {
                    expressionAdapterTree.Text = value;
                    IsVisible = false;
                    RaisePropertyChanged(nameof(Text));
                    RaisePropertyChanged(nameof(IsVisible));
                }
            }
        }

        private string originalExpression;

        public ExpressionAdapterViewModel(IExpressionAdapterTree treeNode)
        {
            expressionAdapterTree = treeNode;
            IsReadOnly = treeNode.Level <= 1;

            if (treeNode.Level <= 1)
            {
                treeNode.UpdateExpressionAdapterRuleText = UpdateRootNodeText;
            }           
            GetSelectedItemCommand = new RelayCommand<ExpressionAdapterViewModel>(GetSelectedItem);
            EditSelectedItemCommand = new RelayCommand<ExpressionAdapterViewModel>(EditSelectedItem);
            HideSelectedItemCommand = new RelayCommand<ExpressionAdapterViewModel>(HideSelectedItem);
            LostFocusCommand = new RelayCommand(LostFocus);
        }

        public RelayCommand<ExpressionAdapterViewModel> GetSelectedItemCommand { get; set; }
        public RelayCommand<ExpressionAdapterViewModel> EditSelectedItemCommand { get; set; }
        public RelayCommand<ExpressionAdapterViewModel> HideSelectedItemCommand { get; set; }
        public RelayCommand LostFocusCommand { get; set; }
        private void LostFocus() => Text = !expressionAdapterTree.UpdateExpressionFromTree(originalExpression, Text) ? originalExpression : Text;
        public void UpdateRootNodeText(string newExpressionText) => Text = newExpressionText;

        private void EditSelectedItem(ExpressionAdapterViewModel _selectedItem)
        {
            if (Level > 1)
            {
                originalExpression = Text;
                _selectedItem.IsVisible = true;
            }
        }

        private void HideSelectedItem(ExpressionAdapterViewModel _selectedItem) => _selectedItem.IsVisible = false;

        private void GetSelectedItem(ExpressionAdapterViewModel _selectedItem)
        {
            selectedItem = _selectedItem;
            if (selectedItem.Text != EditorHelper.Or
                && selectedItem.Text != EditorHelper.And && !selectedItem.Text.StartsWith("Rule"))
            {
                Messenger.Default.Register<NotificationMessage<string>>(this, ReceiveExpressionText);
                Messenger.Default.Send("MsgRequestExpressionText");              
            }
        }

        private void ReceiveExpressionText(NotificationMessage<string> message)
        {
            if(message.Notification.Equals("MsgReceiveExpressionText"))
            {               
                string selectedTextExtended = "";
                string expressionText = message.Content;
                int startIndex;
                if (selectedItem.Parent.Children.Count == 1)
                {
                    startIndex = expressionText.IndexOf(selectedItem.Text);
                }
                else
                {
                    //Left
                    selectedTextExtended = expressionAdapterTree.CheckRightAndLeftSideOfExpressionIfAndOrOr(1) ?
                        selectedItem.Parent.Children[1].Text : selectedTextExtended;
                    //Right
                    selectedTextExtended += expressionAdapterTree.CheckRightAndLeftSideOfExpressionIfAndOrOr(0) ?
                     $" {selectedItem.Parent.Text} {selectedItem.Parent.Children[0].Text}" : "";
                    selectedTextExtended = selectedTextExtended.Trim();
                    startIndex = expressionText.IndexOf(selectedTextExtended) + selectedTextExtended.IndexOf(selectedItem.Text);
                }

                string selectedText = $"{selectedItem.Text},{(startIndex > -1 ? startIndex.ToString() : expressionText.Length.ToString())}";
                expressionAdapterTree.ReturnExpressionTreeTopDown().UpdateSelectedText(selectedText);
                Messenger.Default.Unregister<NotificationMessage<string>>(this, ReceiveExpressionText);
            }
        }
    }
}
