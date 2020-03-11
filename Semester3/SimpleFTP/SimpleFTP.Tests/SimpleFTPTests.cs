using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleFTP.Tests
{
    [TestClass]
    public class SimpleFTPTests
    {
        private Client client;
        private Server server;

        [TestInitialize]
        public void TestInitialize()
        {
            const int port = 7777;
            const string hostname = "localhost";

            client = new Client(hostname, port);
            server = new Server(port);
        }

        [TestMethod]
        public async void ListTest()
        {
            client.Connect();
            await server.Start();

            var response = await client.List("../../../SimpleFTP.Tests/TestFiles");
            var responseSplitted = response.Split(' ');

            Assert.AreEqual(4, int.Parse(responseSplitted[0]));

            Assert.AreEqual("../../../SimpleFTP.Tests/TestFiles/TestFile1.txt", responseSplitted[3]);
            Assert.IsFalse(bool.Parse(responseSplitted[4]));

            Assert.AreEqual("../../../SimpleFTP.Tests/TestFiles/SomeDIrectory", responseSplitted[7]);
            Assert.IsTrue(bool.Parse(responseSplitted[8]));
        }

        [TestMethod]
        public async void GetTest()
        {
            client.Connect();
            await server.Start();

            var response = await client.Get("../../../SimpleFTP.Tests/TestFiles/GetTestFile.txt");
            Assert.AreNotEqual("-1", response);

            var responseSplitted = response.Split(' ');
            Assert.AreEqual(91, long.Parse(responseSplitted[0]));

            var expectedString = "Ah, Mister Secretary Mister Burr, sir Did'ya hear the news about good old General Mercer No";
            Assert.AreEqual(Encoding.ASCII.GetBytes(expectedString), responseSplitted[1]);
        }

        [TestMethod]
        public async void ListNonexistentDirectoryTest()
        {
            client.Connect();
            await server.Start();

            var response = await client.List("../../../SimpleFTP.Tests/NonexistentDirectory");
            Assert.AreEqual("-1", response);
        }

        [TestMethod]
        public async void GetNonexistentFileTest()
        {
            client.Connect();
            await server.Start();

            var response = await client.Get("../../../SimpleFTP.Tests/TestFiles/NonexistentFile.txt");
            Assert.AreEqual("-1", response);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async void DirectoryToGetTest()
        {
            client.Connect();
            await server.Start();

            await client.Get("../../../SimpleFTP.Tests/TestFiles");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async void FileToListTest()
        {
            client.Connect();
            await server.Start();

            await client.List("../../../SimpleFTP.Tests/TestFiles/TestFile1.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async void DifferentPortsTest()
        {
            const string hostname = "localhost";

            client = new Client(hostname, 3456);
            server = new Server(3457);

            await client.Get("../../../SimpleFTP.Tests/TestFiles/TestFile2.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async void ServerNotStartedTest()
        {
            client.Connect();

            await client.List("../../../SimpleFTP.Tests/TestFiles");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async void ClientNotConnectedTest()
        {
            await server.Start();

            await client.List("../../../SimpleFTP.Tests/TestFiles");
        }
    }
}
