using System.Windows;
using WpfEditTime.ViewModel;

namespace WpfEditTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = MainVm.Instance;
        }
    }
}
