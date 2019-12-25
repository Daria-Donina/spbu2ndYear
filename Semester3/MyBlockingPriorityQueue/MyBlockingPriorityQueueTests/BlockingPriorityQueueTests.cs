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
            var threads = new List<Thread>();

            threads.Add(new Thread(() => intQueue.Enqueue(1, 5)));
            threads.Add(new Thread(() => intQueue.Enqueue(2, 4)));
            threads.Add(new Thread(() => intQueue.Enqueue(3, 6)));

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.AreEqual(3, intQueue.Dequeue());
            Assert.AreEqual(1, intQueue.Dequeue());
            Assert.AreEqual(2, intQueue.Dequeue());
        }

        [TestMethod]
        public void DequeueEmpty()
        {
            var threads = new List<Thread>();

            int value = 0;
            var threadDeq = new Thread(() => value = intQueue.Dequeue());

            threadDeq.Start();
            threadDeq.Join();

            var threadInq = new Thread(() => intQueue.Enqueue(4, 1));
            Assert.AreEqual(4, value);
        }
    }
}
