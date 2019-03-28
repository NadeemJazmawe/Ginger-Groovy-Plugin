using Amdocs.Ginger.Plugin.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace GingerGroovyPluginConsole
{
    public class GroovyExecution
    {
        IGingerAction mGingerAction = null;
        public IGingerAction GingerAction
        {
            get
            {
                return mGingerAction;
            }
            set
            {
                mGingerAction = value;
                if (mGingerAction != null)
                {
                    mGingerAction.AddExInfo("\n");
                }
            }
        }

        string mOutputs = string.Empty;

        public enum eExecutionMode { ScriptPath, FreeCommand }
        public eExecutionMode ExecutionMode;

        public List<GroovyPrameters> GroovyPrameters = new List<GroovyPrameters>();

        static string mCommandOutputErrorBuffer = string.Empty;
        static string mCommandOutputBuffer = string.Empty;

        string mGroovyExeFullPath = null;
        public string GroovyExeFullPath
        {
            get
            {
                return mGroovyExeFullPath;
            }

            set                
            {
                try
                {
                    mGroovyExeFullPath = value;                    
                    if (string.IsNullOrEmpty(mGroovyExeFullPath))
                    {
                        mGroovyExeFullPath = Environment.GetEnvironmentVariable("GROOVY_HOME");                        
                    }
                    else
                    {
                        Environment.SetEnvironmentVariable("GROOVY_HOME", mGroovyExeFullPath.Replace("bin",""));
                    }
                    if (!string.IsNullOrEmpty(mGroovyExeFullPath))
                    {
                        if(!mGroovyExeFullPath.Contains("bin"))
                        {
                            mGroovyExeFullPath = Path.Combine(mGroovyExeFullPath, "bin");
                        }

                        if (Path.GetFileName(mGroovyExeFullPath).ToLower().Contains("groovy") == false)
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                mGroovyExeFullPath = Path.Combine(mGroovyExeFullPath, "groovy.bat");
                            }
                            else//linux
                            {
                                mGroovyExeFullPath = Path.Combine(mGroovyExeFullPath, "groovy");
                            }
                        }

                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            mGroovyExeFullPath = Path.GetFullPath(mGroovyExeFullPath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GingerAction.AddError("Failed to get path");
                }
            }
        }

        string mJavaExeFullPath = null;
        public string JavaExeFullPath
        {
            get
            {
                return mJavaExeFullPath;
            }
            set
            {
                try
                {
                    mJavaExeFullPath = value;                  
                    if (string.IsNullOrEmpty(mJavaExeFullPath))
                    {
                        mJavaExeFullPath = Environment.GetEnvironmentVariable("JAVA_HOME");
                    }
                    if (!string.IsNullOrEmpty(mJavaExeFullPath))
                    {                
                        mJavaExeFullPath = Path.Combine(mJavaExeFullPath, "bin");                     
                        if (Path.GetFileName(mJavaExeFullPath).ToLower().Contains("java") == false)
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                mJavaExeFullPath = Path.Combine(mJavaExeFullPath, "java.exe");
                            }
                            else//linux
                            {
                                mJavaExeFullPath = Path.Combine(mJavaExeFullPath, "java");
                            }
                        }

                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            mJavaExeFullPath = Path.GetFullPath(mJavaExeFullPath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GingerAction.AddExInfo("Failed to init the java.exe file path, Error: " + ex.Message);
                }
            }
        }

        string mGroovyScriptPath = null;
        public string GroovyScriptPath
        {
            get
            {
                return mGroovyScriptPath;
            }
            set
            {
                mGroovyScriptPath = value;
                if (!string.IsNullOrEmpty(mGroovyScriptPath))
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        mGroovyScriptPath = Path.GetFullPath(mGroovyScriptPath);
                    }                   
                }
            }
        }

        public void Execute()
        {
            Console.Write("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Execution Started %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            try
            {
                switch(ExecutionMode)
                {
                    case eExecutionMode.ScriptPath:
                        CommandElements command = new CommandElements();                       
                        command = PrepareFreeCommand();
                        ExecuteCommand(command);
                        ParseCommandOutput();
                        break;
                }
            }
            catch(Exception ex)
            {
                GingerAction.AddError("Error while executing script : " + ex.ToString());
            }
        }

        public static bool OutputParamExist(GingerAction GA, string paramName, string paramValue = null)
        {
            IGingerActionOutputValue val = null;
            if (paramValue == null)
            {
                val = GA.Output.OutputValues.Where(x => x.Param == paramName).FirstOrDefault();
            }
            else
            {
                val = GA.Output.OutputValues.Where(x => x.Param == paramName && x.Value.ToString() == paramValue).FirstOrDefault();
            }

            if (val == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private CommandElements PrepareFreeCommand()
        {
            string Arguments = string.Empty;
            CommandElements command = new CommandElements();            
            command.ExecuterFilePath = GroovyExeFullPath;                   
            command.WorkingFolder = Path.GetDirectoryName(GroovyExeFullPath);
            Arguments += string.Format("\"{0}\"", GroovyScriptPath) ;
            if(GroovyPrameters!=null)
            {
                foreach (GroovyPrameters gp in GroovyPrameters)
                {
                    Arguments += " " + gp.Value;
                }
            }            
            command.Arguments = Arguments;
            return command;
        }
        
        public void SetContent(String content)
        {
            try
            {
                GroovyScriptPath = System.IO.Path.GetTempFileName().Replace(".tmp", ".groovy");
                StreamWriter sw = new StreamWriter(GroovyScriptPath);
                sw.Write(content);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create groovy file");
                throw e;
            }            
        }

        static protected void AddCommandOutput(string output)
        {
            mCommandOutputBuffer += output + "\n";
            Console.WriteLine(output);
        }
        static protected void AddCommandOutputError(string error)
        {
            mCommandOutputErrorBuffer += error + "\n";
            Console.WriteLine(error);
        }
        protected void Process_Exited(object sender, EventArgs e)
        {           
            Console.WriteLine("Command Execution Ended");
        }
        private void ParseCommandOutput()
        {
            try
            {
                //Error
                if (!string.IsNullOrEmpty(mCommandOutputErrorBuffer.Trim().Trim('\n')))
                {                                        
                    GingerAction.AddError(string.Format("Console Errors: \n{0}", mCommandOutputErrorBuffer));                    
                }

                //Output values
                Regex rg = new Regex(@"Microsoft.*\n.*All rights reserved.");
                string stringToProcess = rg.Replace(mCommandOutputBuffer, "");
                string[] values = stringToProcess.Split('\n');
                foreach (string dataRow in values)
                {
                    if (dataRow.Length > 0) // Ignore empty lines
                    {
                        string param;
                        string value;
                        int signIndex = dataRow.IndexOf('=');
                        if (signIndex > 0)
                        {
                            param = dataRow.Substring(0, signIndex);
                            //the rest is the value
                            value = dataRow.Substring(param.Length + 1);
                            GingerAction.AddOutput(param, value, "Console Output");
                        }
                    }
                }
            }
            catch (Exception ex)
            {                
                GingerAction.AddError(string.Format("Failed to parse all command console outputs, Error:'{0}'", ex.Message));
            }
        }
        
        public void ExecuteCommand(object commandVal)
        {
            try
            {
                CommandElements commandVals = (CommandElements)commandVal;
                Process process = new Process();            
                if (commandVals.WorkingFolder != null)
                {
                    process.StartInfo.WorkingDirectory = commandVals.WorkingFolder;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    process.StartInfo.FileName = commandVals.ExecuterFilePath;                                  
                    process.StartInfo.Arguments = commandVals.Arguments;
                }
                else//Linux
                {
                    var escapedExecuter = commandVals.ExecuterFilePath.Replace("\"", "\\\"");
                    var escapedArgs = commandVals.Arguments.Replace("\"", "\\\"");
                    process.StartInfo.WorkingDirectory = "";
                    process.StartInfo.FileName = "bash";
                    process.StartInfo.Arguments = $"-c \"{escapedExecuter} {escapedArgs}\"";                 
                }                
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;                
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                mCommandOutputBuffer = string.Empty;
                mCommandOutputErrorBuffer = string.Empty;
                process.OutputDataReceived += (proc, outLine) => { AddCommandOutput(outLine.Data); };
                process.ErrorDataReceived += (proc, outLine) => { AddCommandOutputError(outLine.Data); };
                process.Exited += Process_Exited;                
                Console.Write("--Staring process");
                process.Start();                
                Stopwatch stopwatch = Stopwatch.StartNew();                               
                process.BeginOutputReadLine();
                
                process.BeginErrorReadLine();

                int maxWaitingTime = 1000 * 60 * 60;//1 hour

                process.WaitForExit(maxWaitingTime);
                Console.Write("--Process done");
                stopwatch.Stop();

                if (stopwatch.ElapsedMilliseconds >= maxWaitingTime)
                {
                    GingerAction.AddError("Command processing timeout has reached!");
                }
            }
            catch (Exception ex)
            {
                GingerAction.AddError("Failed to execute the command, Error is: '{0}'" + ex.Message);
                Console.Write(ex.Message);
            }
            finally
            {
                GingerAction.AddExInfo("--Exiting execute command");
            }
        }
        private void Process_ErrorDataReceivedAsync(object sender, DataReceivedEventArgs e)
        {
            mOutputs += e.Data + System.Environment.NewLine;
        }

        private void Process_OutputDataReceivedAsync(object sender, DataReceivedEventArgs e)
        {
            mOutputs += e.Data + System.Environment.NewLine;
        }
    }
}
