using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using WpfExpressionEditor.Interfaces;

namespace WpfExpressionEditor.Model
{
    public class ExpressionAdapterTree : IExpressionAdapterTree
    {
        public string Text { get; set; }
        public int Level { get; set; }
        public Expression Expression { get; set; }
        public ObservableCollection<IExpressionAdapterTree> Children { get; set; }
        public IExpressionAdapterTree Parent { get; set; }
        public Action<string> UpdateExpressionText { get; set; }
        public Action<string> UpdateExpressionAdapterRuleText { get; set; }
        public Action<string> UpdateSelectedText { get; set; }
        public ExpressionAdapterTree(Expression expr, int level, ExpressionAdapterTree node)
        {
            Expression = expr;
            Parent = node;
            Level = level + 1;
            Children = GetChildren();
            Text = ReturnText(Expression);
            if (!string.IsNullOrEmpty(Text) && Text.StartsWith($"{EditorHelper.Not}("))
            {
                _ = Children.Remove(Children[0]);
            }
        }

        private ObservableCollection<IExpressionAdapterTree> GetChildren()
        {
            return Expression == null
                     ? []
                     : (from childExpression in Expression.GetType().GetProperties()
                        where childExpression.PropertyType == typeof(Expression)
                        select new ExpressionAdapterTree((Expression)childExpression.GetValue(Expression, null), Level, this)).Where(g => g.Text != null).ToObservableCollection<IExpressionAdapterTree>();
        }

        private string ReturnText(Expression expr)
        {
            string txt = EditorHelper.Replace(expr?.NodeType.ToString()) + " ";
            string expressionText = EditorHelper.Replace(expr?.ToString());
            return txt switch
            {
                string type when type is null || type.TrimEnd(' ') is "MemberAccess" or "Constant" or "Parameter" or "Call" => null,
                string type when type.StartsWith(EditorHelper.Or) => EditorHelper.Or,
                string type when type.StartsWith(EditorHelper.And) => EditorHelper.And,
                string type when type.StartsWith(EditorHelper.Not) => expressionText,
                _ => (txt + expressionText).Replace("Param_0 =>", "Rule: ").Replace("Equal", "").Trim()
            };
        }
        public bool CheckRightAndLeftSideOfExpressionIfAndOrOr(int index) => Parent.Children[index].Text is not EditorHelper.And and not EditorHelper.Or;

        public string CreateCommandText(IExpressionAdapterTree root)
        {
            var builder = new StringBuilder();
            if (root.Children != null && root.Children.Any())
            {
                _ = builder.Append("(");
                for (int i = root.Children.Count - 1; i >= 0; i--)
                {
                    _ = builder.Append(CreateCommandText(root.Children[i]));

                    if (i > 0)
                    {
                        _ = builder.Append($" {root.Text} ");
                    }
                }

                _ = builder.Append(")");
            }
            else
            {
                _ = builder.Append(root.Text);
            }

            return builder.ToString();
        }

        public (string expressionText, bool checkOk) ReturnExpressionText()
        {
            IExpressionAdapterTree modifiedNode = ReturnExpressionTreeTopDown();
            string expressionText = CreateCommandText(modifiedNode.Children[0]);
            (Expression expression, bool checkOk, _, _) = ExpressionEditorHelper<AutoMappingArguments>.ParseStringToExpression(expressionText);
            if (checkOk)
            {
                var expressionTree = new ExpressionAdapterTree(expression, 0, null);
                expressionText = expressionTree.Text;
                modifiedNode.UpdateExpressionAdapterRuleText(expressionText);
            }

            return (expressionText, checkOk);
        }

        public bool UpdateExpressionFromTree(string originalExpression, string text)
        {
            if ((originalExpression.Trim() is not EditorHelper.Or and not EditorHelper.And)
                && text.Trim() is not EditorHelper.Or and not EditorHelper.And)
            {
                (string newExpression, bool checkOk) = ReturnExpressionText();
                if (checkOk)
                {
                    string expressionEditorExpressionText = newExpression.Replace("Rule:  ", "").TrimStart(' ').TrimEnd(' ');
                    UpdateExpressionText(expressionEditorExpressionText);
                    return true;
                }
                else
                {
                    //MessageLogger.reportWarning("There was an error in the modified formula! Please check your last modification!", "ExpressionWrapper");
                    return false;
                }
            }
            else
            {
                //MessageLogger.reportWarning("You probably misspelled 'Or' or 'And'!!!", "ExpressionWrapper");
                return false;
            }
        }

        public IExpressionAdapterTree ReturnExpressionTreeTopDown()
        {
            IExpressionAdapterTree modifiedNode = this;
            while (modifiedNode.Parent != null && modifiedNode.Level > 1)
            {
                modifiedNode = modifiedNode.Parent;
            }

            return modifiedNode;
        }
    }
}
