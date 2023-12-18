using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfExpressionEditor.Tree
{
    public static class GenerateTree
    {
        public static Node GetTreeFromCharachterString(string str)
        {
            var nodes = new Stack<Node>();
            Node root = null;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != '(' && str[i] != ')')
                {
                    var node = new Node();
                    node.Data = str[i];
                    if (nodes.Count == 0)
                    {
                        root = node;
                    }
                    else
                    {
                        if (nodes.Peek().Left == null)
                        {
                            nodes.Peek().Left = node;
                        }
                        else
                        {
                            nodes.Peek().Right = node;
                        }
                    }
                    nodes.Push(node);
                }
                else if (str[i] == ')')
                {
                    nodes.Pop();
                }
            }
            return root;
        }
        public static NodeString GetTreeFromString(string str)
        {
            var nodes = new Stack<NodeString>();
            NodeString root = new NodeString();
            string nodeValue = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != '(' && str[i] != ')')
                {
                    if (str[i + 1] == '(' || str[i + 1] == ')')
                    {
                        if (!nodeValue.Trim().EndsWith("And"))
                        {

                            var node = new NodeString();
                            node.DataString = nodeValue + str[i];
                            if (nodes.Count == 0)
                            {
                                root.Left = node;
                                nodes.Push(root);
                                nodeValue = "";
                            }
                            else
                            {
                                if (nodes.Peek().Left == null)
                                {
                                    nodes.Peek().Left = node;
                                }
                                else
                                {
                                    nodes.Peek().Right = node;
                                }
                                nodes.Push(node);
                                nodeValue = "";
                            }
                        }
                        else
                        {
                            if (nodes.Count > 0)
                            {
                                nodes.Peek().NodeType = nodeValue;
                                nodeValue = "";
                            }
                        }
                    }
                    else
                    {
                        nodeValue += str[i];
                    }
                }
                else if (str[i] == ')')
                {
                    if (nodes.Count > 0)
                    {
                        nodes.Pop();
                    }
                }
            }
            return root;
        }
        public static int ReturnAndIndex(string str)
        {
            return str.IndexOf("And", 1);
        }
        public static int ReturnOrIndex(string str)
        {
            return str.IndexOf("Or", 1);
        }
        public static int NextOpeningBracket(string str)
        {
            return str.IndexOf('(', 1);
        }
        public static int NextClosingBracket(string str)
        {
            return str.IndexOf(')', 1);
        }
        public static NodeString GetTree(NodeString root, string str)
        {
            int andIndex = ReturnAndIndex(str);
            if (andIndex > -1)
            {
                root.NodeType = "And";
                var nodeLeft = new NodeString();
                nodeLeft.DataString = str.Substring(0, andIndex);
                root.Left = nodeLeft;
                nodeLeft.Parent = root;
                _ = GetTree(nodeLeft, nodeLeft.DataString);
                var nodeRight = new NodeString();
                nodeRight.DataString = str.Substring(andIndex + 3, str.Length - andIndex - 3);
                root.Right = nodeRight;
                nodeRight.Parent = root;
                _ = GetTree(nodeRight, nodeRight.DataString);
            }
            return root;
        }
        public static NodeString CreateNodeLeft(NodeString root, string str, string nodeType)
        {
            var left = new NodeString
            {
                DataString = str
            };
            root.NodeType = nodeType;
            root.Left = left;
            left.Parent = root;
            return left;
        }
        public static NodeString CreateNodeRight(NodeString root, string str, string nodeType)
        {
            var right = new NodeString
            {
                DataString = str
            };
            root.NodeType = nodeType;
            root.Right = right;
            right.Parent = root;
            return right;
        }
        public static NodeString GetTreeFrom_String(NodeString root, string str)
        {
            int closing = ClosingBracketIndex(str);
            int length = str.Length;
            int start = 0;
            while (length - closing == 1)
            {
                start++;
                length--;
                str = str.Substring(start, length - start).Trim();
                closing = ClosingBracketIndex(str);
            }
            int nextOpening = NextOpeningBracket(str);
            int nextClosing = NextClosingBracket(str);

            // It a Node element
            if (closing == 0 && nextClosing > nextOpening)
            {
                int andIndex = ReturnAndIndex(str);
                int orIndex = ReturnOrIndex(str);
                if (str.Substring(andIndex + 4, str.Length - andIndex - 4).Contains("And"))
                {
                    root.NodeType = "And";
                    var node = new NodeString();
                    node.DataString = str.Substring(0, andIndex).Trim();
                    root = node;
                    var nodeLeft = new NodeString();
                    nodeLeft.DataString = str.Substring(andIndex + 4, str.Length - andIndex - 4).Trim();
                    node.Left = nodeLeft;
                    nodeLeft.Parent = node;
                    _ = GetTreeFrom_String(nodeLeft, nodeLeft.DataString);
                }
                else if (str.Substring(andIndex + 4, str.Length - andIndex - 4).Contains("Or"))
                {
                    root.NodeType = "Or";
                    var node = new NodeString();
                    node.DataString = str.Substring(0, orIndex);
                    root = node;
                    var nodeLeft = new NodeString();
                    nodeLeft.DataString = str.Substring(orIndex + 3, str.Length - orIndex - 3).Trim();
                    node.Left = nodeLeft;
                    nodeLeft.Parent = node;
                    _ = GetTreeFrom_String(nodeLeft, nodeLeft.DataString);
                }
            }
            else // It is a child element
            {
                //there are no parenthesis
                if (closing == 0)
                {
                    if (Regex.Matches(str, "And").Count > 0 && Regex.Matches(str, "Or").Count == 0)
                    {
                        int andIndex = ReturnAndIndex(str);
                        if (Regex.Matches(str, "And").Count > 1)
                        {
                            andIndex = ReturnAndIndex(str.Substring(andIndex, str.Length - andIndex));
                        }
                        var nodeLeft = CreateNodeLeft(root, str.Substring(0, andIndex).TrimEnd(' '), "And");
                        _ = GetTreeFrom_String(nodeLeft, nodeLeft.DataString);
                        var nodeRight = CreateNodeRight(root, str.Substring(andIndex + 4, str.Length - andIndex - 4).TrimEnd(' '), "And");
                        _ = GetTreeFrom_String(nodeRight, nodeRight.DataString);
                    }
                    else if (Regex.Matches(str, "Or").Count > 0 && Regex.Matches(str, "And").Count == 0)
                    {
                        int orIndex = ReturnOrIndex(str);
                        if (Regex.Matches(str, "Or").Count > 1)
                        {
                            orIndex = ReturnOrIndex(str.Substring(orIndex, str.Length - orIndex));
                        }
                        var nodeLeft = CreateNodeLeft(root, str.Substring(0, orIndex).TrimEnd(' '), "Or");
                        _ = GetTreeFrom_String(nodeLeft, nodeLeft.DataString);
                        var nodeRight = CreateNodeRight(root, str.Substring(orIndex + 3, str.Length - orIndex - 3).TrimEnd(' '), "Or");
                        _ = GetTreeFrom_String(nodeRight, nodeRight.DataString);
                    }
                } //there are parenthesis
                else if (str.Substring(closing + 1, 4).Contains("And"))
                {
                    root.NodeType = "And";
                    var nodeLeft = CreateNodeLeft(root, str.Substring(0, closing + 1), "And");
                    _ = GetTreeFrom_String(nodeLeft, nodeLeft.DataString);
                    var nodeRight = CreateNodeRight(root, str.Substring(closing + 6, str.Length - closing - 6).Trim(), "And");
                    _ = GetTreeFrom_String(nodeRight, nodeRight.DataString);
                }
                else if (str.Substring(closing + 1, 4).Contains("Or"))
                {
                    root.NodeType = "Or";
                    var nodeLeft = CreateNodeLeft(root, str.Substring(0, closing + 1), "Or");
                    _ = GetTreeFrom_String(nodeLeft, nodeLeft.DataString);
                    var nodeRight = CreateNodeRight(root, str.Substring(closing + 5, str.Length - closing - 5).Trim(), "Or");
                    _ = GetTreeFrom_String(nodeRight, nodeRight.DataString);
                }
            }

            return root;
        }
        public static int ClosingBracketIndex(string str)
        {
            int opening = 0;
            int closing = 0;
            int index = 0;
            for (int i = 0; i <= str.Length - 1; i++)
            {
                if (str[i] == '(')
                {
                    opening++;
                }
                if (str[i] == ')')
                {
                    closing++;
                }
                if (opening == closing)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
        public static NodeString ConvertTreeToListMethode(NodeString root)
        {
            if (root == null)
            {
                return null;
            }

            root.NodeInfo = root.NodeType + " " + root.DataString;
            root.Children = new System.Collections.ObjectModel.ObservableCollection<NodeString>();
            if (root.Left != null)
            {
                root.Children.Add((NodeString)root.Left);
                _ = ConvertTreeToList((NodeString)root.Left);
            }

            if (root.Right != null)
            {
                root.Children.Add((NodeString)root.Right);
                _ = ConvertTreeToList((NodeString)root.Right);
            }




            return root;

        }
        public static NodeString ConvertTreeToList(NodeString root)
        {
            return ConvertTreeToListMethode(root);
        }
    }

}
