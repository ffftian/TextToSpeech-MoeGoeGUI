using NAudioWpfDemo;
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
        public List<TextData> CurrentRoleDataList;//存储当前角色文字的数据

        public Narrator narratorcobj = new Narrator();

        public List<VoiceInfo> voices = new List<VoiceInfo>();


        public TextData[] FirstQQNameArray;//存储所有的出场人
        //输出格式直接按照ID输出，是谁就读对应的ID。



        private string LogPath { get { return TextBox.Text; }set { TextBox.Text = value; } }//文字地址
        private string PlayerQQ ;//玩家QQ号
        private string PlayerName;//玩家名字

        private string VoicePath { get { return VoicePathBox.Text; } set { VoicePathBox.Text = value; } }//语音地址
        private string VoiceName;//语音名字
        public string[] VoiceNameArry;//这条数组用于读取名称，因为我不懂TM如何把object转成正常的string，用 VoiceName = box.SelectedValue.tostring();转出来会多些东西，像是反射出的。


        public NAudioRecorder nAudio;//语音录制
        const string 配置文件 = "\\配置文件.txt";
        const string AllRole = "所有角色";



        LoadModuleModel _vm = new LoadModuleModel();
        public LoadModuleControl()
        {
            _vm = new LoadModuleModel();
            nAudio = new NAudioRecorder();
            nAudio.OnWaveRecording += _vm.DrawVisualWaveform;
            this.DataContext = _vm;
            InitializeComponent();
            InitVoiceEngine();
            InitConfig();

            BrowseButton.Click += BrowseButtonClickCallback;
            BrowseButton2.Click += BrowseButton2ClickCallback;

            测试播放语音.Click += VoiceReciteTo;
            生成所有语音.Click += PlayerVoiceALLButtonClick;
            生成指定语音.Click += PlayerVoiceButtonClick;

            //BrowseButton.Click += BoxClick;
            //BrowseButton2.Click += BoxClick;
            VoiceBoxList.SelectionChanged += BoxClick;
            PlayerBoxList.SelectionChanged += BoxClick;

            PlayerBoxList.SelectionChanged += LocalListSelcet;

            开始录音.Click += RecordVoiceClick;
            停止录音.Click += EndVoiceClick;


            Left.Click += LeftClick;
            Right.Click += RightClick;

            //剧本文本
            Local_Left.Click += LocalLeftClick;
            Local_Right.Click += LocalRightClick;
            SaveButton.Click += SaveClick;
            Local_Number.TextChanged += Local_Number_TextChanged;
        }
        public void InitConfig()
        {
            LogPath = "null";
            VoicePath = "null";
            VoiceName = "null";

            if (File.Exists(System.Windows.Forms.Application.StartupPath + 配置文件))
            {
                try
                {
                    string[] Setting = File.ReadAllLines((System.Windows.Forms.Application.StartupPath + 配置文件));
                    LogPath = Setting[0];
                    PlayerQQ = Setting[1];
                    PlayerName = Setting[2];
                    VoicePath = Setting[3];
                    VoiceName = Setting[4];
                    Number.Text = Setting[5];
                    Local_Number.Text = Setting[6];
                    StreamReader streamReader = new StreamReader(LogPath, Encoding.UTF8);
                    TextAnalysis(streamReader.ReadToEnd());
                    InitBoxItem();
                }
                catch { }
            }

        }
        public void SaveConfig()
        {
            string Conetents = $"{LogPath}\n{PlayerQQ}\n{PlayerName}\n{VoicePath}\n{VoiceName}\n{Number.Text}\n{Local_Number.Text}";
            File.WriteAllText(System.Windows.Forms.Application.StartupPath + 配置文件, Conetents, Encoding.UTF8);
        }
        public void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveConfig();
        }


        private void Local_Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Local_Number.Text.Length==0)
            {
                return;
            }

            var match = Regex.Match(Local_Number.Text, "[0-9]+");
            if (match.Value.Equals(Local_Number.Text))
            {
                if ((Convert.ToInt32(Local_Number.Text) < 0))
                {
                    Local_Number.Text = "0";
                }
                else if ((Convert.ToInt32(Local_Number.Text)) > CurrentRoleDataList.Count)
                {
                    Local_Number.Text = CurrentRoleDataList.Count.ToString();
                }
                处理显示Text(CurrentRoleDataList, 局部显示文本, Local_Number, Local_ID);
            }
        }

        private void LocalLeftClick(object sender, RoutedEventArgs e)
        {
            //int i = 10;
            //int* iptr = &i;
            //  指针 = *;

            if ((Convert.ToInt32(Local_Number.Text) - 1) >= 0)
            {
                Local_Number.Text = (Convert.ToInt32(Local_Number.Text) - 1).ToString();
                处理显示Text(CurrentRoleDataList, 局部显示文本, Local_Number, Local_ID);
            }
        }
        private void LocalRightClick(object sender, RoutedEventArgs e)
        {
            if ((Convert.ToInt32(Local_Number.Text) + 1) < CurrentRoleDataList.Count)
            {
                Local_Number.Text = (Convert.ToInt32(Local_Number.Text) + 1).ToString();
                处理显示Text(CurrentRoleDataList, 局部显示文本, Local_Number, Local_ID);
            }
        }



        #region 整体text查看
         private void LeftClick(object sender, RoutedEventArgs e)
        {
            // Number.COU

            //  object i = 10;+
            //  int* iptr = &i;
            //  指针 = *;

            if ((Convert.ToInt32(Number.Text) - 1) >= 0)
            {
                Number.Text = (Convert.ToInt32(Number.Text) - 1).ToString();
                处理显示Text(textDataList, 显示文本, Number, ID);
            }
        }
        private void RightClick(object sender, RoutedEventArgs e)
        {
            if ((Convert.ToInt32(Number.Text) + 1) < textDataList.Count)
            {
                Number.Text = (Convert.ToInt32(Number.Text) + 1).ToString();
                处理显示Text(textDataList, 显示文本, Number, ID);
            }
            //  System.IO.File
        }
        #endregion
        /// <summary>
        /// 选中一个人物后的回应回调，用于显示ID和文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalListSelcet(object sender, RoutedEventArgs e)
        {
            if(PlayerQQ==AllRole)
            {
                CurrentRoleDataList = textDataList;
            }
            else
            {
                CurrentRoleDataList = (from r in textDataList where r.QQ == PlayerQQ select r).ToList();
                处理显示Text(CurrentRoleDataList, 局部显示文本, Local_Number, Local_ID);
            }
            Local_Left.IsEnabled = true;
            Local_Right.IsEnabled = true;
            Local_Number.IsEnabled = true;
        }


        private void 处理显示Text(List<TextData> datas, TextBox Log, TextBox Number, TextBox id)
        {
            if (datas.Count > Convert.ToInt32(Number.Text))
            {
                Log.Text = datas[Convert.ToInt32(Number.Text)].Log;
            }
            else
            {
                Number.Text = (datas.Count - 1).ToString();
                Log.Text = datas[datas.Count - 1].Log;
            }
            id.Text = datas[Convert.ToInt32(Number.Text)].ID;

            /*
            if (textDataList.Count > Convert.ToInt32(Number.Text))
            {
                显示文本.Text = textDataList[Convert.ToInt32(Number.Text)].Log;
            }
            else
            {
                Number.Text =  (textDataList.Count).ToString();
                显示文本.Text = textDataList[textDataList.Count].Log;
            }
            ID.Text = textDataList[textDataList.Count].ID;
            */

        }
        //private void 处理局部显示Text()
        //{
        //    if (CurrentDataList.Count > Convert.ToInt32(Local_Number.Text))
        //    {
        //        局部显示文本.Text = CurrentDataList[Convert.ToInt32(Local_Number.Text)].Log;
        //    }
        //    else
        //    {
        //        Number.Text = (CurrentDataList.Count).ToString();
        //        局部显示文本.Text = CurrentDataList[CurrentDataList.Count].Log;
        //    }
        //    Local_ID.Text = CurrentDataList[CurrentDataList.Count].ID;
        //}



        //private void SeletInitButton(object sender, RoutedEventArgs e)
        //{
        //    SetSelcetBox();
        //}


        #region 播放与AI生成
        private void BoxClick(object sender, RoutedEventArgs e)
        {

            if (sender is ComboBox)
            {
                ComboBox box = sender as ComboBox;
                if(box.Items.Count==0)
                {
                    return;
                }
                if (box.Name == VoiceBoxList.Name)
                {
                    VoiceBoxList.SelectedItem = box.SelectedValue;//这Value是TM的方法。
                    VoiceName = VoiceNameArry[box.SelectedIndex];
                }
                else if (box.Name == PlayerBoxList.Name)
                {
                    PlayerBoxList.SelectedItem = PlayerBoxList.SelectedValue;
                    if (PlayerBoxList.SelectedValue.ToString() == AllRole)
                    {
                        PlayerQQ = AllRole;
                        PlayerName = "0";
                    }
                    else
                    {
                        PlayerQQ = FirstQQNameArray[PlayerBoxList.SelectedIndex].QQ;
                        PlayerName = FirstQQNameArray[PlayerBoxList.SelectedIndex].RoleName;
                    }
                }
            }
            String voice = VoiceBoxList.SelectedItem?.ToString();
            String player = PlayerBoxList.SelectedItem?.ToString();
            if (voice?.Length > 0)
            {
                测试播放语音.IsEnabled = true;
                生成指定语音.IsEnabled = true;
            }
            if (voice?.Length > 0 && player?.Length > 0 && VoicePath?.Length > 0)
            {
                生成所有语音.IsEnabled = true;
            }
        }


        public void InitBoxItem()
        {
            foreach(ComboBoxItem item in PlayerBoxList.Items)
            {
                if($"{PlayerName}({PlayerQQ})"== item.Content.ToString())
                {
                    PlayerBoxList.SelectedItem = item;
                    LocalListSelcet(null, null);
                    break;
                }
            }
        }



        /// <summary>
        /// 2021：用于将读取的text文本置入角色对话框选择当中。
        /// </summary>
        /// <param name="Data"></param>
        void TextAnalysis(string Data)
        {
            textDataList = QQTool.QQLogSplit<TextData>(Data, ErorrShow);
            PlayerBoxList.Items.Clear();
            FirstQQNameArray = textDataList.DistinctBy(a => a.QQ).ToArray();
            foreach (var QQData in FirstQQNameArray)
            {
                ComboBoxItem box = new ComboBoxItem();
                box.Content = $"{QQData.RoleName}({ QQData.QQ})";
                PlayerBoxList.Items.Add(box);
            }
            ComboBoxItem box2 = new ComboBoxItem();
            box2.Content = $"{AllRole}(0)";
            PlayerBoxList.Items.Add(box2);
            Left.IsEnabled = true;
            Right.IsEnabled = true;
            处理显示Text(textDataList, 显示文本, Number, ID);
        }
        bool cancel = false;
        void ErorrShow(Exception exception,string content)
        {
            if (cancel) return;
            switch(MessageBox.Show(content, exception.Message,MessageBoxButton.OKCancel))
            {
                case MessageBoxResult.Cancel:
                    cancel = true;
                    break;
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
                LogPath = TextBox.Text = data.FileName;//取得名称并输入到界面上。

                #endregion
            TextAnalysis(tempData);
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

                开始录音.IsEnabled = true;

            }
            data.Dispose();
            #endregion
        }




        private void VoiceReciteTo(object sender, RoutedEventArgs e)
        {
            //narratorcobj.TestNarrate(VoiceName);
            //try
            //{
            narratorcobj.SetRate(Convert.ToInt32(RateIuput.Text));
            narratorcobj.SelectVoice(VoiceName);


            if (局部显示文本.Text.Length > 1)
            {
                局部显示文本.Text = 局部显示文本.Text.Replace("@", "at ");
                narratorcobj.synth.SpeakAsync(局部显示文本.Text);
            }
            else
            {
                narratorcobj.synth.SpeakAsync("这里是朗读测试");
            }
        }

        /// <summary>
        /// 批量生产语音并到指定目录下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerVoiceALLButtonClick(object sender, RoutedEventArgs e)
        {
            VoiceProduction();
        }
        private void PlayerVoiceButtonClick(object sender, RoutedEventArgs e)
        {



            var newID = Regex.Replace(CurrentRoleDataList[int.Parse(Local_Number.Text)].ID, MiaoRegexTool.路径非法字符, "");//路径不支持的格式要自动和谐掉
            var newLog = CurrentRoleDataList[int.Parse(Local_Number.Text)].Log.Replace("@", "at");//@处理成at
            narratorcobj.SaveToWave(VoicePath, newID, newLog);

            // var newID = Regex.Replace(item.ID, RegexTool.匹配路径非法字符, "");//路径不支持的格式要自动和谐掉

            //  narratorcobj.SaveToWave(VoicePath, newID, item.Log);
        }

        /// <summary>
        /// 生成语音
        /// </summary>
        public void VoiceProduction()
        {
            narratorcobj.SetRate(Convert.ToInt32(RateIuput.Text));
            IEnumerable<TextData> SelectedLE;
            if (PlayerQQ == AllRole)
            {
                SelectedLE = textDataList;
            }
            else
            {
                SelectedLE = from r in textDataList where r.QQ == PlayerQQ select r;
            }
            narratorcobj.SelectVoice(VoiceName);
            foreach (var item in SelectedLE)
            {
                var newID = Regex.Replace(item.ID, MiaoRegexTool.路径非法字符, "");//路径不支持的格式名称要自动和谐掉
                var newLog = item.Log.Replace("@", "at");//@处理成at
                narratorcobj.SaveToWave(VoicePath, newID, newLog);
            }
        }


        public void InitVoiceEngine()
        {
            VoiceBoxList.Items.Clear();
            VoiceNameArry = new string[narratorcobj.synth.GetInstalledVoices().Count];


            for (int a = 0; a < narratorcobj.synth.GetInstalledVoices().Count; a++)
            {
                VoiceNameArry[a] = narratorcobj.synth.GetInstalledVoices()[a].VoiceInfo.Name;
            }
            //test
            var t = narratorcobj.synth.GetInstalledVoices();

            //初始化显示所有语音
            foreach (var voice in narratorcobj.synth.GetInstalledVoices())
            {
                voices.Add(voice.VoiceInfo);

                ComboBoxItem box = new ComboBoxItem();
                box.Content = voice.VoiceInfo.Name;
                VoiceBoxList.Items.Add(box);
            }
        }
        #endregion;
        /// <summary>
        /// 录制麦克风声音。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordVoiceClick(object sender, RoutedEventArgs e)
        {
            string newID;
            if (CurrentRoleDataList == null || CurrentRoleDataList.Count == 0)
            {
                newID = "temp";
            }
            else
            {
                newID = Regex.Replace(CurrentRoleDataList[Convert.ToInt32(Local_Number.Text)].ID, MiaoRegexTool.路径非法字符, "");//路径不支持的格式要自动和谐掉
            }
            nAudio.SetFilePath(VoicePath, newID);
            nAudio.StartRec();
            开始录音.IsEnabled = false;
            停止录音.IsEnabled = true;
        }

        /// <summary>
        /// 结束录制麦克风声音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndVoiceClick(object sender, RoutedEventArgs e)
        {
            nAudio.StopRec();
            开始录音.IsEnabled = true;
            停止录音.IsEnabled = false;
        }

    }
}
