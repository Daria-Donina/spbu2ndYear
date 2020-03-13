using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFTP
{
    /// <summary>
    /// Class implementing a network server that handles some requests.
    /// </summary>
    public class Server
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
                        await ListResponse(path, writer);
                    }
                    else if (command.StartsWith("2"))
                    {
                        await GetResponse(path, writer);
                    }
                    else
                    {
                        throw new InvalidDataException("The request has the wrong format.");
                    }
                }
            });
        }

        private async Task ListResponse(string path, StreamWriter writer)
        {
            var dirInfo = new DirectoryInfo(path);

            if (!dirInfo.Exists)
            {
                await writer.WriteLineAsync("-1");
                return;
            }

            var files = dirInfo.GetFiles();
            var directories = dirInfo.GetDirectories();

            var response = $"{files.Length + directories.Length}";

            var fullPath = dirInfo.FullName;
            
            foreach (var file in files)
            {
                response += $" {file.FullName.Substring(fullPath.Length + 1)} false";
            }

            foreach (var directory in directories)
            {
                response += $" {directory.FullName.Substring(fullPath.Length + 1)} true";
            }

            await writer.WriteLineAsync(response);
        }

        private async Task GetResponse(string path, StreamWriter writer)
        {
            var fileInfo = new FileInfo(path);

            if(!fileInfo.Exists)
            {
                await writer.WriteLineAsync("-1");
                return;
            }

            await writer.WriteLineAsync($"{fileInfo.Length} {File.ReadAllBytes(path)}");
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            cancellationToken.Cancel();
            listener.Stop();
        }
    }
}
