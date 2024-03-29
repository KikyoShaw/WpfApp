﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfEditTime.Model;

namespace WpfEditTime.UI
{
    public class FxButton : Button
    {
        static FxButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FxButton), new FrameworkPropertyMetadata(typeof(FxButton)));
        }

        public ButtonType ButtonType
        {
            get => (ButtonType)GetValue(ButtonTypeProperty);
            set => SetValue(ButtonTypeProperty, value);
        }

        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register("ButtonType", typeof(ButtonType), typeof(FxButton), new PropertyMetadata(ButtonType.Normal));


        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(FxButton), new PropertyMetadata(null));



        public Thickness IconMargin
        {
            get => (Thickness)GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(FxButton), new PropertyMetadata(new Thickness(0)));



        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FxButton), new PropertyMetadata(new CornerRadius(0)));


        public Brush MouseOverForeground
        {
            get => (Brush)GetValue(MouseOverForegroundProperty);
            set => SetValue(MouseOverForegroundProperty, value);
        }

        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(FxButton), new PropertyMetadata());


        public Brush MousePressedForeground
        {
            get => (Brush)GetValue(MousePressedForegroundProperty);
            set => SetValue(MousePressedForegroundProperty, value);
        }

        public static readonly DependencyProperty MousePressedForegroundProperty =
            DependencyProperty.Register("MousePressedForeground", typeof(Brush), typeof(FxButton), new PropertyMetadata());


        public Brush MouseOverBorderbrush
        {
            get => (Brush)GetValue(MouseOverBorderbrushProperty);
            set => SetValue(MouseOverBorderbrushProperty, value);
        }

        public static readonly DependencyProperty MouseOverBorderbrushProperty =
            DependencyProperty.Register("MouseOverBorderbrush", typeof(Brush), typeof(FxButton), new PropertyMetadata());


        public Brush MouseOverBackground
        {
            get => (Brush)GetValue(MouseOverBackgroundProperty);
            set => SetValue(MouseOverBackgroundProperty, value);
        }

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(FxButton), new PropertyMetadata());


        public Brush MousePressedBackground
        {
            get => (Brush)GetValue(MousePressedBackgroundProperty);
            set => SetValue(MousePressedBackgroundProperty, value);
        }

        public static readonly DependencyProperty MousePressedBackgroundProperty =
            DependencyProperty.Register("MousePressedBackground", typeof(Brush), typeof(FxButton), new PropertyMetadata());

        public static T? FindVisualParent<T>(DependencyObject? obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T obj1)
                    return obj1;

                obj = VisualTreeHelper.GetParent(obj)!;
            }

            return null;
        }
    }
}
