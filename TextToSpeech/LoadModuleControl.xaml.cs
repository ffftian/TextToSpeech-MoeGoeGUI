using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public TextData []QQArray;
        //输出格式直接按照ID输出，是谁就读对应的ID。

        public LoadModuleControl()
        {
            InitializeComponent();


            BrowseButton.Click += BrowseButtonClickCallback;

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
            }

           if (box.Name == PlayerBox.Name)
            {
                PlayerBox.SelectedItem = PlayerBox.SelectedValue;

            }

            String voice = VoiceBox.SelectedItem?.ToString();
            String player = PlayerBox.SelectedItem?.ToString();

            if (voice?.Length>0 && player?.Length>0 )
            {
                CreateVoice.IsEnabled = true;
            }

        }

         

        private void BrowseButtonClickCallback(object sender, RoutedEventArgs e)
        {

            #region 让玩家选择一个txt并填入TxtData中
            System.Windows.Forms.OpenFileDialog data = new System.Windows.Forms.OpenFileDialog();

            data.Filter = "(*.txt)|*.txt|All files (*.*)|*.*";

            data.ShowDialog();
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

           // 是否满足生成条件();
        }

        LinkedList<string> vs = new LinkedList<string>();


        public void InitVoiceEngine()
        {
            VoiceBox.Items.Clear();
            foreach (var voice in narratorcobj.synth.GetInstalledVoices())
            {
                ComboBoxItem box = new ComboBoxItem();
                box.Content = voice.VoiceInfo.Name;
                VoiceBox.Items.Add(box);
            }
        }




    }
}
