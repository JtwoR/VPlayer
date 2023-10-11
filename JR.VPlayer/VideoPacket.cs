using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JR.VPlayer
{
    public struct VideoPacket
    {
        public readonly AVPixelFormat Format;
        public readonly int Width;
        public readonly int Height;
        public readonly long Second;
        public byte[] Data;

        public VideoPacket(AVPixelFormat format, int width, int height, long second,byte[] data)
        {
            this.Format = format;
            this.Width = width;
            this.Height = height;
            this.Second = second;
            this.Data = data;
        }
    }
}
