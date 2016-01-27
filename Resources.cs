using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace modsync
{
    static class Resources
    {
        // load dll included as embedded resource
        public static void RegisterEmbeddedResources()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    String resourceName = assembly.GetName().Name + "." + new AssemblyName(args.Name).Name + ".dll";
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            Byte[] assemblyData = new Byte[stream.Length];
                            stream.Read(assemblyData, 0, assemblyData.Length);
                            return Assembly.Load(assemblyData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Strings.Get("DllError") + ex.Message);
                    Console.ReadKey();
                }
                return null;
            };
        }

        // extract resource
        public static void ExtractFile(string path, string name)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                String resourceName = assembly.GetName().Name + "." + name;
                using (Stream input = assembly.GetManifestResourceStream(resourceName))
                {
                    Byte[] assemblyData = new Byte[input.Length];
                    input.Read(assemblyData, 0, assemblyData.Length);
                    using (Stream output = File.Create(path + "\\" + name))
                    {
                        output.Write(assemblyData, 0, assemblyData.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("DllError") + ex.Message);
                Console.ReadKey();
            }
        }
    }
}
