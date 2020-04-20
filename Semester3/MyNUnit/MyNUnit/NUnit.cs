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

                    RunBeforeClassMethods(methods.Where(method => MethodSelector<BeforeClassAttribute>(method)));

                    ExecuteTests(
                        testClassInstance,
                        methods.Where(method => MethodSelector<BeforeAttribute>(method)),
                        methods.Where(method => MethodSelector<TestAttribute>(method)),
                        methods.Where(method => MethodSelector<AfterClassAttribute>(method)));

                    RunAfterClassMethods(methods.Where(method => MethodSelector<AfterClassAttribute>(method)));

                });
            });
        }

        private static bool MethodSelector<T>(MethodInfo method) where T : Attribute
        {
            var attributes = method.GetCustomAttributes<T>();

            if (attributes.Count() > 1)
            {
                //какое?
                //throw new Exception();
                return false;
            }
            else if (attributes.Count() == 0)
            {
                return false;
            }

            if (method.GetParameters().Length != 0 || method.ReturnType != typeof(void))
            {
                //какое?
                //throw new Exception();
                return false;
            }

            if (typeof(T) == typeof(BeforeClassAttribute) || typeof(T) == typeof(AfterClassAttribute))
            {
                if (!method.IsStatic)
                {
                    //какое?
                    // throw new Exception();
                    return false;
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

        private static void ExecuteTests(
            object testClassInstance,
            IEnumerable<MethodInfo> beforeMethods,
            IEnumerable<MethodInfo> testMethods,
            IEnumerable<MethodInfo> afterMethods)
        {
            Parallel.ForEach(beforeMethods, (method) =>
            {
                method.Invoke(testClassInstance, null);
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
                    test.Invoke(testClassInstance, null);

                    startTime.Stop();

                    WriteMessage($"Test {test.Name} passed");
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
            });


            Parallel.ForEach(afterMethods, (method) =>
            {
                method.Invoke(testClassInstance, null);
            });
        }

        private static void WriteMessage(string message) => Console.WriteLine(message);
    }
}