using System;
using System.Collections.Generic;
using System.Text;
using Amdocs.Ginger.Plugin.Core;

namespace GingerGroovyPluginConsole
{
    [GingerService("GroovyScriptExecuter", "Execute Groovy Script via Ginger")]
    public class GroovyScriptExecuterService
    {
        /// <summary>
        /// Execute Groovy Script
        /// </summary>
        /// <param name="GA"></param>
        /// <param name="GroovyHomePath"></param>
        /// <param name="GroovyScriptPath"></param>
        [GingerAction("ExecuteGroovyScriptFile", "Execute Groovy Script File")]
        public void ExecuteGroovyScriptFile(IGingerAction GA, string OverwriteGroovyHomePath, string GroovyScriptPath, List<GroovyPrameters> GroovyPrameters)
        {
            GroovyExecution groovyExecution = new GroovyExecution();
            groovyExecution.ExecutionMode = GroovyExecution.eExecutionMode.ScriptPath;
            groovyExecution.GingerAction = GA;            
            groovyExecution.GroovyExeFullPath = OverwriteGroovyHomePath;
            groovyExecution.GroovyScriptPath = GroovyScriptPath;
            groovyExecution.GroovyPrameters  = GroovyPrameters;
            groovyExecution.Execute();
        }

        [GingerAction("ExecuteGroovyScript", "Execute Groovy Script")]
        public void ExecuteGroovyScript(IGingerAction GA, string OverwriteGroovyHomePath, string GroovyScriptContent, List<GroovyPrameters> GroovyPrameters)
        {
            GroovyExecution groovyExecution = new GroovyExecution();
            groovyExecution.ExecutionMode = GroovyExecution.eExecutionMode.ScriptPath;
            groovyExecution.GingerAction = GA;            
            groovyExecution.GroovyExeFullPath = OverwriteGroovyHomePath;
            groovyExecution.SetContent(GroovyScriptContent);
            groovyExecution.GroovyPrameters = GroovyPrameters;
            groovyExecution.Execute();
        }
    }
}
