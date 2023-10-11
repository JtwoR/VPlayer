using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JR.VPlayer
{
    public delegate void SilderValueChange(long i);
    public delegate void PlayButtonClick(ManualResetEvent manual);

    public class VPlayer
    {
        private readonly FFMPEGHelper _ffmpegHelper;
        private readonly string _path;
        
        public long VideoLen { get { return _ffmpegHelper.len; } }

        public SilderValueChange SilderValueChange { get; set; }
        public PlayButtonClick PlayButtonClick { get; set; }

        public ManualResetEvent PlayEvent =new ManualResetEvent(true);

        public VPlayer(string path) {
            _path = path;
            _ffmpegHelper = new FFMPEGHelper(path,this);
        }

        public void Run() {
            Task.Run(() => { _ffmpegHelper.Run(); });
            Task.Run(() => { _ffmpegHelper.VideoRun(); });
            Task.Run(() => { _ffmpegHelper.AudioRun(); });

            Task.Run(() =>
            {
                for (; ; )
                {
                    if (SilderValueChange != null) SilderValueChange(_ffmpegHelper.second);
                    Thread.Sleep(1);
                }
            });
        }

        public void Render()=>_ffmpegHelper.Render();
        public void SkipVideo(long second) => _ffmpegHelper.SkipVideo(second);
    }
}
