using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using GalaSoft.MvvmLight;

namespace WpfScrollViewer
{
    public class DataInfo
    {
        public string Name { get; set; }
    }

    public enum MenuEnum
    {
        Navigation3D,
        BasicControls,
        PanningItems,
        BreatheLight,
        RingLoading,
        BallLoading,
        StreamerLoading,
        WaitLoading,
        CycleLoading,
        RollLoading,
        CutImage,
        CropAvatar,
        AnimationAudio,
        AMap,
        ThumbAngle,
        VerifyCode,
        CircularMenu,
        ChatEmoji,
        ProgressBar,
        Dashboard,
        PieControl,
        RoundMenu,
        Password,
        SongWords,
        TimeLine,
        Carousel,
        CarouselEx,
        Pagination,
        ScrollViewer,
        OtherControls,
        ScreenCut,
        TransitionPanel,
        SpotLight,
        DrawerMenu,
        RadarChart,
        BasicBarChart,
        ZooSemy,
        RulerControl,
        RainbowBtn,
        RoundPicker,
        LineChart,
        LogoAnimation,
        Thermometer,
        SnowCanvas,
        LoginWindow,
        Drawing,
        SpeedRockets,
        CountdownTimer,
        NumberCard,
        CropControl,
        Desktop,
        DrawPrize,
        EdgeLight,
        StarrySky,
        Shake,
        BubblleControl,
        CanvasHandWriting,
        Barrage,
        VirtualizingWrapPanel,
        TaskbarInfo
    }

    public class DataVm : ViewModelBase
    {
        private static readonly Lazy<DataVm>
            Lazy = new Lazy<DataVm>(() => new DataVm());

        public static DataVm Instance => Lazy.Value;
        public DataVm()
        {
            NavigateMenuModelList = new ObservableCollection<ListBoxItem>();
            foreach (MenuEnum menuEnum in Enum.GetValues(typeof(MenuEnum)))
            {
                NavigateMenuModelList.Add(new ListBoxItem { Content = menuEnum.ToString() });
            }
            NavigateMenuModelList.Add(new ListBoxItem { Content = "持续更新中" });
        }

        private ObservableCollection<ListBoxItem> _navigateMenuModelList;

        public ObservableCollection<ListBoxItem> NavigateMenuModelList
        {
            get => _navigateMenuModelList;
            set => Set("NavigateMenuModelList", ref _navigateMenuModelList, value);
        }

        private DataInfo _tempDataInfo;
        /// <summary>
        /// 当前选中
        /// </summary>
        public DataInfo TempDataInfo
        {
            get => _tempDataInfo;
            set => Set("TempDataInfo", ref _tempDataInfo, value);
        }

    }
}
