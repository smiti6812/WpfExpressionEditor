using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExpressionEditor.Tree
{
    public class ExpressionNode
    {
        public string Formula { get; set; } 
        public ExpressionNode(NodeString _node) 
        {
            Node = _node;
            Formula = Node.NodeType + " " + Node.DataString;
        }
        public NodeString Node { get; set; }
        public IList<NodeString> Children { get; set; }
    }
}
