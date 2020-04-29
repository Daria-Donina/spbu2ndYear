using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ServerAPI;

namespace FTPClient.Tests
{
    [TestFixture]
    public class ViewModelTests
    {
        private MainViewModel model;
        private object obj;

        [SetUp]
        public void SetUp()
        {
            var server = new Server(5555);

            Task.Run(async () => await server.Start());

            model = new MainViewModel();
            obj = new object();
        }

        [TearDown]
        public void CleanUp()
        {
            model.Close();
        }

        [Test]
        public void ConnectTest()
        {
            Connect();

            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("ViewModelTests.cs"));
            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("bin"));
            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("obj"));
            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("FTPClient.Tests.csproj"));
        }

        private void Connect()
        {
            model.Port = "5555";
            model.Hostname = "localhost";
            var command = model.Connect;
            command.Execute(obj);

            Thread.Sleep(200);
        }

        [Test]
        public async Task DownloadTest()
        {
            Connect();

            await model.OpenFolder("TestFiles");
            model.MarkSelectedItem("TestFile1.txt");

            model.DownloadPath = "../../../TestFiles/DownloadTestResultsFolder";

            var command = model.Download;
            command.Execute(obj);

            Assert.IsTrue(model.DownloadingFiles.Contains("TestFile1.txt"));

            Thread.Sleep(200);

            Assert.IsTrue(model.DownloadedFiles.Contains("TestFile1.txt"));
            Assert.IsFalse(model.DownloadingFiles.Contains("TestFile1.txt"));

            Assert.IsTrue(File.Exists("../../../TestFiles/DownloadTestResultsFolder/TestFile1.txt"));
            Assert.IsFalse(File.Exists("../../../TestFiles/DownloadTestResultsFolder/TestFile2.txt"));
        }

        [Test]
        public async Task DownloadAllTest()
        {
            Connect();

            await model.OpenFolder("TestFiles");

            model.DownloadPath = "../../../TestFiles/DownloadAllTestResultsFolder";

            var command = model.DownloadAll;
            command.Execute(obj);

            Thread.Sleep(200);

            Assert.IsTrue(model.DownloadedFiles.Contains("TestFile1.txt"));
            Assert.IsTrue(model.DownloadedFiles.Contains("TestFile2.txt"));
            Assert.IsFalse(model.DownloadingFiles.Contains("TestFile1.txt"));
            Assert.IsFalse(model.DownloadingFiles.Contains("TestFile2.txt"));

            Assert.IsTrue(File.Exists("../../../TestFiles/DownloadAllTestResultsFolder/TestFile1.txt"));
            Assert.IsTrue(File.Exists("../../../TestFiles/DownloadAllTestResultsFolder/TestFile2.txt"));
        }

        [Test]
        public async Task OpenFolderTest()
        {
            Connect();

            await model.OpenFolder("TestFiles");
            await model.OpenFolder("OpenFolderTestFolder");

            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("SomeFile.txt"));
        }

        [Test]
        public async Task GoBackTest()
        {
            Connect();

            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("ViewModelTests.cs"));

            await model.OpenFolder("TestFiles");
            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("DownloadTestResultsFolder"));

            var command2 = model.GoBack;
            command2.Execute(obj);

            Thread.Sleep(200);

            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("ViewModelTests.cs"));
        }

        [Test]
        public void ManyConnectsTest()
        {
            Connect();
            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("ViewModelTests.cs"));

            Connect();
            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("ViewModelTests.cs"));

            Connect();
            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("ViewModelTests.cs"));
        }

        [Test]
        public async Task ManyDownloadsTest()
        {
            Connect();

            await model.OpenFolder("TestFiles");
            model.MarkSelectedItem("TestFile2.txt");

            model.DownloadPath = "../../../TestFiles/ManyDownloadsTestResultsFolder";

            var command1 = model.Download;
            command1.Execute(obj);

            Thread.Sleep(200);

            var command2 = model.Download;
            command2.Execute(obj);

            Thread.Sleep(200);

            var command3 = model.Download;
            command3.Execute(obj);

            Thread.Sleep(200);

            Assert.AreEqual(1, model.DownloadedFiles.Count);
        }

        [Test]
        public async Task ManyDownloadAllsTest()
        {
            Connect();

            await model.OpenFolder("TestFiles");

            model.DownloadPath = "../../../TestFiles/ManyDownloadsTestResultsFolder";

            var command1 = model.DownloadAll;
            command1.Execute(obj);

            Thread.Sleep(200);

            var command2 = model.DownloadAll;
            command2.Execute(obj);

            Thread.Sleep(200);

            var command3 = model.DownloadAll;
            command3.Execute(obj);

            Thread.Sleep(200);

            Assert.AreEqual(2, model.DownloadedFiles.Count);
        }

        [Test]
        public async Task IncorrectNameOpenFolderTest()
        {
            Connect();

            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("ViewModelTests.cs"));

            await model.OpenFolder("NoName");

            Assert.IsTrue(model.ListOfFilesAndDirectories.Contains("ViewModelTests.cs"));
        }
    }
}