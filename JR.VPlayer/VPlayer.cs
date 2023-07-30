using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JR.VPlayer
{
    public class VPlayer
    {
        private readonly FFMPEGHelper _ffmpegHelper;
        private readonly string _path;
        public VPlayer(string path) {
            _path = path;
            _ffmpegHelper = new FFMPEGHelper(path);
        }

        public void Run() {
            Task.Run(() => { _ffmpegHelper.Run(); });
            Task.Run(() => { _ffmpegHelper.VideoRun(); });
            Task.Run(() => { _ffmpegHelper.AudioRun(); });
        }

        public void Render()=>_ffmpegHelper.Render();
    }
}
