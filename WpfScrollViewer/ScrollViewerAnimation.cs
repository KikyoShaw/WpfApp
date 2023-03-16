using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WpfScrollViewer
{
    public class ScrollViewerAnimation : ScrollViewer
    {
        //记录上一次的滚动位置
        private double _lastLocation;

        private DispatcherTimer _scrollTemplateTimer = null;
        public ScrollViewerAnimation() : base()
        {
            // 用定时器来判断当前控件的visualTree是否加载完成。
            _scrollTemplateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _scrollTemplateTimer.Tick += OnScrollTemplateLoaded;
            _scrollTemplateTimer.Start();
        }

        // 绑定垂直滚动条点击事件。
        // 滚动条被点击后，位置会更新，需要更新_lastLocation后，在下一次滚动时才会得到正确的滚动位置
        private void OnScrollTemplateLoaded(object sender, EventArgs e)
        {
            ScrollBar verScrollBar = (ScrollBar)this.Template.FindName("PART_VerticalScrollBar", this);
            if (verScrollBar != null)
            {
                verScrollBar.MouseMove += VerScrollBarMouseMove; ;
                _scrollTemplateTimer.Stop();
                _scrollTemplateTimer = null;
            }
        }

        private void VerScrollBarMouseMove(object sender, MouseEventArgs e)
        {
            _lastLocation = this.VerticalOffset;
        }

        // 重写回退到顶部的事件
        public new void ScrollToHome()
        {
            // 停止动画
            BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, null);

            // 上一次滚动位置置0
            _lastLocation = 0;
            // 回到顶部
            base.ScrollToHome();
        }

        //重写鼠标滚动事件
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            double wheelChange = e.Delta;
            //可以更改一次滚动的距离倍数 (WheelChange可能为正负数!)
            var newOffset = _lastLocation - wheelChange * 2;
            //Animation并不会改变真正的VerticalOffset(只是它的依赖属性) 所以将VOffset设置到上一次的滚动位置 (相当于衔接上一个动画)
            ScrollToVerticalOffset(_lastLocation);
            //碰到底部和顶部时的处理
            if (newOffset < 0)
                newOffset = 0;
            if (newOffset > ScrollableHeight)
                newOffset = ScrollableHeight;

            AnimateScroll(newOffset);
            _lastLocation = newOffset;
            //告诉ScrollViewer我们已经完成了滚动
            e.Handled = true;
        }

        private void AnimateScroll(double toValue)
        {
            //为了避免重复，先结束掉上一个动画
            BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, null);
            var animation = new DoubleAnimation
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                From = VerticalOffset,
                To = toValue,
                //动画速度
                Duration = TimeSpan.FromMilliseconds(800)
            };
            //考虑到性能，可以降低动画帧数
            //Timeline.SetDesiredFrameRate(Animation, 40);
            BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, animation);
        }
    }
}