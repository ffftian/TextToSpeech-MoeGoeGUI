using NAudio.Dsp;
using NAudio.Wave;
using NAudioWpfDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace TextToSpeech
{
    /// <summary>
    /// LineWaveFormControl.xaml 的交互逻辑
    /// </summary>
    public partial class LineWaveFormControl : UserControl, IWaveFormRenderer
    {
        int renderPosition;
        double yTranslate = 40;
        double yScale = 40;
        int blankZone = 10;

        readonly Polyline topLine = new Polyline();
        readonly Polyline bottomLine = new Polyline();

        readonly Polyline middleLine = new Polyline();


        public LineWaveFormControl()
        {
            SizeChanged += OnSizeChanged;
            InitializeComponent();
            topLine.Stroke = Foreground;
            bottomLine.Stroke = Foreground;
            middleLine.Stroke = Foreground;

            topLine.StrokeThickness = 1;
            bottomLine.StrokeThickness = 1;
            middleLine.StrokeThickness = 1;

            mainCanvas.Children.Add(topLine);
            mainCanvas.Children.Add(bottomLine);
            mainCanvas.Children.Add(middleLine);
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //当我们要垂直缩放时，我们将删除所有内容
            renderPosition = 0;
            ClearAllPoints();

            yTranslate = ActualHeight / 2;
            yScale = ActualHeight / 2;
        }
        private void ClearAllPoints()
        {
            topLine.Points.Clear();
            bottomLine.Points.Clear();
        }
        /// <summary>
        /// 创建一条弧形数组
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="Legth"></param>
        public void AddValue(float[] samples, int Legth)
        {

            DrawWaveformPoint(samples, Legth);


        }

        /// <summary>
        /// 根据采样画波形图，因为是每毫秒采样一次，每秒会画10次。
        /// 得益于他只支持List，只能这样画
        /// </summary>
        public void DrawWaveformPoint(float[] samples, int Legth)
        {
            double yForwardLegth = ActualWidth / Legth;//每个线段画好后前进的距离 
            if (middleLine.Points.Count < samples.Length)
            {
                for (int ptr = 0; ptr < Legth; ptr++)
                {
                    middleLine.Points.Add(new Point(yForwardLegth * ptr, SampleToYPosition(0)));//初始化音频
                }
            }
            else
            {
                for (int ptr = 0; ptr < Legth; ptr++)
                {
                    middleLine.Points[ptr] = new Point(yForwardLegth * ptr, SampleToYPosition(samples[ptr]));//不是不想优化，跟堆栈引用有关。
                }
            }
        }



        public void AddValue(float maxValue, float minValue)
        {
            int pixelWidth = (int)ActualWidth;
            if (pixelWidth > 0)
            {
                CreatePoint(maxValue, minValue);

                if (renderPosition > ActualWidth)
                {
                    renderPosition = 0;
                }
                int erasePosition = (renderPosition + blankZone) % pixelWidth;
                if (erasePosition < topLine.Points.Count)
                {
                    double yPos = SampleToYPosition(0);
                    topLine.Points[erasePosition] = new Point(erasePosition, yPos);
                    bottomLine.Points[erasePosition] = new Point(erasePosition, yPos);
                }
            }
        }
        private double SampleToYPosition(double value)
        {
            return yTranslate + value * yScale;
        }

        private double SampleToYPosition(float value)
        {
            return yTranslate + value * yScale;
        }
        /// <summary>
        /// 创建单一 单一
        /// </summary>
        /// <param name="topValue"></param>
        private void CreatePointOnly(float topValue)
        {

            double topLinePos = SampleToYPosition(topValue);
            Console.WriteLine($"{renderPosition}/{topLinePos}");
            if (renderPosition >= topLine.Points.Count)
            {
                middleLine.Points.Add(new Point(renderPosition, topLinePos));//X轴不变自动推进，Y轴变
            }
            else
            {
                middleLine.Points[renderPosition] = new Point(renderPosition, topLinePos);
            }
            renderPosition++;
        }


        private void CreatePoint(float topValue, float bottomValue)
        {
            double topLinePos = SampleToYPosition(topValue);
            double bottomLinePos = SampleToYPosition(bottomValue);
            if (renderPosition >= topLine.Points.Count)
            {
                topLine.Points.Add(new Point(renderPosition, topLinePos));
                bottomLine.Points.Add(new Point(renderPosition, bottomLinePos));
            }
            else
            {
                topLine.Points[renderPosition] = new Point(renderPosition, topLinePos);
                bottomLine.Points[renderPosition] = new Point(renderPosition, bottomLinePos);
            }
            renderPosition++;
        }

        /// <summary>
        /// Clears the waveform and repositions on the left
        /// </summary>
        public void Reset()
        {
            renderPosition = 0;
            ClearAllPoints();
        }
    }
}

