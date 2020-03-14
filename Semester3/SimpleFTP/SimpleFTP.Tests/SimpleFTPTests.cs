﻿using System;
using System.Text;
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
        }

        private void RunClientAndServer()
        {
            server.Start();
            client.Connect();
        }

        private void CloseClientAndServer()
        {
            client.Disconnect();
            server.Stop();
        }

        [TestMethod]
        public async Task ListTest()
        {
            RunClientAndServer();

            var response = await client.List("../../../SimpleFTP.Tests/TestFiles");
            var responseSplitted = response.Split(' ');

            Assert.AreEqual(4, int.Parse(responseSplitted[0]));

            Assert.IsTrue(Array.Exists(responseSplitted, file => file == "TestFile1.txt"));
            var fileIndex = Array.IndexOf(responseSplitted, "TestFile1.txt");
            Assert.IsFalse(bool.Parse(responseSplitted[fileIndex + 1]));

            Assert.IsTrue(Array.Exists(responseSplitted, directory => directory == "SomeDirectory"));
            var directoryIndex = Array.IndexOf(responseSplitted, "SomeDirectory");
            Assert.IsTrue(bool.Parse(responseSplitted[directoryIndex + 1]));

            CloseClientAndServer();
        }

        [TestMethod]
        public async Task GetTest()
        {
            RunClientAndServer();

            var response = await client.Get("../../../SimpleFTP.Tests/TestFiles/GetTestFile.txt");
            Assert.AreNotEqual("-1", response);

            var responseSplitted = response.Split(' ');
            Assert.AreEqual(91, long.Parse(responseSplitted[0]));

            CloseClientAndServer();
        }

        [TestMethod]
        public async Task GetNonexistentFileTest()
        {
            var response = await GetInvalidTest("../../../SimpleFTP.Tests/TestFiles/NonexistentFile.txt");
            Assert.AreEqual("-1", response);

            CloseClientAndServer();
        }

        [TestMethod]
        public async Task DirectoryToGetTest()
        {
            var response = await GetInvalidTest("../../../SimpleFTP.Tests/TestFiles");
            Assert.AreEqual("-1", response);

            CloseClientAndServer();
        }

        private async Task<string> GetInvalidTest(string path)
        {
            RunClientAndServer();

            return await client.Get(path);
        }

        [TestMethod]
        public async Task FileToListTest()
        {
            var response = await ListInvalidTest("../../../SimpleFTP.Tests/TestFiles/TestFile1.txt");
            Assert.AreEqual("-1", response);

            CloseClientAndServer();
        }

        [TestMethod]
        public async Task ListNonexistentDirectoryTest()
        {
            var response = await ListInvalidTest("../../../SimpleFTP.Tests/NonexistentDirectory");
            Assert.AreEqual("-1", response);

            CloseClientAndServer();
        }

        private async Task<string> ListInvalidTest(string path)
        {
            RunClientAndServer();

            return await client.List(path);
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
            }
        }

        [TestMethod]
        public async Task MultipleClientsTest()
        {
            RunClientAndServer();

            var client1 = new Client("localhost", 7777);
            client1.Connect();

            var response = await client.Get("../../../SimpleFTP.Tests/TestFiles/GetTestFile.txt");
            var response1 = await client1.Get("../../../SimpleFTP.Tests/TestFiles/GetTestFile.txt");

            Assert.AreEqual(response, response1);

            client1.Disconnect();

            CloseClientAndServer();
        }

        [TestMethod]
        public async Task MultipleRequestsTest()
        {
            RunClientAndServer();

            await client.Get("../../../SimpleFTP.Tests/TestFiles/GetTestFile.txt");
            var response1 = await client.Get("../../../SimpleFTP.Tests/TestFiles/TestFile1.txt");
            await client.Get("../../../SimpleFTP.Tests/TestFiles/TestFile2.txt");
            var response4 = await client.List("../../../SimpleFTP.Tests/TestFiles");

            Assert.AreEqual(9, long.Parse(response1[0].ToString()));
            Assert.AreEqual(4, int.Parse(response4[0].ToString()));

            CloseClientAndServer();
        }

        [TestMethod]
        public async Task ListEmptyDirectoryTest()
        {
            RunClientAndServer();

            var response = await client.List("../../../SimpleFTP.Tests/TestFiles/SomeDirectory");
            Assert.AreEqual("0", response);

            CloseClientAndServer();
        }
    }
}