using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace WpfSongLrc
{
    /// <summary>
    /// songLrc class
    /// </summary>
    public class MusicWord
    {
        public TimeSpan RunTime { get; set; }
        public TimeSpan StarTime { get; set; }
        public string SongWords { get; set; }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IEnumerable MusicWordArray
        {
            get => (IEnumerable)GetValue(MusicWordArrayProperty);
            set => SetValue(MusicWordArrayProperty, value);
        }

        public static readonly DependencyProperty MusicWordArrayProperty =
            DependencyProperty.Register("MusicWordArray", typeof(IEnumerable), typeof(MainWindow), 
                new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var musicWords = new List<MusicWord>
            {
                new MusicWord { RunTime = TimeSpan.FromSeconds(1), StarTime = TimeSpan.FromSeconds(0), SongWords = "演唱 : 周杰伦" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(1), StarTime = TimeSpan.FromSeconds(1), SongWords = "作词 : 方文山" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(1), StarTime = TimeSpan.FromSeconds(2), SongWords = "作曲 : 周杰伦" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(4.5), StarTime = TimeSpan.FromSeconds(3), SongWords = "兰亭临帖 行书如行云流水" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(5), StarTime = TimeSpan.FromSeconds(7.5), SongWords = "月下门推 心细如你脚步碎" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(6), StarTime = TimeSpan.FromSeconds(12.5), SongWords = "忙不迭 千年碑易拓却难拓你的美" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(4), StarTime = TimeSpan.FromSeconds(18.5), SongWords = "真迹绝 真心能给谁" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(5), StarTime = TimeSpan.FromSeconds(22.5), SongWords = "牧笛横吹 黄酒小菜又几碟" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(5), StarTime = TimeSpan.FromSeconds(27.5), SongWords = "夕阳余晖 如你的羞怯似醉" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(6), StarTime = TimeSpan.FromSeconds(33.5), SongWords = "摹本易写 而墨香不退与你同留余味" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(4), StarTime = TimeSpan.FromSeconds(37.5), SongWords = "一行朱砂 到底圈了谁" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(5), StarTime = TimeSpan.FromSeconds(41.5), SongWords = "无关风月 我题序等你回" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(5), StarTime = TimeSpan.FromSeconds(46.5), SongWords = "悬笔一绝 那岸边浪千叠" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(5), StarTime = TimeSpan.FromSeconds(51.5), SongWords = "情字何解 怎落笔都不对" },
                new MusicWord { RunTime = TimeSpan.FromSeconds(5), StarTime = TimeSpan.FromSeconds(56.5), SongWords = "而我独缺 你一生的了解" },
            };
            MusicWordArray = musicWords;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
