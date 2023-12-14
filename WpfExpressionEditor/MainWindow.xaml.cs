using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfExpressionEditor.Tree;

namespace WpfExpressionEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IList<ExpressionNode> treeNodes { get; set; }
        public MainWindow()
        {
            InitializeComponent();           
            NodeString root = new NodeString();
            string str = "(c.Quantity > 100 And ((c.Quantity < 250 Or c.Artist == \"Betontod\") And (c.Title ==\"Revolution\" And c.Quantity > 100)))";
            var n = GenerateTree.GetTreeFrom_String(root, str);
            treeNodes = new List<ExpressionNode>();          
            treeNodes = GenerateTree.ConvertTreeToList(n);
            NodeTree.ItemsSource = treeNodes;

        }
    }
}
