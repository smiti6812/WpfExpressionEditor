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

using WpfExpressionEditor.ViewModel;

namespace WpfExpressionEditor.UserControls
{
    /// <summary>
    /// Interaction logic for CrashdataMappingList.xaml
    /// </summary>
    public partial class CrashdataMappingList : UserControl
    {
        public CrashdataMappingList()
        {
            InitializeComponent();
            this.DataContext = new CrashDataMappingItemViewModel();
        }
        private void MainGrid_KeyDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
        }

        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            MainGrid.Focus();
        }
    }
}
