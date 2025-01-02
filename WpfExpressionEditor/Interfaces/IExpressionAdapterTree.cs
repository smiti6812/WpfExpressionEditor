using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WpfExpressionEditor.Interfaces
{
    public interface IExpressionAdapterTree
    {
        Expression Expression { get; set; }

        ObservableCollection<IExpressionAdapterTree> Children { get; set; }

        string Text { get; set; }

        int Level { get; set; }

        IExpressionAdapterTree Parent { get; set; }

        bool CheckRightAndLeftSideOfExpressionIfAndOrOr(int index);

        string CreateCommandText(IExpressionAdapterTree root);

        IExpressionAdapterTree ReturnExpressionTreeTopDown();

        (string expressionText, bool checkOk) ReturnExpressionText();

        bool UpdateExpressionFromTree(string originalExpression, string text);

        Action<string> UpdateExpressionAdapterRuleText { get; set; }

        Action<string> UpdateSelectedText { get; set; }

        Action<string> UpdateExpressionText { get; set; }
    }
}
