using System;
using GalaSoft.MvvmLight;

namespace WpfEditTime.ViewModel
{
    public class MainVm : ViewModelBase
    {
        private static readonly Lazy<MainVm> Lazy = new Lazy<MainVm>(() => new MainVm());
        public static MainVm Instance => Lazy.Value;

        public MainVm()
        {

        }

        //时
        private int _hour = 0;
        public int Hour
        {
            get => _hour;
            set => Set("Hour", ref _hour, value);
        }

        //分
        private int _minute = 0;
        public int Minute
        {
            get => _minute;
            set => Set("Minute", ref _minute, value);
        }

        //秒
        private int _second = 0;
        public int Second
        {
            get => _second;
            set => Set("Second", ref _second, value);
        }
    }
}
