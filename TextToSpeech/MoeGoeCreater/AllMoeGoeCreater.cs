using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextToSpeech;

public class AllTextMoeGoeCreater
{
    //private int CreateCount;
    //private string voiceSavePath;
    //private List<BaseTextData> textDatas;

    //private int Pattern;
    //private string RateLength;


    //private int currentIndex;
    //private int currentCreate;
    //public event Action AllCreaterComplete;
    //public CommandLine cmd;


    //public AllTextMoeGoeCreater(CommandLine cmd,int Pattern, string RateLength, string voiceSavePath, int CreateCount,string playName, string[] textDatas, Action onGenerateComplete)
    //{
    //    this.CreateCount = CreateCount;

    //    onGenerateComplete += GenerateCompleteCallBack;
    //    this.voiceSavePath = voiceSavePath;
    //    this.textDatas = textDatas;
    //    this.RateLength = RateLength;
    //    this.Pattern = Pattern;
    //    this.RateLength = RateLength;
    //}

    //private void GenerateCompleteCallBack()
    //{
    //    BaseTextData data = textDatas[currentIndex];
    //    string savePath = System.IO.Path.Combine(voiceSavePath, data.SaveID, $"{data.SaveID}{currentCreate}.wav");

    //    if (CreateCount == currentCreate)
    //    {
    //        currentIndex++;
    //    }
    //    else
    //    {
    //        currentCreate = 0;
    //    }
    //    if (currentIndex == textDatas.Count)
    //    {
    //        AllCreaterComplete?.Invoke();
    //    }
    //}

    //public void CreateMoeGoeTTS(string name,string savePath, string text)
    //{
    //    switch (Pattern)
    //    {
    //        case 0:
    //            TTS($"{RateLength}[ZH]{text}[ZH]");
    //            //TTS("[ZH]" + LoadModuleControl.GetRoleDialogue + "[ZH]");
    //            break;
    //        case 1:
    //            TTS($"{RateLength}[JA]{text}[JA]");
    //            //TTS("[JA]" + LoadModuleControl.GetRoleDialogue + "[JA]");
    //            break;
    //        case 2:
    //            TTS($"{RateLength} {AnalysisLanguageText(text)}");
    //            break;
    //        case 3:
    //            TTS($"{RateLength}{SpeakerText.Text}");
    //            //TTS(LoadModuleControl.GetRoleDialogue);
    //            break;
    //    }
    //    cmd.Write(savePath);
    //    cmd.Write("y");//要有输出完成回调
    //}

    //private void TTS(string name,string text)
    //{
    //    cmd.Write("t");
    //    cmd.Write(Regex.Replace(text, @"\r?\n", " "));//当初真打问号了。why？
    //    cmd.Write(name);//说话人
    //}

}

