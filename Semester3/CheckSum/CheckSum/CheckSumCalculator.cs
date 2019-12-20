using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CheckSum
{
    class CheckSumCalculator
    {

        private bool IsDirectory(string path)
        {
            var attributes = File.GetAttributes(path);
            return (attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public byte[] Calculate(string path)
        {
            var md5Hash = MD5.Create();

            if (!IsDirectory(path))
            {
                var lines = File.ReadAllLines(path);
                var fileContent = string.Join("", lines);

                return md5Hash.ComputeHash(Encoding.Default.GetBytes(fileContent));
            }
            else
            {
                var directory = Directory.CreateDirectory(path);
                var files = directory.GetFiles();
                var directories = directory.GetDirectories();

                Array.Sort(files);
                Array.Sort(directories);

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
