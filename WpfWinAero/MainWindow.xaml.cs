using Panuon.UI.Silver;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace WpfWinAero
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowX
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        private static bool top = false;
        private static SolidColorBrush brush = new SolidColorBrush();
        private static SolidColorBrush brushfont = new SolidColorBrush();
        private static MainWindow windowMain;

        public MainWindow()
        {
            InitializeComponent();

            WindowBlur.SetIsEnabled(this, true);
           // windowMain = this;
            ////刷新窗口前景色
           // UpdateUi();
            test();
        }

        private int num = 10;
        private bool result = false;

        private void test()
        {
            int temp = 0;
            try
            {
                if (!result)
                    return;
                temp = 20;
            }
            catch /*(Exception e)*/
            {
                //Console.WriteLine(e);
                //throw;
            }
            finally
            {
                num = temp;
            }
            
        }

        private void UpdateUi()
        {
            this.Topmost = top;
            Point inScreen = new Point(0, 0);
            try
            {
                inScreen = PointToScreen(new System.Windows.Point(0, 0));
            }
            catch /*(Exception e)*/
            {
                //Console.WriteLine(e);
                //throw;
            }
            Color color = GetColor((int)inScreen.X - 3, (int)inScreen.Y - 3);
            //MessageBox.Show(color.ToString());
            if (color.R * 0.299 + color.G * 0.578 + color.B * 0.114 >= 192)
            { //浅色
                brush.Color = Color.FromArgb(255, 0, 0, 0);
                brushfont.Color = Color.FromArgb(255, 255, 255, 255);
                WindowXCaption.SetForeground(this, brush);
            }
            else
            {  //深色
                brush.Color = Color.FromArgb(255, 255, 255, 255);
                brushfont.Color = Color.FromArgb(255, 0, 0, 0);
                WindowXCaption.SetForeground(this, brush);
            }
        }

        private Color GetColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero); uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb(byte.Parse(255.ToString()), (byte)(pixel & 0x000000FF), (byte)((int)(pixel & 0x0000FF00) >> 8), (byte)((int)(pixel & 0x00FF0000) >> 16));
            return color;
        }
    }
}
