﻿using System;
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
        public List<TextData> CurrentDataList;//存储当前角色文字的数据

        public Narrator narratorcobj = new Narrator();

        public List<VoiceInfo> voices = new List<VoiceInfo>();


        public TextData[] QQArray;//存储所有的出场人
        //输出格式直接按照ID输出，是谁就读对应的ID。

        private string VoicePath;
        private string VoiceName;
        private string PlayerQQ;

        unsafe int* 指针;

        public string[] VoiceNameArry;//尼玛的，这条数组用于读取名称，因为我不懂TM如何把object转成正常的string，用 VoiceName = box.SelectedValue.tostring();转出来会多些东西


        public NAudioRecorder nAudio;//语音录制

        public LoadModuleControl()
        {
            nAudio = new NAudioRecorder();


            InitializeComponent();


            BrowseButton.Click += BrowseButtonClickCallback;
            BrowseButton2.Click += BrowseButton2ClickCallback;

            测试播放语音.Click += VoiceReciteTo;
            生成所有语音.Click += PlayerVoiceALLButtonClick;
            生成指定语音.Click += PlayerVoiceButtonClick;


            BrowseButton.Click += BoxClick;
            BrowseButton2.Click += BoxClick;
            VoiceBoxList.SelectionChanged += BoxClick;
            PlayerBoxList.SelectionChanged += BoxClick;

            PlayerBoxList.SelectionChanged += LocalListSelcet;

            开始录音.Click += RecordVoiceClick;
            停止录音.Click += EndVoiceClick;

            InitVoiceEngine();

            Left.Click += LeftClick;
            Right.Click += RightClick;

            Local_Left.Click += LocalLeftClick;
            Local_Right.Click += LocalRightClick;
            // Number. += ;

            

        }


        private void LocalLeftClick(object sender, RoutedEventArgs e)
        {
            //int i = 10;
            //int* iptr = &i;
            //  指针 = *;

            if ((Convert.ToInt32(Local_Number.Text) - 1) >= 0)
            {
                Local_Number.Text = (Convert.ToInt32(Local_Number.Text) - 1).ToString();
                处理显示Text(CurrentDataList, 局部显示文本,Local_Number, Local_ID);
            }
        }
        private void LocalRightClick(object sender, RoutedEventArgs e)
        {
            if ((Convert.ToInt32(Local_Number.Text) + 1) < CurrentDataList.Count)
            {
                Local_Number.Text = (Convert.ToInt32(Local_Number.Text) + 1).ToString();
                处理显示Text(CurrentDataList, 局部显示文本, Local_Number, Local_ID);
            }
            //  System.IO.File
        }


        #region 整体text查看
        unsafe private void LeftClick(object sender, RoutedEventArgs e)
        {
            // Number.COU

           //  object i = 10;+
           //  int* iptr = &i;
            //  指针 = *;

            if ((Convert.ToInt32(Number.Text) - 1) >= 0)
            {
                Number.Text = (Convert.ToInt32(Number.Text) - 1).ToString();
                处理显示Text(textDataList,显示文本,Number,ID);
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
            CurrentDataList = (from r in textDataList where r.QQ == PlayerQQ select r).ToList();
            var G = Convert.ToInt32(Number.Text);

            处理显示Text(CurrentDataList, 局部显示文本, Local_Number, Local_ID);
            Local_Left.IsEnabled = true;
           Local_Right.IsEnabled = true;
        }


        private void 处理显示Text(List<TextData> datas,TextBox Log,TextBox Number, TextBox id)
        {
            if (datas.Count > Convert.ToInt32(Number.Text))
            {
                Log.Text = datas[Convert.ToInt32(Number.Text)].Log;
            }
            else
            {
                Number.Text = (datas.Count-1).ToString();
                Log.Text = datas[datas.Count-1].Log;
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
                if (box.Name == VoiceBoxList.Name)
                {
                    VoiceBoxList.SelectedItem = box.SelectedValue;//这Value是TM的方法。

                    // VoiceName = Convert.ToString(VoiceBox.SelectedItem);

                    VoiceName = VoiceNameArry[box.SelectedIndex];
                    // StreamReader rems = new StreamReader(box.SelectedValue);
                    // byte[] g = box.SelectedValue;

                    // VoiceName = box.SelectedValue;
                }
                if (box.Name == PlayerBoxList.Name)
                {
                    PlayerBoxList.SelectedItem = PlayerBoxList.SelectedValue;
                    PlayerQQ = QQArray[PlayerBoxList.SelectedIndex].QQ;
                    // PlayerQQ = Convert.ToInt32(PlayerBox.SelectedValue);


                    //   PlayerID = PlayerBox.SelectedItem as int;

                }
            }
            String voice = VoiceBoxList.SelectedItem?.ToString();
            String player = PlayerBoxList.SelectedItem?.ToString();
            if(voice?.Length > 0)
            {
                测试播放语音.IsEnabled = true;
                生成指定语音.IsEnabled = true;
            }


            if (voice?.Length>0 && player?.Length>0&& VoicePath?.Length>0)
            {
                生成所有语音.IsEnabled = true;
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

                PlayerBoxList.Items.Clear();
                //QQArray =  textDataList.Select(a => a.QQ).Distinct().ToArray();
                QQArray = textDataList.DistinctBy(a => a.QQ).ToArray();
                foreach (var QQData in QQArray)
                {
                    ComboBoxItem box = new ComboBoxItem();
                    box.Content = $"{QQData.RoleName} ({ QQData.QQ})";
                    PlayerBoxList.Items.Add(box);
                }
                Left.IsEnabled = true;
                Right.IsEnabled = true;
                处理显示Text(textDataList,显示文本,Number,ID);
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
            //}
            //catch(Exception at)
            //{

            //}


        }

        /// <summary>
        /// 批量生产语音并到指定目录下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerVoiceALLButtonClick(object sender, RoutedEventArgs e)
        {
            //narratorcobj.SelectVoice(VoiceName);
            // narratorcobj.synth.SelectVoice(VoiceName);

             VoiceProduction();


            //var G = voices[1];



            // narratorcobj.SaveToWave(VoicePath, "测试", "12345");

            //narratorcobj.SaveToWave(VoicePath, "123", "12345");
        }
        private void PlayerVoiceButtonClick(object sender, RoutedEventArgs e)
        {
         


            var newID = Regex.Replace(CurrentDataList[int.Parse(Local_Number.Text)].ID, RegexTool.匹配路径非法字符, "");//路径不支持的格式要自动和谐掉
            var newLog = CurrentDataList[int.Parse(Local_Number.Text)].Log.Replace("@", "at");//@处理成at
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
            var SelectedLE = from r in textDataList where r.QQ == PlayerQQ select r;


            narratorcobj.SelectVoice(VoiceName);

            foreach (var item in SelectedLE)
            {


              var newID =   Regex.Replace(item.ID, RegexTool.匹配路径非法字符, "");//路径不支持的格式要自动和谐掉

                var newLog = item.Log.Replace("@", "at");//@处理成at
                //var 和谐 = item.ID.Replace(" ", "");



                narratorcobj.SaveToWave(VoicePath, newID, newLog);           
            }


            // SaveToWave()

        }


        public void InitVoiceEngine()
        {
            VoiceBoxList.Items.Clear();
            VoiceNameArry = new string[narratorcobj.synth.GetInstalledVoices().Count];


            for(int a=0;a< narratorcobj.synth.GetInstalledVoices().Count; a++)
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
                // box.Content = voice.VoiceInfo.Name.Replace(" Desktop","");
                box.Content = voice.VoiceInfo.Name;


               // desktop

                VoiceBoxList.Items.Add(box);
            }
        }
        #endregion;
        



        private void RecordVoiceClick(object sender, RoutedEventArgs e)//录制声音，麦克风声音小的话嘴贴麦。
        {
            //匹配路径非法字符

             var newID = Regex.Replace(textDataList[Convert.ToInt32(Number.Text)].ID, RegexTool.匹配路径非法字符, "");//路径不支持的格式要自动和谐掉
            nAudio.SetFilePath(VoicePath, newID);

            nAudio.StartRec();


            开始录音.IsEnabled = false;
            停止录音.IsEnabled = true;


        }


        private void EndVoiceClick(object sender, RoutedEventArgs e)//结束录制声音
        {
            nAudio.StopRec();

            开始录音.IsEnabled = true;
            停止录音.IsEnabled = false;
        }

    }
}