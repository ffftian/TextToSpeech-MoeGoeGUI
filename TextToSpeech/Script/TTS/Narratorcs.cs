using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;


public class Narratorcs
{
    /// <summary>
    /// 提供对语音合成引擎访问
    /// </summary>
    private SpeechSynthesizer syn;


    /// <summary>
    /// 音量设置
    /// </summary>
    public int Volumn
    {
        get { return syn.Volume; }
        set { syn.Volume = value; }
    }

    /// <summary>
    /// 创建一个朗读者实例
    /// </summary>
    public void Narrator()
    {
        syn = new SpeechSynthesizer();
    }

    /// <summary>
    /// 朗读一段文本
    /// </summary>
    /// <param name="text">被朗读文本</param>
    public void Narrate(string text)
    {
        syn.SpeakAsync(text);
    }



    /// <summary>
    /// 选择配音
    /// </summary>
    /// <param name="role"></param>
    public void SelectVoice(string role)
    {
        syn.SelectVoice(role);
    }

    public void TestVoice()
    {
        //InstalledVoice voice

        foreach (InstalledVoice voice in syn.GetInstalledVoices())
        {
            VoiceInfo info = voice.VoiceInfo;
            string AudioFormats = "";
            foreach (SpeechAudioFormatInfo fmt in info.SupportedAudioFormats)
            {
                AudioFormats += String.Format("{0}\n",
                fmt.EncodingFormat.ToString());
            }

        }


        /// <summary>
        /// 获取配音声音列表
        /// </summary>
        /// <returns></returns>
        public string[] GetVoice()
    {
        string[] voice = new string[syn.GetInstalledVoices().Count];
        for (int i = 0; i < voice.Length; i++)
        {
            voice[i] = syn.GetInstalledVoices()[i].VoiceInfo.Name;
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
        syn.SetOutputToWaveFile(path);
        syn.Speak(text);
        syn.SetOutputToNull();
    }
}




