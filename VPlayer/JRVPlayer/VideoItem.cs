using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JRVPlayer
{
    public unsafe class VideoItem
    {
        public readonly string format;
        public long len;
        public readonly int width;
        public readonly int height;
        public Queue<VideoFrame> dataQueue;

        public bool isRun = false;
        private readonly int _videoIndex;

        private AVFormatContext* _context;//媒体上下文
        private AVCodecContext* _codeContext;//视频上下文
        
        AVPacket* packet = (AVPacket*)ffmpeg.av_malloc((ulong)sizeof(AVPacket));//帧缓冲区
        AVFrame* pFrame = ffmpeg.av_frame_alloc();//解码缓冲区
        AVFrame* RGBAFrame;
        SwsContext* sws_ctx;

        public VideoItem(AVFormatContext* context,int index) {
            _context = context;
            _videoIndex = index;

            init();
            format = _context->iformat->name->ToString();
            width = _codeContext->width;
            height = _codeContext->height;

            CreateRGBAFrame(AVPixelFormat.AV_PIX_FMT_RGBA,4);//todo 目前逻辑是把视频解码转成jgba格式再使用opengl进行渲染，转yuv暂未开发
            CreateSwsContent();//裁剪
        }

        private void init() {
            FindDecoder();//获取视频编码信息    
        }

        private void FindDecoder() {
            _codeContext = ffmpeg.avcodec_alloc_context3(ffmpeg.avcodec_find_decoder(_context->streams[_videoIndex]->codecpar->codec_id));
            AVStream* stream = _context->streams[_videoIndex];

            AVCodec* _pCodec = ffmpeg.avcodec_find_decoder(_codeContext->codec_id);
            if (_pCodec == null) throw new Exception("没有找到编码器");

            ffmpeg.avcodec_parameters_to_context(_codeContext, stream->codecpar);
            if (ffmpeg.avcodec_open2(_codeContext, _pCodec, null) < 0) throw new Exception("编码器无法打开");

            len = (long)stream->duration * stream->time_base.num / stream->time_base.den;
        }


        private void CreateRGBAFrame(AVPixelFormat format,int num)
        {
            RGBAFrame = ffmpeg.av_frame_alloc();
            ulong size = (ulong)ffmpeg.av_image_get_buffer_size(format, width, height, sizeof(int));
            byte* buffer = (byte*)ffmpeg.av_malloc(size);

            var data = new byte_ptrArray4();
            var linesize = new int_array4();
            ffmpeg.av_image_fill_arrays(ref data, ref linesize, buffer, format, width, height, sizeof(int));
            for (uint i = 0; i < num; i++)
            {
                RGBAFrame->data[i] = data[i];
                RGBAFrame->linesize[i] = linesize[i];
            }
        }

        private void CreateSwsContent() {
            if (_codeContext->pix_fmt == AVPixelFormat.AV_PIX_FMT_NONE) _codeContext->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
            sws_ctx= ffmpeg.sws_getCachedContext(null,width,height, _codeContext->pix_fmt, width, height, AVPixelFormat.AV_PIX_FMT_RGBA, ffmpeg.SWS_FAST_BILINEAR, null, null, null);
        }

        public void VideoRun() {

            while ((ffmpeg.av_read_frame(_context, packet)) >= 0)
            {
                if (!isRun) break;
                //只要视频压缩数据（根据流的索引位置判断）
                if (packet->stream_index == _videoIndex)
                {
                    //解码一帧视频压缩数据，得到视频像素数据
                    int flag = ffmpeg.avcodec_send_packet(_codeContext, packet);
                    int check = ffmpeg.avcodec_receive_frame(_codeContext, pFrame);

                    if (check == ffmpeg.AVERROR_EOF)
                    {

                    }
                    else if (check >= 0)
                    {
                        int second = (int)(pFrame->pts * ffmpeg.av_q2d(_context->streams[_videoIndex]->time_base));
                        int c = ffmpeg.sws_scale(sws_ctx, pFrame->data, pFrame->linesize, 0, height, RGBAFrame->data, RGBAFrame->linesize);

                        dataQueue.Enqueue(new VideoFrame(_codeContext->pix_fmt, width,height, second, new IntPtr(RGBAFrame->data[0])));

                        Thread.Sleep(40);
                    }
                }
                ffmpeg.av_packet_unref(packet);
            }
            //释放资源
            ffmpeg.av_free(packet);
        }

        public void SkipVideo(int SeekTimeInSeconds) {
            AVStream* videoStream = _context->streams[_videoIndex];
            long seekTarget = (long)(SeekTimeInSeconds / av_q2d(videoStream->time_base) / 1000);
            dataQueue.Clear();
            var error = ffmpeg.av_seek_frame(_context, _videoIndex, seekTarget, ffmpeg.AVSEEK_FLAG_BACKWARD);
            if (error < 0)
            {
                // 跳转失败，处理错误
                Console.WriteLine("Failed to seek to specified time. Error code: {0}", error);
                //ffmpeg.avformat_close_input(&_context);
                return;
            }
        }

        private static double av_q2d(AVRational a)
        {
            return a.num / (double)a.den;
        }
    }

}
