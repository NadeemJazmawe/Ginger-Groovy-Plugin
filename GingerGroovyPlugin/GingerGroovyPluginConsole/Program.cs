using System;
using Amdocs.Ginger.Plugin.Core;

namespace GingerGroovyPluginConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Ginger Groovy Plugin");

            using (GingerNodeStarter gingerNodeStarter = new GingerNodeStarter())
            {
                if (args.Length > 0)
                {
                    gingerNodeStarter.StartFromConfigFile(args[0]);
                }
                else
                {
                    gingerNodeStarter.StartNode("Groovy Script Execution Service", new GroovyScriptExecuterService());                    
                }
                gingerNodeStarter.Listen();
            }
        }
    }
}
