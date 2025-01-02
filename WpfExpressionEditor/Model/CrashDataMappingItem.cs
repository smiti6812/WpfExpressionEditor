using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

using MDSng_FileFormats.Schemas;

using WpfExpressionEditor.ViewModel;

namespace WpfExpressionEditor.Model
{
    public class CrashDataMappingItem : BindableBase
    {
        public string SensorLocation { get; set; }
        public string SensorDirection { get; set; }
        public string SensorRotator { get; set; }
        public string ChannelInfo { get; set; }
        public string Location { get; set; }

        public string AssignmentComment { get; set; }

        public string Key {  get; set; }

        private string ruleAsText = "Create Rules";

        public readonly string DefaultRuleTextValue = "Edit Rules";

        public bool IsInRuleEditingMode { get; set; }


        public string RuleAsText
        {
            get => ruleAsText;
            set
            {
                ruleAsText = value;
                OnPropertyChanged(nameof(RuleAsText));
                OnPropertyChanged(nameof(ExpressionAdapters));
            }
        }

        private ObservableCollection<ExpressionAdapterViewModel> expressionAdapters;

        public ObservableCollection<ExpressionAdapterViewModel> ExpressionAdapters
        {
            get => expressionAdapters;
            set
            {
                expressionAdapters = value;
                OnPropertyChanged(nameof(ExpressionAdapters));
            }
        }

        public void UpdateRuleText(string expressionText) => RuleAsText = expressionText;


        private IList<ExpressionAdapter> treeList;

        public IList<ExpressionAdapter> TreeList
        {
            get => treeList;
            set
            {
                treeList = value;
                OnPropertyChanged(nameof(TreeList));
            }
        }
    }
}
