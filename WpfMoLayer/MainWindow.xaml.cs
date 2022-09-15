using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfMoLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PathGeometry _gridGeometry = new PathGeometry();
        private bool _isDown = false;

        //初始值
        private readonly double _originArea;
        //private bool _bShowResult = false;

        public MainWindow()
        {
            InitializeComponent();

            RectangleGeometry rg = new RectangleGeometry();
            rg.Rect = new Rect(0, 0, this.Width, this.Height);
            _gridGeometry = Geometry.Combine(_gridGeometry, rg, GeometryCombineMode.Union, null);
            GridShadow.Clip = _gridGeometry;
            _originArea = _gridGeometry.GetArea();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                EllipseGeometry rg = new EllipseGeometry();
                rg.Center = e.GetPosition(GridShadow);
                rg.RadiusX = 20;
                rg.RadiusY = 20;
                //排除几何图形
                _gridGeometry = Geometry.Combine(_gridGeometry, rg, GeometryCombineMode.Exclude, null);
                GridShadow.Clip = _gridGeometry;

                //var currentArea = _gridGeometry.GetArea();
                //if ((currentArea * 100 / _originArea) < 50 && !_bShowResult)
                //{
                //    _bShowResult = true;
                //    MessageBox.Show("");
                //}
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDown = true;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDown = false;
        }
    }
}
