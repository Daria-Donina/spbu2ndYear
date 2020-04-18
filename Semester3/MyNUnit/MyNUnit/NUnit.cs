using System;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using MyNUnit.Attributes;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace MyNUnit
{
    public class NUnit
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

                Parallel.ForEach(testClasses, async (testClass) =>
                {
                    var methods = testClass.GetMethods();

                    await Task.Run(() => RunBeforeClassMethods(methods.Where(method => MethodSelector<BeforeClassAttribute>(method))));

                    await Task.Run(() => ExecuteTests(
                        methods.Where(method => MethodSelector<BeforeAttribute>(method)),
                        methods.Where(method => MethodSelector<TestAttribute>(method)),
                        methods.Where(method => MethodSelector<AfterClassAttribute>(method))));

                    await Task.Run(() => RunAfterClassMethods(methods.Where(method => MethodSelector<AfterClassAttribute>(method))));
                });
            });
        }

        private static bool MethodSelector<T>(MethodInfo method) where T : Attribute
        {
            var attributes = method.GetCustomAttributes<T>();

            if (attributes.Count() > 1)
            {
                //какое?
                throw new Exception();
            }
            else if (attributes.Count() == 0)
            {
                return false;
            }

            if (method.GetParameters().Length != 0 || method.ReturnType != typeof(void))
            {
                //какое?
                throw new Exception();
            }

            if (typeof(T) == typeof(BeforeClassAttribute) || typeof(T) == typeof(AfterClassAttribute))
            {
                if (!method.IsStatic)
                {
                    //какое?
                    throw new Exception();
                }
            }

            return true;
        }

        private static void RunBeforeClassMethods(IEnumerable<MethodInfo> methods)
        {
            Parallel.ForEach(methods, (method) =>
            {
                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception e)
                {
                    WriteMessage($"'{method.Name}' has failed with an exception {e.GetType()}: '{e.Message}'");
                }
            });
        }

        private static void RunAfterClassMethods(IEnumerable<MethodInfo> methods)
        {
            Parallel.ForEach(methods, (method) =>
            {
                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception e)
                {
                    WriteMessage($"'{method.Name}' has failed with an exception {e.GetType()}: '{e.Message}'");
                }
            });
        }

        private async static void ExecuteTests(
            IEnumerable<MethodInfo> beforeMethods,
            IEnumerable<MethodInfo> afterMethods,
            IEnumerable<MethodInfo> testMethods)
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(beforeMethods, (method) =>
                {
                    method.Invoke(method.DeclaringType, null);
                });
            });

            await Task.Run(() =>
            {
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
            });


            await Task.Run(() =>
            {
                Parallel.ForEach(afterMethods, (method) =>
                {
                    method.Invoke(method.DeclaringType, null);
                });
            });
        }

        private static void WriteMessage(string message) => Console.WriteLine(message);
    }
}