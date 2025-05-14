using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace WpfPanningItems
{
    public class MainVm : ViewModelBase
    {
        private static readonly Lazy<MainVm> Lazy = new Lazy<MainVm>(() => new MainVm());

        public static MainVm Instance => Lazy.Value;

        public MainVm()
        {

        }

        //左侧图片
        private BitmapImage _leftImage;
        public BitmapImage LeftImage
        {
            get => _leftImage;
            set => Set("LeftImage", ref _leftImage, value);
        }

        //中间图片
        private BitmapImage _middleImage;
        public BitmapImage MiddleImage
        {
            get => _middleImage;
            set => Set("MiddleImage", ref _middleImage, value);
        }

        private double _gameBannerWidth = 925d;
        public double GameBannerWidth
        {
            get => _gameBannerWidth;
            set => Set("dActivityCardWidth", ref _gameBannerWidth, value);
        }
        private double _gameBannerHeight = 303d;
        public double GameBannerHeight
        {
            get => _gameBannerHeight;
            set => Set("dActivityCardHeight", ref _gameBannerHeight, value);
        }
    }
}
