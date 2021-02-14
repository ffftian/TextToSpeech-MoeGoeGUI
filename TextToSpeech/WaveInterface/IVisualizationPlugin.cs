using System;
using System.Linq;
using NAudio.Dsp;

namespace NAudioWpfDemo
{
    /// <summary>
    /// 可视化插件接口
    /// </summary>
    interface IVisualizationPlugin
    {
        string Name { get; }
        object Content { get; }
        void OnMaxCalculated(float min, float max);
        //void OnBufferGeting(float[] buffer, int bytes);
        void OnSamples(float[] samples, int length);

    }
}
