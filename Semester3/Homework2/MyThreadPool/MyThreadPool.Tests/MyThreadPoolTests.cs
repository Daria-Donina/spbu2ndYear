﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace MyThreadPool.Tests
{
    [TestClass]
    public class MyThreadPoolTests
    {
        [TestMethod]
        public void ActiveThreadNumberTest()
        {
            var threadpool1 = new MyThreadPool(10);
            var threadpool2 = new MyThreadPool(1);
            var threadpool3 = new MyThreadPool(7);

            Assert.IsTrue(threadpool1.ActiveThreads >= 10);
            Assert.IsTrue(threadpool2.ActiveThreads >= 1);
            Assert.IsTrue(threadpool3.ActiveThreads >= 7);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IncorrectNumberOfThreadsTest() => _ = new MyThreadPool(0);

        [TestMethod]
        public void OneThreadTest()
        {
            var threadpool = new MyThreadPool(1);
            var task1 = threadpool.AddTask(() => 1 + 9);
            var task2 = threadpool.AddTask(() => "test" + " " + "string");
            var task3 = threadpool.AddTask(() => 82349374823 - 8173690442);

            Assert.AreEqual(10, task1.Result);
            Assert.AreEqual("test string", task2.Result);
            Assert.AreEqual(74175684381, task3.Result);

            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
            Assert.IsTrue(task3.IsCompleted);
        } 

        private void FewThreadsTest(int threadNumber)
        {
            var threadpool = new MyThreadPool(threadNumber);
            var task1 = threadpool.AddTask(() => 25 + 26);
            var task2 = threadpool.AddTask(() => 0 + 0 + 0 + 0 + 0);
            var task3 = threadpool.AddTask(() => 82349374823 + 8173690442);

            Assert.AreEqual(51, task1.Result);
            Assert.AreEqual(0, task2.Result);
            Assert.AreEqual(90523065265, task3.Result);

            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
            Assert.IsTrue(task3.IsCompleted);
        }

        [TestMethod]
        public void DifferentThreadNumbersTest()
        {
            FewThreadsTest(5);
            FewThreadsTest(2);
        }

        private IMyTask<int> DivideByZeroSupplier()
        {
            var threadpool = new MyThreadPool(5);

            var zero = 0;
            var task1 = threadpool.AddTask(() => 5 / zero);
            return task1;
        }

        [TestMethod]
        public void IncorrectSupplierNoResultTest()
        {
            var task = DivideByZeroSupplier();
            Thread.Sleep(1000);
            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void IncorrectSupplierResultTest()
        {
            var task = DivideByZeroSupplier();
            _ = task.Result;
        }

        [TestMethod]
        [ExpectedException(typeof(ShutdownException))]
        public void ShutdownAddTaskTest()
        {
            var threadpool = new MyThreadPool(3);
            _ = threadpool.AddTask(() => 1 + 9);
            _ = threadpool.AddTask(() => "test" + " " + "string");
            _ = threadpool.AddTask(() => 82349374823 - 8173690442);

            threadpool.Shutdown();
            threadpool.AddTask(() => "new task");
        }

        [TestMethod]
        public void ShutdownGetResultTest()
        {
            var threadpool = new MyThreadPool(6);
            var task1 = threadpool.AddTask(() =>
            {
                Thread.Sleep(1000);
                return 1 + 9;
            });
            var task2 = threadpool.AddTask(() =>
            {
                Thread.Sleep(1000);
                return "test" + " " + "string";
            });
            var task3 = threadpool.AddTask(() =>
            {
                Thread.Sleep(1000);
                return 82349374823 - 8173690442;
            });

            threadpool.Shutdown();

            Assert.AreEqual(10, task1.Result);
            Assert.AreEqual("test string", task2.Result);
            Assert.AreEqual(74175684381, task3.Result);
        }

        [TestMethod]
        public void ContinueWithSimpleTest()
        {
            var threadpool = new MyThreadPool(3);
            var task = threadpool.AddTask(() => 1 + 9);
            var nextTask = task.ContinueWith((x) => x * 2);

            Assert.AreEqual(20, nextTask.Result);
        }

        [TestMethod]
        public void MultipleContinuesTest()
        {
            var threadpool = new MyThreadPool(2);
            var task1 = threadpool.AddTask(() => "task 1");
            var task2 = task1.ContinueWith((x) => x + " task 2");
            var task3 = task2.ContinueWith((x) => x + " task 3");
            var task4 = task3.ContinueWith((x) => x + " task 4");

            Assert.AreEqual("task 1 task 2 task 3 task 4", task4.Result);
        }

        [TestMethod]
        public void ManyTasksManyContinuesTest()
        {
            var threadpool = new MyThreadPool(8);
            var task1 = threadpool.AddTask(() =>
            {
                Thread.Sleep(1000);
                return "task 1";
            });

            var task2 = task1.ContinueWith((x) => x + " task 2");

            var anotherTask = threadpool.AddTask(() => 67 + 34);

            var task3 = task2.ContinueWith((x) =>
            {
                Thread.Sleep(1000);
                return x + " task 3";
            });

            var anotherTask1 = anotherTask.ContinueWith((x) => x - 1);

            var task4 = task3.ContinueWith((x) => x + " task 4");

            Assert.AreEqual("task 1 task 2 task 3 task 4", task4.Result);
            Assert.AreEqual(100, anotherTask1.Result);
        }

        [TestMethod]
        public void ContinueWithBeforeShutdownTest()
        {
            var threadpool = new MyThreadPool(5);
            var task1 = threadpool.AddTask(() =>
            {
                Thread.Sleep(1000);
                return "task 1";
            });

            var task2 = task1.ContinueWith((x) => x + " task 2");

            var task3 = task2.ContinueWith((x) =>
            {
                Thread.Sleep(1000);
                return x + " task 3";
            });

            var task4 = task3.ContinueWith((x) => x + " task 4");

            threadpool.Shutdown();

            Assert.AreEqual("task 1 task 2 task 3 task 4", task4.Result);
        }

        [TestMethod]
        [ExpectedException(typeof(ShutdownException))]
        public void ContinueWithAfterShutdownTest()
        {
            var threadpool = new MyThreadPool(5);
            var task1 = threadpool.AddTask(() =>
            {
                Thread.Sleep(1000);
                return "task 1";
            });

            var task2 = task1.ContinueWith((x) => x + " task 2");

            var task3 = task2.ContinueWith((x) =>
            {
                Thread.Sleep(1000);
                return x + " task 3";
            });

            threadpool.Shutdown();

            _ = task3.ContinueWith((x) => x + " task 4");

            Assert.AreEqual("task 1 task 2 task 3", task3.Result);
        }

        [TestMethod]
        public void AddTasksFromDifferentThreads()
        {
            var threadpool = new MyThreadPool(5);
            var threads = new Thread[10];
            var tasks = new IMyTask<int>[10];

            for (var i = 0; i < threads.Length; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    tasks[localI] = threadpool.AddTask(() =>
                    {
                        Thread.Sleep(100);
                        return localI * 2 - 1;
                    });
                });
            }

            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Start();
            }

            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Join();
            }

            for (int i = 0; i < tasks.Length; ++i)
            {
                Assert.AreEqual(i * 2 - 1, tasks[i].Result);
            }
        }

        [TestMethod]
        public void ThreadSafetyTest()
        {
            var threadpool = new MyThreadPool(5);
            var threads = new Thread[10];
            var tasks = new IMyTask<int>[6];
            var task1Continues = new IMyTask<int>[2];
            var task2Continue = new IMyTask<int>[1];

            for (var i = 0; i < 4; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    tasks[localI] = threadpool.AddTask(() =>
                    {
                        Thread.Sleep(100);
                        return localI * 2 - 1;
                    });
                });
            }

            for (var i = 4; i < 6; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    task1Continues[localI - 4] = tasks[1].ContinueWith(x => x + 5);
                });
            }

            threads[6] = new Thread(() =>
            {
                task2Continue[0] = tasks[2].ContinueWith(x => x * 3);
            });

            for (var i = 7; i < 9; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    tasks[localI - 3] = threadpool.AddTask(() =>
                    {
                        Thread.Sleep(100);
                        return localI + 7;
                    });
                });
            }

            threads[9] = new Thread(() =>
            {
                threadpool.Shutdown();
            });

            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Start();
            }

            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Join();
            }

            for (int i = 0; i < 4; ++i)
            {
                Assert.AreEqual(i * 2 - 1, tasks[i].Result);
            }

            for (int i = 4; i < 6; ++i)
            {
                Assert.AreEqual(i + 10, tasks[i].Result);
            }

            Assert.AreEqual(6, task1Continues[0].Result);
            Assert.AreEqual(6, task1Continues[1].Result);

            for (int i = 4; i < 6; ++i)
            {
                Assert.AreEqual(i + 10, tasks[i].Result);
            }

            Assert.AreEqual(9, task2Continue[0].Result);
        }
    }
}
