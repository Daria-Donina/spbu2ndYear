using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyBlockingPriorityQueue;

namespace MyBlockingPriorityQueueTests
{
    [TestClass]
    public class BlockingPriorityQueueTests
    {
        private BlockingPriorityQueue<int> intQueue;
        [TestInitialize]
        public void Initialize()
        {
            intQueue = new BlockingPriorityQueue<int>();
        }

        [TestMethod]
        public void DequeueTest()
        {
            intQueue.Enqueue(1, 3);
            intQueue.Enqueue(2, 7);
            intQueue.Enqueue(3, 6);
            intQueue.Enqueue(4, 4);
        }

        [TestMethod]
        public void DequeueFromHead()
        {
            intQueue.Enqueue(1, 6);

            const int amountOfThreads = 10;
            var threads = new List<Thread>();

            for (int i = 0; i < amountOfThreads; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    Assert.AreEqual(1, intQueue.Dequeue());
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        [TestMethod]
        public void DequeueEmpty()
        {

        }
    }
}
