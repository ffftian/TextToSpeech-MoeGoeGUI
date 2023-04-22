using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using Microsoft.Win32;
using Newtonsoft.Json;

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
        /// <summary>
        /// 模型位置
        /// </summary>
        private string W2V2Path { get { return emotionalModelPath.Text; } set { emotionalModelPath.Text = value; } }

        public int Pattern { get { return PatternBox.SelectedIndex; } set { PatternBox.SelectedIndex = value; } }


        private double Deviation
        {
            get
            {
                return DeviationSlider.Value * random.NextDouble();
            }

        }

        private string RateLength
        {
            get
            {
                return $"[LENGTH={RateSlider.Value + Deviation}]";
            }
        }
        private int CreateCount
        {
            get
            {
                return (int)CountSlider.Value;
            }
            set
            {
                CountSlider.Value = value;
                CountInput.Text = value.ToString();
            }
        }
        private int CurrentCreateCount;

        public SpeakerText[] SpeakerTextData;
        ///// <summary>
        ///// VITS，实际朗读者朗读用的数据
        ///// </summary>
        //public string[] SpeakerTextData;
        ///// <summary>
        ///// 情感识别保存位置
        ///// </summary>
        //public string[] EmotionData;


        //public int Rate { get { return RateIuput} }

        public CommandLine cmd;
        //public MoeGoeOutPut moeGoeOutPut;
        const string MoeGoe配置文件 = "\\MoeGoe配置文件.txt";

        private bool generateLock = false;
        public bool GenerateComplete
        {
            get
            {

                return generateLock;
            }
            set
            {
                生成指定语音.IsEnabled = value;
                生成所有语音.IsEnabled = value;
                generateLock = value;
            }
        }
        private Random random;


        public MoeGoeTextToSpeechControl()
        {
            random = new Random();

            InitializeComponent();
            LoadConfig();
            InitSpeakerTextCashe();
            OpenEXE.Click += OpenMoeGoe_Click;
            OpenModel.Click += OpenModel_Click;
            OpenConfig.Click += OpenConfig_Click;
            OpenEmotionalModel.Click += OpenEmotionalModel_Click;
            OpenEmotionalData.Click += OpenEmotionalData_Click;

            生成指定语音.Click += 生成指定语音_Click;
            生成所有语音.Click += 生成所有语音_Click;

            启用语音生成控制台.Click += 启用语音生成控制台_Click;
            禁用语音生成控制台.Click += 禁用语音生成控制台_Click;

            RateSlider.ValueChanged += RateSlider_ValueChanged;
            CountSlider.ValueChanged += NumberSlider_ValueChanged;
            DeviationSlider.ValueChanged += DeviationSlider_ValueChanged;
            //RateIuput.TextChanged += RateIuput_TextChanged;

            SpeakerText.TextChanged += SpeakerText_TextChanged;


            批量修改命名音频.Click += 批量修改命名音频_Click;
            翻译成日文.Click += 翻译成日文_Click;

        }

        BaiduTranslation baiduTranslation = new BaiduTranslation();
        private void 翻译成日文_Click(object sender, RoutedEventArgs e)
        {
            SpeakerText.Text = baiduTranslation.GetTranslation(SpeakerTextData[LoadModuleControl.Local_Ptr].SpeakerTextData);
            SpeakerText.IsEnabled = false;
            Task.Delay(1000);
            SpeakerText.IsEnabled = true;
        }

        private void 导入翻译()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "配置文件|*.txt"
            };
            if ((bool)ofd.ShowDialog())
            {
                
            }
        }

        private void 批量修改命名音频_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(LoadModuleControl.VoiceSavePath))
            {
                string[] files = Directory.GetFiles(LoadModuleControl.VoiceSavePath);

                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileInfo = new FileInfo(files[i]);
                    int index = files[i].LastIndexOf(')');
                    string path = files[i].Substring(0, index + 1) + ".wav";
                    if (path == files[i]) continue;

                    try
                    {
                        fileInfo.MoveTo(path);
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message, "失败", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                }
                MessageBox.Show("命名整合完成");
            }
            else
            {
                MessageBox.Show("命名失败", "失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeviationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DeviationInput.Text = e.NewValue.ToString();
        }

        private void NumberSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CountInput.Text = ((int)e.NewValue).ToString();
        }

        private void RateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //RateInput.Text = e.NewValue;
            RateInput.Text = e.NewValue.ToString();
            //RateInput.Text = String.Format("{0:F}", e.NewValue);

        }

        public void InitializeBindWindow()
        {
            LoadModuleControl.SaveButton.Click += OnSaveButton;
            LoadModuleControl.PlayerBoxList.SelectionChanged += PlayerBoxList_SelectionChanged;
            LoadModuleControl.Local_Number.TextChanged += Local_Number_TextChanged;
            //Item绑定时不会触发Button的呼叫
            if (LoadModuleControl.CurrentRoleDataList != null)
            {
                DeserializeCasheData(SpeakerTextPath);
                Local_Number_TextChanged(null, null);
            }


        }
        /// <summary>
        /// 索引此时已经被LoadModule中的处理显示Text变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Local_Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SpeakerTextData == null) return;
            SpeakerText.Text = SpeakerTextData[LoadModuleControl.Local_Ptr].SpeakerTextData;
            emotionData.Text = SpeakerTextData[LoadModuleControl.Local_Ptr].EmotionSubName;
        }

        /// <summary>
        /// 朗读框文本变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeakerText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ///文本内容更改赋值给保存数组
            SpeakerTextData[LoadModuleControl.Local_Ptr].SpeakerTextData = SpeakerText.Text;
        }

        string SpeakerTextPath
        {
            get
            {
                return $"{System.Environment.CurrentDirectory}\\Cashe\\{LoadModuleControl.LogFileName}-{LoadModuleControl.currentName}({LoadModuleControl.currentGroup}).txt";
            }
        }

        public string ConvertToPath(string name)
        {
            return $"{System.Environment.CurrentDirectory}\\Cashe\\{LoadModuleControl.LogFileName}-{name}.txt";
        }

        /// <summary>
        /// 角色切换时读取新文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerBoxList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Items.Count == 0) return;


            int index = e.RemovedItems.Count;
            string lastName = null;
            if (index != 0)
            {
                lastName = (e.RemovedItems[0] as ComboBoxItem).Content.ToString();//上一项
            }
            string currentName = (e.AddedItems[0] as ComboBoxItem).Content.ToString();//这一项

            if (!string.IsNullOrEmpty(lastName))
            {
                SerializableCasheData(ConvertToPath(lastName));
            }
            if (!string.IsNullOrEmpty(currentName))
            {
                DeserializeCasheData(ConvertToPath(currentName));
            }
            Local_Number_TextChanged(null, null);
        }



        public void InitSpeakerTextCashe()
        {
            string path = $"{System.Windows.Forms.Application.StartupPath}\\Cashe";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 序列化并保存当前朗读者文本
        /// </summary>
        public void SerializableCasheData(string path)
        {
            if (SpeakerTextData == null) return;



            string textJsonData = JsonConvert.SerializeObject(SpeakerTextData, Formatting.Indented);
            //var g =  JsonConvert.SerializeObject(SpeakerTextData);

            //string textJsonData = LitJson.JsonMapper.ToJson(SpeakerTextData);
            //FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            TextWriter sw = new StreamWriter(path, false, Encoding.UTF8);
            //JsonTextWriter jsonTextWriter = new JsonTextWriter(sw);
            sw.Write(textJsonData);
            sw.Flush();
            sw.Close();
            //fs.Close();

            #region  以内置二进制序列化进行的保存
            //BinaryFormatter formatter = new BinaryFormatter();
            //FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //formatter.Serialize(stream, SpeakerTextData);
            //stream.Close();
            #endregion

        }
        /// <summary>
        /// 读取保存的序列化文本
        /// </summary>
        public void DeserializeCasheData(string path)
        {
            if (!File.Exists(path))
            {
                SpeakerTextData = new SpeakerText[LoadModuleControl.CurrentRoleDataList.Count];
                for (int i = 0; i < SpeakerTextData.Length; i++)
                {
                    SpeakerTextData[i] = new SpeakerText();
                    SpeakerTextData[i].SpeakerTextData = LoadModuleControl.CurrentRoleDataList[i].log;
                }
            }
            else
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                TextReader sr = new StreamReader(fs, Encoding.UTF8);
                //jsonReader = new JsonTextReader(sr);
                SpeakerTextData = JsonConvert.DeserializeObject<SpeakerText[]>(sr.ReadToEnd());

                //SpeakerTextData = LitJson.JsonMapper.ToObject<string[]>(sr.ReadToEnd());
                sr.Close();
                fs.Close();

                if (SpeakerTextData.Length < LoadModuleControl.CurrentRoleDataList.Count)
                {
                    SpeakerText[] copyArray = new SpeakerText[LoadModuleControl.CurrentRoleDataList.Count];
                    SpeakerTextData.CopyTo(copyArray, 0);

                    for (int i = SpeakerTextData.Length; i < LoadModuleControl.CurrentRoleDataList.Count; i++)
                    {
                        copyArray[i] = new SpeakerText();
                        copyArray[i].SpeakerTextData = LoadModuleControl.CurrentRoleDataList[i].log;
                    }
                    SpeakerTextData = copyArray;
                }
            }

            #region 以内置二进制序列化进行的读取
            //if (!File.Exists(path))
            //{
            //    SpeakerTextData = new string[LoadModuleControl.CurrentRoleDataList.Count];
            //    for (int i = 0; i < SpeakerTextData.Length; i++)
            //    {
            //        SpeakerTextData[i] = LoadModuleControl.CurrentRoleDataList[i].log;
            //    }
            //}
            //else
            //{
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
            //    SpeakerTextData = (string[])formatter.Deserialize(stream);
            //    stream.Close();
            //    if(SpeakerTextData.Length < LoadModuleControl.CurrentRoleDataList.Count)
            //    {
            //        string []copyArray = new string[LoadModuleControl.CurrentRoleDataList.Count];
            //        SpeakerTextData.CopyTo(copyArray, 0);

            //        for(int i= SpeakerTextData.Length; i< LoadModuleControl.CurrentRoleDataList.Count;i++)
            //        {
            //            copyArray[i] = LoadModuleControl.CurrentRoleDataList[i].log;
            //        }
            //        SpeakerTextData = copyArray;
            //    }
            //}
            #endregion
        }


        //private void RateIuput_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //Regex re = new Regex("^[0-9]+$");
        //    //Regex re = new Regex(@"[^\d]*");
        //    TextBox s = e.Source as TextBox;
        //    s.Text = Regex.Replace(s.Text, @"[^\d]*", "");

        //    //if (re.IsMatch(s))
        //    //{
        //    //    e.Handled = true;
        //    //}
        //    //else
        //    //{
        //    //    e.Handled = false;
        //    //}
        //}
        /// <summary>
        /// 主要的语音合成启动 GetStart()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 启用语音生成控制台_Click(object sender, RoutedEventArgs e)
        {
            cmd = new CommandLine();
            cmd.OutputHandler += Cmd_OutputHandler;
            cmd.ErrorHandler += Cmd_ErrorHandler;
            cmd.Write($"\"{MoeGoePath}\" --escape");
            //cmd.Write($"\"{MoeGoePath}\"");
            cmd.Write(ModelPath);
            cmd.Write(ConfigPath);
            if (!string.IsNullOrEmpty(W2V2Path))
            {
                cmd.Write(W2V2Path);
            }


            启用语音生成控制台.IsEnabled = false;
            禁用语音生成控制台.IsEnabled = true;
            OpenEXE.IsEnabled = false;
            OpenModel.IsEnabled = false;
            OpenConfig.IsEnabled = false;


            生成指定语音.IsEnabled = true;
            生成所有语音.IsEnabled = true;
            AnalyzeSpeakersToBox();//AnalyzeSpeakers
        }
        private void 禁用语音生成控制台_Click(object sender, RoutedEventArgs e)
        {
            cmd.process.Dispose();
            cmd = null;

            启用语音生成控制台.IsEnabled = true;
            禁用语音生成控制台.IsEnabled = false;

            OpenEXE.IsEnabled = true;
            OpenModel.IsEnabled = true;
            OpenConfig.IsEnabled = true;

            AnalyzeRolesClear();
        }

        private void AnalyzeRolesClear()
        {
            speakerBox.Items.Clear();
        }
        private List<string> AnalyzeSpeakers()
        {

            string json = File.ReadAllText(ConfigPath);
            Match match = Regex.Match(json,
               "\"speakers\"\\s*:\\s*\\[((?:\\s*\"(?:(?:\\\\.)|[^\\\\\"])*\"\\s*,?\\s*)*)\\]");
            List<string> datas = new List<string>();
            if (match.Success)
            {
                MatchCollection matches = Regex.Matches(match.Groups[1].Value,
                    "\"((?:(?:\\\\.)|[^\\\\\"])*)\"");
                if (matches.Count > 0)
                {
                    for (int i = 0; i < matches.Count; i++)
                    {
                        datas.Add(Regex.Unescape(matches[i].Groups[1].Value));
                    }
                }
            }
            return datas;
        }
        private void AnalyzeSpeakersToBox()
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

        private bool AnalyzePath()
        {
            if (string.IsNullOrEmpty(MoeGoePath) || string.IsNullOrEmpty(ModelPath) || string.IsNullOrEmpty(ConfigPath))
            {
                启用语音生成控制台.IsEnabled = false;
                return false;
            }
            else
            {
                启用语音生成控制台.IsEnabled = true;
                return true;
            }
        }

        private void Cmd_OutputHandler(CommandLine sender, DataReceivedEventArgs e)
        {
            string value = e.Data;

        }
        public event Action OnGenerateComplete;

        private void Cmd_ErrorHandler(CommandLine sender, DataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Debug输出.Text = e.Data;
                //完成输出
                if ("Speaker ID: Path to save: Successfully saved!" == e.Data)
                {
                    GenerateComplete = true;
                    OnGenerateComplete?.Invoke();
                }
            }
            );
        }

        public void LoadConfig()
        {
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + MoeGoe配置文件))
                {
                    string[] Setting = File.ReadAllLines((System.Windows.Forms.Application.StartupPath + MoeGoe配置文件));
                    MoeGoePath = Setting[0];
                    ModelPath = Setting[1];
                    ConfigPath = Setting[2];
                    Pattern = int.Parse(Setting[3]);
                    RateSlider.Value = double.Parse(Setting[4]);
                    RateInput.Text = Setting[4];
                    CreateCount = int.Parse(Setting[5]);
                    DeviationInput.Text = Setting[6];
                    DeviationSlider.Value = double.Parse(Setting[6]);
                    W2V2Path = Setting[7];
                }
            }
            catch { }
            AnalyzePath();
        }
        public void SaveConfig()
        {
            string Conetents = $"{MoeGoePath}\n{ModelPath}\n{ConfigPath}\n{Pattern}\n{RateSlider.Value}\n{CreateCount}\n{DeviationSlider.Value}\n{W2V2Path}";
            File.WriteAllText(System.Windows.Forms.Application.StartupPath + MoeGoe配置文件, Conetents, Encoding.UTF8);
        }

        private void OnSaveButton(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            SerializableCasheData(SpeakerTextPath);
        }

        #region 生成单个语音
        private void 生成指定语音_Click(object sender, RoutedEventArgs e)
        {
            if (speakerBox.SelectedIndex == -1)
            {
                MessageBox.Show("还没有选择朗读者");
                return;
            }

            if (CreateCount == 1)
            {
                Directory.CreateDirectory(LoadModuleControl.VoiceSavePath);
                string savePath = System.IO.Path.Combine(LoadModuleControl.VoiceSavePath, LoadModuleControl.GetRoleDialogueID + ".wav");
                CreateMoeGoeTTS(savePath, SpeakerText.Text, speakerBox.SelectedIndex.ToString());
            }
            else
            {
                string saveFolder = System.IO.Path.Combine(LoadModuleControl.VoiceSavePath, LoadModuleControl.GetRoleDialogueID);
                CreateMultipleMoeGoeTTS(saveFolder);
            }
        }
        public void CreateMultipleMoeGoeTTS(string saveFolder)
        {
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            CurrentCreateCount = 0;
            MutipleGenerateComplete();
            OnGenerateComplete = MutipleGenerateComplete;
        }

        private void MutipleGenerateComplete()
        {
            string savePath = System.IO.Path.Combine(LoadModuleControl.VoiceSavePath, LoadModuleControl.GetRoleDialogueID, $"{LoadModuleControl.GetRoleDialogueID}{CurrentCreateCount}.wav");
            CreateMoeGoeTTS(savePath, SpeakerText.Text, speakerBox.SelectedIndex.ToString());
            CurrentCreateCount++;
            if (CurrentCreateCount == CreateCount)
            {
                生成指定语音.Content = "生成指定语音";
                OnGenerateComplete -= MutipleGenerateComplete;
                生成指定语音.IsEnabled = true;
            }
            else
            {
                生成指定语音.Content = $"{CurrentCreateCount}/{CreateCount}";
                生成指定语音.IsEnabled = false;
            }
        }
