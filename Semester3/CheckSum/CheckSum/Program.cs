using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckSum
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopWatchSingular = new Stopwatch();
            var stopWatchMultithreaded = new Stopwatch();

            var calculator = new CheckSumCalculator();
            var firstPath = "someString";

            var threadSafeCalculator = new CheckSumThreadSafeCalculator();
            var secondPath = "anotherString";

            stopWatchSingular.Start();
            calculator.Calculate(firstPath);
            stopWatchSingular.Stop();

            stopWatchMultithreaded.Start();
            calculator.Calculate(secondPath);
            stopWatchMultithreaded.Stop();

            var timespanFirst = stopWatchSingular.Elapsed;
            var timespanSecond = stopWatchMultithreaded.Elapsed;

            string elapsedTimeFirst = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timespanFirst.Hours, timespanFirst.Minutes, timespanFirst.Seconds,
                timespanFirst.Milliseconds / 10);
            Console.WriteLine("RunTime of the singular calculator" + elapsedTimeFirst);

            string elapsedTimeSecond = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timespanSecond.Hours, timespanSecond.Minutes, timespanSecond.Seconds,
                timespanSecond.Milliseconds / 10);
            Console.WriteLine("RunTime of the multithreaded calculator" + elapsedTimeSecond);
        }
    }
}
