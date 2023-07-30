using OpenTK.Audio.OpenAL;
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
        JR.VPlayer.VPlayer player;
        public MainWindow(string path)
        {
            InitializeComponent();

            var mainSettings = new GLWpfControlSettings { MajorVersion = 4, MinorVersion = 5, GraphicsProfile = ContextProfile.Compatability, GraphicsContextFlags = ContextFlags.Debug }; ;
            OpenTkControl.Start(mainSettings);
            player = new JR.VPlayer.VPlayer(path);
            player.Run();


            //播放进度条
            //this.Slider.Maximum = player.currentVideoLen;
            //player.UpdateSlider(
            //    (second) =>
            //    {
            //        this.Slider.Value = second;
            //    });

        }

        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            player.Render();
        }

   

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //// 获取新的值
            //double newValue = e.NewValue;
            //if (player != null)
            //{
            //    player.Skip((int)newValue * 24000);
            //}
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //if (player != null)
            //{
            //    player.Skip((int)this.Slider.Value*1000);
            //    player.sliderStatus = SliderStatus.Complete;
            //}

        }
        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            //if (player != null)
            //{
            //    player.sliderStatus = SliderStatus.Start;
            //}

        }

        private void Thumb_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (player != null)
            //{
            //    player.Skip((int)this.Slider.Value * 1000);
            //    player.sliderStatus = SliderStatus.Complete;
            //}
        }

        private void Thumb_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (player != null)
            //{
            //    player.Skip((int)this.Slider.Value * 1000);
            //    player.sliderStatus = SliderStatus.Complete;
            //}
        }

        private void Slider_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Slider_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Slider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
