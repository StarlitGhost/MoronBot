using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities;

using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;

namespace Utility
{
    public class PythonInterface : Function
    {
        private ScriptEngine engine;
        private ScriptScope scope;
        private ScriptSource source;

        public PythonInterface()
        {
            Help = "A C# function which passes messages to other functions written in python.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;

            ReloadScript();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            List<IRCResponse> responses = new List<IRCResponse>();

            if (Regex.IsMatch(message.Command, @"pyrestart"))
            {
                ReloadScript();
                responses.Add(new IRCResponse(ResponseType.Say, "PythonInterface reloaded!", message.ReplyTo));
            }
            else
            {
                try
                {
                    IList<object> pyResponses = (IList<object>)engine.Operations.Invoke(scope.GetVariable("ProcessMessage"), message);
                    if (pyResponses.Count > 0)
                        responses = pyResponses.Cast<IRCResponse>().ToList();
                }
                catch (System.Exception ex)
                {
                    Logger.Write("Python Error: " + ex.Message, Settings.Instance.ErrorFile);
                    DynamicStackFrame[] frames = PythonOps.GetDynamicStackFrames(ex);
                    foreach (DynamicStackFrame frame in frames)
                        Logger.Write("\t" +
                            frame.GetFileName() + " " +
                            frame.GetFileLineNumber() + " " +
                            frame.GetMethodName(), Settings.Instance.ErrorFile);
                }
            }

            return responses;
        }

        void ReloadScript()
        {
            engine = Python.CreateEngine();

            engine.Runtime.LoadAssembly(typeof(Function).Assembly);
            engine.Runtime.LoadAssembly(typeof(BotMessage).Assembly);
            engine.Runtime.LoadAssembly(typeof(IRCResponse).Assembly);
            engine.Runtime.LoadAssembly(typeof(Settings).Assembly);

            ICollection<string> paths = engine.GetSearchPaths();
            paths.Add(Settings.Instance.FunctionPath);
            string pathFunctionPython = Path.Combine(Settings.Instance.FunctionPath, "Python");
            paths.Add(pathFunctionPython);
            paths.Add(Path.Combine(pathFunctionPython, "libs"));
            engine.SetSearchPaths(paths);

            scope = engine.CreateScope();
            source = engine.CreateScriptSourceFromFile(Path.Combine(Settings.Instance.FunctionPath, "PythonInterface.py"));

            try
            {
                source.Execute(scope);
            }
            catch (System.Exception ex)
            {
                Logger.Write("Python Error: " + ex.Message, Settings.Instance.ErrorFile);
                DynamicStackFrame[] frames = PythonOps.GetDynamicStackFrames(ex);
                foreach (DynamicStackFrame frame in frames)
                    Logger.Write("\t" +
                        frame.GetFileName() + " " +
                        frame.GetFileLineNumber() + " " +
                        frame.GetMethodName(), Settings.Instance.ErrorFile);
            }
        }
    }
}
