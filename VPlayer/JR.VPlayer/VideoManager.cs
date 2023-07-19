using JR.VPlayer.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JR.VPlayer
{
    public class VideoManager
    {
        public ConcurrentQueue<VideoPacket> dataQueue = new ConcurrentQueue<VideoPacket>();
        private ConcurrentQueue<VideoPacket> _videoQueue = new ConcurrentQueue<VideoPacket>();
        private RenderHelper render=new RenderHelper();

        public void Run() {
            Thread.Sleep(20);

            for(;;)
            {
                VideoPacket video;
                while (dataQueue.TryDequeue(out video))
                {
                    _videoQueue.Enqueue(video);
                    Thread.Sleep(40);
                }
            }
        }

        public void Render() {
            VideoPacket video;
            if (_videoQueue.TryDequeue(out video))
            {
                render.DrawTexture(video.Width, video.Height, video.Data);
                video.Data = new byte[0];
            }
        }
    }
}
