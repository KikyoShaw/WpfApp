using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WpfShared.Helper;

namespace WpfBanner
{
    internal enum CarouselLocation
    {
        Left,
        Right,
        Center
    }

    internal enum CarouselZIndex
    {
        Left = 20,
        Center = 30,
        Right = 20,
        LeftMask = 40,
        RightMask = 40
    }


    [DefaultProperty("Children")]
    [ContentProperty("Children")]
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    [TemplatePart(Name = PartContentDockName, Type = typeof(Canvas))]
    [TemplatePart(Name = PartButtonDockName, Type = typeof(StackPanel))]
    public class Banner : Control, IAddChild
    {
        private const string PartContentDockName = "PART_ContentDock";
        private const string PartButtonDockName = "PART_ButtonDock";

        #region timer

        private readonly Timer _playTimer = new Timer();

        #endregion


        static Banner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Banner),
                new FrameworkPropertyMetadata(typeof(Banner)));
        }

        public Banner()
        {
            LoadTimer();
            Loaded += MasterCarousel_Loaded;
        }

        // Using a DependencyProperty as the backing store for IsStartAinimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartAnimationProperty =
            DependencyProperty.Register("IsStartAnimation", typeof(bool), typeof(Banner),
                new PropertyMetadata(default(bool), OnIsStartAnimationPropertyChangedCallback));

        public bool IsStartAnimation
        {
            get => (bool)GetValue(IsStartAnimationProperty);
            set => SetValue(IsStartAnimationProperty, value);
        }

        private static void OnIsStartAnimationPropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Banner control))
                return;

            if (bool.TryParse(e.NewValue?.ToString(), out var bResult))
            {
                if (bResult)
                    control.Start();
                else
                    control.Stop();
            }
        }

        // Using a DependencyProperty as the backing store for PlaySpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaySpeedProperty =
            DependencyProperty.Register("PlaySpeed", typeof(double), typeof(Banner),
                new PropertyMetadata(2000d, OnPlaySpeedPropertyChangedCallBack));

        public double PlaySpeed
        {
            get => (double)GetValue(PlaySpeedProperty);
            set => SetValue(PlaySpeedProperty, value);
        }

        private static void OnPlaySpeedPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Banner control))
                return;

            if (!double.TryParse(e.NewValue?.ToString(), out var vResult))
                return;

            control.Stop();
            control.ResetInterval(vResult);
            control.Start();
        }

        // Using a DependencyProperty as the backing store for Childrens.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Banner),
                new PropertyMetadata(default(IEnumerable), OnItemsSourcePropertyChangedCallBack));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemsSourcePropertyChangedCallBack(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Banner carousel))
                return;

            var vOldEvent = e.OldValue?.GetType()?.GetEvent("CollectionChanged");
            if (vOldEvent != null)
                vOldEvent.RemoveEventHandler(e.OldValue,
                    new NotifyCollectionChangedEventHandler(carousel.ChildrenPropertyChanged));

            var vEvent = e.NewValue?.GetType()?.GetEvent("CollectionChanged");
            if (vEvent != null)
                vEvent.AddEventHandler(e.NewValue,
                    new NotifyCollectionChangedEventHandler(carousel.ChildrenPropertyChanged));
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<object> Children { get; } = new List<object>();


        public void AddChild(object value)
        {
            throw new NotImplementedException();
        }

        public void AddText(string text)
        {
            throw new NotImplementedException();
        }


        #region override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _partContentDock = GetTemplateChild(PartContentDockName) as Canvas;
            _partButtonDock = GetTemplateChild(PartButtonDockName) as StackPanel;

            if (_partContentDock == null || _partButtonDock == null)
                throw new Exception("Some element is not in template!");
        }

        #endregion

        private void ChildrenPropertyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }


        private bool LoadTimer()
        {
            _playTimer.Interval = PlaySpeed;
            _playTimer.Elapsed += PlayTimer_Elapsed;

            return true;
        }

        private bool ResetInterval(double interval)
        {
            if (interval <= 0)
                return false;

            _playTimer.Interval = interval;

            return true;
        }

        private bool CalculationShellReletiveProperty()
        {
            //计算当前目标区域的尺寸
            if (_partContentDock == null)
                return false;

            var vWidth = _partContentDock.ActualWidth;
            var vHeight = _partContentDock.ActualHeight;

            if (vWidth == 0 || vHeight == 0)
                return false;

            if (vWidth == _shellWidth && vHeight == _shellHeight)
                return false;

            _shellWidth = vWidth;
            _shellHeight = vHeight;

            //计算 元素的默认长度和宽度
            _elementWidth = _shellWidth * _elementScale;
            _elementHeight = _shellHeight;

            _leftDockLeft = 0;
            _centerDockLeft = 0 + _shellWidth * _dockOffset;
            _rightDockLeft = _shellWidth - _elementWidth;

            return true;
        }

        private bool LoadCarousel()
        {
            if (Children.Count <= 0 && ItemsSource == null)
                return false;

            if (_partButtonDock != null)
                foreach (var item in _partButtonDock.Children)
                    if (item is FrameworkElement frameworkElement)
                    {
                        frameworkElement.MouseEnter -= Border_MouseEnter;
                        frameworkElement.PreviewMouseDown -= Border_PreviewMouseDown;
                    }

            _mapFrameworkes.Clear();
            _mapCarouselLocationFramewokes.Clear();
            _bufferLinkedList.Clear();

            _partContentDock?.Children.Clear();
            _partButtonDock?.Children.Clear();


            if (Children.Count > 0)
            {
                _carouselSize = Children.Count;

                for (var i = 0; i < _carouselSize; i++)
                {
                    var vItem = Children[i];
                    FrameworkElement frameworkElement;
                    if (vItem is FrameworkElement item)
                    {
                        frameworkElement = item;
                    }
                    else
                    {
                        var vContent = new ContentControl();
                        vContent.HorizontalContentAlignment = HorizontalAlignment.Center;
                        vContent.VerticalContentAlignment = VerticalAlignment.Center;
                        vContent.Content = vItem;
                        frameworkElement = vContent;
                    }

                    frameworkElement.Width = _elementWidth;
                    frameworkElement.Height = _elementHeight;

                    frameworkElement.RenderTransformOrigin = new Point(0.5, 1);
                    var vTransformGroup = new TransformGroup
                    {
                        Children =
                        {
                            new ScaleTransform { ScaleY = ScaleRatio },
                            new SkewTransform(),
                            new RotateTransform(),
                            new TranslateTransform()
                        }
                    };
                    frameworkElement.RenderTransform = vTransformGroup;

                    var border = new Border
                    {
                        Margin = new Thickness(5),
                        Width = 20,
                        Height = 6,
                        Background = Brushes.Gray,
                        Tag = i
                    };

                    border.MouseEnter += Border_MouseEnter;
                    border.PreviewMouseDown += Border_PreviewMouseDown;

                    _mapResources[i] = vItem;
                    _mapFrameworkes[i] = frameworkElement;

                    _partContentDock?.Children.Add(frameworkElement);
                    _partButtonDock?.Children.Add(border);

                    //第一个元素居中并且放大显示
                    if (i == 0)
                    {
                        if (vTransformGroup.Children[0] is ScaleTransform vScaleTransform) vScaleTransform.ScaleY = ScaleRatioEx;
                        frameworkElement.SetValue(Canvas.LeftProperty, _centerDockLeft);
                        Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Center);
                        _mapCarouselLocationFramewokes.Add(CarouselLocation.Center, i);
                    }
                    else if (i == 1)
                    {
                        frameworkElement.SetValue(Canvas.LeftProperty, _rightDockLeft);
                        Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Right);
                        _mapCarouselLocationFramewokes.Add(CarouselLocation.Right, i);
                    }
                    else if (i == _carouselSize - 1)
                    {
                        frameworkElement.SetValue(Canvas.LeftProperty, _leftDockLeft);
                        Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Left);
                        _mapCarouselLocationFramewokes.Add(CarouselLocation.Left, i);
                    }
                    else
                    {
                        _bufferLinkedList.AddLast(i);
                        frameworkElement.SetValue(Canvas.LeftProperty, _centerDockLeft);
                        Panel.SetZIndex(frameworkElement, i);
                    }
                }
            }
            else
            {
                _carouselSize = ItemsSource.Count();

                var nIndex = 0;
                foreach (var item in ItemsSource)
                {
                    FrameworkElement frameworkElement;
                    if (item is FrameworkElement element)
                    {
                        frameworkElement = element;
                    }
                    else
                    {
                        var vContent = new ContentControl();
                        vContent.HorizontalContentAlignment = HorizontalAlignment.Center;
                        vContent.VerticalContentAlignment = VerticalAlignment.Center;
                        vContent.Content = item;
                        frameworkElement = vContent;
                    }

                    frameworkElement.Width = _elementWidth;
                    frameworkElement.Height = _elementHeight;

                    frameworkElement.RenderTransformOrigin = new Point(0.5, 1);
                    var vTransformGroup = new TransformGroup
                    {
                        Children =
                        {
                            new ScaleTransform { ScaleY = ScaleRatio },
                            new SkewTransform(),
                            new RotateTransform(),
                            new TranslateTransform()
                        }
                    };
                    frameworkElement.RenderTransform = vTransformGroup;

                    var border = new Border
                    {
                        Width = 25,
                        Height = 25,
                        CornerRadius = new CornerRadius(25),
                        Background = Brushes.Gray,
                        Tag = nIndex
                    };

                    border.MouseEnter += Border_MouseEnter;
                    border.PreviewMouseDown += Border_PreviewMouseDown;

                    _mapResources[nIndex] = item;
                    _mapFrameworkes[nIndex] = frameworkElement;

                    _partContentDock?.Children.Add(frameworkElement);
                    _partButtonDock?.Children.Add(border);

                    //第一个元素居中并且放大显示
                    if (vTransformGroup.Children[0] is ScaleTransform vScaleTransform) vScaleTransform.ScaleY = ScaleRatioEx;
                    frameworkElement.SetValue(Canvas.LeftProperty, _centerDockLeft);
                    Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Center);
                    _mapCarouselLocationFramewokes.Add(CarouselLocation.Center, nIndex);
                }
            }

            return true;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            _IsStoryboardWorking = false;
        }

        private void PlayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher?.BeginInvoke(new Action(() => PlayCarouselRightToLeft()),
                DispatcherPriority.Background);
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            if (sender is FrameworkElement frameworkElement)
                if (int.TryParse(frameworkElement.Tag?.ToString(), out var nResult))
                    PlayCarouselWithIndex(nResult);
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
                if (int.TryParse(frameworkElement.Tag?.ToString(), out var nResult))
                    PlayCarouselWithIndex(nResult);
        }

        private void MasterCarousel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Stop();

            if (CalculationShellReletiveProperty())
            {
                LoadCarousel();
                Start();
            }
        }

        private void MasterCarousel_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
                return;

            Stop();

            if (CalculationShellReletiveProperty())
            {
                LoadCarousel();
                Start();
            }

            _isLoaded = true;
        }

        #region feild

        private bool _isLoaded;
        private Canvas _partContentDock;
        private StackPanel _partButtonDock;

        #endregion

        #region render relative

        private const double ScaleRatio = 0.95;
        private const double ScaleRatioEx = 1;

        private double _shellWidth;
        private double _shellHeight;

        private double _elementWidth;
        private double _elementHeight;
        private readonly double _elementScale = 0.6;

        private double _centerDockLeft;
        private double _leftDockLeft;
        private double _rightDockLeft;
        private readonly double _dockOffset = 0.2;

        private int _carouselSize;

        #endregion

        #region

        private readonly Dictionary<int, object> _mapResources = new Dictionary<int, object>();
        private readonly Dictionary<int, FrameworkElement> _mapFrameworkes = new Dictionary<int, FrameworkElement>();

        private readonly Dictionary<CarouselLocation, int> _mapCarouselLocationFramewokes =
            new Dictionary<CarouselLocation, int>();

        private readonly LinkedList<int> _bufferLinkedList = new LinkedList<int>();

        #endregion

        #region StoryBoard

        private Storyboard _storyboard;
        private readonly double _AnimationTime = 0.5;
        private readonly double _DelayAnimationTime = 0.7;

        private bool _IsAinimationStart;
        private bool _IsStoryboardWorking;

        #endregion


        #region 动画

        //从左边向右依次播放
        private bool PlayCarouselLeftToRight()
        {
            if (_storyboard == null)
            {
                _storyboard = new Storyboard();
                _storyboard.Completed += Storyboard_Completed;
            }

            if (_IsStoryboardWorking)
                return false;

            _IsStoryboardWorking = true;

            _storyboard?.Children.Clear();

            var nNextIndex = -1;

            //右边的动画移动到中间 后层
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Right, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //右边移动到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _centerDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _bufferLinkedList.AddFirst(vResult);
                }

                _mapCarouselLocationFramewokes[CarouselLocation.Right] = -1;
            }

            //中间的动画移动到右边
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Center, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    //置于左边上层
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Right,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //从中间到左边
                    var animation2 = new DoubleAnimation
                    {
                        //BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                        To = _rightDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLocation.Right] = vResult;
                }

                _mapCarouselLocationFramewokes[CarouselLocation.Center] = -1;
            }

            //左边的动画移动到中间 上层
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Left, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    //置于上层
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Center,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //从左到中
                    var animation2 = new DoubleAnimation
                    {
                        To = _centerDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatioEx,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLocation.Center] = vResult;

                    nNextIndex = vResult - 1;
                    if (nNextIndex < 0)
                        nNextIndex = _carouselSize - 1;
                }

                _mapCarouselLocationFramewokes[CarouselLocation.Left] = -1;
            }

            //后层记录推送到前台左侧

            if (nNextIndex >= 0)
            {
                _bufferLinkedList.Remove(nNextIndex);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(nNextIndex);

                if (vFrameworker != null)
                {
                    //右侧置顶
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Left,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //从中间移动到右侧
                    var animation2 = new DoubleAnimation
                    {
                        To = _leftDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLocation.Left] = nNextIndex;
                }
            }
            else
            {
                if (_bufferLinkedList.Count > 0)
                {
                    var vResult = _bufferLinkedList.LastOrDefault();
                    _bufferLinkedList.RemoveLast();

                    var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                    if (vFrameworker != null)
                    {
                        //右侧置顶
                        var animation1 = new Int32Animation
                        {
                            To = (int)CarouselZIndex.Left,
                            Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation1, vFrameworker);
                        Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                        _storyboard.Children.Add(animation1);

                        //从中间移动到右侧
                        var animation2 = new DoubleAnimation
                        {
                            To = _leftDockLeft,
                            Duration = TimeSpan.FromSeconds(_AnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation2, vFrameworker);
                        Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                        _storyboard.Children.Add(animation2);

                        var animation3 = new DoubleAnimation
                        {
                            Duration = TimeSpan.FromSeconds(_AnimationTime),
                            To = ScaleRatio,
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation3, vFrameworker);
                        Storyboard.SetTargetProperty(animation3,
                            new PropertyPath(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        _storyboard.Children.Add(animation3);

                        _mapCarouselLocationFramewokes[CarouselLocation.Left] = vResult;
                    }
                }
            }

            _storyboard?.Begin();

            return true;
        }

        //从右向左依次播放
        private bool PlayCarouselRightToLeft()
        {
            if (_storyboard == null)
            {
                _storyboard = new Storyboard();
                _storyboard.Completed += Storyboard_Completed;
            }

            _IsStoryboardWorking = true;

            _storyboard?.Children.Clear();

            var nNextIndex = -1;

            //左边的动画移动到中间 后层
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Left, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //从左到中
                    var animation2 = new DoubleAnimation
                    {
                        To = _centerDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _bufferLinkedList.AddLast(vResult);
                }

                _mapCarouselLocationFramewokes[CarouselLocation.Left] = -1;
            }

            //中间的动画移动到左边
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Center, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    //置于左边上层
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Left,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //从中间到左边
                    var animation2 = new DoubleAnimation
                    {
                        //BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                        To = _leftDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLocation.Left] = vResult;
                }

                _mapCarouselLocationFramewokes[CarouselLocation.Center] = -1;
            }

            //右边的动画移动到中间
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Right, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    //置于上层
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Center,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //右边移动到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _centerDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatioEx,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLocation.Center] = vResult;

                    nNextIndex = vResult + 1;
                    if (nNextIndex >= _carouselSize)
                        nNextIndex = 0;
                }

                _mapCarouselLocationFramewokes[CarouselLocation.Right] = -1;
            }

            //后层记录推送到前台

            if (nNextIndex >= 0)
            {
                _bufferLinkedList.Remove(nNextIndex);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(nNextIndex);

                if (vFrameworker != null)
                {
                    //右侧置顶
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Right,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //从中间移动到右侧
                    var animation2 = new DoubleAnimation
                    {
                        To = _rightDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLocation.Right] = nNextIndex;
                }
            }
            else
            {
                if (_bufferLinkedList.Count > 0)
                {
                    var vResult = _bufferLinkedList.FirstOrDefault();
                    _bufferLinkedList.RemoveFirst();

                    var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                    if (vFrameworker != null)
                    {
                        //右侧置顶
                        var animation1 = new Int32Animation
                        {
                            To = (int)CarouselZIndex.Right,
                            Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation1, vFrameworker);
                        Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                        _storyboard.Children.Add(animation1);

                        //从中间移动到右侧
                        var animation2 = new DoubleAnimation
                        {
                            To = _rightDockLeft,
                            Duration = TimeSpan.FromSeconds(_AnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation2, vFrameworker);
                        Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                        _storyboard.Children.Add(animation2);

                        var animation3 = new DoubleAnimation
                        {
                            Duration = TimeSpan.FromSeconds(_AnimationTime),
                            To = ScaleRatio,
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation3, vFrameworker);
                        Storyboard.SetTargetProperty(animation3,
                            new PropertyPath(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        _storyboard.Children.Add(animation3);

                        _mapCarouselLocationFramewokes[CarouselLocation.Right] = vResult;
                    }
                }
            }

            _storyboard?.Begin();

            return true;
        }

        //当用户点击其中某个位置的动画时
        private bool PlayCarouselWithIndex(int nIndex)
        {
            //检查 nIndex是否有效
            if (nIndex < 0 || nIndex >= _carouselSize)
                return false;

            //判断当前选中的是否处于中间播放位置
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Center, -1);
                if (vResult == nIndex)
                    return true;
            }

            //判断如果当前选中的在左侧等待区 播放顺序是从左向右
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Left, -1);
                if (vResult == nIndex)
                    return PlayCarouselLeftToRight();
            }

            //判断如果当前选中的在右侧等待区 播放顺序是从右向左
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Right, -1);
                if (vResult == nIndex)
                    return PlayCarouselRightToLeft();
            }

            //其他情况 
            return PlayCarouselWithIndexOutRange(nIndex);
        }

        private bool PlayCarouselWithIndexOutRange(int nIndex)
        {
            //检查 nIndex是否有效
            if (nIndex < 0 || nIndex >= _carouselSize)
                return false;

            //计算前后动画位置
            var vPre = nIndex - 1;
            if (vPre < 0)
                vPre = _carouselSize - 1;

            var vNext = nIndex + 1;
            if (vNext >= _carouselSize)
                vNext = 0;

            if (_storyboard == null)
            {
                _storyboard = new Storyboard();
                _storyboard.Completed += Storyboard_Completed;
            }

            if (_IsStoryboardWorking)
                return false;

            _IsStoryboardWorking = true;

            _storyboard?.Children.Clear();

            //清空队列
            _bufferLinkedList.Clear();

            //先将队列归位 全部置于中间后面隐藏
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Right, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);
                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //回到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _centerDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);
                }

                _mapCarouselLocationFramewokes[CarouselLocation.Right] = -1;
            }

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Center, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);
                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //回到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _centerDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);
                }

                _mapCarouselLocationFramewokes[CarouselLocation.Center] = -1;
            }

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLocation.Left, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);
                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    //回到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _centerDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);
                }

                _mapCarouselLocationFramewokes[CarouselLocation.Left] = -1;
            }

            //再调出目标位置动画
            for (var i = 0; i < _carouselSize; i++)
                if (i == vPre) //放左侧
                {
                    if (_mapFrameworkes.ContainsKey(i))
                    {
                        var vFrameworker = _mapFrameworkes.GetValueOrDefault(i);

                        if (vFrameworker != null)
                        {
                            //置于左边上层
                            var animation1 = new Int32Animation
                            {
                                To = (int)CarouselZIndex.Left,
                                Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation1, vFrameworker);
                            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                            _storyboard.Children.Add(animation1);

                            //从中间到左边
                            var animation2 = new DoubleAnimation
                            {
                                BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                                To = _leftDockLeft,
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation2, vFrameworker);
                            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                            _storyboard.Children.Add(animation2);

                            //缩放
                            var animation3 = new DoubleAnimation
                            {
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                To = ScaleRatio,
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation3, vFrameworker);
                            Storyboard.SetTargetProperty(animation3,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                            _storyboard.Children.Add(animation3);

                            _mapCarouselLocationFramewokes[CarouselLocation.Left] = i;
                        }
                    }
                }
                else if (i == nIndex) //放中间
                {
                    if (_mapFrameworkes.ContainsKey(i))
                    {
                        var vFrameworker = _mapFrameworkes.GetValueOrDefault(i);

                        if (vFrameworker != null)
                        {
                            //置于中间上层
                            var animation1 = new Int32Animation
                            {
                                To = (int)CarouselZIndex.Center,
                                Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation1, vFrameworker);
                            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                            _storyboard.Children.Add(animation1);

                            //到中间
                            var animation2 = new DoubleAnimation
                            {
                                BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                                To = _centerDockLeft,
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation2, vFrameworker);
                            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                            _storyboard.Children.Add(animation2);

                            //缩放
                            var animation3 = new DoubleAnimation
                            {
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                To = ScaleRatioEx,
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation3, vFrameworker);
                            Storyboard.SetTargetProperty(animation3,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                            _storyboard.Children.Add(animation3);

                            _mapCarouselLocationFramewokes[CarouselLocation.Center] = i;
                        }
                    }
                }
                else if (i == vNext) //放右侧
                {
                    if (_mapFrameworkes.ContainsKey(i))
                    {
                        var vFrameworker = _mapFrameworkes.GetValueOrDefault(i);

                        if (vFrameworker != null)
                        {
                            //置于右边上层
                            var animation1 = new Int32Animation
                            {
                                To = (int)CarouselZIndex.Right,
                                Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation1, vFrameworker);
                            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                            _storyboard.Children.Add(animation1);

                            //到右边
                            var animation2 = new DoubleAnimation
                            {
                                BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                                To = _rightDockLeft,
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation2, vFrameworker);
                            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                            _storyboard.Children.Add(animation2);

                            //缩放
                            var animation3 = new DoubleAnimation
                            {
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                To = ScaleRatio,
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation3, vFrameworker);
                            Storyboard.SetTargetProperty(animation3,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                            _storyboard.Children.Add(animation3);

                            _mapCarouselLocationFramewokes[CarouselLocation.Right] = i;
                        }
                    }
                }
                else
                {
                    _bufferLinkedList.AddLast(i);
                }

            _storyboard?.Begin();

            return true;
        }

        private bool Start()
        {
            if (!IsStartAnimation)
                return true;

            if (_IsAinimationStart)
                return true;

            _IsAinimationStart = true;
            _playTimer.Start();
            return true;
        }

        private bool Stop()
        {
            if (_IsAinimationStart)
            {
                _IsAinimationStart = false;
                _playTimer.Stop();
            }

            return true;
        }

        #endregion
    }
}