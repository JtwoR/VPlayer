using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRVPlayer
{
    public class MainPlayer
    {
        private RenderHelper render;
        private DataCenter dataCenter;

        public long currentVideoLen;

        public Action<int> _updateSlider = null;

        public MainPlayer(string path) {

            render = new RenderHelper();

            dataCenter = new DataCenter(path);

            currentVideoLen = dataCenter.currentVideoLen;
        }

        public void UpdateSlider(Action<int> action) { _updateSlider = action; }

        public void Run() {
            dataCenter.Run();
        }

        public void Render() {

            VideoFrame video;
            if (dataCenter.videoData.TryDequeue(out video)) { 
                render.DrawTexture(video.width, video.height, video.data);
                if (_updateSlider != null) _updateSlider.Invoke(video.second);
            };
        }

        public void Skip(int SeekTimeInSeconds) {
            dataCenter.Skip(SeekTimeInSeconds);
        }
    }
}
