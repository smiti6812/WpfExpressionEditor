using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using WpfExpressionEditor.ViewModel;

namespace WpfExpressionEditor
{
    public class ExpressionAdapter 
    {
        private readonly Expression Expr;
        public RelayCommand<ExpressionAdapter> GetSelectedItemCommand { get; set; }

        private ExpressionEditorViewModel viewModel;
        public ExpressionAdapter(Expression expr)
        {
            Expr = expr;
            GetSelectedItemCommand = new RelayCommand<ExpressionAdapter>(GetSelectedItem);
        }
        public ExpressionAdapter(Expression expr, ExpressionEditorViewModel _viewModel)
        {
            Expr = expr;
            GetSelectedItemCommand = new RelayCommand<ExpressionAdapter>(GetSelectedItem);
            viewModel = _viewModel;
        }
        private void GetSelectedItem(ExpressionAdapter adapter)
        {           
            viewModel.SelectedText = adapter.Text;          
        }
        public string Text
        {
            get
            {
                if (Expr == null)
                {
                    return null;
                }
                else
                {

                    string nodeType = Expr.NodeType.ToString().Replace("OrElse", "Or").Replace("AndAlso", "And")
                .Replace("Call", "").Replace("Lambda", "") + " ";
                    string expressionText = Expr.ToString().Replace("OrElse", "Or").Replace("AndAlso", "And")
                .Replace("Call", "").Replace("Param_0.", "").Replace("Lambda : Param_0 =>", "");
                    if (nodeType.TrimEnd(' ') is not "MemberAccess" and not "Constant"
                        and not "Parameter" and not "Call")
                    {
                        return (nodeType + expressionText).Replace("Param_0 =>", "Rule: ").Replace("Equal","");
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public ObservableCollection<ExpressionAdapter> Children => Expr == null
                    ? new ObservableCollection<ExpressionAdapter>()
                    : (from childExpression in Expr.GetType().GetProperties()
                       where childExpression.PropertyType == typeof(Expression)
                       select new ExpressionAdapter((Expression)childExpression.GetValue(Expr, null), viewModel)).Where(g => g.Text != null).ToObservableCollection<ExpressionAdapter>();
    }
}
