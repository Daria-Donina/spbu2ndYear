using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkChat
{
    class Server
    {
        TcpListener listener;
        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"Listening on port {port}");
        }

        public async void Start()
        {
            var client = await listener.AcceptTcpClientAsync();
            while (true)
            {
                Writer(client.GetStream());
                Reader(client.GetStream());
            }
        }

        public void Writer(NetworkStream stream)
        {
            Task.Run(async () =>
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                while (true)
                {
                    string lineToSend = Console.ReadLine();
                    await writer.WriteLineAsync(lineToSend);
                }
            });
        }

        public void Reader(NetworkStream stream)
        {
            Task.Run(async () =>
            {
                var reader = new StreamReader(stream);
                var data = await reader.ReadLineAsync();
                Console.WriteLine($"Received: {data}");
            });
        }
    }
}
