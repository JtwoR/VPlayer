using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRVPlayer
{
    public unsafe class FFMPEGHelper
    {
        public AVFormatContext* context = null;
        private int? _videoIndex;
        private int? _audioIndex;
        public bool IsInit = false;

        unsafe public FFMPEGHelper(string path) {
            init(path);
        }

        public VideoItem CreateVideoItem() {
            if (!_videoIndex.HasValue) throw new Exception("初始化无视频流信息");
            return new VideoItem(context, _videoIndex.Value);
        }

        private void init(string path)
        {
            AVFormatContext* ofmt_ctx = null;
            IsInit = ffmpeg.avformat_open_input(&ofmt_ctx, path, null, null) > 0 ? true : false;
            context = ofmt_ctx;

            SetVideoAudioIndex();
        }

        private void SetVideoAudioIndex()
        {
            for (int i = 0; i < context->nb_streams; i++)
            {
                if (context->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    _videoIndex = i;
                }
                else if (context->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
                {
                    _audioIndex = i;
                }
            }
        }


    }
}
