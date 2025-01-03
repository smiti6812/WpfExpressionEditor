using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Mail;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using MoreLinq;

using NHibernate.Util;

using WpfExpressionEditor.Model;
using WpfExpressionEditor.UserControls;

namespace WpfExpressionEditor.ViewModel
{
    public class CrashDataMappingItemViewModel : BindableBase
    {
        private const string RuleEditorName = "SensorLocationEditor";

        private ExpressionEditor editor;

        private Dictionary<string, Func<AutoMappingArguments, bool>> ruleset;
        public Dictionary<string, Func<AutoMappingArguments, bool>> Ruleset
        {
            get => ruleset;
            set
            {
                ruleset = value;
                OnPropertyChanged(nameof(Ruleset));
            }
        }
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
            SelectedItems = new List<CrashDataMappingItem>(selectedItems.Cast<CrashDataMappingItem>());
            SelectedItems.ForEach(item => item.IsInRuleEditingMode = true);
            Messenger.Default.Send(new NotificationMessage<IList<CrashDataMappingItem>>(CrashDataMappingItems, "MsgChannelsOfSelectedCrash"));
            Messenger.Default.Send(new NotificationMessage<IList<CrashDataMappingItem>>(SelectedItems, "MsgMappingRuleEditorSelectedItems"));
        }
        public List<CrashDataMappingItem> SelectedItems { get; set; }

