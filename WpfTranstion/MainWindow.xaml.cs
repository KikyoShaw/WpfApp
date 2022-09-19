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

namespace WpfTranstion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double _dWidth = 0.0;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _dWidth = this.ActualWidth / 8;
            Height = this.ActualWidth;
            var leftX = 0.0;
            TranstionCanvas.Height = ActualHeight * 0.8;
            var color = (Color)ColorConverter.ConvertFromString("#FFB6C1")!;
            for (int i = 0; i < 10; i++)
            {
                var name = $"Part_Rectangle_{i}";
                var rect = new Rectangle
                {
                    Width = _dWidth,
                    Height = ActualHeight,
                    Fill = new SolidColorBrush(color),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    Name = name,
                    RenderTransform = new SkewTransform
                    {
                        AngleX = -25
                    }
                };
                TranstionCanvas.Children.Add(rect);
                if (!leftX.Equals(0.0))
                    leftX = leftX + _dWidth - 1;
                else
                    leftX = -_dWidth - 4;
                Canvas.SetLeft(rect, leftX);
            }
        }

        private void HandleButton_OnClick(object sender, RoutedEventArgs e)
        {
            var doubleAnimation = new DoubleAnimation
            {
                To = 0,
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
            };
            if (HandleButton.Content.ToString()!.Equals("Next"))
            {
                foreach (Rectangle item in TranstionCanvas.Children)
                {
                    var names = item.Name.Split('_');
                    DoubleAnimationDuration(doubleAnimation, names);
                    doubleAnimation.Completed += (s, n) =>
                    {
                        HandleButton.Content = "Last";
                    };
                    item.BeginAnimation(Rectangle.WidthProperty, doubleAnimation);
                }
            }
            else
            {
                doubleAnimation.To = _dWidth;
                doubleAnimation.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut };
                foreach (Rectangle item in TranstionCanvas.Children)
                {
                    var names = item.Name.Split('_');
                    DoubleAnimationDuration(doubleAnimation, names);
                    doubleAnimation.Completed += (s, n) =>
                    {
                        HandleButton.Content = "Next";
                    };
                    item.BeginAnimation(Rectangle.WidthProperty, doubleAnimation);
                }
            }
        }

        private void DoubleAnimationDuration(DoubleAnimation doubleAnimation, string[] names)
        {
            if (names[2].Equals("7") || names[2].Equals("2"))
            {
                doubleAnimation.Duration = TimeSpan.FromMilliseconds(200);
            }
            else if (names[2].Equals("0") || names[2].Equals("6"))
            {
                doubleAnimation.Duration = TimeSpan.FromMilliseconds(250);
            }
            else if (names[2].Equals("4") || names[2].Equals("9"))
            {
                doubleAnimation.Duration = TimeSpan.FromMilliseconds(300);
            }
            else if (names[2].Equals("1") || names[2].Equals("5"))
            {
                doubleAnimation.Duration = TimeSpan.FromMilliseconds(400);
            }
            else
                doubleAnimation.Duration = TimeSpan.FromMilliseconds(500);
        }
    }
}
