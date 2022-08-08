using System;
using System.Windows;
using NAudio.Wave;


//win32和win64程序语音姬不互通，需要切程序版本才可以调32位下的语音和64位的语音的语音姬。
//语音姬的相关设置位于注册表内。
public class WaveInEventFloat : EventArgs
{
    private byte[] bit = new byte[4];
    public float[] samples;
    public int samplesLength;
    public int channels;

    public WaveInEventFloat(int channels) { this.channels = channels; }
    public WaveInEventFloat(in byte[] Buffer, in int RecordLength, int channels)//这个回头想尝试自己写C#指针看看内存地址是否相等。
    {
        this.channels = channels;
        ReAnalysis(Buffer, RecordLength);
    }
    public void ReAnalysis(in byte[] Buffer, in int RecordLength)
    {
        this.samplesLength = RecordLength / 2;
        this.samples = new float[samplesLength];
        for (int n = 0, t = 0; n < RecordLength; n += 2, t++)
        {
            samples[t] = BitConverter.ToInt16(Buffer, n) / 32768f;
        }
        #region 留作教训，wav格式并不是标准4位float存一格，而是2位float存一格
        //for (int i = 0, t = 0; i < RecordLength; i += 4, t++)
        //{


        //    bit[0] = Buffer[i];
        //    bit[1] = Buffer[i + 1];
        //    bit[2] = Buffer[i + 2];
        //    bit[3] = Buffer[i + 3];
        //    samples[t] = BitConverter.ToSingle(bit,0);
        //}
        #endregion
    }
    public void ReAnalysis(WaveInEventArgs e)
    {
        //this.samplesLength = RecordLength / 2;
        var buffer = new WaveBuffer(e.Buffer);
        //this.samples = new float[samplesLength];
        samples = buffer.FloatBuffer;
        //for (int index = 0; index < e.BytesRecorded/4;index++)
        //{
        //    samples[index] =  buffer.FloatBuffer[index];
        //}
    }

}
/// <summary>
/// 录制声卡输出
/// </summary>
public class NAudioRecordSoundcard
{
    private string filePath = string.Empty;
    public WasapiLoopbackCapture wasapiLoopbackCapture = null;
    public WaveFileWriter waveFile = null;
    public WaveInEventFloat cacheFloat;
    public event EventHandler<WaveInEventFloat> OnWaveRecording;
    
    /// <summary>
    /// 录音结束后保存的文件路径
    /// </summary>
    /// <param name="fileName">保存wav文件的路径名</param>
    public void SetFilePath(string fileName, string name)
    {
        this.filePath = fileName + "//" + name + ".wav";
    }
    public void StartRec()
    {
        wasapiLoopbackCapture = new WasapiLoopbackCapture();
        cacheFloat = new WaveInEventFloat(1);
        //wasapiLoopbackCapture.WaveFormat;
        waveFile = new WaveFileWriter(filePath, wasapiLoopbackCapture.WaveFormat);
        wasapiLoopbackCapture.StartRecording();
        wasapiLoopbackCapture.DataAvailable += WasapiLoopbackCapture_DataAvailable;
        wasapiLoopbackCapture.RecordingStopped += WasapiLoopbackCapture_RecordingStopped;
    }

    private void WasapiLoopbackCapture_DataAvailable(object sender, WaveInEventArgs e)
    {
        if (waveFile != null)
        {
            waveFile.Write(e.Buffer, 0, e.BytesRecorded);
            //waveFile.Flush();
            //cacheFloat.ReAnalysis(e);
            //OnWaveRecording.Invoke(this, cacheFloat);
        }
    }

    public void StopRec()
    {
        if (wasapiLoopbackCapture != null)
        {
            wasapiLoopbackCapture.StopRecording();
        }
    }
    private void WasapiLoopbackCapture_RecordingStopped(object sender, StoppedEventArgs e)
    {
        wasapiLoopbackCapture.Dispose();
        wasapiLoopbackCapture = null;
        waveFile.Dispose();
        waveFile = null;
    }
}
/// <summary>
/// 录制麦克风输入
/// </summary>
public class NAudioRecorder
{
    public WaveIn waveSource = null;
    public WaveFileWriter waveFile = null;
    private string filePath = string.Empty;
    public WaveInEventFloat cacheFloat;
    public event EventHandler<WaveInEventFloat> OnWaveRecording;
    /// <summary>
    /// 录音结束后保存的文件路径
    /// </summary>
    /// <param name="fileName">保存wav文件的路径名</param>
    public void SetFilePath(string fileName, string name)
    {
        this.filePath = fileName + "//" + name + ".wav";
    }
    /// <summary>
    /// 开始录音
    /// </summary>
    public void StartRec()
    {
        waveSource = new WaveIn();
        waveSource.WaveFormat = new WaveFormat(24000, 16, 1); // 24000rate,16bit,通道数
        cacheFloat = new WaveInEventFloat(1);
        //waveSource.WaveFormat = new WaveFormat(8000, 16, 2); // 24000rate,16bit,通道数
        waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);//撰写回调
        waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);//停止回调  
        waveFile = new WaveFileWriter(filePath, waveSource.WaveFormat);
        int length = waveSource.DeviceNumber;
        if (length != 0)
        {
            waveSource.StartRecording();
        }
        else
        {
            MessageBox.Show("未检测到音频设备", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    /// <summary>
    /// 停止录音
    /// </summary>
    public void StopRec()
    {

        if (waveSource != null)
        {
            waveSource.StopRecording();
        }
    }

    /// <summary>
    /// 开始录音回调函数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
    {
        if (waveFile != null)//数组的值是设定采样率，4个byte存一个float
        {
            waveFile.Write(e.Buffer, 0, e.BytesRecorded);
            waveFile.Flush();
            cacheFloat.ReAnalysis(e.Buffer, e.BytesRecorded);
            OnWaveRecording.Invoke(this, cacheFloat);
        }
    }

    /// <summary>
    /// 录音结束回调函数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
    {
        waveSource.Dispose();
        waveSource = null;
        waveFile.Dispose();
        waveFile = null;
    }
}
