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
                while (true)
                {
                    var command = await reader.ReadLineAsync();
                    if (command.StartsWith("1"))
                    {
                        var answer = await ListResponse(command.Substring(2));
                    }
                    else if (command.StartsWith("2"))
                    {
                        var answer = await GetResponse(command.Substring(2));
                    }
                    else
                    {
                        throw new InvalidDataException("The request has the wrong format.");
                    }
                }
            });
        }

        private async Task<string> ListResponse(string path)
        {

        }

        private async Task<string> GetResponse(string path)
        {

        }

        public void Stop()
        {
            cancellationToken.Cancel();
            listener.Stop();
        }
    }
}
