using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public AudioManager() {
            var AudioDevice = ALC.OpenDevice("");
            var AudioContext = ALC.CreateContext(AudioDevice, new ALContextAttributes());
            ALC.MakeContextCurrent(AudioContext);
            sourceHandle = AL.GenSource();
        }

        public void Run() {
            Thread.Sleep(20);
            int[] m_buffers = new int[4];
            AL.GenBuffers(4, m_buffers);
            for(;;)
            {
                int check = 0;
                for (int i = 0; i < 4; i++)
                {
                    check = SoundCallback(m_buffers[i]);
                }
                Play();
                if (check == 0) break;
            }

            for(;;)
            {
                int processed = 0;
                AL.GetSource(sourceHandle, ALGetSourcei.BuffersProcessed, out processed);
                while (processed > 0)
                {
                    int bufferID = 0;
                    AL.SourceUnqueueBuffers(sourceHandle, 1, &bufferID);
                    SoundCallback(bufferID);
                    processed--;
                }
                Play();
                Thread.Sleep(1);
            }
        }

        private int SoundCallback(int index)
        {
            AudioPacket frame;
            if (!dataQueue.TryDequeue(out frame))
                return -1;
            AL.BufferData<byte>(index, ALFormat.Stereo16, frame.Data, frame.Samplerate);
            AL.SourceQueueBuffers(sourceHandle, 1, &index);
            frame.Data = new byte[0];
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
