using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WpfPanning
{
    public class PanningItems : Selector
    {
        private DispatcherTimer _timer;
        static PanningItems()
        {
            SelectedItemProperty.OverrideMetadata(typeof(PanningItems), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedItemChanged)));
            FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(Grid));
            frameworkElementFactory.SetValue(ClipToBoundsProperty, true);
            FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ContentPresenter));
            Binding binding = new Binding();
            binding.RelativeSource = RelativeSource.TemplatedParent;
            binding.Path = new PropertyPath(PreviousItemProperty);
            frameworkElementFactory2.SetBinding(ContentPresenter.ContentProperty, binding);
            Binding binding2 = new Binding();
            binding2.RelativeSource = RelativeSource.TemplatedParent;
            binding2.Path = new PropertyPath(ItemTemplateProperty);
            frameworkElementFactory2.SetBinding(ContentPresenter.ContentTemplateProperty, binding2);
            frameworkElementFactory2.Name = "Previous";
            frameworkElementFactory.AppendChild(frameworkElementFactory2);
            FrameworkElementFactory frameworkElementFactory3 = new FrameworkElementFactory(typeof(ContentPresenter));
            Binding binding3 = new Binding();
            binding3.RelativeSource = RelativeSource.TemplatedParent;
            binding3.Path = new PropertyPath(SelectedItemProperty);
            frameworkElementFactory3.SetBinding(ContentPresenter.ContentProperty, binding3);
            Binding binding4 = new Binding();
            binding4.RelativeSource = RelativeSource.TemplatedParent;
            binding4.Path = new PropertyPath(ItemTemplateProperty);
            frameworkElementFactory3.SetBinding(ContentPresenter.ContentTemplateProperty, binding4);
            frameworkElementFactory3.Name = "Current";
            frameworkElementFactory.AppendChild(frameworkElementFactory3);
            FrameworkElementFactory frameworkElementFactory4 = new FrameworkElementFactory(typeof(ContentPresenter));
            Binding binding5 = new Binding();
            binding5.RelativeSource = RelativeSource.TemplatedParent;
            binding5.Path = new PropertyPath(NextItemProperty);
            frameworkElementFactory4.SetBinding(ContentPresenter.ContentProperty, binding5);
            Binding binding6 = new Binding();
            binding6.RelativeSource = RelativeSource.TemplatedParent;
            binding6.Path = new PropertyPath(ItemTemplateProperty);
            frameworkElementFactory4.SetBinding(ContentPresenter.ContentTemplateProperty, binding6);
            frameworkElementFactory4.Name = "Next";
            frameworkElementFactory.AppendChild(frameworkElementFactory4);
            ControlTemplate controlTemplate = new ControlTemplate(typeof(PanningItems));
            controlTemplate.VisualTree = frameworkElementFactory;
            Style style = new Style(typeof(PanningItems));
            Setter item = new Setter(TemplateProperty, controlTemplate);
            style.Setters.Add(item);
            style.Seal();
            StyleProperty.OverrideMetadata(typeof(PanningItems), new FrameworkPropertyMetadata(style));
        }

        public PanningItems()
        {
            SelectedIndex = 0;
        }

        public Orientation ScrollDirection
        {
            get => (Orientation)GetValue(ScrollDirectionProperty);
            set => SetValue(ScrollDirectionProperty, value);
        }

        public double FlickTolerance
        {
            get => (double)GetValue(FlickToleranceProperty);
            set => SetValue(FlickToleranceProperty, value);
        }

        public object PreviousItem
        {
            get => GetValue(PreviousItemProperty);
            set => SetValue(PreviousItemProperty, value);
        }

        public object NextItem
        {
            get => GetValue(NextItemProperty);
            set => SetValue(NextItemProperty, value);
        }

        public bool LoopContents
        {
            get => (bool)GetValue(LoopContentsProperty);
            set => SetValue(LoopContentsProperty, value);
        }

        public double SliderValue
        {
            get => (double)GetValue(SliderValueProperty);
            set => SetValue(SliderValueProperty, value);
        }
        public int Seconds
        {
            get => (int)GetValue(SecondsProperty);
            set => SetValue(SecondsProperty, value);
        }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _previousTransform = new TranslateTransform();
            _previous = (ContentPresenter)Template.FindName("Previous", this);
            if (_previous != null)
            {
                _previous.Opacity = 0.0;
                _previous.RenderTransform = _previousTransform;
            }
            _currentTransform = new TranslateTransform();
            _current = (ContentPresenter)Template.FindName("Current", this);
            if (_current != null)
            {
                _current.RenderTransform = _currentTransform;
            }
            _nextTransform = new TranslateTransform();
            _next = (ContentPresenter)Template.FindName("Next", this);
            if (_next != null)
            {
                _next.Opacity = 0.0;
                _next.RenderTransform = _nextTransform;
            }
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Seconds) };
           
            _timer.Tick += delegate
            {
                StartAnimateSliderValueTo();
            };
            _timer.Start();
        }

        void StartAnimateSliderValueTo()
        {
            var w = SystemParameters.WorkArea.Width;
            var h = SystemParameters.WorkArea.Height;
           
            Vector vector;
            if (ScrollDirection == Orientation.Horizontal)
            {
                var point = new Point(w, 0);
                var point1 = new Point(w / 5, 0);
                vector = point - point1;
                SliderValue = vector.X / _current.ActualWidth;
            }
            else
            {
                var point = new Point(0, h);
                var point1 = new Point(0, h/3);
                vector = point - point1;
                SliderValue = vector.Y / _current.ActualHeight;
            }
            if (Math.Abs(SliderValue) >= FlickTolerance)
            {
                _isDragging = false;
                int num = SelectedIndex;
                if (num != -1)
                {
                    if (SliderValue > 0.0)
                    {
                        num--;
                        SliderValue -= 1.0;
                    }
                    else
                    {
                        num++;
                        SliderValue += 1.0;
                    }
                    num += Items.Count;
                    num %= Items.Count;
                    SelectedIndex = num;
                }
                AnimateSliderValueTo(0.0);
            }
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            CaptureMouse();
            OnGestureDown(e.GetPosition(this));
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            OnGestureUp();
            ReleaseMouseCapture();
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            OnGestureMove(e.GetPosition(this));
            base.OnMouseMove(e);
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            OnGestureUp();
            base.OnLostMouseCapture(e);
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            CaptureTouch(e.TouchDevice);
            OnGestureDown(e.GetTouchPoint(this).Position);
            base.OnTouchDown(e);
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            OnGestureUp();
            ReleaseTouchCapture(e.TouchDevice);
            base.OnTouchUp(e);
        }

        protected override void OnTouchMove(TouchEventArgs e)
        {
            OnGestureMove(e.GetTouchPoint(this).Position);
            base.OnTouchMove(e);
        }

        protected override void OnLostTouchCapture(TouchEventArgs e)
        {
            OnGestureUp();
            base.OnLostTouchCapture(e);
        }
        private static void SecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panningItems = (PanningItems)d;
            if (panningItems == null)return;
            if (panningItems._timer != null)
            {
                panningItems._timer.Stop();
                panningItems._timer.Interval = TimeSpan.FromSeconds(panningItems.Seconds);
                panningItems._timer.Start();
            }


        }
        private static void FlickToleranceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceFlickTolerance(DependencyObject sender, object value)
        {
            double val = (double)value;
            return Math.Max(Math.Min(1.0, val), 0.0);
        }

        private static void SliderValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanningItems panningItems = (PanningItems)sender;
            if (panningItems._previous != null && panningItems._current != null && panningItems._next != null)
            {
                panningItems._previous.Opacity = 1.0;
                panningItems._next.Opacity = 1.0;
                if (panningItems.ScrollDirection == Orientation.Horizontal)
                {
                    panningItems._previousTransform.X = panningItems._current.ActualWidth * (panningItems.SliderValue - 1.0);
                    panningItems._currentTransform.X = panningItems._current.ActualWidth * panningItems.SliderValue;
                    panningItems._nextTransform.X = panningItems._current.ActualWidth * (panningItems.SliderValue + 1.0);
                    panningItems._previousTransform.Y = 0.0;
                    panningItems._currentTransform.Y = 0.0;
                    panningItems._nextTransform.Y = 0.0;
                    return;
                }
                panningItems._previousTransform.X = 0.0;
                panningItems._currentTransform.X = 0.0;
                panningItems._nextTransform.X = 0.0;
                panningItems._previousTransform.Y = panningItems._current.ActualHeight * (panningItems.SliderValue - 1.0);
                panningItems._currentTransform.Y = panningItems._current.ActualHeight * panningItems.SliderValue;
                panningItems._nextTransform.Y = panningItems._current.ActualHeight * (panningItems.SliderValue + 1.0);
            }
        }

        private static void SelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanningItems panningItems = (PanningItems)sender;
            int selectedIndex = panningItems.SelectedIndex;
            if (selectedIndex == -1 || panningItems.Items.Count == 0)
            {
                panningItems.PreviousItem = null;
                panningItems.NextItem = null;
                return;
            }
            if (selectedIndex == 0)
            {
                panningItems.PreviousItem = panningItems.LoopContents ? panningItems.Items[^1] : null;
            }
            else
            {
                panningItems.PreviousItem = panningItems.Items[selectedIndex - 1];
            }
            if (selectedIndex != panningItems.Items.Count - 1)
            {
                panningItems.NextItem = panningItems.Items[selectedIndex + 1];
                return;
            }
            if (panningItems.LoopContents)
            {
                panningItems.NextItem = panningItems.Items[0];
                return;
            }
            panningItems.NextItem = null;
        }

        private void OnGestureDown(Point point)
        {
            _touchDown = point;
            _isDragging = true;
        }

        private void OnGestureUp()
        {
            if (_isDragging)
            {
                AnimateSliderValueTo(0.0);
            }
            _isDragging = false;
        }

        private void OnGestureMove(Point point)
        {
            if (_isDragging)
            {
                Vector vector = point - _touchDown;
                if (ScrollDirection == Orientation.Horizontal)
                {
                    SliderValue = vector.X / _current.ActualWidth;
                }
                else
                {
                    SliderValue = vector.Y / _current.ActualHeight;
                }
                if (Math.Abs(SliderValue) >= FlickTolerance)
                {
                    _isDragging = false;
                    int num = SelectedIndex;
                    if (num != -1)
                    {
                        if (SliderValue > 0.0)
                        {
                            num--;
                            SliderValue -= 1.0;
                        }
                        else
                        {
                            num++;
                            SliderValue += 1.0;
                        }
                        num += Items.Count;
                        num %= Items.Count;
                        SelectedIndex = num;
                    }
                    AnimateSliderValueTo(0.0);
                }
            }
        }

        private void AnimateSliderValueTo(double target)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation(target, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimation.FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop;
            doubleAnimation.Completed += delegate (object o, EventArgs e)
            {
                SliderValue = 0.0;
            };
            BeginAnimation(SliderValueProperty, doubleAnimation);
        }

        public static readonly DependencyProperty ScrollDirectionProperty = DependencyProperty.Register("ScrollDirection", typeof(Orientation), typeof(PanningItems), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty FlickToleranceProperty = DependencyProperty.Register("FlickTolerance", typeof(double), typeof(PanningItems), new PropertyMetadata(0.25, new PropertyChangedCallback(FlickToleranceChanged), new CoerceValueCallback(CoerceFlickTolerance)));

        public static readonly DependencyProperty PreviousItemProperty = DependencyProperty.Register("PreviousItem", typeof(object), typeof(PanningItems), new PropertyMetadata(null));

        public static readonly DependencyProperty NextItemProperty = DependencyProperty.Register("NextItem", typeof(object), typeof(PanningItems), new PropertyMetadata(null));

        public static readonly DependencyProperty LoopContentsProperty = DependencyProperty.Register("LoopContents", typeof(bool), typeof(PanningItems), new PropertyMetadata(true));

        public static readonly DependencyProperty SliderValueProperty = DependencyProperty.Register("SliderValue", typeof(double), typeof(PanningItems), new PropertyMetadata(0.0, new PropertyChangedCallback(SliderValueChanged)));

        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds", typeof(int), typeof(PanningItems), new PropertyMetadata(5, new PropertyChangedCallback(SecondsChanged)));

       

        private Point _touchDown;

        private bool _isDragging;

        private TranslateTransform _previousTransform;

        private TranslateTransform _currentTransform;

        private TranslateTransform _nextTransform;

        private ContentPresenter _previous;

        private ContentPresenter _current;

        private ContentPresenter _next;
    }
}
