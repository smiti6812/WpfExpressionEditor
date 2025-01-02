using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using FluentNHibernate.Conventions;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using WpfExpressionEditor.Interfaces;
using WpfExpressionEditor.Model;

namespace WpfExpressionEditor.ViewModel
{
    public class ExpressionEditorViewModel : ExpressionEditorGenericViewModel<CrashDataMappingItem>, IDisposable
    {
        private Dictionary<string, Func<AutoMappingArguments, bool>> selectedRuleset;

        private ObservableCollection<CrashDataMappingItem> mappableChannelList;

        private ObservableCollection<CrashDataMappingItem> selectedItems;
        public CrashDataMappingItemViewModel CrashDataMappingItemViewModel { get; set; }
        public ExpressionEditorViewModel() : base()
        {
            Messenger.Default.Register<string>(this, ReceiveMessage);
            Messenger.Default.Register<NotificationMessage<IList<CrashDataMappingItem>>>(this, ReceiveMappableChannelList);
            Messenger.Default.Register<NotificationMessage<IList<CrashDataMappingItem>>>(this, ReceiveMappingRuleEditorSelectedItems);
        }
        public override void SaveAndSend()
        {
            if (string.IsNullOrEmpty(ExpressionText))
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to remove selected rules?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    RunUpdate();
                }
                else if (selectedItems != null && !selectedItems.IsEmpty())
                {
                    ExpressionText = selectedItems[0].RuleAsText.Replace(selectedItems[0].DefaultRuleTextValue, "");
                }
            }
            else
            {
                RunUpdate();
                Messenger.Default.Send(new NotificationMessage<IList<CrashDataMappingItem>>(mappableChannelList, "MsgChannelsOfSelectedCrash"));
                Messenger.Default.Send(new NotificationMessage<Dictionary<string, Func<AutoMappingArguments, bool>>>(selectedRuleset, "MsgSelectedRulesetUpdate"));
            }

            RuleTextOk = false;
        }
        public void RunUpdate()
        {
            ExpressionAdapterTree exprAdapterTree = new(Expression, 0, null);
            exprAdapterTree.UpdateSelectedText = UpdateSelectedText;
            ExpressionTree = Expression != null && RuleTextOk ? new ObservableCollection<ExpressionAdapterTree>() { exprAdapterTree } : null;
            ExpressionText = string.IsNullOrEmpty(ExpressionText) ? "" : ExpressionTree[0].Text.Replace("Rule:  ", "").TrimStart(' ').TrimEnd(' ');
            selectedRuleset = [];
            foreach (CrashDataMappingItem selectedItem in selectedItems)
            {
                selectedItem.ExpressionAdapters = [GetExpressionAdapterViewModelCollection(exprAdapterTree, new(exprAdapterTree, ExpressionText), selectedItem)];
                string ruleAsText = string.IsNullOrWhiteSpace(ExpressionText) ? selectedItem.DefaultRuleTextValue : ExpressionText;
                var updatedItem = mappableChannelList.Where(c => c.SensorLocation == selectedItem.SensorLocation && c.SensorDirection ==
                selectedItem.SensorDirection);
                updatedItem.ToList().ForEach(c => c.RuleAsText = ruleAsText);
                updatedItem.ToList().ForEach(c => c.ExpressionAdapters = string.IsNullOrEmpty(ExpressionText) ? null : selectedItem.ExpressionAdapters);
                selectedRuleset.Add(selectedItem.Key, ruleAsText == selectedItem.DefaultRuleTextValue ? null : ExpressionEditorHelper<AutoMappingArguments>.GetQueryFunc(ruleAsText));
            }
        }
        private void ReceiveMappableChannelList(NotificationMessage<IList<CrashDataMappingItem>> message)
        {
            if (message.Notification.Equals("MsgChannelsOfSelectedCrash"))
            {
                mappableChannelList = message.Content.ToObservableCollection();
            }
        }

        private void ReceiveMappingRuleEditorSelectedItems(NotificationMessage<IList<CrashDataMappingItem>> message)
        {
            if (message.Notification.Equals("MsgMappingRuleEditorSelectedItems"))
            {
                selectedItems = message.Content.ToObservableCollection();
            }
        }

        public ExpressionAdapterViewModel GetExpressionAdapterViewModelCollection(IExpressionAdapterTree tree, ExpressionAdapterViewModel root, CrashDataMappingItem crashDataMappingItem)
        {
            foreach (var node in tree.Children)
            {
                root.Children ??= [];
                ExpressionAdapterViewModel newAdapterViewModel = new(node, ExpressionText);
                newAdapterViewModel.ExpressionAdapterTree.UpdateExpressionText = UpdateExpressionText;
                newAdapterViewModel.ExpressionAdapterTree.UpdateExpressionText += crashDataMappingItem.UpdateRuleText;
                newAdapterViewModel.Parent = root;
                root.Children.Add(newAdapterViewModel);
                _ = GetExpressionAdapterViewModelCollection(node, newAdapterViewModel, crashDataMappingItem);
            }

            return root;
        }

        public void UpdateExpressionText(string expressionText) => ExpressionText = expressionText;

        private void ReceiveMessage(string message)
        {
            if (message.Equals("MsgProjectClosing"))
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Messenger.Default.Unregister<NotificationMessage<IList<CrashDataMappingItem>>>(this, ReceiveMappableChannelList);
                Messenger.Default.Unregister<NotificationMessage<IList<CrashDataMappingItem>>>(this, ReceiveMappingRuleEditorSelectedItems);           
            }
        }
    }
}
