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
        private PathGeometry _geometry = new PathGeometry();
        private bool _bDown = false;

        //初始值
        private readonly double _originArea;
        //private bool _bShowResult = false;

        public MainWindow()
        {
            InitializeComponent();

            //绘制蒙层
            RectangleGeometry g = new RectangleGeometry
            {
                Rect = new Rect(0, 0, this.Width, this.Height)
            };
            //组合几何图形合并
            _geometry = Geometry.Combine(_geometry, g, GeometryCombineMode.Union, null);
            GridShadow.Clip = _geometry;
            //_originArea = _geometry.GetArea();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_bDown)
            {
                //鼠标滑动刮开区域大小
                EllipseGeometry g = new EllipseGeometry
                {
                    Center = e.GetPosition(GridShadow),
                    RadiusX = 20,
                    RadiusY = 20
                };
                //绘组合几何图形排除
                _geometry = Geometry.Combine(_geometry, g, GeometryCombineMode.Exclude, null);
                GridShadow.Clip = _geometry;

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
            _bDown = true;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _bDown = false;
        }
    }
}
