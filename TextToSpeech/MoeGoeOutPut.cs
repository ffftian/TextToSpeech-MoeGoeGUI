using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextToSpeech
{
    public class CommandLine
    {
        public readonly Process process;

        public delegate void onOutputHandler(CommandLine sender, string e);
        public event onOutputHandler OutputHandler;
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
                process.OutputDataReceived += Command_OutputDataReceived;
                process.ErrorDataReceived += Command_ErrorDataReceived;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }

        void Command_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnOutput(e.Data);
        }

        void Command_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnOutput(e.Data);
        }

        private void OnOutput(string data)
        {
            OutputHandler?.Invoke(this, data);
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

    public class MoeGoeOutPut
    {
        public CommandLine cmd = new CommandLine();

        public MoeGoeOutPut(string EXEPATH, string MODELPATH, string CONFIGPATH)
        {
            cmd.OutputHandler += Cmd_OutputHandler;
            cmd.Write("\"" + EXEPATH + "\"");
            cmd.Write(MODELPATH);
            cmd.Write(CONFIGPATH);
        }


        /// <summary>
        /// 保存tts，理论上这样就可以了
        /// </summary>
        /// <param name="Path"></param>
        public void OutPutTTS(string text,string SAVEPATH)
        {
            cmd.Write("t");
            cmd.Write(Regex.Replace(text, @"\r?\n", " "));
            cmd.Write(SAVEPATH);
            cmd.Write("y");
        }
        /// <summary>
        /// 保存音频转换
        /// </summary>
        public void OutPutVC()
        {


        }

        private void Cmd_OutputHandler(CommandLine sender, string e)
        {

        }

        ~MoeGoeOutPut()
        {
            cmd.process.Close();
        }
    }
}
