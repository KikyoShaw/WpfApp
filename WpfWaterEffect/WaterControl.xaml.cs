using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfWaterEffect
{
    /// <summary>
    /// WaterControl.xaml 的交互逻辑
    /// </summary>
    public partial class WaterControl : UserControl
    {
        
        public WaterControl()
        {
            InitializeComponent();
        }

        //进度
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(WaterControl),
            new PropertyMetadata(new PropertyChangedCallback(OnValuePropertyChange)));
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValuePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WaterControl obj = ((WaterControl)d as WaterControl);
            if (obj != null)
                obj.DoValueUrlChange();
        }

        private StreamGeometry _streamGeometry;

        private void DoValueUrlChange()
        {
            this.Canvas.Children.Clear();

            if (Value >= 100)
            {
                this.Canvas.Background = Brushes.DarkGreen;
                return;
            }
            else if (Value <= 0)
            {
                this.Canvas.Background = Brushes.LightGreen;
                return;
            }

            _streamGeometry = GetSinGeometry(this.Width, -5, 1 / 30.0, -this.Width + this.Width * Value / 100, this.Height - this.Height * Value / 100);
            GeometryGroup group = new GeometryGroup();
            group.Children.Add(_streamGeometry);

            Path myPath = new Path
            {
                Fill = Brushes.DarkGreen,
                Data = group
            };

            this.Canvas.Children.Add(myPath);
        }

        /// <summary>
        /// 得到正弦曲线
        /// </summary>
        /// <param name="waveWidth">水纹宽度</param>
        /// <param name="waveA">水纹振幅</param>
        /// <param name="waveW">水纹周期</param>
        /// <param name="offsetX">位移</param>
        /// <param name="currentK">当前波浪高度</param>
        /// <returns></returns>
        private StreamGeometry GetSinGeometry(double waveWidth, double waveA, double waveW, double offsetX, double currentK)
        {
            StreamGeometry g = new StreamGeometry();
            using StreamGeometryContext ctx = g.Open();
            ctx.BeginFigure(new Point(0, waveWidth), true, true);
            for (int x = 0; x < waveWidth; x += 1)
            {
                double y = waveA * Math.Sin(x * waveW + offsetX) + currentK;
                ctx.LineTo(new Point(x, y), true, true);
            }
            ctx.LineTo(new Point(waveWidth, waveWidth), true, true);
            return g;
        }
    }
}
