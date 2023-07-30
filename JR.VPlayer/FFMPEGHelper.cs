using FFmpeg.AutoGen;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGL.GL;

namespace JR.VPlayer
{
    public unsafe class FFMPEGHelper
    {
        private AVFormatContext* _context;

        private int _videoIndex;
        private int _audioIndex;
        public bool IsInit = false;

        public long len;
        public int width;
        public int height;

        private AudioManager _audioManager = new AudioManager();
        private VideoManager _videoManager = new VideoManager();

        unsafe public FFMPEGHelper(string path)
        {
            init(path);
        }

        private void init(string path)
        {
            AVFormatContext* ofmt_ctx = null;
            IsInit = ffmpeg.avformat_open_input(&ofmt_ctx, path, null, null) > 0 ? true : false;
            _context = ofmt_ctx;

            SetVideoAudioIndex();
            InitVideoDecoder();
            InitAudioDecoder();
        }

        private void SetVideoAudioIndex()
        {
            for (int i = 0; i < _context->nb_streams; i++)
            {
                if (_context->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    _videoIndex = i;
                }
                else if (_context->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
                {
                    _audioIndex = i;
                }
            }
        }

        #region 视频
        private AVCodecContext* _videoContext;
        private SwsContext* _videoSwContext;
        private void InitVideoDecoder()
        {
            _videoContext = ffmpeg.avcodec_alloc_context3(ffmpeg.avcodec_find_decoder(_context->streams[_videoIndex]->codecpar->codec_id));
            AVStream* stream = _context->streams[_videoIndex];

            AVCodec* _pCodec = ffmpeg.avcodec_find_decoder(_videoContext->codec_id);
            if (_pCodec == null) throw new Exception("没有找到编码器");

            ffmpeg.avcodec_parameters_to_context(_videoContext, stream->codecpar);
            if (ffmpeg.avcodec_open2(_videoContext, _pCodec, null) < 0) throw new Exception("编码器无法打开");

            len = (long)stream->duration * stream->time_base.num / stream->time_base.den;
            width = _videoContext->width;
            height = _videoContext->height;

            if (_videoContext->pix_fmt == AVPixelFormat.AV_PIX_FMT_NONE) _videoContext->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
            _videoSwContext = ffmpeg.sws_getCachedContext(null, width, height, _videoContext->pix_fmt, width, height, AVPixelFormat.AV_PIX_FMT_RGBA, ffmpeg.SWS_FAST_BILINEAR, null, null, null);
        }

        private void VideoDecode(AVPacket* packet, AVFrame* pFrame)
        {
            //解码一帧视频压缩数据，得到视频像素数据
            int flag = ffmpeg.avcodec_send_packet(_videoContext, packet);
            int check = ffmpeg.avcodec_receive_frame(_videoContext, pFrame);

            if (check == ffmpeg.AVERROR_EOF)
            {

            }
            else if (check >= 0)
            {
                int second = (int)(pFrame->pts * ffmpeg.av_q2d(_context->streams[_videoIndex]->time_base));

                AVFrame* RGBAFrame;
                RGBAFrame = ffmpeg.av_frame_alloc();
                ulong size = (ulong)ffmpeg.av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_RGBA, width, height, sizeof(int));
                byte* buffer = (byte*)ffmpeg.av_malloc(size);

                var data = new byte_ptrArray4();
                var linesize = new int_array4();
                ffmpeg.av_image_fill_arrays(ref data, ref linesize, buffer, AVPixelFormat.AV_PIX_FMT_RGBA, width, height, sizeof(int));

                for (uint i = 0; i < 4; i++)
                {
                    RGBAFrame->data[i] = data[i];
                    RGBAFrame->linesize[i] = linesize[i];
                }

                int c = ffmpeg.sws_scale(_videoSwContext, pFrame->data, pFrame->linesize, 0, height, RGBAFrame->data, RGBAFrame->linesize);

                byte[] arr = new byte[size];
                for (ulong i = 0; i < size; i++)
                {
                    arr[i] = RGBAFrame->data[0][i];
                }

                var item = new VideoPacket(_videoContext->pix_fmt, width, height, second, arr);

                _videoManager.dataQueue.Enqueue(item);
                ffmpeg.av_freep(&buffer);
                ffmpeg.av_frame_free(&RGBAFrame);
            }
        }
        #endregion

        #region 音频
        private AVCodecContext* _audioContext;
        private AVCodec* _audioCodec;
        private AVFrame* _audioRawBuffer;
        private SwrContext* _audioSwContext;
        private long _audioDuration;

