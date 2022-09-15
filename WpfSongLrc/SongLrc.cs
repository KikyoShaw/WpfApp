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
            var gradientBrush = new LinearGradientBrush
            {
                EndPoint = new Point(1, 0.5),
                StartPoint = new Point(0, 0.5)
            };
            var stop1 = new GradientStop(Colors.White, 0);
            var stop2 = new GradientStop(Colors.White, 1);
            var stop3 = new GradientStop(Colors.Red, 1);
            RegisterName("GradientStop1", stop1);
            RegisterName("GradientStop2", stop2);
            RegisterName("GradientStop3", stop3);
            gradientBrush.GradientStops.Add(stop1);
            gradientBrush.GradientStops.Add(stop2);
            gradientBrush.GradientStops.Add(stop3);
            Foreground = gradientBrush;
            Loaded += (s, e) => { Animate(); };
        }

        private void Animate()
        {
            var storyboard = new Storyboard();
            var animation1 = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = Duration,
                BeginTime = StartDuration
            };
            Storyboard.SetTargetName(animation1, "GradientStop2");
            Storyboard.SetTargetProperty(animation1,
                new PropertyPath(GradientStop.OffsetProperty));

            var animation2 = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = Duration,
                BeginTime = StartDuration
            };
            Storyboard.SetTargetName(animation2, "GradientStop3");
            Storyboard.SetTargetProperty(animation2,
                new PropertyPath(GradientStop.OffsetProperty));

            storyboard.Children.Add(animation1);
            storyboard.Children.Add(animation2);
            storyboard.Begin(this);
        }
    }
}
