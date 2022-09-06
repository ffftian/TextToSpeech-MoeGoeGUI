using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace TextToSpeech
{
    public delegate void OnOutputHandler(CommandLine sender, string e);
    public class CommandLine
    {
        public readonly Process process;

        public event OnOutputHandler OutputHandler;
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
                //process.OutputDataReceived += Command_OutputDataReceived;
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
         ~CommandLine()
        {
            process.Dispose();
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

    //public class MoeGoeOutPut
    //{
    //    public EventHandler<string> eventHandler;
    //    public CommandLine cmd = new CommandLine();
    //    private TextBox textBox;
    //    public MoeGoeOutPut(string EXEPATH, string MODELPATH, string CONFIGPATH, TextBox textBox)
    //    {
    //        cmd.OutputHandler += Cmd_OutputHandler;
    //        this.textBox = textBox;
    //        cmd.Write("\"" + EXEPATH + "\"");
    //        cmd.Write(MODELPATH);
    //        cmd.Write(CONFIGPATH);
    //    }

    //    private void Cmd_OutputHandler(CommandLine sender, string e)
    //    {
    //        Application.Current.Dispatcher.Invoke(new Action(()=> textBox.Text = e));
    //    }



    //    /// <summary>
    //    /// 保存tts，理论上这样就可以了
    //    /// </summary>
    //    /// <param name="Path"></param>
    //    public async void OutPutTTS(string text,string savePath)
    //    {
    //        cmd.Write("t");
    //        await Task.Delay(1000);
    //        cmd.Write(Regex.Replace(text, @"\r?\n", " "));
    //        await Task.Delay(1000);
    //        cmd.Write(savePath);
    //        await Task.Delay(1000);
    //        cmd.Write("y");
    //    }
    //    /// <summary>
    //    /// 保存音频转换
    //    /// </summary>
    //    public void OutPutVC()
    //    {


    //    }

    //    ~MoeGoeOutPut()
    //    {
    //        cmd.process.Close();
    //    }
    //}
}
