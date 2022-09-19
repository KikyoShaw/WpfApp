using System;
using System.Collections;
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

namespace WpfTurntable
{
    public class MenuItemModel
    {
        public double Angle { get; set; }
        public string Title { get; set; }
        public Brush FillColor { get; set; }
        public ImageSource IconImage { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IEnumerable MenuArray
        {
            get => (IEnumerable)GetValue(MenuArrayProperty);
            set => SetValue(MenuArrayProperty, value);
        }

        public static readonly DependencyProperty MenuArrayProperty =
            DependencyProperty.Register("MenuArray", typeof(IEnumerable), typeof(MainWindow), 
                new PropertyMetadata(null));


        public List<int> ListAngle
        {
            get => (List<int>)GetValue(ListAngleProperty);
            set => SetValue(ListAngleProperty, value);
        }

        public static readonly DependencyProperty ListAngleProperty =
            DependencyProperty.Register("ListAngle", typeof(List<int>), typeof(MainWindow), 
                new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ListAngle = new List<int>();
            var menuItemModels = new List<MenuItemModel>();
            var angle = 0;
            var anglePrize = 2000;
            for (int i = 0; i <= 7; i++)
            {
                var prizeTitle = i == 0 ? "谢谢参与" : $"{i}等奖";
                angle += 45;
                anglePrize += 45;
                ListAngle.Add(anglePrize);
                menuItemModels.Add(new MenuItemModel { Angle = angle, Title = prizeTitle });
            }

            MenuArray = menuItemModels;
        }
    }
}
