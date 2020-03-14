using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFTP
{
    /// <summary>
    /// Class implementing a network server that handles some requests.
    /// </summary>
    public class Server : IDisposable
    {
        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();

        private readonly TcpListener listener;

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        /// <summary>
        /// Runs the server.
        /// </summary>
        public void Start()
        {
            listener.Start();

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    RequestHandler(client.GetStream());
                }
            });
        }

        private void RequestHandler(NetworkStream stream)
        {
            Task.Run(async () =>
            {
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream) { AutoFlush = true };

                while (!cancellationToken.IsCancellationRequested)
                {
                    var command = await reader.ReadLineAsync();
                    var path = command.Substring(2);

                    if (command.StartsWith("1"))
                    {
                        var response = List(path);
                        await writer.WriteLineAsync(response);
                    }
                    else if (command.StartsWith("2"))
                    {
                        var response = Get(path);
                        await writer.WriteLineAsync(response);
                    }
                    else
                    {
                        throw new InvalidDataException("The request has the wrong format.");
                    }
                }
            });
        }

        private static string List(string path)
        {
            var dirInfo = new DirectoryInfo(path);

            if (!dirInfo.Exists)
            {
                return "-1";
            }

            var files = dirInfo.GetFiles();
            var directories = dirInfo.GetDirectories();

            var response = new StringBuilder();

            response.Append($"{files.Length + directories.Length}");

            var fullPath = dirInfo.FullName;
            
            foreach (var file in files)
            {
                response.Append($" {file.FullName.Substring(fullPath.Length + 1)} false");
            }

            foreach (var directory in directories)
            {
                response.Append($" {directory.FullName.Substring(fullPath.Length + 1)} true");
            }

            return response.ToString();
        }

        private static string Get(string path)
        {
            var fileInfo = new FileInfo(path);

            if(!fileInfo.Exists)
            {
                return "-1";
            }

            return $"{fileInfo.Length} {File.ReadAllBytes(path)}";
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            cancellationToken.Cancel();
            listener.Stop();
        }

        /// <summary>
        /// Releases resources used by the server.
        /// </summary>
        public void Dispose() => cancellationToken.Dispose();
    }
}
