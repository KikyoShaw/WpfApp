﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfShared.Helper;

namespace WpfVerifyCode
{
    /// <summary>
    /// VerifyCodeControl.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyCodeControl : UserControl
    {
        private const string ImageTemplateName = "PART_Image";
        private const string StrCode = "abcdefhkmnprstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(VerifyCodeControl),
                new PropertyMetadata(new PropertyChangedCallback(OnVerifyCodeImageSourcePropertyChange)));

        private Size _size = new Size(70, 23);

        [Obsolete("GetFormattedText")]
        public VerifyCodeControl()
        {
            InitializeComponent();
            Loaded += VerifyCodeControl_Loaded;
        }

        public ImageSource VerifyCodeImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(VerifyCodeControl),
                new PropertyMetadata(null));

        public int ImageHeight
        {
            get => (int)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(VerifyCodeControl),
                new PropertyMetadata(null));

        public int ImageWidth
        {
            get => (int)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        private static void OnVerifyCodeImageSourcePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VerifyCodeControl obj = ((VerifyCodeControl)d as VerifyCodeControl);
            if (obj != null)
                obj.DoVerifyCodeImageSourceChange();
        }

        [Obsolete("GetFormattedText")]
        private void VerifyCodeControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.VerifyCodeImageSource = CreateVerificationCodeImage(GetVerificationCodeByRand(4), ImageWidth, ImageHeight);
        }

        private void DoVerifyCodeImageSourceChange()
        {
           this.VerifyCodeImage.Source = this.VerifyCodeImageSource;
        }

        /// <summary>
        /// 获取验证码字符串
        /// </summary>
        /// <param name="strNum">验证码个数</param>
        /// <returns></returns>
        private string GetVerificationCodeByRand(int strNum)
        {
            try
            {
                var charArray = StrCode.ToCharArray();
                string randomCode = "";
                int index = -1;
                var rand = new Random(Guid.NewGuid().GetHashCode());
                for (var i = 0; i < strNum; i++)
                {
                    if (index != -1)
                        rand = new Random(i * index * (int)DateTime.Now.Ticks);
                    var t = rand.Next(StrCode.Length - 1);
                    if (!string.IsNullOrWhiteSpace(randomCode))
                        while (randomCode.ToLower().Contains(charArray[t].ToString().ToLower()))
                            t = rand.Next(StrCode.Length - 1);
                    if (index == t)
                        return GetVerificationCodeByRand(strNum);
                    index = t;

                    randomCode += charArray[t];
                }

                return randomCode;
            }
            catch /*(Exception e)*/
            {
                //Console.WriteLine(e);
                //throw;
            }

            return null;
        }

        /// <summary>
        /// 绘制并获取验证码图片资源
        /// </summary>
        /// <param name="code">验证码字符串</param>
        /// <param name="width">验证码宽度</param>
        /// <param name="height">验证码长度</param>
        /// <param name="hasLine">是否绘制线条</param>
        /// <param name="hasDot">是否绘制噪点</param>
        /// <returns></returns>
        [Obsolete("GetFormattedText")]
        private ImageSource CreateVerificationCodeImage(string code, int width, int height, bool hasLine = false, bool hasDot = false)
        {
            if (string.IsNullOrWhiteSpace(code) || width <= 0 || height <= 0)
                return null;

            var drawingVisual = new DrawingVisual();
            var random = new Random(Guid.NewGuid().GetHashCode());
            using (var dc = drawingVisual.RenderOpen())
            {
                //绘制背景
                dc.DrawRectangle(Brushes.White, null, new Rect(_size));
                //绘制验证码
                int index = 0;
                foreach (char c in code)
                {
                    var ctr = c.ToString();
                    var textColor = new SolidColorBrush(Color.FromRgb((byte)random.Next(0, 255),
                        (byte)random.Next(0, 255),
                        (byte)random.Next(0, 255)));
                    var formattedText = DrawingContextHelper.GetFormattedText(ctr, textColor,
                        FlowDirection.LeftToRight, 20, FontWeights.Bold);
                    dc.DrawText(formattedText,
                        new Point( 10 + index * 14, (_size.Height - formattedText.Height) / 2));
                    index++;
                }

                //绘制线条
                if (hasLine)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var x1 = random.Next(width - 1);
                        var y1 = random.Next(height - 1);
                        var x2 = random.Next(width - 1);
                        var y2 = random.Next(height - 1);
                        var c = new SolidColorBrush(Color.FromRgb((byte)random.Next(0, 255), (byte)random.Next(0, 255),
                            (byte)random.Next(0, 255)));
                        dc.DrawGeometry(c, new Pen(c, 0.5D),
                            new LineGeometry(new Point(x1, y1), new Point(x2, y2)));
                    }
                }

                //绘制噪点
                if (hasDot)
                {
                    for (var i = 0; i < 100; i++)
                    {
                        var x = random.Next(width - 1);
                        var y = random.Next(height - 1);
                        var c = new SolidColorBrush(Color.FromRgb((byte)random.Next(0, 255), (byte)random.Next(0, 255),
                            (byte)random.Next(0, 255)));
                        dc.DrawGeometry(c, new Pen(c, 1D),
                            new LineGeometry(new Point(x - 0.5, y - 0.5), new Point(x + 0.5, y + 0.5)));
                    }
                }

                dc.Close();
            }

            //转成图片
            var renderBitmap = new RenderTargetBitmap(70, 23, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            return BitmapFrame.Create(renderBitmap);
        }

        [Obsolete("Obsolete")]
        private void VerifyCodeImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.VerifyCodeImageSource = CreateVerificationCodeImage(GetVerificationCodeByRand(4), ImageWidth, ImageHeight);
        }
    }
}
