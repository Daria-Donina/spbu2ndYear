using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckSum
{
    class CheckSumThreadSafeCalculator
    {
        private readonly object lockDirectory = new object();
        private readonly object lockFile = new object();
        private readonly object locker = new object();

        private bool IsDirectory(string path)
        {
            var attributes = File.GetAttributes(path);
            return (attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public byte[] Calculate(string path)
        {
            using (var md5Hash = MD5.Create())
            {
                if (!IsDirectory(path))
                {
                    lock (locker)
                    {
                        var lines = File.ReadAllLines(path);
                        var fileContent = string.Join("", lines);

                        return md5Hash.ComputeHash(Encoding.Default.GetBytes(fileContent));
                    }
                }
                else
                {
                    var directory = Directory.CreateDirectory(path);

                    Monitor.Enter(lockDirectory);
                    var directories = directory.GetDirectories();
                    Array.Sort(directories);
                    Monitor.Exit(lockDirectory);

                    Monitor.Enter(lockFile);
                    var files = directory.GetFiles();
                    Array.Sort(files);
                    Monitor.Exit(lockFile);

                    var stringToHash = "";

                    foreach (var item in directories)
                    {
                        stringToHash += item.FullName;
                    }

                    foreach (var file in files)
                    {
                        stringToHash += Calculate(file.FullName);
                    }

                    return md5Hash.ComputeHash(Encoding.Default.GetBytes(stringToHash));
                }
            }
        }
    }
}
