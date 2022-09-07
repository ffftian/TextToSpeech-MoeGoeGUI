using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeech
{
    public delegate void OnOutputHandler(CommandLine sender, DataReceivedEventArgs e);
    public class CommandLine
    {
        public readonly Process process;

        public event OnOutputHandler OutputHandler;
        public event OnOutputHandler ErrorHandler;
        public CommandLine()
        {
            try
            {
                process = new Process()
                {
                    StartInfo = new ProcessStartInfo("cmd.exe")
                    {
                        Arguments = "/k",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.ErrorDataReceived += Command_ErrorDataReceived;
                process.OutputDataReceived += Command_OutputDataReceived;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }
        ~CommandLine()
        {
            process.Dispose();
        }

        void Command_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OutputHandler?.Invoke(this, e);
        }

        void Command_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            ErrorHandler?.Invoke(this, e);
        }

        public void Write(string data)
        {
            try
            {
                process.StandardInput.WriteLine(data);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }
    }
}
