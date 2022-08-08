using NAudio.Wave;
using NAudioWpfDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeech
{
    
    public partial class LoadModuleModel
    {
        private IVisualizationPlugin selectedVisualization;//选中的可视化插件，这个类

        public LoadModuleModel()
        {
            selectedVisualization = new LineWaveFormVisualization();
        }


        public void DrawVisualWaveform(object sender, WaveInEventFloat e)
        {
            selectedVisualization.OnSamples(e.samples, e.samplesLength);
        }

        //public void Test()
        //{
        //    Random k = new Random();

        //    selectedVisualization.OnMaxCalculated(k.Next(0,1), k.Next(0,1));
        //}




        public object Visualization
        {
            get
            {
                return this.selectedVisualization.Content;
            }
        }

    }
}
