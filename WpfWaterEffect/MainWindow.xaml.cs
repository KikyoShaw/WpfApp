using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfWaterEffect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Timer _frameTimer = null;

        public MainWindow()
        {
            InitializeComponent();
            _frameTimer = new Timer();
            _frameTimer.Interval = 50;
            _frameTimer.Elapsed += OnFrameChange;
            _frameTimer.Start();
        }

        private void OnFrameChange(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (this.WaterControl.Value >= 100)
                {
                    this.WaterControl.Value = 100;
                    _frameTimer?.Stop();
                }
                else
                {
                    this.WaterControl.Value++;
                }
            });
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.WaterControl.Value = 0;
            _frameTimer?.Start();
        }
    }
}
