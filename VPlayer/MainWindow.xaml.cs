using OpenTK.Audio.OpenAL;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VPlayer
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JR.VPlayer.VPlayer player;
        public MainWindow(string path)
        {
            InitializeComponent();

            var mainSettings = new GLWpfControlSettings { MajorVersion = 4, MinorVersion = 5, GraphicsProfile = ContextProfile.Compatability, GraphicsContextFlags = ContextFlags.Debug }; ;
            OpenTkControl.Start(mainSettings);
            player = new JR.VPlayer.VPlayer(path);
            player.Run();


            player.SilderValueChange += this.PlayerUI.videoPlayer.SilderValueChange;
            this.PlayerUI.videoPlayer.SetVPlayer(player);
        }

        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            player.Render();
        }
    }
}
