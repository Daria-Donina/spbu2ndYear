using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleFTP
{
    /// <summary>
    /// Class implementing a network client that allows to execute some requests.
    /// </summary>
    public class Client
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
        public async Task<string> List(string path)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            var request = $"1 {path}";
            await writer.WriteLineAsync(request);
            return await reader.ReadLineAsync();
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
            return await reader.ReadLineAsync();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Disconnect()
        {
            writer.Close();
            reader.Close();
            tcpClient.Close();
            tcpClient.Dispose();
        }
    }
}
