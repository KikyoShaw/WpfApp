using System;
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
            this.VerifyCodeImageSource = CreateCheckCodeImage(CreateCode(4), ImageWidth, ImageHeight);
        }

        private void DoVerifyCodeImageSourceChange()
        {
           this.VerifyCodeImage.Source = this.VerifyCodeImageSource;
        }

        private string CreateCode(int strLength)
        {
            var charArray = StrCode.ToCharArray();
            var randomCode = "";
            var temp = -1;
            var rand = new Random(Guid.NewGuid().GetHashCode());
            for (var i = 0; i < strLength; i++)
            {
                if (temp != -1)
                    rand = new Random(i * temp * (int)DateTime.Now.Ticks);
                var t = rand.Next(StrCode.Length - 1);
                if (!string.IsNullOrWhiteSpace(randomCode))
                    while (randomCode.ToLower().Contains(charArray[t].ToString().ToLower()))
                        t = rand.Next(StrCode.Length - 1);
                if (temp == t)
                    return CreateCode(strLength);
                temp = t;

                randomCode += charArray[t];
            }

            return randomCode;
        }

        [Obsolete("GetFormattedText")]
        private ImageSource CreateCheckCodeImage(string checkCode, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(checkCode))
                return null;
            if (width <= 0 || height <= 0)
                return null;
            var drawingVisual = new DrawingVisual();
            var random = new Random(Guid.NewGuid().GetHashCode());
            using (var dc = drawingVisual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, new Pen(Foreground, 1), new Rect(_size));
                var formattedText = DrawingContextHelper.GetFormattedText(checkCode, Foreground,
                    FlowDirection.LeftToRight, 20, FontWeights.Bold);
                dc.DrawText(formattedText,
                    new Point((_size.Width - formattedText.Width) / 2, (_size.Height - formattedText.Height) / 2));

                for (var i = 0; i < 10; i++)
                {
                    var x1 = random.Next(width - 1);
                    var y1 = random.Next(height - 1);
                    var x2 = random.Next(width - 1);
                    var y2 = random.Next(height - 1);

                    dc.DrawGeometry(Brushes.Silver, new Pen(Brushes.Silver, 0.5D),
                        new LineGeometry(new Point(x1, y1), new Point(x2, y2)));
                }

                for (var i = 0; i < 100; i++)
                {
                    var x = random.Next(width - 1);
                    var y = random.Next(height - 1);
                    var c = new SolidColorBrush(Color.FromRgb((byte)random.Next(0, 255), (byte)random.Next(0, 255),
                        (byte)random.Next(0, 255)));
                    dc.DrawGeometry(c, new Pen(c, 1D),
                        new LineGeometry(new Point(x - 0.5, y - 0.5), new Point(x + 0.5, y + 0.5)));
                }

                dc.Close();
            }

            var renderBitmap = new RenderTargetBitmap(70, 23, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            return BitmapFrame.Create(renderBitmap);
        }

        [Obsolete("Obsolete")]
        private void VerifyCodeImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.VerifyCodeImageSource = CreateCheckCodeImage(CreateCode(4), ImageWidth, ImageHeight);
        }
    }
}
