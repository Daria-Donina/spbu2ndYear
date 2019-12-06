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

namespace TicTacToe
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;
        public MainWindow()
        {
            InitializeComponent();
            game = new Game();
        }

        public void OnClickButton(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            var row = button.Name[6];
            var column = button.Name[7];

            if (true)
            {
                button.Content = "x";

                game.Map[row][column] = true;
            }
            else
            {
                button.Content = "0";

                game.Map[row][column] = false;
            }
            button.IsEnabled = false;
        }

    }
}
