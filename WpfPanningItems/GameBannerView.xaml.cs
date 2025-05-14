using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfPanningItems
{
    /// <summary>
    /// GameBannerView.xaml 的交互逻辑
    /// </summary>
    public partial class GameBannerView : UserControl
    {
        private DispatcherTimer _autoSwitchTimer = null;
        private readonly List<BitmapImage> _bannerImageList = new List<BitmapImage>();
        private int _imageIndex = 0;

        public GameBannerView()
        {
            InitializeComponent();
            DataContext = MainVm.Instance;

            for (int i = 0; i < 5; i++)
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri($"pack://application:,,,/WpfPanningItems;Component/Resources/{i}.jpg");
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze();
                _bannerImageList.Add(img);
            }

            InitImageCard();
        }

        /// <summary>
        /// 初始化定时器
        /// </summary>
        private void InitAutoSwitchTimer()
        {
            if (_autoSwitchTimer == null)
            {
                _autoSwitchTimer = new DispatcherTimer();
                _autoSwitchTimer.Interval = TimeSpan.FromSeconds(5);
                _autoSwitchTimer.Tick += AutoSwitchTimer_Tick;
            }

            if(_autoSwitchTimer.IsEnabled)
                _autoSwitchTimer.Stop();

            _autoSwitchTimer.Start();
        }

        private void AutoSwitchTimer_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(SwitchNextImage);
        }

        private void InitImageCard()
        {
            var v1 = _bannerImageList[_imageIndex];
            //MainVm.Instance.LeftImage = v1;
            //MainVm.Instance.MiddleImage = v1;
            OrgActImg.Source = v1;
            CurActImg.Source = v1;
            CurActCard.Margin = new Thickness(0, 0, 0, 0);
            OrgActCard.Margin = new Thickness(999, 0, 0, 0);

            UpdateDotIndicator();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if(_bannerImageList.Count <= 1)
                return;
            ActInteraction.Visibility = Visibility.Visible;
            _autoSwitchTimer.Stop();
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_bannerImageList.Count <= 1)
                return;
            ActInteraction.Visibility = Visibility.Collapsed;
            _autoSwitchTimer.Start();
        }

        private void LeftIcon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchPrevImage();
        }

        private void RightIcon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchNextImage();
        }

        /// <summary>
        /// 切换下一张
        /// </summary>
        private void SwitchNextImage()
        {
            if(_switchActCardAnimationPlaying)
                return;

            _imageIndex++;
            if (_imageIndex >= _bannerImageList.Count)
                _imageIndex = 0;
            var v2 = _bannerImageList[_imageIndex];

            //MainVm.Instance.MiddleImage = v2;
            CurActImg.Source = v2;

            DoSwitchImageAnimation(true);
        }

        /// <summary>
        /// 切换上一张
        /// </summary>
        private void SwitchPrevImage()
        {
            if (_switchActCardAnimationPlaying)
                return;

            _imageIndex--;
            if (_imageIndex < 0)
                _imageIndex = _bannerImageList.Count - 1;
            var v2 = _bannerImageList[_imageIndex];
            //MainVm.Instance.MiddleImage = v2;
            CurActImg.Source = v2;

            DoSwitchImageAnimation(false);
        }

        /// <summary>
        /// 同步圆点
        /// </summary>
        private void UpdateDotIndicator()
        {
            DotIndicatorPanel.Children.Clear();
            for (int i = 0; i < _bannerImageList.Count; i++)
            {
                var dot = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Margin = new Thickness(4, 0, 4, 0),
                    Fill = i == _imageIndex
                        ? new SolidColorBrush(Colors.White)
                        : new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                    Opacity = 0.9,
                    Tag = i,
                };
                dot.MouseLeftButtonDown += Dot_MouseLeftButtonDown;
                DotIndicatorPanel.Children.Add(dot);
            }
        }

        /// <summary>
        /// 圆点点击事件
        /// </summary>
        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_switchActCardAnimationPlaying) return;

            if (sender is Ellipse { Tag: int index } && index != _imageIndex)
            {
                bool isNext = index > _imageIndex;
                _imageIndex = index;
                var img = _bannerImageList[_imageIndex];
                CurActImg.Source = img;
                DoSwitchImageAnimation(isNext);
            }
        }

        #region 动画

        private Storyboard _switchStoryboard = null;
        private ThicknessAnimation _switchAnimation1 = null;
        private ThicknessAnimation _switchAnimation2 = null;
        const double AnimationDuration = 0.25;
        private bool _switchActCardAnimationPlaying = false;

        /// <summary>
        /// 初始化动画组件
        /// </summary>
        private void InitSwitchStoryboard()
        {
            if (_switchStoryboard == null)
            {
                _switchStoryboard = new Storyboard();
                _switchStoryboard.Completed += (s, e) => {
                    InitImageCard();
                    _switchActCardAnimationPlaying = false;
                    _switchStoryboard.Stop();
                    _switchStoryboard.Children.Clear();
                    _switchAnimation1 = null;
                    _switchAnimation2 = null;
                };
            }
        }

        /// <summary>
        /// 动画能力
        /// </summary>
        private void DoSwitchImageAnimation(bool isNext)
        {
            InitSwitchStoryboard();
            var w = CurActCard.ActualWidth;
            var marginLeft = isNext ? w : -w;
            CurActCard.Margin = new Thickness(marginLeft, 0, 0, 0);
            OrgActCard.Margin = new Thickness(0, 0, 0, 0);
            _switchAnimation1 = new ThicknessAnimation
            {
                From = new Thickness(marginLeft, 0, 0, 0),
                To = new Thickness(0, 0, 0, 0),
                Duration = new Duration(TimeSpan.FromSeconds(AnimationDuration)),
                BeginTime = TimeSpan.FromSeconds(0.0),
            };
            _switchAnimation2 = new ThicknessAnimation
            {
                From = new Thickness(0, 0, 0, 0),
                To = new Thickness(-marginLeft, 0, 0, 0),
                Duration = new Duration(TimeSpan.FromSeconds(AnimationDuration)),
                BeginTime = TimeSpan.FromSeconds(0.0),
            };

            _switchStoryboard.Children.Add(_switchAnimation1);
            _switchStoryboard.Children.Add(_switchAnimation2);

            Storyboard.SetTarget(_switchAnimation1, CurActCard);
            Storyboard.SetTarget(_switchAnimation2, OrgActCard);
            Storyboard.SetTargetProperty(_switchAnimation1, new PropertyPath("Margin"));
            Storyboard.SetTargetProperty(_switchAnimation2, new PropertyPath("Margin"));

            _switchStoryboard.Begin();
            _switchActCardAnimationPlaying = true;
        }

        #endregion

        public const double DefaultWidth = 925d;
        public const double DefaultHeight = 303d;
        private void GameBannerView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var iWidth = e.NewSize.Width;
            if (iWidth < 1)
                return;

            iWidth = Math.Min(iWidth, 1340);
            var itemHeight = iWidth * DefaultHeight / DefaultWidth;
            MainVm.Instance.GameBannerHeight = itemHeight;
            MainVm.Instance.GameBannerWidth = iWidth;

            System.Diagnostics.Debug.WriteLine($"GameBannerView_OnSizeChanged: GameBannerWidth:{iWidth}, GameBannerHeight:{itemHeight}");
        }

        private void GameBannerView_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool.TryParse(e.NewValue.ToString(), out var result);
            if (result)
                InitAutoSwitchTimer();
            else
                _autoSwitchTimer.Stop();
        }
    }
}
