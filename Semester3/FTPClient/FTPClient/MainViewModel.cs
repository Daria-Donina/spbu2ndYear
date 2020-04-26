using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace FTPClient
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;



        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public string Address { get; set; } = "";

        public MainViewModel()
        {
            Address += "!";
        }
    }
}
