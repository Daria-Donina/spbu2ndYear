using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleFTP
{
    /// <summary>
    /// Class implementing a network client that allows to execute some requests.
    /// </summary>
    public class Client : IDisposable
    {
        private readonly string hostname;

        private readonly int port;

        public Client(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        private StreamReader reader;

        private StreamWriter writer;

        private TcpClient tcpClient;

        /// <summary>
        /// Returns true if the client is connected to the server and false otherwise.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Connects client to the server.
        /// </summary>
        public void Connect()
        {
            tcpClient = new TcpClient(hostname, port);
            var stream = tcpClient.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };
            IsConnected = true;
        }

        /// <summary>
        /// Returns a list of files contained in a directory on the server.
        /// </summary>
        /// <param name="path"> Relative path to the directory. </param>
        /// <returns> String containing a list of files' and directories' names and their total number. </returns>
        public async Task<(List<DirectoryInfo>, List<FileInfo>)> List(string path)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            var request = $"1 {path}";
            await writer.WriteLineAsync(request);

            return HandleListResponse(await reader.ReadLineAsync());
        }

        private (List<DirectoryInfo>, List<FileInfo>) HandleListResponse(string response)
        {
            if (response == "-1")
            {
                throw new ArgumentException("Directory does not exist");
            }

            var directories = new List<DirectoryInfo>();
            var files = new List<FileInfo>();

            var responseArray = response.Split(' ');

            for (int i = 2; i < responseArray.Length; ++i)
            {
                if (i % 2 != 0)
                {
                    continue;
                }

                if (bool.Parse(responseArray[i]))
                {
                    directories.Add(new DirectoryInfo(responseArray[i - 1]));
                }
                else
                {
                    files.Add(new FileInfo(responseArray[i - 1]));
                }
            }

            return (directories, files);
        }

        /// <summary>
        /// Downloads a file from the server.
        /// </summary>
        /// <param name="path"> Relative file path. </param>
        /// <returns> String containing size of the file and its content in bytes. </returns>
        public async Task<string> Get(string path)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            var request = $"2 {path}";
            await writer.WriteLineAsync(request);
            var response = await reader.ReadLineAsync();

            if (response == "-1")
            {
                throw new ArgumentException("File does not exist");
            }

            return response;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Dispose()
        {
            writer?.Dispose();
            reader?.Dispose();
            tcpClient?.Dispose();
        }
    }
}
