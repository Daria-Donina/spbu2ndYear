using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetworkChat
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var server = new Server(int.Parse(args[0]));
                server.Start();

            }
            else
            {
                var client = new Client(int.Parse(args[0]), IPAddress.Parse(args[1]));
                client.Start();
            }
        }
    }
}
