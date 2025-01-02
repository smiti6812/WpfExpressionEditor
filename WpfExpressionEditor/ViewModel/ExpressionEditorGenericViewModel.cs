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
                FieldsAndValues = ExpressionEditorHelper<T>.GetPropertiesAndValuesOfGenericType(Item);
                OnPropertyChanged(nameof(Item));
            }
        }

        private IList<ExpressionAdapterTree> expressionTree;
        public IList<ExpressionAdapterTree> ExpressionTree
        {
            get => expressionTree;
            set
            {
                expressionTree = value;
                OnPropertyChanged(nameof(ExpressionTree));
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
                PropertyAndValueSelected = SelectedField != null && SelectedPropertyValue != null;
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
                PropertyAndValueSelected = SelectedField != null && SelectedPropertyValue != null;
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

        private int selectionLength;

        public int SelectionLength
        {
            get => selectionLength;
            set
            {
                selectionLength = value;
                OnPropertyChanged(nameof(SelectionLength));
            }
        }

        private int selectionStart;
        public int SelectionStart
        {
            get => selectionStart;
            set
            {
                selectionStart = value;
                OnPropertyChanged(nameof(SelectionStart));
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

        private bool propertyAndValueSelected;
        public bool PropertyAndValueSelected
        {
            get => propertyAndValueSelected;
            set
            {
                propertyAndValueSelected = value;
                OnPropertyChanged(nameof(PropertyAndValueSelected));
            }
        }

        private Expression<Func<T, bool>> expression;
        public Expression<Func<T, bool>> Expression
        {
            get => expression;
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
        public RelayCommand ClearEditorCommand { get; set; }
        public ExpressionEditorGenericViewModel()
        {
            OperatorCommand = new RelayCommand<string>(SelectedOperator);
            CheckFormulaCommand = new RelayCommand(CheckFormula);
            AddFieldCommand = new RelayCommand<string>(AddField);
            AddPropertyValueCommand = new RelayCommand<string>(AddValue);
            SaveCommand = new RelayCommand(SaveAndSend);
            RuleTextOk = false;
            ExpressionTextChangedCommand = new RelayCommand(ExpressionTextChanged);
            ClearEditorCommand = new RelayCommand(ClearEditor);
        }

        private void ExpressionTextChanged() => RuleTextOk = string.IsNullOrEmpty(ExpressionText);

        private void SelectedOperator(string command)
        {
            ExpressionText = ExpressionText.Trim();
            (ExpressionText, ErrorText) = command switch
            {
                EditorHelper.NotEqual => AddRelationalOperator($" {EditorHelper.NotEqual} "),
                EditorHelper.Equal => AddRelationalOperator($" {EditorHelper.Equal} "),
                EditorHelper.StartsWith => AddStringFunction($"{EditorHelper.StartsWith}"),
                EditorHelper.EndsWith => AddStringFunction($"{EditorHelper.EndsWith}"),
                EditorHelper.Contains => AddStringFunction($"{EditorHelper.Contains}"),
                EditorHelper.And => AddLogicalOperator($" {EditorHelper.And} "),
                EditorHelper.Or => AddLogicalOperator($" {EditorHelper.Or} "),
                EditorHelper.Not => AddNotFunction($" {EditorHelper.Not} "),
                EditorHelper.Brackets => InsertBrackets(),
                _ => (ExpressionText, ErrorText)
            };
        }
        public void UpdateSelectedText(string newSelectedText) => SelectedText = newSelectedText;

        private void ClearEditor()
        {
            ExpressionText = "";
            ErrorText = "";
        }

        private (string exprText, string errorText) InsertBrackets() => ExpressionEditorHelper<T>.CanInsertBrackets(ExpressionText, SelectionStart, SelectionLength);

        private (string exprText, string errorText) AddNotFunction(string _function) => ExpressionEditorHelper<T>.CanAddNotFunction(_function, SelectionStart, SelectionLength, ExpressionText);

        private (string exprText, string errorText) AddLogicalOperator(string _operator) => ExpressionEditorHelper<T>.CanAddLogicalOperator(_operator, ExpressionText, SelectionStart, SelectionLength, FieldsAndValues.Keys);

        private (string exprText, string errorText) AddStringFunction(string _function)
        {
            (string exprText, string errorText) = ExpressionEditorHelper<T>.CanAddStringFunction(_function, SelectedField, SelectedPropertyValue, SelectionStart, SelectionLength, ExpressionText, FieldsAndValues.Keys);
            SelectedPropertyValue = null;
            SelectedField = null;
            return (exprText, errorText);
        }

        private (string exprText, string errorText) AddRelationalOperator(string _operator)
        {
            (string exprText, string errorText) = ExpressionEditorHelper<T>.CanAddRelationalOperator(_operator, SelectedField, SelectedPropertyValue, SelectionStart, SelectionLength, ExpressionText, FieldsAndValues.Keys);
            SelectedPropertyValue = null;
            SelectedField = null;
            return (exprText, errorText);
        }

        public virtual void SaveAndSend()
        {
            Messenger.Default.Send(new NotificationMessage<string>(this, ExpressionText, "MsgSendRuleText"));
            RuleTextOk = false;
        }

        private void AddValue(string value)
        {
            (ExpressionText, ErrorText) = ExpressionEditorHelper<T>.CanAddValue(value, ExpressionText, SelectionStart, SelectionLength);
            SelectedField = null;
            SelectedPropertyValue = null;
        }

        private void AddField(string item)
        {
            (ExpressionText, ErrorText) = ExpressionEditorHelper<T>.CanAddField(item, ExpressionText, SelectionStart, SelectionLength);
            SelectedPropertyValue = null;
            SelectedField = null;
        }

        private void CheckFormula()
        {
            (Expression, bool checkOk, string msg, _) = ExpressionEditorHelper<T>.ParseStringToExpression(ExpressionText);
            RuleTextOk = string.IsNullOrEmpty(ExpressionText) || checkOk;
            ExpressionTree = new List<ExpressionAdapterTree>() { new ExpressionAdapterTree(Expression, 0, null) };
            ErrorText = RuleTextOk ? string.Empty : msg;
        }

        public (bool checkOk, string msg) CheckFormula(string formula)
        {
            (_, bool checkOk, string msg, _) = ExpressionEditorHelper<T>.ParseStringToExpression(formula);
            return (checkOk, msg);
        }
    }
}
