using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFTP
{
    class Client
    {
        private TcpClient tcpClient;

        public Client(string hostname, int port)
        {
            tcpClient = new TcpClient(hostname, port);
        }

        private StreamReader reader;

        private StreamWriter writer;

        private async void Connect()
        {
            var stream = tcpClient.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        /// <summary>
        /// Returns a list of files contained in a directory on the server.
        /// </summary>
        /// <param name="path"> Relative path to the directory. </param>
        /// <returns> String containing a list of files' and directories' names and their total number. </returns>
        public void List(string path)
        {
            var request = $"1 {path}";
        }

        /// <summary>
        /// Downloads a file from the server.
        /// </summary>
        /// <param name="path"> Relative file path. </param>
        /// <returns> String containing size of the file and its content in bytes. </returns>
        public async void Get(string path)
        {
            var request = $"2 {path}";
            await writer.WriteLineAsync(request);
            await reader.ReadLineAsync();
        }
    }
}
