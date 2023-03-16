using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfSongLrc
{
    public class SongLrc : TextBlock
    {
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan),
                typeof(SongLrc), new PropertyMetadata(TimeSpan.FromSeconds(1)));

        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public static readonly DependencyProperty StartDurationProperty =
            DependencyProperty.Register("StartDuration", typeof(TimeSpan), typeof(SongLrc),
                new PropertyMetadata(TimeSpan.FromSeconds(1)));

        public TimeSpan StartDuration
        {
            get => (TimeSpan)GetValue(StartDurationProperty);
            set => SetValue(StartDurationProperty, value);
        }

        public SongLrc()
        {
            NameScope.SetNameScope(this, new NameScope());
            var brush = new LinearGradientBrush
            {
                EndPoint = new Point(1, 0.5),
                StartPoint = new Point(0, 0.5)
            };
            var s1 = new GradientStop(Colors.Red, 0);
            RegisterName("step1", s1);
            brush.GradientStops.Add(s1);
            var s2 = new GradientStop(Colors.Red, 1);
            RegisterName("step2", s2);
            brush.GradientStops.Add(s2);
            var s3 = new GradientStop(Colors.White, 1);
            RegisterName("step3", s3);
            brush.GradientStops.Add(s3);
            Foreground = brush;
            Loaded += (s, e) => { StartAnimate(); };
        }

        private void StartAnimate()
        {
            try
            {
                var storyBoard = new Storyboard();
                //当前选中歌词渲染跑马灯特效
                var a1 = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = Duration,
                    BeginTime = StartDuration
                };
                Storyboard.SetTargetName(a1, "step2");
                Storyboard.SetTargetProperty(a1, new PropertyPath(GradientStop.OffsetProperty));
                storyBoard.Children.Add(a1);

                //当前选中歌词底色
                var a2 = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = Duration,
                    BeginTime = StartDuration
                };
                Storyboard.SetTargetName(a2, "step3");
                Storyboard.SetTargetProperty(a2, new PropertyPath(GradientStop.OffsetProperty));
                storyBoard.Children.Add(a2);
                storyBoard.Begin(this);
            }
            catch /*(Exception ex)*/
            {
                //Console.WriteLine(e);
                //throw;
            }
        }
    }
}
