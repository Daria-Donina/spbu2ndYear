using System;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using MyNUnit.Attributes;
using System.Linq;
using System.Collections.Generic;

namespace MyNUnit
{
    public static class NUnit
    {
        private static readonly object locker = new object();

        /// <summary>
        /// Runs tests from all assemblies from a given path.
        /// </summary>
        /// <param name="path"> A path containing assemblies with tests.</param>
        /// <returns> Result of the tests.</returns>
        public static TestInfo[] RunTests(string path)
        {
            var assemblyPaths = Directory.GetFiles(path, "*.dll");

            var assemblies = new BlockingCollection<Assembly>();
            foreach (var assemblyPath in assemblyPaths)
            {
                assemblies.Add(Assembly.LoadFrom(assemblyPath));
            }

            var testInfos = new List<TestInfo>();

            Parallel.ForEach(assemblies, assembly =>
            {
                var testClasses = assembly.GetTypes();

                Parallel.ForEach(testClasses, testClass =>
                {
                    var testClassInstance = Activator.CreateInstance(testClass);

                    var methods = testClass.GetMethods();

                    Parallel.ForEach(methods.Where(method => MethodSelector<BeforeClassAttribute>(method)),
                        method => method.Invoke(null, null));

                    var testInfosForClass = ExecuteTests(
                        testClassInstance,
                        methods.Where(method => MethodSelector<BeforeAttribute>(method)),
                        methods.Where(method => MethodSelector<TestAttribute>(method)),
                        methods.Where(method => MethodSelector<AfterAttribute>(method)));

                    Parallel.ForEach(methods.Where(method => MethodSelector<AfterClassAttribute>(method)),
                        method => method.Invoke(null, null));

                    lock (locker)
                    {
                        testInfos = Queryable.Concat(testInfos.AsQueryable(), testInfosForClass).ToList();
                    }
                });
            });

            return testInfos.ToArray();
        }

        private static bool MethodSelector<T>(MethodInfo method) where T : Attribute
        {
            var attributes = method.GetCustomAttributes<T>();

            if (attributes.Count() > 1)
            {
                throw new InvalidOperationException($"Method {method.Name} must only have one attribute");
            }
            else if (attributes.Count() == 0)
            {
                return false;
            }

            if (method.GetParameters().Length != 0 || method.ReturnType != typeof(void))
            {
                throw new InvalidOperationException($"Method {method.Name} return type must be void and it must not have parameters");
            }

            if (typeof(T) == typeof(BeforeClassAttribute) || typeof(T) == typeof(AfterClassAttribute))
            {
                if (!method.IsStatic)
                {
                    throw new InvalidOperationException($"Method {method.Name} must be static");
                }
            }

            return true;
        }

        private static TestInfo[] ExecuteTests(
            object testClassInstance,
            IEnumerable<MethodInfo> beforeMethods,
            IEnumerable<MethodInfo> testMethods,
            IEnumerable<MethodInfo> afterMethods)
        {
            var testInfos = new BlockingCollection<TestInfo>();

            Parallel.ForEach(testMethods, test =>
            {
                var testInfo = new TestInfo(test.Name);

                var attribute = test.GetCustomAttribute<TestAttribute>();

                if (attribute.Ignore != null)
                {
                    testInfo.IsIgnored = true;
                    testInfo.IgnoreReason = attribute.Ignore;
                }
                else
                {
                    Parallel.ForEach(beforeMethods, method => method.Invoke(testClassInstance, null));

                    var time = Stopwatch.StartNew();

                    try
                    {
                        test.Invoke(testClassInstance, null);

                        time.Stop();
                        testInfo.ExecutionTime = time.Elapsed;

                        if (attribute.Expected is null)
                        {
                            testInfo.IsPassed = true;
                        }

                    }
                    catch (Exception exception)
                    {
                        time.Stop();
                        testInfo.ExecutionTime = time.Elapsed;

                        if (attribute.Expected == exception.InnerException.GetType())
                        {
                            testInfo.IsPassed = true;
                        }
                    }

                    Parallel.ForEach(afterMethods, method => method.Invoke(testClassInstance, null));
                }

                testInfos.Add(testInfo);
            });

            return testInfos.ToArray();
        }

        /// <summary>
        /// Prints results of the tests.
        /// </summary>
        /// <param name="testInfos">Tests to print result for.</param>
        public static void PrintResults(TestInfo[] testInfos)
        {
            foreach (var test in testInfos)
            {
                if (test.IsIgnored)
                {
                    Console.WriteLine($"{test.Name}() is ignored\nReason: {test.IgnoreReason}\n\n");
                    continue;
                }

                if (test.IsPassed)
                {
                    Console.WriteLine($"{test.Name}() is passed");
                }
                else
                {
                    Console.WriteLine($"{test.Name}() is failed");
                }

                Console.WriteLine($"Execution time: {test.ExecutionTime}\n\n");
            }
        }
    }
}