using JRVPlayer;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainPlayer player;
        public MainWindow()
        {
            InitializeComponent();
            var mainSettings = new GLWpfControlSettings { MajorVersion = 4, MinorVersion = 5, GraphicsProfile = ContextProfile.Compatability, GraphicsContextFlags = ContextFlags.Debug }; ;
            OpenTkControl.Start(mainSettings);

            player = new MainPlayer("D://test.mp4");

            //播放进度条
            this.Slider.Maximum = player.currentVideoLen;
            player.UpdateSlider(
                (second) =>
                {
                    this.Slider.Value = second;
                });
          
            player.Run();

            
        }

        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            player.Render();
        }

   

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // 获取新的值
            double newValue = e.NewValue;
            if (player != null)
            {
                player.Skip((int)newValue * 24000);
            }
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            // 处理拖动结束事件
            if (player != null)
            {
                player.Skip((int)this.Slider.Value*1000);
            }

        }
    }
}
