using System;
using System.Collections.Generic;
using System.Text;
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
using System.Windows.Threading;

namespace WpfBanner
{
    /// <summary>
    /// NewBanner.xaml 的交互逻辑
    /// </summary>
    public partial class NewBanner : UserControl
    {
        public NewBanner()
        {
            InitializeComponent();

            //将三张banner(border)添加到banner集合
            Banners.Add(Location.Left, left);
            Banners.Add(Location.Center, center);
            Banners.Add(Location.Right, right);

            //控件加载完成后
            Loaded += (e, c) =>
            {
                //首次启动设置三张banner的位置、大小信息
                SetLocation(Location.Left, left);
                SetLocation(Location.Center, center);
                SetLocation(Location.Right, right);

                //启动定时器，用于自动播放滚动动画
                TimerAnimationStart();
            };

            //初始化缓动函数
            //quadraticease的easeout mode是从快到慢
            //参考了博客：http://www.cnblogs.com/xwlyun/archive/2012/09/11/2680579.html
            easeFunction = new QuadraticEase()
            {
                EasingMode = EasingMode.EaseOut
            };

            //初始化定时器
            timeranimation = new DispatcherTimer();
            //设置定时器的间隔时间
            timeranimation.Interval = TimeSpan.FromSeconds(timeranimation_time);
        }

        //代码所用涉及时间单位均是：秒
        #region 一些变量
        //左、中、右三张banner的位置
        double leftlocation = 0, centerlocation = 0, rightlocation = 0;
        //每张banner的动画执行时间
        double AnimationTime = 0.4;
        //非中间banner的遮盖层透明度
        double bopacity = 0.65;
        //没有交互时动画自动执行间隔时间
        double timeranimation_time = 4;
        //动画播放状态（当前动画是否在执行）
        bool isplay = false;
        //三个banner border变量，用于暂时保存
        Border b_left, b_center, b_right;
        //通用缓动函数，提升动画流畅感
        EasingFunctionBase easeFunction;
        //banner集合，用于定位和记录border的位置（左，中，右）
        Dictionary<Location, Border> Banners = new Dictionary<Location, Border>();
        DispatcherTimer timeranimation;
        #endregion

        #region 交互事件

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            //鼠标移入控件时显示两个“左/右”图标按钮
            toleftbtn.Opacity = 1;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            //鼠标移出控件时隐藏两个“左/右”图标按钮

            toleftbtn.Opacity = 0;
        }

        private void toleftbtn_Click(object sender, RoutedEventArgs e)
        {
            //向左图标按钮点击
            LeftAnimation();
        }

        private void torightbtn_Click(object sender, RoutedEventArgs e)
        {
            RightAnimation();
        }
        #endregion

