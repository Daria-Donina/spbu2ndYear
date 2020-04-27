using System;
using System.Collections.Generic;
using System.IO;
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

namespace FTPClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel model;
        public MainWindow()
        {
            InitializeComponent();

            model = new MainViewModel();
            DataContext = model;
        }

        private async void HandleItemDoubleClick(object sender, RoutedEventArgs e)
        {
            var listViewItem = sender as ListViewItem;
            await model.OpenFolder(listViewItem.Content as string);
        }

        private void HandleItemGotFocus(object sender, RoutedEventArgs e)
        {
            var listViewItem = sender as ListViewItem;
            model.MarkSelectedItem(listViewItem.Content as string);
        }
    }
}
