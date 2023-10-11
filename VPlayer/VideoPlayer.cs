using AduSkin.Controls.Metro;
using AduSkin.Utility.AduMethod;
using JR.VPlayer;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VPlayer;

namespace AduVideoPlayer.Control
{
    public class VideoPlayer : System.Windows.Controls.Control
    {

        private JR.VPlayer.VPlayer _vPlayer { get; set; }

        /// <summary>
        /// 进度条
        /// </summary>
        public AduFlatSilder PART_Slider { get; set; }

        /// <summary>
        /// 开始按钮
        /// </summary>
        public Button PART_Btn_Play { get; set; }

        /// <summary>
        /// 暂停按钮
        /// </summary>
        public Button PART_Btn_Pause { get; set; }

        /// <summary>
        /// Loading动画 todo UI的布局要调整
        /// </summary>
        public AduLoading PART_Loading { get; set; }

        /// <summary>
        /// 总时长-文本
        /// </summary>
        public Run PART_Time_Total { get; set; }

        /// <summary>
        /// 当前时间-文本
        /// </summary>
        public Run PART_Time_Current { get; set; }

        //public bool IsDropSlider { get; set; } = true;

        public VideoPlayer() {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoPlayer), new FrameworkPropertyMetadata(typeof(VideoPlayer)));
           
        }

        public override void OnApplyTemplate()
        {
            if (this.GetTemplateChild("PART_Slider") != null) this.PART_Slider = this.GetTemplateChild("PART_Slider") as AduFlatSilder;
            if (this.GetTemplateChild("PART_Btn_Play") != null) this.PART_Btn_Play = this.GetTemplateChild("PART_Btn_Play") as Button;
            if (this.GetTemplateChild("PART_Btn_Pause") != null) this.PART_Btn_Pause = this.GetTemplateChild("PART_Btn_Pause") as Button;
            if (this.GetTemplateChild("PART_Loading") != null) this.PART_Loading = this.GetTemplateChild("PART_Loading") as AduLoading;
            if (this.GetTemplateChild("PART_Time_Total") != null) this.PART_Time_Total = this.GetTemplateChild("PART_Time_Total") as Run;
            if (this.GetTemplateChild("PART_Time_Current") != null) this.PART_Time_Current = this.GetTemplateChild("PART_Time_Current") as Run;

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.PART_Slider.Maximum = _vPlayer.VideoLen* 1000;
                this.PART_Time_Total.Text = TimeSpan.FromMilliseconds(_vPlayer.VideoLen * 1000).ToString("hh\\:mm\\:ss");
            });

            this.PART_Slider.AllowDrop = true;
            this.PART_Slider.DropValueChanged += PART_Slider_DropValueChanged;
            
            if (this.GetTemplateChild("PART_Btn_Play") != null) this.PART_Btn_Play.Click += PART_Btn_Play_Click;
            if (this.GetTemplateChild("PART_Btn_Pause") != null) this.PART_Btn_Pause.Click += PART_Btn_Pause_Click;

        }

        private void PART_Slider_DropValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {     
            _vPlayer.SkipVideo((long)e.NewValue);
        }

        private void PART_Btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            _vPlayer.PlayEvent.Reset();
            this.PART_Btn_Play.Visibility = Visibility.Visible;
            this.PART_Btn_Pause.Visibility = Visibility.Collapsed;
            //this.PART_Loading.IsActived = false;
            //this.PART_Loading.Visibility = Visibility.Collapsed;
        }

        private void PART_Btn_Play_Click(object sender, RoutedEventArgs e)
        {
            _vPlayer.PlayEvent.Set();
            this.PART_Btn_Play.Visibility = Visibility.Collapsed;
            this.PART_Btn_Pause.Visibility = Visibility.Visible;
            //this.PART_Loading.IsActived = true;
            //this.PART_Loading.Visibility = Visibility.Visible;
        }

        public void SetVPlayer(JR.VPlayer.VPlayer VPlayer) {
            this._vPlayer = VPlayer;
        }

        public void SilderValueChange(long i) {
            if (Application.Current == null) return;
            if (this.PART_Slider == null) return;
            
            Application.Current.Dispatcher.Invoke(() =>
            {
                //拖拽的判断是使用开源的AduSkin改的，监听了拖拽进度条方块的事件
                if (this.PART_Slider.IsDropSlider) {

                    return;
                }
                if (this.PART_Slider!=null) this.PART_Slider.Value = i;
                if (this.PART_Time_Current != null) this.PART_Time_Current.Text = TimeSpan.FromMilliseconds(i).ToString("hh\\:mm\\:ss");
            });
        }


    }


}