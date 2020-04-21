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
                    var testClassInstance = Activator.CreateInstance(testClass);

                    var methods = testClass.GetMethods();

                    Parallel.ForEach(methods.Where(method => MethodSelector<BeforeClassAttribute>(method)),
                        method => method.Invoke(null, null));

                    ExecuteTests(
                        testClassInstance,
                        methods.Where(method => MethodSelector<BeforeAttribute>(method)),
                        methods.Where(method => MethodSelector<TestAttribute>(method)),
                        methods.Where(method => MethodSelector<AfterAttribute>(method)));

                    Parallel.ForEach(methods.Where(method => MethodSelector<AfterClassAttribute>(method)),
                        method => method.Invoke(null, null));

                });
            });
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

        private static void ExecuteTests(
            object testClassInstance,
            IEnumerable<MethodInfo> beforeMethods,
            IEnumerable<MethodInfo> testMethods,
            IEnumerable<MethodInfo> afterMethods)
        {
            Parallel.ForEach(testMethods, (test, state) =>
            {
                Parallel.ForEach(beforeMethods, method => method.Invoke(testClassInstance, null));

                var attribute = test.GetCustomAttribute<TestAttribute>();

                if (attribute.Ignore != null)
                {
                    WriteMessage($"Test {test.Name} ignored\n {attribute.Ignore}");

                    state.Break();
                }

                var startTime = Stopwatch.StartNew();

                try
                {
                    test.Invoke(testClassInstance, null);

                    startTime.Stop();

                    if (attribute.Expected != null)
                    {
                        WriteMessage($"Test {test.Name} did not throw expected exception");
                    }
                    else
                    {
                        WriteMessage($"Test {test.Name} passed");
                    }
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
                        WriteMessage($"Test {test.Name} failed with an exception {exception.GetType()}: {exception.Message}");
                    }
                }
                finally
                {
                    WriteMessage($"Execution time: {startTime.Elapsed}\n \n");
                }

                Parallel.ForEach(afterMethods, method => method.Invoke(testClassInstance, null));
            });
        }

        private static void WriteMessage(string message) => Console.WriteLine(message);
    }
}