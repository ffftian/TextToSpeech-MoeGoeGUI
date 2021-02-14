using NAudioWpfDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeech
{
    /// <summary>
    /// 可视化画线中介，这应该是什么设计模式但尼玛的绕爆了好吗？
    /// </summary>
    class LineWaveFormVisualization: IVisualizationPlugin
    {
        private readonly LineWaveFormControl polylineWaveFormControl = new LineWaveFormControl();

        public string Name => "Polyline WaveForm Visualization";

        public object Content => polylineWaveFormControl;

        public void OnMaxCalculated(float min, float max)
        {
            polylineWaveFormControl.AddValue(max, min);
        }

        public void OnSamples(float[] samples, int length)
        {
            polylineWaveFormControl.AddValue(samples, length);
        }
    }
}
