using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextToSpeech
{
    /// <summary>
    /// MoeGoeTextToSpeechControl.xaml 的交互逻辑
    /// </summary>
    public partial class MoeGoeTextToSpeechControl : UserControl
    {
        public LoadModuleControl LoadModuleControl
        {
            get
            {
                return MainWindow.LoadModuleControl;
            }
        }

        public MainWindow MainWindow { get; set; }
        private string MoeGoePath { get { return EXEPathBox.Text; } set { EXEPathBox.Text = value; } }
        private string ModelPath { get { return ModelPathBox.Text; } set { ModelPathBox.Text = value; } }
        private string ConfigPath { get { return ConfigPathBox.Text; } set { ConfigPathBox.Text = value; } }

        public CommandLine cmd;
        //public MoeGoeOutPut moeGoeOutPut;
        const string MoeGoe配置文件 = "\\MoeGoe配置文件.txt";
        public MoeGoeTextToSpeechControl()
        {
            InitializeComponent();
            InitConfig();
            OpenEXE.Click += OpenMoeGoe_Click;
            OpenModel.Click += OpenModel_Click;
            OpenConfig.Click += OpenConfig_Click;

            生成指定语音.Click += 生成指定语音_Click;
            生成指定语音.IsEnabled = false;
            生成所有语音.Click += 生成所有语音_Click;
            生成所有语音.IsEnabled = false;

            启用语音生成控制台.Click += 启用语音生成控制台_Click;
        }
        /// <summary>
        /// 主要的语音合成启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 启用语音生成控制台_Click(object sender, RoutedEventArgs e)
        {
            cmd = new CommandLine();
            cmd.OutputHandler += Cmd_OutputHandler;
            cmd.Write("\"" + MoeGoePath + "\"");
            cmd.Write(ModelPath);
            cmd.Write(ConfigPath);
            //moeGoeOutPut = new MoeGoeOutPut(MoeGoePath, ModelPath, ConfigPath,Debug输出);
            生成指定语音.IsEnabled = true;
            生成所有语音.IsEnabled = true;
            AnalyzeRole();
        }
        private void AnalyzeRole()
        {
            string json = File.ReadAllText(ConfigPath);
            Match match = Regex.Match(json,
               "\"speakers\"\\s*:\\s*\\[((?:\\s*\"(?:(?:\\\\.)|[^\\\\\"])*\"\\s*,?\\s*)*)\\]");
            if (match.Success)
            {
                MatchCollection matches = Regex.Matches(match.Groups[1].Value,
                    "\"((?:(?:\\\\.)|[^\\\\\"])*)\"");
                if (matches.Count > 0)
                {
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string speaker = Regex.Unescape(matches[i].Groups[1].Value);
                        speakerBox.Items.Add(speaker);
                    }
                    return;
                }
            }
            MessageBox.Show("读取失败！", "",
                MessageBoxButton.OK, MessageBoxImage.Error);

        }


        private void Cmd_OutputHandler(CommandLine sender, string e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => Debug输出.Text = e));
        }

        public void InitializeBindWindow()
        {
            LoadModuleControl.SaveButton.Click += SaveButton_Click;
        }

        public void InitConfig()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + MoeGoe配置文件))
            {
                string[] Setting = File.ReadAllLines((System.Windows.Forms.Application.StartupPath + MoeGoe配置文件));
                MoeGoePath = Setting[0];
                ModelPath = Setting[1];
                ConfigPath = Setting[2];
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string Conetents = $"{MoeGoePath}\n{ModelPath}\n{ConfigPath}";
            File.WriteAllText(System.Windows.Forms.Application.StartupPath + MoeGoe配置文件, Conetents, Encoding.UTF8);
        }

        private void 生成指定语音_Click(object sender, RoutedEventArgs e)
        {
            string savePath = System.IO.Path.Combine(LoadModuleControl.VoicePath, LoadModuleControl.GetRoleDialogueID + ".wav");
            TTS("[ZH]" + LoadModuleControl.GetRoleDialogue + "[ZH]");
            cmd.Write(savePath);
            cmd.Write("y");//要有输出完成回调

        }

        private void TTS(string text)
        {
            cmd.Write("t");
            cmd.Write(Regex.Replace(text, @"\r?\n", " "));
            //cmd.Write("0");//说话人
            cmd.Write(speakerBox.SelectedIndex.ToString());//说话人
        }


        private void 生成所有语音_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenMoeGoe_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "可执行文件|*.exe"
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MoeGoePath = openFileDialog.FileName;
            }
            openFileDialog.Dispose();

        }

        private void OpenModel_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "模型文件|*.pth|所有文件|*.*"
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ModelPath = ofd.FileName;
            }
            ofd.Dispose();
        }

        private void OpenConfig_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "配置文件|*.json"
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ConfigPath = ofd.FileName;
            }
            ofd.Dispose();
        }


    }
}
