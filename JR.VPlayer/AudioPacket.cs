using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JR.VPlayer
{
    public unsafe struct AudioPacket
    {
        public readonly int Samplerate;
        public byte[] Data;
        public long second;

        public AudioPacket(byte[] data, int size, int samplerate, long second)
        {
            this.Data = data;
            this.Samplerate = samplerate;
            this.second = second;
        }
    }
}
