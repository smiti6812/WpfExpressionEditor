using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExpressionEditor.Tree
{
    public class ExpressionNode
    {
        public NodeString Node { get; set; }

        public NodeString Parent { get; set; }
        public string Formula { get; set; } 
        public ExpressionNode(NodeString _node) 
        {
            Node = _node;         
            Formula = _node.NodeType + " " + _node.DataString;
        }      
        public IList<NodeString> Children { get; set; }

        public override string ToString()
        {
            return Node.ToString();
        }
    }
}
