using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExpressionEditor.Tree
{
    public class Node
    {
        public Node Parent { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public char Data { get; set; }
    }
    public class NodeString : Node
    {
        public NodeString MathOperator { get; set; }
        public string DataString { get; set; }
        public string NodeType { get; set; }
    }
}
