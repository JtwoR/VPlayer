using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JR.VPlayer
{
    public unsafe class AudioManager
    {
        public ConcurrentQueue<AudioPacket> dataQueue=new ConcurrentQueue<AudioPacket>();
        private int sourceHandle;
        public long second;
        private readonly VPlayer _vPlayer;

        public AudioManager(VPlayer vPlayer) {
            var AudioDevice = ALC.OpenDevice("");
            var AudioContext = ALC.CreateContext(AudioDevice, new ALContextAttributes());
            ALC.MakeContextCurrent(AudioContext);
            sourceHandle = AL.GenSource();
            _vPlayer = vPlayer;
        }

        int[] m_buffers = new int[4];

        public void Run() {
            Thread.Sleep(20);
            
            AL.GenBuffers(4, m_buffers);
            for (; ; )
            {
                int check = 0;
                for (int i = 0; i < 4; i++)
                {
                    _vPlayer.PlayEvent.WaitOne();
                    //check = SoundCallback(m_buffers[i]);
                    AudioPacket frame;
                    if (dataQueue.TryDequeue(out frame))
                    {
                        check = SoundCallback(m_buffers[i], frame);
                    }
                    else
                    {
                        check = -1;
                    }
                }
                Play();
                if (check == 0) break;
            }

            for (;;)
            {
                _vPlayer.PlayEvent.WaitOne();
                int processed = 0;
                AL.GetSource(sourceHandle, ALGetSourcei.BuffersProcessed, out processed);
                int check = 0;
                while (processed > 0)
                {
                    _vPlayer.PlayEvent.WaitOne();
                    AudioPacket frame;
                    if (dataQueue.TryDequeue(out frame))
                    {
                        int bufferID = 0;
                        AL.SourceUnqueueBuffers(sourceHandle, 1, &bufferID);
                        check = SoundCallback(bufferID,frame);
                    }
                    processed--;
                }
                Play();
                Thread.Sleep(1);
            }
        }

        public void ClearBuffers() {

            //AL.SourceStop(sourceHandle);
            //for (int i = 0; i < 4; i++)
            //{
            //    //check = SoundCallback(m_buffers[i]);
            //    int index = m_buffers[i];
            //    AL.SourceUnqueueBuffers(sourceHandle, 1, &index);
            //    AL.BufferData<byte>(index, ALFormat.Stereo16, new byte[0], 0);
            //    AL.SourceQueueBuffers(sourceHandle, 1, &index);
            //    Play();
            //}

            //AL.SourceStop(sourceHandle);
            int processed = 0;
            AL.GetSource(sourceHandle, ALGetSourcei.BuffersProcessed, out processed);
            while (processed > 0)
            {
                int bufferID = 0;
                AL.SourceUnqueueBuffers(sourceHandle, 1, &bufferID);
                AL.BufferData<byte>(bufferID, ALFormat.Stereo16, new byte[0], 0);
                AL.SourceQueueBuffers(sourceHandle, 1, &bufferID);
                //Play();
                processed--;
            }
        }

        private int SoundCallback(int index, AudioPacket frame)
        {
            
            //AudioPacket frame;
            //if (!dataQueue.TryDequeue(out frame)) {
            //    return -1;
            //}  
            AL.BufferData<byte>(index, ALFormat.Stereo16, frame.Data, frame.Samplerate);
            AL.SourceQueueBuffers(sourceHandle, 1, &index);
            frame.Data = null;
            second = frame.second;
            return 0;
        }

        private int Play()
        {
            int state;
            AL.GetSource(sourceHandle, ALGetSourcei.SourceState, out state);
            if (state == ALSourceState.Stopped.GetHashCode() || state == ALSourceState.Initial.GetHashCode())
            {
                AL.SourcePlay(sourceHandle);
            }

            return 0;
        }
    }
}
