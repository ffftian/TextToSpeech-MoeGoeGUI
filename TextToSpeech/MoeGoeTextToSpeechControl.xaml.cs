using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    [Obsolete("分离窗口撰写的情况下")]
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
        private string ModelPath { get{ return ModelPathBox.Text; }set { ModelPathBox.Text = value; } }
        private string ConfigPath { get { return ConfigPathBox.Text; } set { ConfigPathBox.Text = value; } }

        public MoeGoeOutPut moeGoeOutPut;

        public MoeGoeTextToSpeechControl()
        {
            InitializeComponent();
            OpenEXE.Click += OpenMoeGoe_Click;
            OpenModel.Click += OpenModel_Click;
            OpenConfig.Click += OpenConfig_Click;

            生成指定语音.Click += 生成指定语音_Click;
            生成所有语音.Click += 生成所有语音_Click;
            //
        }
        public void InitializeBindWindow()
        {
            moeGoeOutPut = new MoeGoeOutPut(MoeGoePath, ModelPath, ConfigPath);
        }


        private void 生成指定语音_Click(object sender, RoutedEventArgs e)
        {
            

            moeGoeOutPut.OutPutTTS(LoadModuleControl, LoadModuleControl.VoicePath);

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