#endregion
        #region 生成该角色所有语音
        private Dictionary<string,int> ValidGroupID(List<string> speakers)
        {
            Dictionary<string,int> useTarget = new Dictionary<string,int>();
            for (int i = 0; i < LoadModuleControl.textDataList.Count; i++)
            {
                if (!useTarget.ContainsKey(LoadModuleControl.textDataList[i].GroupID))
                {

                    for(int j=0;j< speakers.Count;j++)
                    {
                        if(speakers[j] == LoadModuleControl.textDataList[i].GroupID)
                        {
                            useTarget.Add(speakers[j], j);
                        }
                    }
                }
            }
            return useTarget;
        }
        /// <summary>
        /// 可匹配到的角色
        /// </summary>
        Dictionary<string,int> useTarget;
        private void 生成所有语音_Click(object sender, RoutedEventArgs e)
        {
            if (启用语音生成控制台.IsEnabled)
            {
                MessageBox.Show("还没有启用控制台");
                return;
            }

            if (LoadModuleControl.currentGroup == LoadModuleControl.AllRole)
            {
                string matching = string.Empty;
                useTarget = ValidGroupID(AnalyzeSpeakers());
                foreach (var data in useTarget)
                {
                    matching += $"{data}\n";
                }
                MessageBox.Show("生成所有角色会根据GroupID进行匹配\n以下为文本检索到的角色\n" +
                matching, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                生成指定语音.IsEnabled = false;
                生成所有语音.IsEnabled = false;
                if(CreateCount==1)
                {
                    CreateAllSpeakerTextMoeGoeTTS();
                }
                else
                {
                    CreateAllSpeakerAllTextMoeGoeTTS();
                }
            }
            else
            {
                if (speakerBox.SelectedIndex == -1)
                {
                    MessageBox.Show("还没有选择朗读者");
                    return;
                }
                MessageBox.Show("生成所有语音需要较长时间，该项会覆盖已生成该角色语音，生成请不要关闭窗口", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                生成指定语音.IsEnabled = false;
                生成所有语音.IsEnabled = false;
                if (CreateCount == 1)
                {
                    Directory.CreateDirectory(LoadModuleControl.VoiceSavePath);
                    CreateSpeakerAllTextMoeGoeTTS();
                }
                else
                {
                    CreateSpeakerAllTextMultipleMoeGoeTTS();
                }
            }
        }
        private int AllIndex;
        #region 对照角色id和朗读者创建所有的语音
        public void CreateAllSpeakerTextMoeGoeTTS()
        {
            AllIndex = 0;
            CreateAllSpeakerTextMoeGoeTTSComplete();
            OnGenerateComplete = CreateAllSpeakerTextMoeGoeTTSComplete;
        }
        async void CreateAllSpeakerTextMoeGoeTTSComplete()
        {
            BaseTextData textData = LoadModuleControl.CurrentRoleDataList[AllIndex];
            Directory.CreateDirectory(LoadModuleControl.VoiceSavePath);
            string saveID = textData.SaveID;
            string savePath = System.IO.Path.Combine(LoadModuleControl.VoiceSavePath, $"{saveID}.wav");
            if ((bool)自动翻译成日文Toggle.IsChecked)
            {
                SpeakerTextData[AllIndex].SpeakerTextData = baiduTranslation.GetTranslation(SpeakerTextData[AllIndex].SpeakerTextData);
                await Task.Delay(200);//自动翻译需要等待降低翻译频率
            }
            int index;
            if (useTarget.TryGetValue(textData.GroupID, out index))
            {
                CreateMoeGoeTTS(savePath, SpeakerTextData[AllIndex].SpeakerTextData, index.ToString());
                AllIndex++;
                生成所有语音.Content = saveID;
                if (AllIndex == SpeakerTextData.Length)
                {
                    生成所有语音.Content = "生成所有语音";
                    OnGenerateComplete -= AllTextMultipleGenerateComplete;
                    生成所有语音.IsEnabled = true;
                    return;
                }
            }
            else
            {
                AllIndex++;
                CreateAllSpeakerTextMoeGoeTTSComplete();
            }
        }

        /// <summary>
        /// 基于文本中所有角色创建所有的MoeGoeTTS
        /// </summary>
        public void CreateAllSpeakerAllTextMoeGoeTTS()
        {
            CurrentCreateCount = 0;
            AllIndex = 0;
            MoeGoeTextToSpeechControlAllMultipleTextToSpeakerGenerateComplete();
            OnGenerateComplete = MoeGoeTextToSpeechControlAllMultipleTextToSpeakerGenerateComplete;
        }
        public async void MoeGoeTextToSpeechControlAllMultipleTextToSpeakerGenerateComplete()
        {
            BaseTextData textData = LoadModuleControl.CurrentRoleDataList[AllIndex];
            string saveID = textData.SaveID;
            string saveFolder = System.IO.Path.Combine(LoadModuleControl.VoiceSavePath, saveID);
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            string savePath = System.IO.Path.Combine(LoadModuleControl.VoiceSavePath, saveID, $"{saveID}{CurrentCreateCount}.wav");
            if ((bool)自动翻译成日文Toggle.IsChecked && CurrentCreateCount == 0)//需要改翻译位置
            {
                SpeakerTextData[AllIndex].SpeakerTextData = baiduTranslation.GetTranslation(SpeakerTextData[AllIndex].SpeakerTextData);
                await Task.Delay(200);//自动翻译需要等待降低翻译频率
            }
            int index;
            if (useTarget.TryGetValue(textData.GroupID,out index))
            {
                CreateMoeGoeTTS(savePath, SpeakerTextData[AllIndex].SpeakerTextData, index.ToString());
                if (CurrentCreateCount - 1 == CreateCount)
                {
                    AllIndex++;
                    CurrentCreateCount = 0;
                    生成所有语音.Content = $"{CurrentCreateCount}/{CreateCount}";
                }
                else
                {
                    CurrentCreateCount++;
                    生成所有语音.Content = $"{CurrentCreateCount}/{CreateCount}";
                }
                if (AllIndex == SpeakerTextData.Length)
                {
                    生成所有语音.Content = "生成所有语音";
                    OnGenerateComplete -= AllTextMultipleGenerateComplete;
                    生成所有语音.IsEnabled = true;
                    return;
                }
            }
           else
            {
                AllIndex++;
                MoeGoeTextToSpeechControlAllMultipleTextToSpeakerGenerateComplete();
            }
            //
        }

        public void CreateSpeakerAllTextMoeGoeTTS()
        {
            AllIndex = 0;
            CreateSpeakerAllOnGenerateComplete();
            OnGenerateComplete = CreateSpeakerAllOnGenerateComplete;

        }
        private async void CreateSpeakerAllOnGenerateComplete()
        {
            string saveID = LoadModuleControl.CurrentRoleDataList[AllIndex].SaveID;
            string savePath = System.IO.Path.Combine(LoadModuleControl.VoiceSavePath, $"{saveID}.wav");
            //这里可以加是否要翻译成日文

            if ((bool)自动翻译成日文Toggle.IsChecked && CurrentCreateCount == 0)//需要改翻译位置
            {
                SpeakerTextData[AllIndex].SpeakerTextData = baiduTranslation.GetTranslation(SpeakerTextData[AllIndex].SpeakerTextData);
                await Task.Delay(200);//自动翻译需要等待降低翻译频率
            }
            CreateMoeGoeTTS(savePath, SpeakerTextData[AllIndex].SpeakerTextData, speakerBox.SelectedIndex.ToString());
            AllIndex++;
            生成所有语音.Content = saveID;
            if (AllIndex == SpeakerTextData.Length)
            {
                生成所有语音.Content = "生成所有语音";
                OnGenerateComplete -= AllTextMultipleGenerateComplete;
                生成所有语音.IsEnabled = true;
            }
        }
        /// <summary>
        /// 创建选中角色的MoeTTS
        /// </summary>
        public void CreateSpeakerAllTextMultipleMoeGoeTTS()
        {
            CurrentCreateCount = 0;
            AllIndex = 0;
            AllTextMultipleGenerateComplete();
            OnGenerateComplete = AllTextMultipleGenerateComplete;
        }
        private async void AllTextMultipleGenerateComplete()
        {
            string saveID = LoadModuleControl.CurrentRoleDataList[AllIndex].SaveID;
            string saveFolder = System.IO.Path.Combine(LoadModuleControl.VoiceSavePath, saveID);
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            string savePath = System.IO.Path.Combine(LoadModuleControl.VoiceSavePath, saveID, $"{saveID}{CurrentCreateCount}.wav");
            //这里可以加是否要翻译成日文

            if ((bool)自动翻译成日文Toggle.IsChecked && CurrentCreateCount == 0)//需要改翻译位置
            {
                SpeakerTextData[AllIndex].SpeakerTextData = baiduTranslation.GetTranslation(SpeakerTextData[AllIndex].SpeakerTextData);
                await Task.Delay(200);//自动翻译需要等待降低翻译频率
            }
            CreateMoeGoeTTS(savePath, SpeakerTextData[AllIndex].SpeakerTextData, speakerBox.SelectedIndex.ToString());


            if (CurrentCreateCount - 1 == CreateCount)
            {
                AllIndex++;
                CurrentCreateCount = 0;
                生成所有语音.Content = $"{CurrentCreateCount}/{CreateCount}";
            }
            else
            {
                CurrentCreateCount++;
                生成所有语音.Content = $"{CurrentCreateCount}/{CreateCount}";
            }
            if (AllIndex == SpeakerTextData.Length)
            {
                生成所有语音.Content = "生成所有语音";
                OnGenerateComplete -= AllTextMultipleGenerateComplete;
                生成所有语音.IsEnabled = true;
            }
        }
        #endregion


        public void CreateMoeGoeTTS(string savePath, string text, string speakerNumber)
        {
            GenerateComplete = false;
            switch (Pattern)
            {
                case 0:
                    TTS($"{RateLength}[ZH]{text}[ZH]", speakerNumber);
                    break;
                case 1:
                    TTS($"{RateLength}[JA]{text}[JA]", speakerNumber);
                    break;
                case 2:
                    TTS($"{RateLength} {AnalysisLanguageText(text)}", speakerNumber);
                    break;
                case 3:
                    TTS($"{RateLength}{text}", speakerNumber);
                    break;
            }
            if (!string.IsNullOrEmpty(W2V2Path))//情感音频
            {
                cmd.Write(SpeakerTextData[LoadModuleControl.Local_Ptr].EmotionPath);
            }

            cmd.Write(savePath);
            cmd.Write("y");//要有输出完成回调

        }

        private string AnalysisLanguageText(string text)
        {
            string[] textFragment = text.Split(new string[] { "[", "]" }, StringSplitOptions.None);

            StringBuilder stringBuilderSplicing = new StringBuilder();


            for (int i = 0; i < textFragment.Length; i++)
            {
                if (i % 2 == 0)
                {
                    if (!string.IsNullOrEmpty(textFragment[i]))
                    {
                        stringBuilderSplicing.AppendLine("[ZH]");
                        stringBuilderSplicing.Append(textFragment[i]);
                        stringBuilderSplicing.AppendLine("[ZH]");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(textFragment[i]))
                    {
                        stringBuilderSplicing.AppendLine("[JA]");
                        stringBuilderSplicing.Append(textFragment[i]);
                        stringBuilderSplicing.AppendLine("[JA]");
                    }
                }
            }
            return stringBuilderSplicing.ToString();
            //Regex regex = new Regex(".?(?=[\\[\\]])|(.+)");


            //MatchCollection matchCollection =  regex.Matches(text);

            //foreach (Match mat in matchCollection)
            //{
            //    var g =  mat.Value;
            //}
            //regex.Matches


        }
        private void TTS(string text,string speaker)
        {
            cmd.Write("t");
            cmd.Write(Regex.Replace(text, @"\r?\n", " "));
            //cmd.Write("0");//说话人
            cmd.Write(speaker);//说话人
        }
        private void TTS(string text)
        {
            cmd.Write("t");
            cmd.Write(Regex.Replace(text, @"\r?\n", " "));
            //cmd.Write("0");//说话人
            cmd.Write(speakerBox.SelectedIndex.ToString());//说话人
        }

        #endregion


        private void OpenMoeGoe_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "可执行文件|*.exe"
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MoeGoePath = openFileDialog.FileName;
                AnalyzePath();
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
                AnalyzePath();
            }
            ofd.Dispose();
        }

        private void OpenConfig_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "配置文件|*.json"
            };
            if ((bool)ofd.ShowDialog())
            {
                ConfigPath = ofd.FileName;
                AnalyzePath();
            }
        }

        private void OpenEmotionalData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "数据文件|*.npy|音频文件|*.wav;*.mp3;*.ogg;*.opus"
            };
            if ((bool)ofd.ShowDialog())
            {
                emotionData.Text = SpeakerTextData[LoadModuleControl.Local_Ptr].EmotionPath = ofd.FileName;

            }
            ///获取文件夹位置
            //FolderPicker folderPicker = new FolderPicker();
            //if ((bool)folderPicker.ShowDialog())
            //{
            //    EmotionDataFolderPath = folderPicker.InputPath;
            //}
        }

        private void OpenEmotionalModel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "模型文件|model.onnx"
            };
            if ((bool)ofd.ShowDialog())
            {
                W2V2Path = ofd.FileName;
            }
        }

    }
}
