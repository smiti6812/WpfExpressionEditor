using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using static Antlr.Runtime.Tree.TreeWizard;

namespace WpfExpressionEditor
{
    public static class ExpressionExtensions
    {
        public static IEnumerable<Expression> Flatten(this Expression expr)
        {
            return Visitor.Flatten(expr);
        }
    }
    public sealed class Visitor : ExpressionVisitor
    {
        private readonly Action<Expression> nodeAction;

        private Visitor(Action<Expression> nodeAction)
        {
            this.nodeAction = nodeAction;
        }

        public override Expression Visit(Expression node)
        {
            nodeAction(node);
            return base.Visit(node);
        }

        public static IEnumerable<Expression> Flatten(Expression expr)
        {
            var ret = new List<Expression>();
            var visitor = new Visitor(t => ret.Add(t));
            visitor.Visit(expr);
            return ret;
        }
    }

}
