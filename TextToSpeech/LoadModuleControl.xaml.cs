﻿using NAudioWpfDemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace TextToSpeech
{
    /// <summary>
    /// LoadModuleControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoadModuleControl : UserControl
    {
        public MainWindow MainWindow { get; set; }

        //public List<QQTextData> textDataList;//存储所有文字的数据
        //public List<QQTextData> CurrentRoleDataList;//存储当前角色文字的数据

        public List<BaseTextData> textDataList;//存储所有文字的数据
        public List<BaseTextData> CurrentRoleDataList;//存储当前角色文字的数据

        public Narrator narratorcobj = new Narrator();

        public List<VoiceInfo> voices = new List<VoiceInfo>();

        public BaseTextData[] FirstQQNameArray;//存储所有的出场人
        ///public QQTextData[] FirstQQNameArray;//存储所有的出场人
        //输出格式直接按照ID输出，是谁就读对应的ID。



        public string LogFilePath { get { return TextBox.Text; }set { TextBox.Text = value; } }//日志保存地址
        public string LogFileName;

        public string currentGroup;//玩家QQ号
        public string currentName;//玩家名字

        public string VoiceSavePath
        {
            get
            {
                if(PathHasPlayerName)
                {
                    return $"{VoicePath}/{currentName}";
                }
                else
                {
                    return $"{VoicePath}";
                }
            }
        }

        public string VoicePath { get { return VoicePathBox.Text; } set { VoicePathBox.Text = value; } }//语音保存地址



        public string VoiceName;//语音名字
        public string[] VoiceNameArray;//这条数组用于读取名称，因为我不懂TM如何把object转成正常的string，用 VoiceName = box.SelectedValue.tostring();转出来会多些东西，像是反射出的。


        public bool PathHasPlayerName
        {
            get
            {
                return 文件夹路径包含角色名称.IsChecked.Value;
            }
            set
            {
                文件夹路径包含角色名称.IsChecked = value;
            }
        }

        public int Local_Ptr { get; set; }

        public NAudioRecorder nAudioMicrophone;//语音录制
        public NAudioRecordSoundcard nAudioSoundcard;//电脑音频录制

        const string 配置文件 = "\\配置文件.txt";
        const string AllRole = "所有角色";



        LoadModuleModel _vm;
        public LoadModuleControl()
        {

            _vm = new LoadModuleModel();
            nAudioMicrophone = new NAudioRecorder();
            nAudioSoundcard = new NAudioRecordSoundcard();
            nAudioMicrophone.OnWaveRecording += _vm.DrawVisualWaveform;
            nAudioSoundcard.OnWaveRecording += _vm.DrawVisualWaveform;

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
            PlayerBoxList.SelectionChanged += LocalListSelect;


            开始录音1.Click += RecordMicrophoneVoiceClick;
            开始录音2.Click += RecordComputerVoiceClick;
            停止录音.Click += EndVoiceClick;


            Left.Click += LeftClick;
            Right.Click += RightClick;

            //剧本文本
            Local_Left.Click += LocalLeftClick;
            Local_Right.Click += LocalRightClick;
            SaveButton.Click += SaveClick;
            Local_Number.TextChanged += Local_Number_TextChanged;



        }

        private void 文件夹路径包含角色名称_Click(object sender, RoutedEventArgs e)
        {
           
        }

        public void InitConfig()
        {
            LogFilePath = "null";
            VoicePath = null;
            VoicePath = "null";
            VoiceName = "null";

            if (File.Exists(System.Windows.Forms.Application.StartupPath + 配置文件))
            {
                try
                {
                    string[] Setting = File.ReadAllLines((System.Windows.Forms.Application.StartupPath + 配置文件));
                    LogFilePath = Setting[0];
                    currentGroup = Setting[1];
                    currentName = Setting[2];
                    VoicePath = Setting[3];
                    VoiceName = Setting[4];
                    Number.Text = Setting[5];
                    Local_Number.Text = Setting[6];
                    PathHasPlayerName = bool.Parse(Setting[7]);
                    StreamReader streamReader = new StreamReader(LogFilePath, Encoding.UTF8);
                    TextAnalysis(streamReader.ReadToEnd());
                    InitBoxItem();
                    LogFileName = LogFilePath.Substring(LogFilePath.LastIndexOf('\\') + 1).Replace(".txt","");
                }
                catch { }
            }

        }
        public void SaveConfig()
        {
            string Conetents = $"{LogFilePath}\n{currentGroup}\n{currentName}\n{VoicePath}\n{VoiceName}\n{Number.Text}\n{Local_Number.Text}\n{PathHasPlayerName}";
            File.WriteAllText(System.Windows.Forms.Application.StartupPath + 配置文件, Conetents, Encoding.UTF8);
        }
        public async void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            SaveButton.Content = "保存设置(完成)";
            SaveButton.IsEnabled = false;
            await Task.Delay(2000);
            SaveButton.Content = "保存设置";
            SaveButton.IsEnabled = true;
        }


        private void Local_Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Local_Number.Text.Length==0)
            {
                return;
            }

            var match = Regex.Match(Local_Number.Text, "[0-9]+");//检测下标是否为数字
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
                //处理显示Text(CurrentRoleDataList, 局部显示文本, Local_Number, Local_ID);
            }
        }
        private void LocalRightClick(object sender, RoutedEventArgs e)
        {
            if ((Convert.ToInt32(Local_Number.Text) + 1) < CurrentRoleDataList.Count)
            {
                Local_Number.Text = (Convert.ToInt32(Local_Number.Text) + 1).ToString();
                //ok
                //处理显示Text(CurrentRoleDataList, 局部显示文本, Local_Number, Local_ID);
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
        private void LocalListSelect(object sender, RoutedEventArgs e)
        {
            if(currentGroup==AllRole)
            {
                CurrentRoleDataList = textDataList;
            }
            else
            {
                CurrentRoleDataList = (from text in textDataList where text.GroupID == currentGroup select text).ToList();
            }
            处理显示Text(CurrentRoleDataList, 局部显示文本, Local_Number, Local_ID);
            Local_Left.IsEnabled = true;
            Local_Right.IsEnabled = true;
            Local_Number.IsEnabled = true;
        }


        private void 处理显示Text(List<BaseTextData> datas, TextBox Log, TextBox Number, TextBox id)
        {

            if (datas.Count > Convert.ToInt32(Number.Text))
            {
                Local_Ptr = Convert.ToInt32(Number.Text);
                Log.Text = datas[Local_Ptr].log;
            }
            else
            {
                Local_Ptr = datas.Count - 1;
                Number.Text = (datas.Count - 1).ToString();
                Log.Text = datas[datas.Count - 1].log;
            }
            id.Text = datas[Convert.ToInt32(Number.Text)].SaveID;

        }
        //private void 处理局部显示Text()
        //{
        //    if (CurrentDataList.Count > Convert.ToInt32(Local_Number.Text))
        //    {
        //        局部显示文本.Text = CurrentDataList[Convert.ToInt32(Local_Number.Text)].log;
        //    }
        //    else
        //    {
        //        Number.Text = (CurrentDataList.Count).ToString();
        //        局部显示文本.Text = CurrentDataList[CurrentDataList.Count].log;
        //    }
        //    Local_ID.Text = CurrentDataList[CurrentDataList.Count].id;
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
                if (box.Items.Count == 0) return;

                if (box.Name == VoiceBoxList.Name)
                {
                    VoiceBoxList.SelectedItem = box.SelectedValue;//这Value是TM的方法。
                    VoiceName = VoiceNameArray[box.SelectedIndex];
                }
                else if (box.Name == PlayerBoxList.Name)//书覅有是玩家选择栏
                {
                    //PlayerBoxList.SelectedItem = PlayerBoxList.SelectedValue;
                    //if (PlayerBoxList.SelectedValue as string == AllRole)
                    //{
                    //    currentGroup = AllRole;
                    //    currentName = "0";
                    //}
                    //else
                    //{
                   
                      if(PlayerBoxList.SelectedIndex< FirstQQNameArray.Length)
                    {
                        currentGroup = FirstQQNameArray[PlayerBoxList.SelectedIndex].GroupID;
                        currentName = FirstQQNameArray[PlayerBoxList.SelectedIndex].name;
                    }
                       else
                    {
                        currentGroup = AllRole;
                        currentName = AllRole;
                    }
                    //} 
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
                if($"{currentName}({currentGroup})"== item.Content.ToString())
                {
                    PlayerBoxList.SelectedItem = item;
                    LocalListSelect(null, null);
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
            PlayerBoxList.Items.Clear();//清空时会触发回调导致问题，所以需要先进行Items的清理，再去读取新的QQ资源。
            textDataList = TextTool.LogSplit(Data, this.ErorrShow);
            //textDataList = global::TextTool.QQLogSplit<BaseTextData>(Data, this.ErorrShow);
            FirstQQNameArray = textDataList.DistinctBy(a => a.GroupID).ToArray();
            foreach (var QQData in FirstQQNameArray)
            {
                ComboBoxItem box = new ComboBoxItem();
                box.Content = $"{QQData.name}({ QQData.GroupID})";
                PlayerBoxList.Items.Add(box);
            }
            ComboBoxItem box2 = new ComboBoxItem();
            box2.Content = $"{AllRole}";
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
        /// <summary>
        /// 选中外部文档文件,切换文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButtonClickCallback(object sender, RoutedEventArgs e)
        {

            #region 让玩家选择一个txt并填入TxtData中
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

            openFileDialog.Filter = "(*.txt)|*.txt|All files (*.*)|*.*";

            //   openFileDialog.ShowDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//用于指示让文件夹展开对文件选中才允许运行。
            {
                StreamReader streamReader = new StreamReader(openFileDialog.OpenFile(), Encoding.UTF8);
                string tempData = streamReader.ReadToEnd();
                LogFilePath = TextBox.Text = openFileDialog.FileName;//取得名称并输入到界面上。

                #endregion
            TextAnalysis(tempData);
            Local_Number.Text = "0";
            }
            openFileDialog.Dispose();

            // 是否满足生成条件();
        }


        private void BrowseButton2ClickCallback(object sender, RoutedEventArgs e)
        {
            #region 让玩家选择保存声音的路径
            System.Windows.Forms.FolderBrowserDialog data = new System.Windows.Forms.FolderBrowserDialog();



            if (data.ShowDialog() == System.Windows.Forms.DialogResult.OK)//用于指示让文件夹展开对文件筐。
            {
                VoicePath = data.SelectedPath;

                开始录音1.IsEnabled = true;
                开始录音2.IsEnabled = true;


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
        public string GetRoleDialogueID
        {
            get
            {
                return CurrentRoleDataList[int.Parse(Local_Number.Text)].SaveID;
            }
        }
        public string GetRoleDialogue
        {
            get
            {
                return CurrentRoleDataList[int.Parse(Local_Number.Text)].log.Replace("@", "at");
            }
        }


        private void PlayerVoiceButtonClick(object sender, RoutedEventArgs e)
        {
            string newID = GetRoleDialogueID;//路径不支持的格式要自动和谐掉
            string newLog = GetRoleDialogue;//@处理成at
            narratorcobj.SaveToWave(VoiceSavePath, newID, newLog);

            // var newID = Regex.Replace(item.id, RegexTool.匹配路径非法字符, "");//路径不支持的格式要自动和谐掉

            //  narratorcobj.SaveToWave(VoicePath, newID, item.log);
        }

        /// <summary>
        /// 生成语音
        /// </summary>
        public void VoiceProduction()
        {
            narratorcobj.SetRate(Convert.ToInt32(RateIuput.Text));
            IEnumerable<BaseTextData> SelectedLE;
            if (currentGroup == AllRole)
            {
                SelectedLE = textDataList;
            }
            else
            {
                SelectedLE = from r in textDataList where r.GroupID == currentGroup select r;
            }
            narratorcobj.SelectVoice(VoiceName);
            foreach (var item in SelectedLE)
            {
                var newID = item.SaveID;//路径不支持的格式名称要自动和谐掉
                var newLog = item.log.Replace("@", "at");//@处理成at
                narratorcobj.SaveToWave(VoiceSavePath, newID, newLog);
            }
        }


        public void InitVoiceEngine()
        {
            VoiceBoxList.Items.Clear();
            VoiceNameArray = new string[narratorcobj.synth.GetInstalledVoices().Count];


            for (int a = 0; a < narratorcobj.synth.GetInstalledVoices().Count; a++)
            {
                VoiceNameArray[a] = narratorcobj.synth.GetInstalledVoices()[a].VoiceInfo.Name;
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

        private bool RecordStatusCheck()
        {
            if(Directory.Exists(VoiceSavePath) && File.Exists(LogFilePath))
            {
                return true;
            }
            else
            {
                MessageBox.Show("路径不正常，请检查Log路径和导出路径", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }


        }

        /// <summary>
        /// 录制麦克风声音。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordMicrophoneVoiceClick(object sender, RoutedEventArgs e)
        {
            if (!RecordStatusCheck()) return;

            string newID;
            if (CurrentRoleDataList == null || CurrentRoleDataList.Count == 0)
            {
                newID = "temp";
            }
            else
            {
                newID = CurrentRoleDataList[Convert.ToInt32(Local_Number.Text)].SaveID;//路径不支持的格式要自动和谐掉
            }
            nAudioMicrophone.SetFilePath(VoiceSavePath, newID);
            nAudioMicrophone.StartRec();
            开始录音1.IsEnabled = false;
            开始录音2.IsEnabled = false;
            停止录音.IsEnabled = true;
        }

     
        /// <summary>
        /// 开始录制电脑内部声音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordComputerVoiceClick(object sender, RoutedEventArgs e)
        {
            if (!RecordStatusCheck()) return;

            string newID;
            if (CurrentRoleDataList == null || CurrentRoleDataList.Count == 0)
            {
                newID = "temp";
            }
            else
            {
                newID =CurrentRoleDataList[Convert.ToInt32(Local_Number.Text)].SaveID;//路径不支持的格式要自动和谐掉
            }
            nAudioSoundcard.SetFilePath(VoiceSavePath, newID);
            nAudioSoundcard.StartRec();
            开始录音1.IsEnabled = false;
            开始录音2.IsEnabled = false;
            停止录音.IsEnabled = true;
        }
        /// <summary>
        /// 结束录制声音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndVoiceClick(object sender, RoutedEventArgs e)
        {
            nAudioMicrophone.StopRec();
            nAudioSoundcard.StopRec();
            开始录音1.IsEnabled = true;
            开始录音2.IsEnabled = true;
            停止录音.IsEnabled = false;
        }

    }
}
