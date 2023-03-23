using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRVPlayer
{
    public unsafe class VideoFrame
    {
        public readonly AVPixelFormat format;
        public readonly int width;
        public readonly int height;
        public readonly IntPtr data;
        public readonly int second;

        public VideoFrame(AVPixelFormat _format,int _width,int _height,int _second,IntPtr _data) {
            this.format = _format;
            this.width = _width;
            this.height = _height;
            this.second = _second;
            this.data = _data;

        }


    }
}