        private unsafe void InitAudioDecoder()
        {
            _audioContext = ffmpeg.avcodec_alloc_context3(ffmpeg.avcodec_find_decoder(_context->streams[_audioIndex]->codecpar->codec_id));
            if (_audioContext == null)
            {
                throw new Exception("Motion: Failed to get audio codec context");
            }

            _audioCodec = ffmpeg.avcodec_find_decoder(_audioContext->codec_id);
            if (_audioCodec == null)
            {
                throw new Exception("Motion: Failed to find audio codec");
            }

            ffmpeg.avcodec_parameters_to_context(_audioContext, _context->streams[_audioIndex]->codecpar);
            if (ffmpeg.avcodec_open2(_audioContext, _audioCodec, null) != 0)
            {
                throw new Exception("Motion: Failed to load audio codec");
            }

            _audioRawBuffer = ffmpeg.av_frame_alloc();
            if (_audioRawBuffer == null)
            {
                throw new Exception("Motion: Failed to allocate audio buffer");
            }

            AVChannelLayout outChannelLayout;
            ffmpeg.av_channel_layout_default(&outChannelLayout, _audioContext->ch_layout.nb_channels);

            SwrContext* _audioSwContextInit = ffmpeg.swr_alloc();
            ffmpeg.swr_alloc_set_opts2(&_audioSwContextInit,
                                        &outChannelLayout, AVSampleFormat.AV_SAMPLE_FMT_S16,
                                        _audioContext->sample_rate, &_audioContext->ch_layout,
                                        _audioContext->sample_fmt, _audioContext->sample_rate, 0,
                                        null);
            ffmpeg.swr_init(_audioSwContextInit);
            _audioSwContext = _audioSwContextInit;
        }

        private void AudioDecode(AVPacket* packet, AVFrame* pFrame)
        {
            if (ffmpeg.avcodec_send_packet(_audioContext, packet) < 0) return;
            int check = ffmpeg.avcodec_receive_frame(_audioContext, _audioRawBuffer);

            if (check == ffmpeg.AVERROR_EOF)
            {

            }
            else if (check >= 0)
            {
                _audioDuration = ffmpeg.av_rescale_q(packet->duration, _context->streams[_audioIndex]->time_base, new AVRational() { num = 1, den = 1000000 });
                uint outSizeCandidate = Convert.ToUInt32(_audioContext->sample_rate * 8 * ((double)_audioDuration) / 1000000.0);
                byte* convertData = (byte*)ffmpeg.av_malloc(sizeof(byte) * outSizeCandidate);

                var left_channel = _audioRawBuffer->data[0];//左声道数据
                var right_channel = _audioRawBuffer->data[1];//右声道数据
                int out_samples = ffmpeg.swr_convert(_audioSwContext, &convertData, (int)outSizeCandidate, &left_channel, _audioRawBuffer->nb_samples);//重采样
                out_samples = ffmpeg.swr_convert(_audioSwContext, &convertData, (int)outSizeCandidate, &right_channel, _audioRawBuffer->nb_samples);//重采样

                int Audiobuffer_size = ffmpeg.av_samples_get_buffer_size(null, _audioContext->ch_layout.nb_channels, out_samples, AVSampleFormat.AV_SAMPLE_FMT_S16, 1);

                byte[] arr = new byte[Audiobuffer_size];
                for (long i = 0; i < Audiobuffer_size; i++)
                {
                    arr[i] = convertData[i];
                }

                var audioPacket = new AudioPacket(arr, Audiobuffer_size, _audioContext->sample_rate);

                ffmpeg.av_free(convertData);
                _audioManager.dataQueue.Enqueue(audioPacket);

            }
        }
        #endregion

        public void Run()
        {
            AVPacket* packet = ffmpeg.av_packet_alloc();
            AVFrame* pFrame = ffmpeg.av_frame_alloc();//解码缓冲区

            for(;;)
            {
                if (_videoManager.dataQueue.Count > 1000 || _audioManager.dataQueue.Count > 2000)
                {
                    Thread.Sleep(1);
                    continue;
                }

                if (ffmpeg.av_read_frame(_context, packet) < 0) break;

                //if (!isRun) continue;

                if (packet->stream_index == _videoIndex) VideoDecode(packet, pFrame);
                else if (packet->stream_index == _audioIndex) AudioDecode(packet, pFrame);

                ffmpeg.av_packet_unref(packet);
            }
            ffmpeg.av_free(packet);
        }

        public void VideoRun()=>_videoManager.Run();
        public void AudioRun() => _audioManager.Run();
        public void Render() => _videoManager.Render();

        ~FFMPEGHelper()
        {
            //todo 释放
            ffmpeg.avformat_free_context(_context);
            //ffmpeg.avcodec_flush_buffers(_codeContext);
        }
    }
}
