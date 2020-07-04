using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;


public class Narrator
{
    /// <summary>
    /// 提供对语音合成引擎访问
    /// </summary>
    public SpeechSynthesizer synth;







    /// <summary>
    /// 音量设置
    /// </summary>
    public int Volumn
    {
        get { return synth.Volume; }
        set { synth.Volume = value; }
    }

    /// <summary>
    /// 创建一个朗读者实例
    /// </summary>
    public  Narrator()
    {
        synth = new SpeechSynthesizer();
    }

    /// <summary>
    /// 朗读一段文本
    /// </summary>
    /// <param name="text">被朗读文本</param>
    public void Narrate(string text)
    {
        synth.SpeakAsync(text);
    }


    public List<string> GetInstalledVoicesName()
    {
        var g = from ss in synth.GetInstalledVoices() select ss.VoiceInfo.Name;


        return g.ToList();
    }


    /// <summary>
    /// 选择配音
    /// </summary>
    /// <param name="role"></param>
    public void SelectVoice(string role)
    {
        synth.SelectVoice(role);
    }

        /// <summary>
        /// 获取配音声音列表
        /// </summary>
        /// <returns></returns>
        public string[] GetVoice()
    {
        string[] voice = new string[synth.GetInstalledVoices().Count];
        for (int i = 0; i < voice.Length; i++)
        {
            voice[i] = synth.GetInstalledVoices()[i].VoiceInfo.Name;
        }
        return voice;
    }

    /// <summary>
    /// 生成声音并导出到文件
    /// </summary>
    /// <param name="path">文件名地址</param>
    /// <param name="text">被朗读文本</param>
    public void ExportToWave(string path, string text)
    {
        synth.SetOutputToWaveFile(path);
        synth.Speak(text);
        synth.SetOutputToNull();
    }

    public void ExportToWave(string path, string text,string DialogName)
    {
        synth.SetOutputToWaveFile(path);
        

        synth.Speak(text);
        synth.SetOutputToNull();
    }



}




