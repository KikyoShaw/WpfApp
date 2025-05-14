using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfPanningItems
{
    public class CanvasWithPhoto : Canvas
    {
        public CanvasWithPhoto()
        {
            //背景颜色


            //注册鼠标事件,滑动效果
            this.MouseDown += new System.Windows.Input.MouseButtonEventHandler(CanvasWithPhoto_MouseDown);
            this.MouseUp += new System.Windows.Input.MouseButtonEventHandler(CanvasWithPhoto_MouseUp);
            this.Loaded += new RoutedEventHandler(CanvasWithPhoto_Loaded);

            //动画效果
            this.RenderTransform = _translate;

        }

        void CanvasWithPhoto_Loaded(object sender, RoutedEventArgs e)
        {
            _totalPage = this.InternalChildren.Count;
            //throw new NotImplementedException();
        }

        void CanvasWithPhoto_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _endPoint = e.GetPosition(App.Current.MainWindow);

            //X轴滑动的距离
            double offsetX = _startPoint.X - _endPoint.X;
            if (offsetX > 10 && Index < _totalPage)
            {
                ++Index;
            }
            else if (offsetX < -10 && Index > 1)
            {
                --Index;
            }

            Move(Index);
        }

        //X轴的移动动画
        private void Move(int Index)
        {
            DoubleAnimation animation = new DoubleAnimation(-(Index - 1) * _width, TimeSpan.FromMilliseconds(500));
            animation.DecelerationRatio = 0.2;
            animation.AccelerationRatio = 0.2;
            _translate.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        void CanvasWithPhoto_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(App.Current.MainWindow);
        }

        #region 绘制窗口

        //分配容器的大小
        protected override Size MeasureOverride(Size constraint)
        {

            // 总宽度等于子元素数量 * _width，高度取 _height
            double totalWidth = InternalChildren.Count * _width;
            double totalHeight = _height;

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(_width, _height));
            }

            // 返回整体大小，避免容器尺寸不对
            return new Size(totalWidth, totalHeight);
        }

        //分配子项的大小
        protected override Size ArrangeOverride(Size finalSize)
        {
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                InternalChildren[i].Arrange(new Rect(i * _width, 0, _width, _height));
            }
            return finalSize;
        }

        #endregion

        #region Fields

        //预计图片的长高
        private double _width = 0d;
        private double _height = 0d;

        public void SetSize(double width, double height)
        {
            _width = width;
            _height = height;
            InvalidateMeasure();
            InvalidateArrange();
        }

        //滑动的起点以及终点的坐标
        Point _startPoint = new Point();
        Point _endPoint = new Point();

        int _totalPage = 0;

        //当前页
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(CanvasWithPhoto), new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OniIndexChanged)));
        public static void OniIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CanvasWithPhoto c = (CanvasWithPhoto)sender;
            int newValue = (int)e.NewValue;
            int oldValue = (int)e.OldValue;
            c.Index = (int)e.NewValue;
            c.OnIndexChanged(oldValue, newValue);
        }
        private void OnIndexChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> args
                = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            args.RoutedEvent = IndexChangedEvent;
            RaiseEvent(args);
            Move(newValue);
        }

        public static RoutedEvent IndexChangedEvent =
            EventManager.RegisterRoutedEvent("IndexChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(CanvasWithPhoto));
        public event RoutedPropertyChangedEventHandler<int> IndexChanged
        {
            add { AddHandler(IndexChangedEvent, value); }
            remove { RemoveHandler(IndexChangedEvent, value); }
        }

        //动画
        TranslateTransform _translate = new TranslateTransform();

        #endregion

    }
}
