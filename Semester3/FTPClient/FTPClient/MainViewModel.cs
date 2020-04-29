using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using ClientAPI;

namespace FTPClient
{
    /// <summary>
    /// Class representing View Model of the app.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private Client client;
        private (List<DirectoryInfo>, List<FileInfo>) filesAndDirectories;
        private int port = 7777;
        private string serverPath = rootPath;
        private const string rootPath = "../../..";
        private bool isConnected;
        private string selectedFile;
        private string hostname = "localhost";

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The DNS name of the remote host to which you intend to connect.
        /// </summary>
        public string Hostname
        {
            get => hostname;
            set
            {
                if (!isConnected)
                {
                    hostname = value;
                }
            }
        }

        /// <summary>
        /// The port number of the remote host to which you intend to connect.
        /// </summary>
        public string Port
        {
            get => port.ToString();
            set
            {
                if (int.TryParse(value, out int result) && !isConnected)
                {
                    port = result;
                }
            }
        }

        /// <summary>
        /// Connects the client to the server.
        /// </summary>
        public RelayCommand Connect
        {
            get
            {
                return new RelayCommand(async obj =>
                {
                    try
                    {
                        Close();

                        client = new Client(Hostname, port);

                        client.Connect();
                        isConnected = true;

                        serverPath = rootPath;

                        await RefreshList();
                    }
                    catch (SocketException)
                    {
                        if (port == 0)
                        {
                            MessageBox.Show("Port is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("Hostname is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        MessageBox.Show("Port is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }

        /// <summary>
        /// List of files and directories on the server that client sees.
        /// </summary>
        public ObservableCollection<string> ListOfFilesAndDirectories { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Path to download files to.
        /// </summary>
        public string DownloadPath { get; set; } = "";

        /// <summary>
        /// List of files that are downloading.
        /// </summary>
        public ObservableCollection<string> DownloadingFiles { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// List of downloaded files.
        /// </summary>
        public ObservableCollection<string> DownloadedFiles { get; set; } = new ObservableCollection<string>();

        private async Task RefreshList()
        {
            filesAndDirectories = await client.List(serverPath);

            ListOfFilesAndDirectories.Clear();

            foreach (var item in filesAndDirectories.Item1)
            {
                ListOfFilesAndDirectories.Add(item.Name);
            }
            foreach (var item in filesAndDirectories.Item2)
            {
                ListOfFilesAndDirectories.Add(item.Name);
            }
        }

        private bool IsFile(string name) => !filesAndDirectories.Item1.Exists(item => item.Name == name);

        /// <summary>
        /// Opens folder when it's double-clicked.
        /// </summary>
        /// <param name="name">Name of the folder.</param>
        public async Task OpenFolder(string name)
        {
            if (IsFile(name))
            {
                return;
            }

            serverPath += $"/{name}";
            await RefreshList();
        }

        /// <summary>
        /// Goes one folder back.
        /// </summary>
        public RelayCommand GoBack
        {
            get
            {
                return new RelayCommand(async obj =>
                {
                    var index = serverPath.LastIndexOf("/");

                    serverPath = serverPath.Substring(0, index);
                    await RefreshList();

                }, obj => serverPath != rootPath);
            }
        }

        /// <summary>
        /// Downloads file that has the focus.
        /// </summary>
        public RelayCommand Download
        {
            get
            {
                return new RelayCommand(async obj =>
                {
                    try
                    {
                        await GetFile(selectedFile);
                    }
                    catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Download path is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }, obj => isConnected && selectedFile != null);
            }
        }

        /// <summary>
        /// Downloads all the files in the current folder that client sees.
        /// </summary>
        public RelayCommand DownloadAll
        {
            get
            {
                return new RelayCommand(async obj =>
                {
                    try
                    {
                        foreach (var file in Directory.GetFiles(serverPath))
                        {
                            await GetFile(file.Substring(file.LastIndexOf("\\") + 1));
                        }
                    }
                    catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Download path is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }, obj => isConnected);
            }
        }

        private async Task GetFile(string fileName)
        {
            try
            {
                DownloadingFiles.Add(fileName);

                if (!DownloadPath.EndsWith('/'))
                {
                    DownloadPath += "/";
                }

                var fileInfo = await client.Get(serverPath + "/" + fileName);
                using var fileWriter = new StreamWriter(new FileStream(DownloadPath + fileName, FileMode.Create));
                await fileWriter.WriteAsync(fileInfo);

                DownloadingFiles.Remove(fileName);

                if (DownloadedFiles.Contains(fileName))
                {
                    return;
                }

                DownloadedFiles.Add(fileName);
            }
            finally
            {
                DownloadingFiles.Remove(fileName);
            }
        }

        /// <summary>
        /// Saves the file that has focus.
        /// </summary>
        /// <param name="name"></param>
        public void MarkSelectedItem(string name)
        {
            if (IsFile(name))
            {
                selectedFile = name;
            }
            else
            {
                selectedFile = null;
            }
        }

        /// <summary>
        /// Disposes client and server.
        /// </summary>
        public void Close()
        {
            client?.Dispose();

            isConnected = false;

            DownloadingFiles.Clear();
            DownloadedFiles.Clear();

            client = null;
        }
    }
}