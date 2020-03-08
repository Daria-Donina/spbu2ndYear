using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFTP
{
    class Server
    {
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();

        private TcpListener listener;

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async void Start()
        {
            listener.Start();

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                Reader(client.GetStream());
            }
        }

        private void Reader(NetworkStream stream)
        {
            Task.Run(async () =>
            {
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream);

                while (true)
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

        public void Stop()
        {
            cancellationToken.Cancel();
            listener.Stop();
        }
    }
}
