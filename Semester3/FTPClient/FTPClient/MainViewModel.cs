using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleFTP;

namespace FTPClient
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Client client;
        private Server server;
        private (List<DirectoryInfo>, List<FileInfo>) filesAndDirectories;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public string Address { get; set; } = "localhost";

        public int Port { get; set; } = 7777;

        public string SelectedFile { get; set; }

        public bool IsConnected { get; set; }

        public RelayCommand Connect
        {
            get
            {
                return new RelayCommand(async obj =>
                {
                    Close();

                    server = new Server(Port);
                    client = new Client(Address, Port);

                    server.Start();
                    client.Connect();
                    IsConnected = true;

                    ServerPath = rootPath;
                    FilesAndDirectories = await client.List(ServerPath);
                });
            }
        }

        public string ServerPath { get; set; } = rootPath;

        private const string rootPath = "../../..";

        public ObservableCollection<string> ListOfFilesAndDirectories { get; set; } = new ObservableCollection<string>();

        public (List<DirectoryInfo>, List<FileInfo>) FilesAndDirectories
        {
            get => filesAndDirectories;
            set
            {
                filesAndDirectories = value;
                RefreshList();
            }
        }

        private void RefreshList()
        {
            ListOfFilesAndDirectories.Clear();

            foreach (var item in FilesAndDirectories.Item1)
            {
                ListOfFilesAndDirectories.Add(item.Name);
            }
            foreach (var item in FilesAndDirectories.Item2)
            {
                ListOfFilesAndDirectories.Add(item.Name);
            }
        }

        private bool IsFile(string name) => !FilesAndDirectories.Item1.Exists(item => item.Name == name);

        public async Task OpenFolder(string name)
        {
            if (IsFile(name))
            {
                return;
            }

            ServerPath += $"/{name}";
            FilesAndDirectories = await client.List(ServerPath);
        }

        public RelayCommand GoBack
        {
            get
            {
                return new RelayCommand(async obj =>
                {
                    var index = ServerPath.LastIndexOf("/");

                    ServerPath = ServerPath.Substring(0, index);
                    FilesAndDirectories = await client.List(ServerPath);

                }, obj => ServerPath != rootPath);
            }
        }

        public RelayCommand Download
        {
            get
            {
                return new RelayCommand(async obj =>
                {
                    await GetFile(SelectedFile);
                }, obj => IsConnected && SelectedFile != null);
            }
        }

        public RelayCommand DownloadAll
        {
            get
            {
                return new RelayCommand(async obj =>
                {
                    foreach (var file in Directory.GetFiles(ServerPath))
                    {
                        await GetFile(file.Substring(file.LastIndexOf("\\") + 1));
                    }

                }, obj => IsConnected);
            }
        }

        public string DownloadPath { get; set; }

        public ObservableCollection<string> DownloadingFiles { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> DownloadedFiles { get; set; } = new ObservableCollection<string>();

        private async Task GetFile(string fileName)
        {
            DownloadingFiles.Add(fileName);

            var fileInfo = await client.Get(ServerPath + "/" + fileName);
            using var fileWriter = new StreamWriter(new FileStream(DownloadPath + fileName, FileMode.Create));
            await fileWriter.WriteAsync(fileInfo);

            DownloadingFiles.Remove(fileName);

            if (DownloadedFiles.Contains(fileName))
            {
                return;
            }

            DownloadedFiles.Add(fileName);
        }

        public void MarkSelectedItem(string name)
        {
            if (IsFile(name))
            {
                SelectedFile = name;
            }
            else
            {
                SelectedFile = null;
            }
        }

        private void Close()
        {
            client?.Dispose();
            server?.Stop();
            server?.Dispose();

            IsConnected = false;

            DownloadingFiles.Clear();
            DownloadedFiles.Clear();
        }
    }
}