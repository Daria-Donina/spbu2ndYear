using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyNUnit
{
    class NUnit
    {
        public static void RunTests(string path)
        {
            var assemblyPaths = Directory.GetFiles(path, "*.dll");

            var assemblies = new List<Assembly>();
            foreach (var assemblyPath in assemblyPaths)
            {
                assemblies.Add(Assembly.LoadFrom(assemblyPath));
            }

            foreach(var assembly in assemblies)
            {
                var testClasses = assembly.GetTypes();

                foreach (var testClass in testClasses)
                {
                    var beforeClassMethods = new List<MethodInfo>();
                    var afterClassMethods = new List<MethodInfo>();
                    var beforeMethods = new List<MethodInfo>();
                    var afterMethods = new List<MethodInfo>();
                    var testMethods = new List<MethodInfo>();

                    foreach (var method in testClass.GetMethods())
                    {
                        foreach (var attribute in Attribute.GetCustomAttributes(method))
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
                        }
                    }

                    BeforeClassAndAfterClassMethodsExecution(beforeClassMethods);

                    ExecuteTests(beforeMethods, afterMethods, testMethods);

                    BeforeClassAndAfterClassMethodsExecution(afterClassMethods);
                }
            }
        }

        private static void BeforeClassAndAfterClassMethodsExecution(List<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                if (!method.IsStatic)
                {
                    throw new InvalidOperationException("Method with BeforeClass or AfterClass attribute must be static");
                }

                method.Invoke(null, null);
            }
        }

        private static void ExecuteTests(List<MethodInfo> beforeMethods, List<MethodInfo> afterMethods, 
            List<MethodInfo> testMethods)
        {
            foreach (var method in beforeMethods)
            {
                method.Invoke(method.DeclaringType, null);
            }

            foreach (var test in testMethods)
            {
                var attribute = test.GetCustomAttribute<TestAttribute>(false);
                
                if (!(attribute.Ignore is null))
                {
                    WriteMessage($"Test {test.Name} ignored\n {attribute.Ignore}");

                    break;
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
            }

            foreach (var method in afterMethods)
            {
                method.Invoke(method.DeclaringType, null);
            }
        }

        private static void WriteMessage(string message) => Console.WriteLine(message);
    }
}
