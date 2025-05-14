using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfPanningItems
{
    internal class ClippingBorder : Border
    {
        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.Register("BorderRadius", typeof(CornerRadius), typeof(ClippingBorder), new PropertyMetadata(new CornerRadius(0d)));
        public CornerRadius BorderRadius
        {
            get => (CornerRadius)GetValue(BorderRadiusProperty);
            set => SetValue(BorderRadiusProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BorderRadiusProperty)
            {
                UpdateClip();
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateClip();
        }

        private void UpdateClip()
        {
            var clip = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight), BorderRadius.TopLeft, BorderRadius.TopLeft);
            clip.Freeze();
            Clip = clip;
        }
    }
}
