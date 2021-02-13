using System;
using NAudio.Wave;

//win32和win64程序语音姬不互通，需要切程序版本才可以调32位下的语音和64位的语音的语音姬。
//语音姬的相关设置位于注册表内。
public class NAudioRecorder
{
    public WaveIn waveSource = null;
    public WaveFileWriter waveFile = null;
    private string filePath = string.Empty;

    //音频增益搞不了,只能播的时候让unity放大或者想想看其他方法处理了。

    /// <summary>
    /// 开始录音
    /// </summary>
    public void StartRec()
    {
        waveSource = new WaveIn();
        waveSource.WaveFormat = new WaveFormat(22050, 16, 1); // 24000rate,16bit,通道数
        waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);//撰写回调
        waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);//停止回调  
        waveFile = new WaveFileWriter(filePath, waveSource.WaveFormat);
        waveSource.StartRecording();
    }
    /// <summary>
    /// 停止录音
    /// </summary>
    public void StopRec()
    {
        waveSource.StopRecording();

        // Close Wave(Not needed under synchronous situation)
        if (waveSource != null)
        {
            waveSource.Dispose();
            waveSource = null;
        }

        if (waveFile != null)
        {
            waveFile.Dispose();
            waveFile = null;
        }
    }

    /// <summary>
    /// 录音结束后保存的文件路径
    /// </summary>
    /// <param name="fileName">保存wav文件的路径名</param>
    public void SetFilePath(string fileName, string name)
    {
        this.filePath = fileName + "//" + name + ".wav";
    }

    /// <summary>
    /// 开始录音回调函数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
    {
        if (waveFile != null)
        {
            waveFile.Write(e.Buffer, 0, e.BytesRecorded);
            waveFile.Flush();
        }
    }

    /// <summary>
    /// 录音结束回调函数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
    {
        if (waveSource != null)
        {
            waveSource.Dispose();
            waveSource = null;
        }

        if (waveFile != null)
        {
            waveFile.Dispose();
            waveFile = null;
        }
    }
}
