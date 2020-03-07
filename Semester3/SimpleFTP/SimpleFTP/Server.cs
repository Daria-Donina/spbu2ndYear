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

        public void Start()
        {

        }

        private void GetResponse()
        {

        }

        private void ListResponse()
        {

        }

        public void Stop()
        {
            cancellationToken.Cancel();
        }
    }
}
