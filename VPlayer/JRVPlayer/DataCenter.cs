using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRVPlayer
{
    public class DataCenter
    {
        private FFMPEGHelper FFMPEG;
        private VideoItem _videoItem;

        public Queue<VideoFrame> videoData=new Queue<VideoFrame>();
        public long currentVideoLen;

        public DataCenter(string path) {
            FFMPEG = new FFMPEGHelper(path);

            _videoItem = FFMPEG.CreateVideoItem();
            _videoItem.dataQueue = videoData;
            currentVideoLen = _videoItem.len;
        }

        public void Run() {
            Task.Run(() =>
            {
                _videoItem.isRun = true;
                _videoItem.VideoRun();
            });
        }

        public void Skip(int SeekTimeInSeconds) {
            _videoItem.SkipVideo(SeekTimeInSeconds);
        }
    }
}
