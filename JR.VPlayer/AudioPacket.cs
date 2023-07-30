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

        public AudioPacket(byte[] data, int size, int samplerate)
        {
            this.Data = data;
            this.Samplerate = samplerate;
        }
    }
}
