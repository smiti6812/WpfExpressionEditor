using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using WpfExpressionEditor.Model;

namespace WpfExpressionEditor.ViewModel
{
    public class ExpressionEditorGenericViewModel<T> : BindableBase
    {
        private T item;
        public T Item
        {
            get => item;
            set
            {
                item = value;
                FieldsAndValues = helper.GetPropertiesAndValuesOfGenericType(Item);
                OnPropertyChanged(nameof(Item));
            }
        }

        private readonly ExpressionEditorHelper<T> helper;

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
        private bool isTextSelected;

        public bool IsTextSelected
        {
            get => isTextSelected;
            set
            {
                isTextSelected = value;
                OnPropertyChanged(nameof(IsTextSelected));
            }
        }

        private string selectedField;
        public string SelectedField
        {
            get => selectedField;
            set
            {
                selectedField = value;
                OnPropertyChanged(nameof(SelectedField));
            }
        }

        private string selectedPropertyValue;
        public string SelectedPropertyValue
        {
            get => selectedPropertyValue;
            set
            {
                selectedPropertyValue = value;
                OnPropertyChanged(nameof(SelectedPropertyValue));
            }
        }

        private string selectedText;

        public string SelectedText
        {
            get => selectedText;
            set
            {
                selectedText = value;
                OnPropertyChanged(nameof(SelectedText));
            }
        }

        private string expressionText;

        public string ExpressionText
        {
            get => expressionText;
            set
            {
                expressionText = value;              
                OnPropertyChanged(nameof(ExpressionText));
            }
        }
        private bool ruleTextOk;

        public bool RuleTextOk
        {
            get => ruleTextOk;
            set
            {
                ruleTextOk = value;
                OnPropertyChanged(nameof(RuleTextOk));
            }
        }

        private string errorText;

        public string ErrorText
        {
            get => errorText;
            set
            {
                errorText = value;
                OnPropertyChanged(nameof(ErrorText));
            }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        private Expression<Func<T, bool>> expression;

        public Expression<Func<T, bool>> Expression
        {
            get
            {
                return expression;
            }
            set
            {
                expression = value;
                OnPropertyChanged(nameof(Expression));
            }
        }

        public ObservableCollection<T> Items { get; set; }

        private Dictionary<string, string> fieldsAndValues;
        public Dictionary<string, string> FieldsAndValues
        {
            get => fieldsAndValues;
            set
            {
                fieldsAndValues = value;
                OnPropertyChanged(nameof(FieldsAndValues));
            }
        }
        public RelayCommand<string> OperatorCommand { get; set; }
        public RelayCommand CheckFormulaCommand { get; set; }
        public RelayCommand<string> AddFieldCommand { get; set; }
        public RelayCommand<string> AddPropertyValueCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ExpressionTextChangedCommand { get; set; }

        public ExpressionEditorGenericViewModel()
        {
            OperatorCommand = new RelayCommand<string>(SelectedOperator);
            CheckFormulaCommand = new RelayCommand(CheckFormula);
            AddFieldCommand = new RelayCommand<string>(AddField);
            AddPropertyValueCommand = new RelayCommand<string>(AddValue);
            SaveCommand = new RelayCommand(SaveAndSend);
            RuleTextOk = false;
            ExpressionTextChangedCommand = new RelayCommand(ExpressionTextChanged);
            helper = new ExpressionEditorHelper<T>();
        }    

        private void ExpressionTextChanged() => RuleTextOk = string.IsNullOrEmpty(ExpressionText);

        private void SelectedOperator(string command)
        {
            switch (command)
            {
                case "Equal":
                    ExpressionText += " Equal ";
                    break;
                case "StartsWith":
                    ExpressionText += ".StartsWith(\"\")";
                    break;
                case "EndsWith":
                    ExpressionText += ".EndsWith(\"\")";
                    break;
                case "Contains":
                    ExpressionText += ".Contains(\"\")";
                    break;
                case "And":
                    ExpressionText += " And ";
                    break;
                case "Or":
                    ExpressionText += " Or ";
                    break;
                case "Not":
                    ExpressionText += " Not ";
                    break;
                default:
                    break;
            }
        }

        public virtual void SaveAndSend()
        {
            Messenger.Default.Send(new NotificationMessage<string>(this, ExpressionText, "MsgSendRuleText"));
            RuleTextOk = false;
        }

        public int IndexOfStringFunction(string expression) => expression.EndsWith("(") ? expression.Length - expression.IndexOf("EndsWith") : 0;

        private void AddValue(string value)
        {
            if (ExpressionText.Contains("(\"\")"))
            {
                ExpressionText = ExpressionText.Substring(0, ExpressionText.Length - IndexOfStringFunction(ExpressionText) - 4) + "(\"" + value + "\")";
            }
            else
            {
                ExpressionText += "\""+ value + "\"";
            }
        }
        private void AddField(string item) => ExpressionText += item;
        private void CheckFormula()
        {
            (Expression, bool ChekOk, string Msg) = helper.ParseStringToExpression(ExpressionText);
            RuleTextOk = string.IsNullOrEmpty(ExpressionText) || ChekOk;
            TreeList = Expression != null ? new List<ExpressionAdapter>() { new ExpressionAdapter(expression) } : null;
            ErrorText = ChekOk ? string.Empty : Msg;
        }
    }
}
