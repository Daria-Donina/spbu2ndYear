using System;

namespace MyNUnit
{
    static class Program
    {
        static void Main(string[] args)
        {
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