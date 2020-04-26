using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace SimpleFTP.Tests
{
    [TestClass]
    public class NotConnectedClientTest
    {
        private Client client;
        private Server server;

        [TestInitialize]
        public void TestInitialize()
        {
            const int port = 7777;
            const string hostname = "localhost";

            server = new Server(port);
            client = new Client(hostname, port);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ClientNotConnectedTest()
        {
            try
            {
                server.Start();
                await client.List("../../../SimpleFTP.Tests/TestFiles");
            }
            finally
            {
                server.Stop();
                server.Dispose();
            }
        }
    }
}
