﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerAPI
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
        public async Task Start()
        {
            listener.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                RequestHandler(client.GetStream());
            }
        }

        private void RequestHandler(NetworkStream stream)
        {
            Task.Run(async () =>
            {
                using (var writer = new StreamWriter(stream) { AutoFlush = true })
                using (var reader = new StreamReader(stream))
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var command = await reader.ReadLineAsync();

                        if (command is null)
                        {
                            break;
                        }

                        var path = command.Substring(2);

                        if (command.StartsWith("1"))
                        {
                            var response = List(path);
                            await writer.WriteLineAsync(response);
                        }
                        else
                        {
                            var response = Get(path);
                            await writer.WriteLineAsync(response);
                        }
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

            foreach (var directory in directories)
            {
                response.Append($" {directory.FullName.Substring(fullPath.Length + 1)} true");
            }

            foreach (var file in files)
            {
                response.Append($" {file.FullName.Substring(fullPath.Length + 1)} false");
            }

            return response.ToString();
        }

        private static string Get(string path)
        {
            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                return "-1";
            }

            var buff = File.ReadAllText(path);
            return buff;
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            cancellationToken?.Cancel();
            listener?.Stop();
        }

        /// <summary>
        /// Releases resources used by the server.
        /// </summary>
        public void Dispose() => cancellationToken.Dispose();
    }
}