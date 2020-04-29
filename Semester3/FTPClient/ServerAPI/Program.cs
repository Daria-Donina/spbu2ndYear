using System.Threading.Tasks;

namespace ServerAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new Server(7777);
            await server.Start();
        }
    }
}
