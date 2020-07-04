using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TextToSpeech
{
    /// <summary>
    /// LoadModuleControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoadModuleControl : UserControl
    {
        public List<TextData> textDataList;
        public Narrator narratorcobj = new Narrator();

        public List<VoiceInfo> voices = new List<VoiceInfo>();


        public TextData []QQArray;
        //输出格式直接按照ID输出，是谁就读对应的ID。

        private string VoicePathData;
        private string VoiceName;

        public string[] VoiceNameArry;//尼玛的，这条数组用于读取名称，因为我不懂TM如何把object转成正常的string，用 VoiceName = box.SelectedValue.tostring();转出来会多些东西

        public LoadModuleControl()
        {
            InitializeComponent();


            BrowseButton.Click += BrowseButtonClickCallback;
            BrowseButton2.Click += BrowseButton2ClickCallback;

            CreateVoice.Click += VoiceCreateTo;
            PlayerVoice.Click += VoiceTestPlay;

            VoiceBox.MouseDown += BoxClick;
            PlayerBox.MouseDown += BoxClick;



            VoiceBox.SelectionChanged += BoxClick;
            


            PlayerBox.SelectionChanged += BoxClick;

            InitVoiceEngine();
            // SelectBox.SelectionChanged += SeletInitButton;//点击下拉回调

            // ComboBox.SelectionChanged

        }


        //private void SeletInitButton(object sender, RoutedEventArgs e)
        //{
        //    SetSelcetBox();
        //}


        private void BoxClick(object sender, RoutedEventArgs e)
        {
           var box=  sender as ComboBox;

           if( box.Name == VoiceBox.Name)
            {
                VoiceBox.SelectedItem = box.SelectedValue;



                VoiceName = VoiceNameArry[box.SelectedIndex];
               // StreamReader rems = new StreamReader(box.SelectedValue);
                // byte[] g = box.SelectedValue;

                // VoiceName = box.SelectedValue;




            }

           if (box.Name == PlayerBox.Name)
            {
                PlayerBox.SelectedItem = PlayerBox.SelectedValue;


            }

            String voice = VoiceBox.SelectedItem?.ToString();
            String player = PlayerBox.SelectedItem?.ToString();
            if(voice?.Length > 0)
            {
                PlayerVoice.IsEnabled = true;
            }


            if (voice?.Length>0 && player?.Length>0&& VoicePathData?.Length>0)
            {
                CreateVoice.IsEnabled = true;
            }

        }

         

        private void BrowseButtonClickCallback(object sender, RoutedEventArgs e)
        {

            #region 让玩家选择一个txt并填入TxtData中
            System.Windows.Forms.OpenFileDialog data = new System.Windows.Forms.OpenFileDialog();

            data.Filter = "(*.txt)|*.txt|All files (*.*)|*.*";

            //   data.ShowDialog();
            if (data.ShowDialog() == System.Windows.Forms.DialogResult.OK)//用于指示让文件夹展开对文件选中才允许运行。
            {
                StreamReader streamReader = new StreamReader(data.OpenFile(), Encoding.UTF8);
                string tempData = streamReader.ReadToEnd();
                TextBox.Text = data.FileName;//取得名称并输入到界面上。
                textDataList = TextTool.QQLogSplit(tempData);

                #endregion

                PlayerBox.Items.Clear();
                //QQArray =  textDataList.Select(a => a.QQ).Distinct().ToArray();
                QQArray = textDataList.DistinctBy(a => a.QQ).ToArray();
                foreach (var QQData in QQArray)
                {
                    ComboBoxItem box = new ComboBoxItem();
                    box.Content = $"{QQData.RoleName} ({ QQData.QQ})";
                    PlayerBox.Items.Add(box);
                }
            }
            data.Dispose();

           // 是否满足生成条件();
        }
        private void BrowseButton2ClickCallback(object sender, RoutedEventArgs e)
        {
            #region 让玩家选择保存声音的路径
            System.Windows.Forms.FolderBrowserDialog data = new System.Windows.Forms.FolderBrowserDialog();



            if (data.ShowDialog() == System.Windows.Forms.DialogResult.OK)//用于指示让文件夹展开对文件筐。
            {
                VoicePathData = VoicePath.Text = data.SelectedPath;
            }
            data.Dispose();
            #endregion
        }

        private void VoiceCreateTo(object sender, RoutedEventArgs e)
        {
            //创建语音名称，为ID
          //  narratorcobj.c


            //选择对应语音机
            //narratorcobj.SelectVoice(VoiceName);
            //narratorcobj.synth.SelectVoice("Microsoft Lili");
            //narratorcobj.synth.s;
            //    var G =   narratorcobj.synth.GetInstalledVoices()[3];



        }


        private void VoiceTestPlay(object sender, RoutedEventArgs e)
        {
            //narratorcobj.SelectVoice(VoiceName);
            narratorcobj.synth.SelectVoice(VoiceName);

            //var G = voices[1];

            // narratorcobj.synth.SelectVoiceByHints(G.Gender, G.Age,0,G.Culture);

            // narratorcobj.Narrate("12345");
            narratorcobj.ExportToWave(VoicePathData, "12345");


        }


        LinkedList<string> vs = new LinkedList<string>();


        public void InitVoiceEngine()
        {
            VoiceBox.Items.Clear();
            VoiceNameArry = new string[narratorcobj.synth.GetInstalledVoices().Count];


            for(int a=0;a< narratorcobj.synth.GetInstalledVoices().Count; a++)
            {
                VoiceNameArry[a] = narratorcobj.synth.GetInstalledVoices()[a].VoiceInfo.Name;
            }


            foreach (var voice in narratorcobj.synth.GetInstalledVoices())
            {
                voices.Add(voice.VoiceInfo);

                ComboBoxItem box = new ComboBoxItem();
                // box.Content = voice.VoiceInfo.Name.Replace(" Desktop","");
                box.Content = voice.VoiceInfo.Name;


               // desktop

                VoiceBox.Items.Add(box);
            }
        }




    }
}
