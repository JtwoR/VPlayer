using System;

namespace JRVPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            FFMPEGHelper ffmpeg = new FFMPEGHelper("D://test.mp4");

            VideoItem video = ffmpeg.CreateVideoItem();

            Console.ReadKey();
        }
    }
}
