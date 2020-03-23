using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;

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

                    TestExecution(beforeMethods, afterMethods, testMethods);

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

        private static void TestExecution(List<MethodInfo> beforeMethods, List<MethodInfo> afterMethods, 
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
                    //и возможно нужно вернуть само сообщение
                    break;
                }

                if (!(attribute.Expected is null))
                {
                    //что-то сделать
                }

                try
                {
                    test.Invoke(test.DeclaringType, null);
                }
                catch (Exception exception)
                {
                    //проверить, совпадает ли тип исключения с ожидаемым, тем самым выяснив упал тест или прошел
                }
            }

            foreach (var method in afterMethods)
            {
                method.Invoke(method.DeclaringType, null);
            }
        }
    }
}