        private bool CanOpen() => SelectedCrashDataMappingItem != null;
        private void OnOpenEditor()
        {
            ExpressionEditorViewModel.Item = SelectedCrashDataMappingItem;
            ExpressionEditorViewModel.Items = CrashDataMappingItems;
            //ExpressionEditorViewModel.IsVisible = true;
            ExpressionEditorViewModel.CrashDataMappingItemViewModel = this;
            ExpressionEditorViewModel.ExpressionText = SelectedCrashDataMappingItem.RuleAsText != "Create Rules" ? SelectedCrashDataMappingItem.RuleAsText : "";
            ExpressionEditorViewModel.FieldsAndValues = ExpressionEditorViewModel.FieldsAndValues.Where(c => c.Key != "RuleAsText" && c.Key != "SensorRotator").ToDictionary<string, string>();
            LoadRuleEditorData(SelectedCrashDataMappingItem);
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

        private void ReceiveSelectedChannels(NotificationMessage<IList<CrashDataMappingItem>> message)
        {
            if (message.Notification.Equals("MsgChannelsOfSelectedCrash"))
            {
                if (CrashDataMappingItems.Any(channel => channel.RuleAsText != channel.DefaultRuleTextValue))
                {
                    var tempMappebleChannelList = message.Content.ToObservableCollection();
                    foreach (CrashDataMappingItem pair in CrashDataMappingItems)
                    {
                        var updateMappedPairModel = tempMappebleChannelList.FirstOrDefault(c => c.SensorLocation == pair.SensorLocation);
                        if (updateMappedPairModel is CrashDataMappingItem)
                        {
                            updateMappedPairModel.RuleAsText = pair.RuleAsText;
                            updateMappedPairModel.ExpressionAdapters = pair.ExpressionAdapters;
                        }
                    }

                    CrashDataMappingItems = tempMappebleChannelList;
                    CreateRuleset();
                }
                else
                {
                    CrashDataMappingItems = message.Content.ToObservableCollection();
                    CreateRuleset();
                }
            }
        }

        private void CreateRuleset()
        {
            Ruleset = [];
            foreach (CrashDataMappingItem pairModel in CrashDataMappingItems)
            {
                Ruleset.Add(pairModel.Key, !string.IsNullOrEmpty(pairModel.RuleAsText) && !pairModel.RuleAsText.Equals(pairModel.DefaultRuleTextValue) ? ExpressionEditorHelper<AutoMappingArguments>.GetQueryFunc(pairModel.RuleAsText) : null);
            }
        }

        private void LoadRuleEditorData(CrashDataMappingItem selectedItem)
        {
            CrashDataMappingItem channelDB = ReturnRuleEditorChannelData(selectedItem);
            ExpressionEditorViewModel.ExpressionText = selectedItem.RuleAsText != selectedItem.DefaultRuleTextValue ? selectedItem.RuleAsText : "";
            ExpressionEditorViewModel.ErrorText = string.Empty;
            ExpressionEditorViewModel.Item = channelDB;
            editor = new()
            {
                Title = $"{selectedItem.SensorLocation} - RuleEditor",
                DataContext = ExpressionEditorViewModel
            };
            editor.Show();
        }

        public CrashDataMappingItem ReturnRuleEditorChannelData(CrashDataMappingItem selectedItem)
        {
            var channelDB = selectedItem;
            CrashDataMappingItem newChannelDB = channelDB == null ? new() : new CrashDataMappingItem
            {
                SensorLocation = channelDB.SensorLocation,
                Location = "Location",
                SensorDirection = channelDB.SensorDirection,
                AssignmentComment = ""
            };

            return newChannelDB;
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
                new CrashDataMappingItem{ Key="crash1", SensorLocation = "ECU-45",SensorDirection = "-M45",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ Key="crash2",SensorLocation = "ECU+45",SensorDirection = "P45",SensorRotator = "",ChannelInfo = "15TUNNCSMIRDACYPTunnel Central Crash Sensor Unit Acc Next To Y" },
                new CrashDataMappingItem{ Key="crash3",SensorLocation = "ECUY_lowg",SensorDirection = "-Y",SensorRotator = "",ChannelInfo = "15TUNNCSMIRDACYPTunnel Central Crash Sensor Unit Acc Next To Y" },
                new CrashDataMappingItem{ Key="crash4",SensorLocation = "ECUZ_lowg",SensorDirection = "-Z",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACZPTunnel Central Crash Sensor Unit Acc Z" },
                new CrashDataMappingItem{ Key="crash5",SensorLocation = "VdsX",SensorDirection = "-X",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ Key="crash6",SensorLocation = "RollrateX",SensorDirection = "-Y",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ Key="crash7",SensorLocation = "PASFL",SensorDirection = "Y",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash8",SensorLocation = "PASFR",SensorDirection = "-Y",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash9",SensorLocation = "PASML",SensorDirection = "Y",SensorRotator = "",ChannelInfo = "15TUNNCSMIRDACYPTunnel Central Crash Sensor Unit Acc Next To Y" },
                new CrashDataMappingItem{ Key="crash10",SensorLocation = "PASMR",SensorDirection = "0",SensorRotator = "",ChannelInfo = "15TUNNCSMIRDACYPTunnel Central Crash Sensor Unit Acc Next To Y" },
                new CrashDataMappingItem{ Key="crash11",SensorLocation = "PPSFL",SensorDirection = "0",SensorRotator = "",ChannelInfo = "11DOORMIMI00PR0PLH Front Door Pressure" },
                new CrashDataMappingItem{ Key="crash12",SensorLocation = "UFSL",SensorDirection = "-X",SensorRotator = "",ChannelInfo = "11DOORMIMI00PR0PLH Front Door Pressure" },
                new CrashDataMappingItem{ Key="crash13",SensorLocation = "UFSR",SensorDirection = "-X",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ Key="crash14",SensorLocation = "PTSFL",SensorDirection = "0",SensorRotator = "",ChannelInfo = "15TUNNCSMI00ACXPTunnel Central Crash Sensor Unit Acc X" },
                new CrashDataMappingItem{ Key="crash15",SensorLocation = "PTSFR",SensorDirection = "0",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash16",SensorLocation = "REF_DIS_X",SensorDirection = "0",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash17",SensorLocation = "REF_VEL_X",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash18",SensorLocation = "REF_ACC_X",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash19",SensorLocation = "REF_ACC_Y",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash20",SensorLocation = "REF_ACC_Z",SensorDirection = "Y",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash21",SensorLocation = "REF_ANG_X",SensorDirection = "Z",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash22",SensorLocation = "ECU-45",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
                new CrashDataMappingItem{ Key="crash23",SensorLocation = "REF_RAT_X",SensorDirection = "X",SensorRotator = "",ChannelInfo = "" },
            };
        }
    }
}
