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
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            var server = new Server();

            server.On();
            client.On();
            
           
        }
    }
}
