using System.Windows;
using System.Windows.Controls;

namespace WpfPanningItems
{
    /// <summary>
    /// PanningItemsView.xaml 的交互逻辑
    /// </summary>
    public partial class PanningItemsView : UserControl
    {
        public PanningItemsView()
        {
            InitializeComponent();
        }

        private void CanvasWithPhoto_IndexChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            CanvasWithPhoto c = (CanvasWithPhoto)sender;
            UpdataRadioBtn(c.Index);
        }

        private void UpdataRadioBtn(int index)
        {
            if (RbtnPanel.Children[index - 1] is RadioButton)
                ((RadioButton)RbtnPanel.Children[index - 1]).IsChecked = true;
        }

        private void RbtnPanel_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = (RadioButton)e.OriginalSource;
            for (int i = 0; i < RbtnPanel.Children.Count; i++)
            {
                if (btn == RbtnPanel.Children[i] && i + 1 != this.PhotoCanvas.Index)
                    this.PhotoCanvas.Index = i + 1;
            }
        }

        private void PanningItemsView_Loaded(object sender, RoutedEventArgs e)
        {
            int counts = this.PhotoCanvas.Children.Count;

            for (int i = 0; i < counts; i++)
            {
                RadioButton btn = new RadioButton();
                btn.Style = this.FindResource("RbtnStyle") as Style;
                btn.GroupName = "Index";
                RbtnPanel.Children.Add(btn);
            }

            ((RadioButton)RbtnPanel.Children[0]).IsChecked = true;

            var h = ImageGrid.ActualHeight;
            var w = ImageGrid.ActualWidth;
            PhotoCanvas.SetSize(w, h);
        }

        private void ImageGrid_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var h = e.NewSize.Height;
            var w = e.NewSize.Width;
            PhotoCanvas.SetSize(w, h);
        }
    }
}
