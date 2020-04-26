using System;
using System.Threading.Tasks;
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

            server = new Server(port);
            client = new Client(hostname, port);

            server.Start();
            client.Connect();
        }

        [TestCleanup]
        public void TestFinalize()
        {
            client.Dispose();
            server.Stop();
            server.Dispose();
        }

        [TestMethod]
        public async Task ListTest()
        {
            var response = await client.List("../../../SimpleFTP.Tests/TestFiles");
            var directories = response.Item1;
            var files = response.Item2;

            Assert.IsTrue(files.Exists(file => file.Name == "TestFile1.txt"));
            Assert.IsTrue(directories.Exists(directory => directory.Name == "SomeDirectory"));
        }

        [TestMethod]
        public async Task GetTest()
        {
            var response = await client.Get("../../../SimpleFTP.Tests/TestFiles/GetTestFile.txt");
            Assert.AreEqual(91, response.Item1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetNonexistentFileTest() => await client.Get("../../../SimpleFTP.Tests/TestFiles/NonexistentFile.txt");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DirectoryToGetTest() => await client.Get("../../../SimpleFTP.Tests/TestFiles");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task FileToListTest() => await client.List("../../../SimpleFTP.Tests/TestFiles/TestFile1.txt");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ListNonexistentDirectoryTest() => await client.List("../../../SimpleFTP.Tests/NonexistentDirectory");

        [TestMethod]
        public async Task MultipleClientsTest()
        {
            var client1 = new Client("localhost", 7777);

            try
            {
                client1.Connect();

                var response = await client.Get("../../../SimpleFTP.Tests/TestFiles/GetTestFile.txt");
                var response1 = await client1.Get("../../../SimpleFTP.Tests/TestFiles/GetTestFile.txt");

                Assert.AreEqual(response, response1);
            }
            finally
            {
                client1.Dispose();
            }
        }

        [TestMethod]
        public async Task MultipleRequestsTest()
        {
            await client.Get("../../../SimpleFTP.Tests/TestFiles/GetTestFile.txt");
            var response1 = await client.Get("../../../SimpleFTP.Tests/TestFiles/TestFile1.txt");
            await client.Get("../../../SimpleFTP.Tests/TestFiles/TestFile2.txt");
            var response4 = await client.List("../../../SimpleFTP.Tests/TestFiles");

            Assert.AreEqual(9, response1.Item1);
            Assert.AreEqual(4, response4.Item1.Count + response4.Item2.Count);
        }
    }
}