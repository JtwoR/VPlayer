using JR.VPlayer.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly AudioManager _audioManager;
        private readonly VPlayer _vPlayer;

        public VideoManager(AudioManager audioManager, VPlayer vPlayer) {
            _audioManager = audioManager;
            _vPlayer= vPlayer;
        }

        public void Run() {
            Thread.Sleep(20);

            for(;;)
            {
                VideoPacket video;
                while (dataQueue.TryDequeue(out video))
                {
                    _vPlayer.PlayEvent.WaitOne();

                    //音频帧与图像帧有误差，以音频时间为准，对图像播放速度进行调整
                    if (_audioManager.second <= video.Second)
                    {
                        _videoQueue.Enqueue(video);
                        Thread.Sleep(40);
                    }
                    else {
                        _videoQueue.Enqueue(video);
                        Thread.Sleep(20);
                    }
                    
                }
            }
        }

        public void Render() {
            VideoPacket video;
            if (_videoQueue.TryDequeue(out video))
            {
                //_vPlayer.PlayEvent.WaitOne();
                render.DrawTexture(video.Width, video.Height, video.Data);
                video.Data = null;
            }
        }
    }
}
