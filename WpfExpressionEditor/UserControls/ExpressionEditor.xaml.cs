using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using GalaSoft.MvvmLight.Messaging;

namespace WpfExpressionEditor.UserControls
{
    /// <summary>
    /// Interaction logic for ExpressionEditor.xaml
    /// </summary>
    public partial class ExpressionEditor : Window
    {       
        public ExpressionEditor()
        {
            InitializeComponent();
            Messenger.Default.Register<string>(this, HighlightSelectedText);            
        }

        private void HighlightSelectedText(string text)
        {
            int start = ExpressionText.Text.IndexOf(text.Replace("Or ","").Replace("And ",""));
            ExpressionText.Select(start, text.Length);
                    
        }

        private void ExpressionText_TextChanged(object sender, TextChangedEventArgs e)
        {            
        }
    }
}
