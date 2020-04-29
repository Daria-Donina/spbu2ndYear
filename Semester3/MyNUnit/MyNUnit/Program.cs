using System;

namespace MyNUnit
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Enter one path to command line arguments");
                return;
            }

            try
            {
                NUnit.PrintResults(NUnit.RunTests(args[0]));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message}");
            }
        }
    }
}