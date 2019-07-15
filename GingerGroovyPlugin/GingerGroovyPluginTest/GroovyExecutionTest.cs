using Microsoft.VisualStudio.TestTools.UnitTesting;
using GingerGroovyPluginConsole;
using Amdocs.Ginger.Plugin.Core;
using System.Threading;
using System.Collections.Generic;

namespace GingerGroovyPluginTest
{
    [TestClass]
    public class GroovyExecutionTest
    {
        [TestMethod]
        public void SimpleScriptTest()
        {
            //Arrange
            GroovyScriptExecuterService groovyScriptExecuterService = new GroovyScriptExecuterService();
            GingerAction GA = new GingerAction();

            List<GroovyPrameters> groovyPrameters = new List<GroovyPrameters>();
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 1", Value = "10" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 2", Value = "20" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 4", Value = "30" });
            //Act
            groovyScriptExecuterService.ExecuteGroovyScriptFile(GA, "", @"C:\\Work\Groovy\\BasicScript.groovy", groovyPrameters);

            //Assert
            string str = string.Empty;
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            foreach (IGingerActionOutputValue s in GA.Output.OutputValues)
            {
                str = s.Value.ToString();
            }
            Assert.AreEqual("30", str);
        }

        [TestMethod]
        public void ScriptTestGroovyFolder()
        {
            //Arrange
            GroovyScriptExecuterService groovyScriptExecuterService = new GroovyScriptExecuterService();
            GingerAction GA = new GingerAction();

            List<GroovyPrameters> groovyPrameters = new List<GroovyPrameters>();
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 1", Value = "10" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 2", Value = "20" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 4", Value = "30" });
            //Act
            groovyScriptExecuterService.ExecuteGroovyScriptFile(GA, @"C:\Work\Groovy\Groovy bin folder\groovy 2.5.6", @"C:\\Work\Groovy\\BasicScript.groovy", groovyPrameters);

            //Assert
            string str = string.Empty;
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            foreach (IGingerActionOutputValue s in GA.Output.OutputValues)
            {
                str = s.Value.ToString();
            }
            Assert.AreEqual("30", str);
        }

        [TestMethod]
        public void ScriptTestBinFolder()
        {
            //Arrange
            GroovyScriptExecuterService groovyScriptExecuterService = new GroovyScriptExecuterService();
            GingerAction GA = new GingerAction();

            List<GroovyPrameters> groovyPrameters = new List<GroovyPrameters>();
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 1", Value = "10" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 2", Value = "20" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 4", Value = "30" });
            //Act
            groovyScriptExecuterService.ExecuteGroovyScriptFile(GA, @"C:\Work\Groovy\Groovy bin folder\groovy 2.5.6\bin", @"C:\\Work\Groovy\\BasicScript.groovy", groovyPrameters);

            //Assert
            string str = string.Empty;
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");
            foreach (IGingerActionOutputValue s in GA.Output.OutputValues)
            {
                str = s.Value.ToString();
            }
            Assert.AreEqual("30", str);
        }
        
        [TestMethod]
        public void SimpleScriptContent()
        {
            //Arrange
            GroovyScriptExecuterService groovyScriptExecuterService = new GroovyScriptExecuterService();
            GingerAction GA = new GingerAction();
            List<GroovyPrameters> groovyPrameters = new List<GroovyPrameters>();
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 1", Value = "10" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 2", Value = "20" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 4", Value = "30" });

            //Act
            groovyScriptExecuterService.ExecuteGroovyScript(GA, @"C:\Work\Groovy\groovy-2.5.5\bin", @"'println '\''Hello, World!'\'", groovyPrameters);           
        }

        [TestMethod]
        public void RunGroovyScriptOnLinux()
        {
            //Arrange
            GroovyScriptExecuterService groovyScriptExecuterService = new GroovyScriptExecuterService();
            GingerAction GA = new GingerAction();
            List<GroovyPrameters> groovyPrameters = new List<GroovyPrameters>();
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 1", Value = "10" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 2", Value = "20" });
            groovyPrameters.Add(new GroovyPrameters() { Param = "Param 4", Value = "30" });

            //Act
            groovyScriptExecuterService.ExecuteGroovyScriptFile(GA, "", @"/home/ginger/ginger_tests/Groovy_Plugin_Test/BasicScript.groovy", groovyPrameters);

            //Assert              
            string str = string.Empty;
            Assert.AreEqual((GA.Output != null && GA.Output.OutputValues.Count > 0), true, "Execution Output values found validation");       
            foreach(IGingerActionOutputValue s in GA.Output.OutputValues)
            {
                str = s.Value.ToString();
            }
            Assert.AreEqual("30", str);
        }
    }
}
