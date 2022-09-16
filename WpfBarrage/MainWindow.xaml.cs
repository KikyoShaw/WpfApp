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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfBarrage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<TimeSpan, List<Border>> _dicBorder;
        private long _num, _index;
        private double _right, _top;
        public MainWindow()
        {
            InitializeComponent();

            _dicBorder = new Dictionary<TimeSpan, List<Border>>();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _num = (int)(ActualHeight - MyGrid.ActualHeight) / 40;
            var list = new List<string>
            {
                "6666",
                "RNG",
                "EDG",
                "TES",
                "JDG",
                "S12",
                "夺冠"
            };
            foreach (var item in list)
            {
                AddBarrage(item);

            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            AddBarrage(TbBarrage.Text);
        }

        private void AddBarrage(string text)
        {
            _index++;
            TimeSpan time = default;

            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb((byte)_random.Next(1, 255),
                (byte)_random.Next(1, 255), (byte)_random.Next(1, 233)));

            Color color = brush.Color;

            var linearGradientBrush = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                MappingMode = BrushMappingMode.RelativeToBoundingBox,
                GradientStops = new GradientStopCollection
                {
                    new GradientStop { Color = Colors.Transparent, Offset = 2},
                    new GradientStop { Color = color },
                },

            };
            var border = new Border()
            {
                Background = linearGradientBrush,
                Height = 40,
                CornerRadius = new CornerRadius(20),
                Padding = new Thickness(40, 0, 40, 0)

            };

            var textBlock = new TextBlock()
            {
                Text = text,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
            };
            border.Child = textBlock;
            MyCanvas.Children.Add(border);
            border.Loaded += delegate
            {

                time = TimeSpan.FromMilliseconds(border.ActualWidth * 100);
                _right = _right == 0 ? ActualWidth + border.ActualWidth : _right;
                var y = ActualHeight - MyGrid.ActualHeight - border.ActualHeight;
                _top = _top + 40 >= y ? border.ActualHeight : _top;
                Canvas.SetLeft(border, _right);
                Canvas.SetTop(border, _top);
                var doubleAnimation = new DoubleAnimation
                {
                    From = _right,
                    To = -(ActualWidth + border.ActualWidth),
                    Duration = time
                };
                doubleAnimation.Completed += (s, e) =>
                {
                    var animationClock = s as AnimationClock;
                    if (animationClock == null) return;
                    var duration = animationClock.Timeline.Duration;
                    _dicBorder.TryGetValue(duration.TimeSpan, out var bordersList);
                    if (bordersList != null && bordersList.Count > 0)
                    {
                        foreach (var item in bordersList)
                        {
                            MyCanvas.Children.Remove(item);
                        }
                        _dicBorder.Remove(duration.TimeSpan);
                    }
                };
                border.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
                _top += border.ActualHeight + 20;
                if (!_dicBorder.ContainsKey(time))
                    _dicBorder.Add(time, new List<Border> { border });
                else
                {
                    _dicBorder.TryGetValue(time, out var bordersList);
                    bordersList?.Add(border);
                }
            };

            if (_index > _num)
            {
                _index = 0;
            }

        }
    }
}
