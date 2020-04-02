using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace MyNUnit
{
    class NUnit
    {
        public static void RunTests(string path)
        {
            var assemblyPaths = Directory.GetFiles(path, "*.dll");

            var assemblies = new BlockingCollection<Assembly>();
            foreach (var assemblyPath in assemblyPaths)
            {
                assemblies.Add(Assembly.LoadFrom(assemblyPath));
            }

            Parallel.ForEach(assemblies, (assembly) =>
            {
                var testClasses = assembly.GetTypes();

                Parallel.ForEach(testClasses, (testClass) =>
                {
                    var beforeClassMethods = new BlockingCollection<MethodInfo>();
                    var afterClassMethods = new BlockingCollection<MethodInfo>();
                    var beforeMethods = new BlockingCollection<MethodInfo>();
                    var afterMethods = new BlockingCollection<MethodInfo>();
                    var testMethods = new BlockingCollection<MethodInfo>();

                    #region initialization of collections
                    Parallel.ForEach(testClass.GetMethods(), (method) =>
                    {
                        Parallel.ForEach(Attribute.GetCustomAttributes(method), (attribute) =>
                        {
                            if (attribute.GetType() == typeof(BeforeClassAttribute))
                            {
                                beforeClassMethods.Add(method);
                            }

                            if (attribute.GetType() == typeof(AfterClassAttribute))
                            {
                                afterClassMethods.Add(method);
                            }

                            if (attribute.GetType() == typeof(BeforeAttribute))
                            {
                                beforeMethods.Add(method);
                            }

                            if (attribute.GetType() == typeof(AfterAttribute))
                            {
                                afterMethods.Add(method);
                            }

                            if (attribute.GetType() == typeof(TestAttribute))
                            {
                                testMethods.Add(method);
                            }
                        });
                    });
                    #endregion

                    RunBeforeClassMethods(beforeClassMethods);

                    ExecuteTests(beforeMethods, afterMethods, testMethods);

                    RunAfterClassMethods(afterClassMethods);
                });
            });
        }

        private static void RunBeforeClassMethods(BlockingCollection<MethodInfo> methods) 
            => RunBeforeOrAfterClassMethods(methods);

        private static void RunAfterClassMethods(BlockingCollection<MethodInfo> methods) 
            => RunBeforeOrAfterClassMethods(methods);

        private static void RunBeforeOrAfterClassMethods(BlockingCollection<MethodInfo> methods)
        {
            Parallel.ForEach(methods, (method) =>
            {
                if (!method.IsStatic)
                {
                    throw new InvalidOperationException("Method with BeforeClass or AfterClass attribute must be static");
                }

                method.Invoke(null, null);
            });
        }

        private static void ExecuteTests(
            BlockingCollection<MethodInfo> beforeMethods, 
            BlockingCollection<MethodInfo> afterMethods,
            BlockingCollection<MethodInfo> testMethods)
        {
            Parallel.ForEach(beforeMethods, (method) =>
            {
                method.Invoke(method.DeclaringType, null);
            });

            Parallel.ForEach(testMethods, (test, state) =>
            {
                var attribute = test.GetCustomAttribute<TestAttribute>();

                if (!(attribute.Ignore is null))
                {
                    WriteMessage($"Test {test.Name} ignored\n {attribute.Ignore}");

                    state.Break();
                }

                var startTime = Stopwatch.StartNew();

                try
                {
                    var result = test.Invoke(test.DeclaringType, null);

                    startTime.Stop();

                    WriteMessage($"Test {test.Name} passed\n Result: {result}");
                }
                catch (Exception exception)
                {
                    startTime.Stop();

                    if (attribute.Expected == exception.GetType())
                    {
                        WriteMessage($"Test {test.Name} passed");
                    }
                    else
                    {
                        WriteMessage($"Test {test.Name} failed\n Expected {attribute.Expected}," +
                            $" but received {exception.GetType()}");
                    }
                }
                finally
                {
                    WriteMessage($"Execution time: {startTime.Elapsed}\n \n");
                }
            });

            Parallel.ForEach(afterMethods, (method) =>
            {
                method.Invoke(method.DeclaringType, null);
            });
        }

        private static void WriteMessage(string message) => Console.WriteLine(message);
    }
}
