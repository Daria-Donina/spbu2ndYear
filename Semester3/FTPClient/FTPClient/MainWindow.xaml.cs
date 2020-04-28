using System.Windows;
using System.Windows.Controls;

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
