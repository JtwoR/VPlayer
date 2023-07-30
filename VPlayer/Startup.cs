using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPlayer
{
    public class Startup
    {
        [System.STAThreadAttribute()]
        public static void Main(string[] args) 
        {
            VPlayer.App app = new VPlayer.App();
            app.Run(new MainWindow(args.Count() > 0 ? args[0]: $@"D://test2.mp4"));
            
        }

    }
}
