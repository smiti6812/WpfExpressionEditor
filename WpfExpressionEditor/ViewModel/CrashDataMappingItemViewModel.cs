using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using WpfExpressionEditor.Model;

namespace WpfExpressionEditor.ViewModel
{
    public class CrashDataMappingItemViewModel : BindableBase
    {
        public ExpressionEditorViewModel ExpressionEditorViewModel { get; set; }
        public RelayCommand OpenExpressionEditorCommand { get; set; }
        public RelayCommand<IList> SelectionChangedCommand { get; set; }
        public RelayCommand<CrashDataMappingItem> EditRulesCommand { get; set; }
        public CrashDataMappingItemViewModel()
        {
            LoadCrashDataMappingItems();
            OpenExpressionEditorCommand = new RelayCommand(OnOpenEditor, CanOpen);
            SelectionChangedCommand = new RelayCommand<IList>(SelectionChanged);
            ExpressionEditorViewModel = new ExpressionEditorViewModel();
            EditRulesCommand = new RelayCommand<CrashDataMappingItem>(EditRules);
        }
        private void EditRules(CrashDataMappingItem item)
        {
            string rules = "";
        }

        private void SelectionChanged(IList selectedItems)
        {
            ExpressionEditorViewModel.IsVisible = false;
            ExpressionEditorViewModel.ExpressionText = "";
            ExpressionEditorViewModel.RuleTextOk = false;
            SelectedItems = new List<CrashDataMappingItem>(selectedItems.Cast<CrashDataMappingItem>());
        }
        public List<CrashDataMappingItem> SelectedItems { get; set; }

        private bool CanOpen() => SelectedCrashDataMappingItem != null;
        private void OnOpenEditor()
        {
            ExpressionEditorViewModel.Item = SelectedCrashDataMappingItem;
            ExpressionEditorViewModel.Items = CrashDataMappingItems;
            ExpressionEditorViewModel.IsVisible = true;
            ExpressionEditorViewModel.CrashDataMappingItemViewModel = this;
            ExpressionEditorViewModel.ExpressionText = SelectedCrashDataMappingItem.RuleAsText != "Create Rules" ? SelectedCrashDataMappingItem.RuleAsText : "";
        }

        private ObservableCollection<CrashDataMappingItem> crashDataMappingItems;
        public ObservableCollection<CrashDataMappingItem> CrashDataMappingItems
        {
            get => crashDataMappingItems;
            set
            {
                crashDataMappingItems = value;
                OnPropertyChanged(nameof(CrashDataMappingItems));
            }
        }

        private CrashDataMappingItem selectedCrashDataMappingItem;
        public CrashDataMappingItem SelectedCrashDataMappingItem
        {
            get => selectedCrashDataMappingItem;
            set
            {
                selectedCrashDataMappingItem = value;
                OpenExpressionEditorCommand.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(SelectedCrashDataMappingItem));
            }
        }
        public void LoadCrashDataMappingItems()
        {

            CrashDataMappingItems = new ObservableCollection<CrashDataMappingItem>()
            {
                new CrashDataMappingItem{ SensorLocation = "ECU-45",SensorDirection = "-M45",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ SensorLocation = "ECU+45",SensorDirection = "P45",SensorRotator = "",ChannelInfo = "15TUNNCSMIRDACYPTunnel Central Crash Sensor Unit Acc Next To Y" },
                new CrashDataMappingItem{ SensorLocation = "ECUY_lowg",SensorDirection = "-Y",SensorRotator = "",ChannelInfo = "15TUNNCSMIRDACYPTunnel Central Crash Sensor Unit Acc Next To Y" },
                new CrashDataMappingItem{ SensorLocation = "ECUZ_lowg",SensorDirection = "-Z",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACZPTunnel Central Crash Sensor Unit Acc Z" },
                new CrashDataMappingItem{ SensorLocation = "VdsX",SensorDirection = "-X",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ SensorLocation = "RollrateX",SensorDirection = "-Y",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ SensorLocation = "PASFL",SensorDirection = "Y",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "PASFR",SensorDirection = "-Y",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "PASML",SensorDirection = "Y",SensorRotator = "",ChannelInfo = "15TUNNCSMIRDACYPTunnel Central Crash Sensor Unit Acc Next To Y" },
                new CrashDataMappingItem{ SensorLocation = "PASMR",SensorDirection = "0",SensorRotator = "",ChannelInfo = "15TUNNCSMIRDACYPTunnel Central Crash Sensor Unit Acc Next To Y" },
                new CrashDataMappingItem{ SensorLocation = "PPSFL",SensorDirection = "0",SensorRotator = "",ChannelInfo = "11DOORMIMI00PR0PLH Front Door Pressure" },
                new CrashDataMappingItem{ SensorLocation = "UFSL",SensorDirection = "-X",SensorRotator = "",ChannelInfo = "11DOORMIMI00PR0PLH Front Door Pressure" },
                new CrashDataMappingItem{ SensorLocation = "UFSR",SensorDirection = "-X",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ SensorLocation = "PTSFL",SensorDirection = "0",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ SensorLocation = "PTSFR",SensorDirection = "0",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "REF_DIS_X",SensorDirection = "0",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "REF_VEL_X",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "REF_ACC_X",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "REF_ACC_Y",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "REF_ACC_Z",SensorDirection = "Y",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "REF_ANG_X",SensorDirection = "Z",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "ECU-45",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ SensorLocation = "REF_RAT_X",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
            };
        }
    }
}