        //左切换动画时三张banner向右滚动
        /*
         * 即中间的border移动至：右
         * 右边的border移动至：左
         * 左边的border移动至：中
         */
        #region 左切换动画
        public void LeftAnimation()
        {
            //启动动画时停止定时器
            timeranimation.Stop();
            //设置动画播放状态为真
            isplay = true;

            //启动相应的动画
            LefttoCenterAnimation();

            CentertoRightAnimation();

            RighttoLeftAnimation();


        }
        //【仅注释此方法，以下代码均大多重复故不再注释】
        #region 左切换动画-中向右动画
        public void CentertoRightAnimation()
        {
            //记录动画结束后的位置，即当动画结束后中间的BORDER移动到右变成了右，代码：Banners[Location.Right] = b_center;
            b_center = Banners[Location.Center];

            //设置一下border的显示层级
            Grid.SetZIndex(Banners[Location.Center], 2);

            //获取透明遮盖图层，设置透明度
            /*
             * <Grid Background="Black" Panel.ZIndex="2"></Grid>
             * 透明遮盖图层在设计代码中的每个border内
             * 
             */
            GetOpacityGrid(b_center).Opacity = bopacity;

            //定义一个缩放转换对象（用于banner从大到小的动画）
            ScaleTransform scale = new ScaleTransform();
            //需要设置中心点y坐标为控件的高度，不设置的话border是靠在顶部执行动画的。
            scale.CenterY = this.ActualHeight;

            //定义一个水平移动转换对象
            TranslateTransform ts = new TranslateTransform();

            //定义一个转换集合
            TransformGroup group = new TransformGroup();
            //将上面的缩放、平移转换对象添加到集合
            group.Children.Add(scale);
            group.Children.Add(ts);

            //将转换集合赋予给中心banner
            Banners[Location.Center].RenderTransform = group;


            //定义一个缩放动画
            DoubleAnimation scaleAnimation = new DoubleAnimation()
            {
                //从1（100%即默认比例）
                From = 1,
                //到0.95（95%即从默认比例缩小到95%的比例大小）
                To = 0.95,
                //设置缓动函数
                EasingFunction = easeFunction,
                //动画执行所需时间
                Duration = TimeSpan.FromSeconds(AnimationTime)


            };

            //定义一个移动动画（用于banner从左到右.....移动动画等）
            DoubleAnimation moveAnimation = new DoubleAnimation()
            {
                //从中心banner位置
                From = centerlocation,
                //移动到右banner位置
                To = rightlocation,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)

            };
            //启动缩放动画
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            //启动平移动画
            ts.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
        }

        #endregion
        #region 左切换动画-右向左动画
        public void RighttoLeftAnimation()
        {
            b_right = Banners[Location.Right];

            Grid.SetZIndex(Banners[Location.Right], 1);
            GetOpacityGrid(b_right).Opacity = bopacity;

            ScaleTransform scale = new ScaleTransform();   //缩放  
            scale.CenterY = this.ActualHeight;
            TranslateTransform ts = new TranslateTransform();//平移


            TransformGroup group = new TransformGroup();
            group.Children.Add(scale);
            group.Children.Add(ts);


            Banners[Location.Right].RenderTransform = group;


            DoubleAnimation scaleAnimation = new DoubleAnimation()
            {
                From = 0.85,
                To = 0.95,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)


            };

            DoubleAnimation moveAnimation = new DoubleAnimation()
            {

                From = rightlocation,
                To = leftlocation,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)

            };

            //scaleAnimation.Completed += new EventHandler(scaleAnimation_Completed);  
            //  AnimationClock clock = scaleAnimation.CreateClock();  
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            //启动平移动画
            ts.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
        }

        #endregion
        #region 左切换动画-左向中动画
        public void LefttoCenterAnimation()
        {
            b_left = Banners[Location.Left];

            Grid.SetZIndex(Banners[Location.Left], 3);

            GetOpacityGrid(b_left).Opacity = 0;

            ScaleTransform scale = new ScaleTransform();   //缩放  

            //scale.CenterX = 0;
            scale.CenterY = this.ActualHeight;
            TranslateTransform ts = new TranslateTransform();//平移


            TransformGroup group = new TransformGroup();
            group.Children.Add(scale);
            group.Children.Add(ts);


            Banners[Location.Left].RenderTransform = group;


            DoubleAnimation scaleAnimation = new DoubleAnimation()
            {

                From = 0.95,
                To = 1,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)


            };

            DoubleAnimation moveAnimation = new DoubleAnimation()
            {

                From = leftlocation,
                To = centerlocation,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)

            };

            scaleAnimation.Completed += new EventHandler(LeftAnimation_Completed);
            //  AnimationClock clock = scaleAnimation.CreateClock();  
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            //启动平移动画
            ts.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
        }

        #endregion

        #region 动画结束
        private void LeftAnimation_Completed(object sender, EventArgs e)
        {
            //动画结束后将banner集合的位置重新设置
            Banners[Location.Left] = b_right;
            Banners[Location.Center] = b_left;
            Banners[Location.Right] = b_center;
            //此时动画结束
            isplay = false;
            //启动定时器
            TimerAnimationStart();
        }
        #endregion
        #endregion

        #region 右切换动画
        public void RightAnimation()
        {
            timeranimation.Stop();

            isplay = true;

            LefttoRightAnimation();

            CentertoLeftAnimation();

            RighttoCenterAnimation();


        }

        #region 右切换动画-左向右动画
        public void LefttoRightAnimation()
        {
            b_left = Banners[Location.Left];
            Grid.SetZIndex(Banners[Location.Left], 1);
            GetOpacityGrid(b_left).Opacity = bopacity;
            ScaleTransform scale = new ScaleTransform();   //缩放  
            scale.CenterY = this.ActualHeight;
            TranslateTransform ts = new TranslateTransform();//平移
            TransformGroup group = new TransformGroup();
            group.Children.Add(scale);

            group.Children.Add(ts);
            Banners[Location.Left].RenderTransform = group;

            DoubleAnimation scaleAnimation = new DoubleAnimation()
            {
                From = 0.85,
                To = 0.95,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)


            };
            DoubleAnimation moveAnimation = new DoubleAnimation()
            {

                From = leftlocation,
                To = rightlocation,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)

            };

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            //启动平移动画
            ts.BeginAnimation(TranslateTransform.XProperty, moveAnimation);

        }

        #endregion
        #region 右切换动画-中向左动画
        public void CentertoLeftAnimation()
        {
            b_center = Banners[Location.Center];

            Grid.SetZIndex(Banners[Location.Center], 2);
            GetOpacityGrid(b_center).Opacity = bopacity;

            ScaleTransform scale = new ScaleTransform();   //缩放  
            scale.CenterY = this.ActualHeight;
            TranslateTransform ts = new TranslateTransform();//平移


            TransformGroup group = new TransformGroup();
            group.Children.Add(scale);
            group.Children.Add(ts);


            Banners[Location.Center].RenderTransform = group;


            DoubleAnimation scaleAnimation = new DoubleAnimation()
            {
                From = 1,
                To = 0.95,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)


            };

            DoubleAnimation moveAnimation = new DoubleAnimation()
            {

                From = centerlocation,
                To = leftlocation,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)

            };

            //scaleAnimation.Completed += new EventHandler(scaleAnimation_Completed);  
            //  AnimationClock clock = scaleAnimation.CreateClock();  
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            //启动平移动画
            ts.BeginAnimation(TranslateTransform.XProperty, moveAnimation);








        }

        #endregion
        #region 右切换动画-右向中动画
        public void RighttoCenterAnimation()
        {
            b_right = Banners[Location.Right];
            //SetZindex(b_right);
            Grid.SetZIndex(Banners[Location.Right], 3);

            GetOpacityGrid(b_right).Opacity = 0;

            ScaleTransform scale = new ScaleTransform();   //缩放  

            //scale.CenterX = 0;
            scale.CenterY = this.ActualHeight;
            TranslateTransform ts = new TranslateTransform();//平移


            TransformGroup group = new TransformGroup();
            group.Children.Add(scale);
            group.Children.Add(ts);


            Banners[Location.Right].RenderTransform = group;


            DoubleAnimation scaleAnimation = new DoubleAnimation()
            {
                To = 1,
                From = 0.95,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)


            };

            DoubleAnimation moveAnimation = new DoubleAnimation()
            {

                From = rightlocation,
                To = centerlocation,
                EasingFunction = easeFunction,
                Duration = TimeSpan.FromSeconds(AnimationTime)

            };

            scaleAnimation.Completed += new EventHandler(RightAnimation_Completed);
            //  AnimationClock clock = scaleAnimation.CreateClock();  
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            //启动平移动画
            ts.BeginAnimation(TranslateTransform.XProperty, moveAnimation);









        }

        #endregion
        #region 动画结束
        private void RightAnimation_Completed(object sender, EventArgs e)
        {
            Banners[Location.Left] = b_center;
            Banners[Location.Center] = b_right;
            Banners[Location.Right] = b_left;
            isplay = false;
            TimerAnimationStart();
        }
        #endregion
        #endregion

        #region 初始化设置控件位置
        //定义一个枚举类用于标识左中右三个banner(border)，方便操作时获取到。
        public enum Location
        {
            Left, Center, Right
        }
        //设置三个border的大小和位置
        public void SetLocation(Location l, Border g)
        {

            //banner的大小
            g.Width = 540;
            g.Height = 200;

            //给除中间banner外的border缩小一些
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = 0.95;
            scaleTransform.ScaleY = 0.95;
            scaleTransform.CenterY = this.ActualHeight;
            //获取设置遮盖层的透明度
            Grid opacity_grid = GetOpacityGrid(g);
            opacity_grid.Opacity = bopacity;
            switch (l)
            {
                case Location.Left:

                    TranslateTransform tt_left = new TranslateTransform()
                    {
                        X = 0
                    };
                    TransformGroup group_left = new TransformGroup();
                    group_left.Children.Add(tt_left);
                    group_left.Children.Add(scaleTransform);
                    g.RenderTransform = group_left;

                    break;
                case Location.Center:
                    opacity_grid.Opacity = 0;

                    TransformGroup group_center = new TransformGroup();

                    //计算中心banner的x位置
                    centerlocation = (this.ActualWidth - g.ActualWidth) / 2;
                    TranslateTransform tt_center = new TranslateTransform()
                    {
                        X = centerlocation
                    };
                    group_center.Children.Add(tt_center);
                    g.RenderTransform = group_center;
                    Grid.SetZIndex(g, 3);
                    break;
                case Location.Right:
                    //Grid.SetZIndex(g, 3);

                    //计算右banner的X位置
                    rightlocation = (this.ActualWidth - Banners[Location.Left].ActualWidth * 0.95);

                    TranslateTransform tt_right = new TranslateTransform()
                    {
                        X = rightlocation
                    };
                    TransformGroup group_right = new TransformGroup();
                    //这里要注意先后顺序，否则位置会有偏差，必须先缩放再设置X坐标。
                    group_right.Children.Add(scaleTransform);
                    group_right.Children.Add(tt_right);
                    g.RenderTransform = group_right;


                    break;
            }

        }
        #endregion

        //获取透明覆盖层
        //代码来自：https://www.cnblogs.com/udoless/p/3381411.html
        #region 获取透明覆盖层
        public Grid GetOpacityGrid(DependencyObject g)
        {
            Grid opacity_grid = GetChild<Grid>(g, typeof(Grid));
            opacity_grid = GetChild<Grid>(opacity_grid, typeof(Grid));
            return opacity_grid;
        }
        public T GetChild<T>(DependencyObject obj, Type typename) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == typename))
                {
                    return ((T)child);
                }

            }
            return null;
        }
        #endregion

        #region 定时唤醒动画
        public void TimerAnimationStart()
        {
            if (timeranimation.IsEnabled == false)
            {

                timeranimation.Start();
            }
            timeranimation.Tick += (e, c) =>
            {

                timeranimation.Stop();
                if (isplay == false)
                {
                    RightAnimation();
                }
            };
        }
        #endregion
    }
}
