using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace TextToSpeech
{
    /// <summary>
    /// LoadModuleControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoadModuleControl : UserControl
    {
        public List<TextData> textDataList;//存储所有文字的数据
        public Narrator narratorcobj = new Narrator();

        public List<VoiceInfo> voices = new List<VoiceInfo>();


        public TextData []QQArray;//存储所有的出场人
        //输出格式直接按照ID输出，是谁就读对应的ID。

        private string VoicePath;
        private string VoiceName;
        private string PlayerQQ;

        public string[] VoiceNameArry;//尼玛的，这条数组用于读取名称，因为我不懂TM如何把object转成正常的string，用 VoiceName = box.SelectedValue.tostring();转出来会多些东西

        public LoadModuleControl()
        {
            InitializeComponent();


            BrowseButton.Click += BrowseButtonClickCallback;
            BrowseButton2.Click += BrowseButton2ClickCallback;

            测试播放语音.Click += VoiceReciteTo;
            生成对应语音.Click += PlayerVoiceButtonClick;

            BrowseButton.Click += BoxClick;
            BrowseButton2.Click += BoxClick;
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
            if (sender is ComboBox)
            {
                ComboBox box = sender as ComboBox;
                if (box.Name == VoiceBox.Name)
                {
                    VoiceBox.SelectedItem = box.SelectedValue;//这Value是TM的反射。

                    // VoiceName = Convert.ToString(VoiceBox.SelectedItem);

                    VoiceName = VoiceNameArry[box.SelectedIndex];
                    // StreamReader rems = new StreamReader(box.SelectedValue);
                    // byte[] g = box.SelectedValue;

                    // VoiceName = box.SelectedValue;
                }
                if (box.Name == PlayerBox.Name)
                {
                    PlayerBox.SelectedItem = PlayerBox.SelectedValue;
                    PlayerQQ = QQArray[PlayerBox.SelectedIndex].QQ;
                    // PlayerQQ = Convert.ToInt32(PlayerBox.SelectedValue);


                    //   PlayerID = PlayerBox.SelectedItem as int;

                }
            }




            String voice = VoiceBox.SelectedItem?.ToString();
            String player = PlayerBox.SelectedItem?.ToString();
            if(voice?.Length > 0)
            {
                测试播放语音.IsEnabled = true;
            }


            if (voice?.Length>0 && player?.Length>0&& VoicePath?.Length>0)
            {
                生成对应语音.IsEnabled = true;
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
                VoicePath = VoicePathBox.Text = data.SelectedPath;
            }
            data.Dispose();
            #endregion
        }


       

        private void VoiceReciteTo(object sender, RoutedEventArgs e)
        {
            narratorcobj.SelectVoice(VoiceName);
            narratorcobj.synth.SpeakAsync("这里是对象朗读测试");


        }

        /// <summary>
        /// 批量生产语音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerVoiceButtonClick(object sender, RoutedEventArgs e)
        {
            //narratorcobj.SelectVoice(VoiceName);
            // narratorcobj.synth.SelectVoice(VoiceName);

             VoiceProduction();


            //var G = voices[1];



            // narratorcobj.SaveToWave(VoicePath, "测试", "12345");

            //narratorcobj.SaveToWave(VoicePath, "123", "12345");
        }



        public void VoiceProduction()
        {
            var SelectedLE = from r in textDataList where r.QQ == PlayerQQ select r;


            narratorcobj.SelectVoice(VoiceName);

            foreach (var item in SelectedLE)
            {


              var newID =   Regex.Replace(item.ID, "[: *?]","");//路径不支持的格式要自动和谐掉

                //var 和谐 = item.ID.Replace(" ", "");

                narratorcobj.SaveToWave(VoicePath, newID, item.Log);           
            }


            // SaveToWave()

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
