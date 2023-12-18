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

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using WpfExpressionEditor.Model;

namespace WpfExpressionEditor.ViewModel
{
    public class ExpressionEditorViewModel : ExpressionEditorGenericViewModel<CrashDataMappingItem>
    {
        public CrashDataMappingItemViewModel CrashDataMappingItemViewModel { get; set; }
        public ExpressionEditorViewModel() : base()
        {
            
        }
        public override void SaveAndSend()
        {   
            Item.RuleAsText = string.IsNullOrEmpty(ExpressionText) ? "Create Rules" : ExpressionText;
            TreeList = Expression != null && RuleTextOk ? new List<ExpressionAdapter>() { new ExpressionAdapter(Expression, this) } : null;
            foreach (var selectedItem in CrashDataMappingItemViewModel.SelectedItems)
            {
                var updatedItem = CrashDataMappingItemViewModel.CrashDataMappingItems.Where(c => c.SensorLocation == selectedItem.SensorLocation && c.SensorDirection == selectedItem.SensorDirection
                && c.ChannelInfo == selectedItem.ChannelInfo).ToList();
                updatedItem.ForEach(c => c.RuleAsText = Item.RuleAsText);
                updatedItem.ForEach(c => c.TreeList = string.IsNullOrEmpty(ExpressionText) ? null : TreeList.ToObservableCollection<ExpressionAdapter>());
            }

            RuleTextOk = false;
        }       
    }
}
