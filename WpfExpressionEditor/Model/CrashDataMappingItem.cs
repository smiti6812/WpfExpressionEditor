using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string RuleAsText { get; set; } = "Create Rules";

        private IList<ExpressionAdapter> treeList;

        public IList<ExpressionAdapter> TreeList
        {
            get
            {
                return treeList;
            }
            set
            {
                treeList = value;
                OnPropertyChanged(nameof(TreeList));
            }
        }
    }
}
