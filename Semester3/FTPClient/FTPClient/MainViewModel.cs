using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using SimpleFTP;

namespace FTPClient
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private RelayCommand connect;
        private Client client;
        private Server server;
        private ObservableCollection<FileSystemInfo> listOfFilesAndDirectories;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public string Address { get; set; }

        public int Port { get; set; }

        public RelayCommand Connect
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    server = new Server(Port);
                    client = new Client(Address, Port);

                    server.Start();
                    client.Connect();
                });
            }
        }

        public ObservableCollection<FileSystemInfo> ListOfFilesAndDirectories
        {
            get => listOfFilesAndDirectories;
            set
            {
                
            }
        }
    }
