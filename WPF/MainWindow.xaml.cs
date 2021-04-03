using System;
using System.Windows;
using System.Windows.Input;

namespace WPF
{
    public partial class MainWindow
    {
        private readonly DevLogos _devLogos;

        private Action _callback;
        private Action _callback1;
        private bool _video1 = true;

        private readonly MenuBackground _menuBackground;

        public MainWindow()
        {
            InitializeComponent();

            MainResources.MainWindow = this;
            
            _menuBackground = new MenuBackground();
            MainPanel.Children.Add(_menuBackground);

            _devLogos = new DevLogos(DevLogosDone);
            MainPanel.Children.Add(_devLogos);
            
            VideoMediaElement1.MediaOpened += (sender, e) => { VideoMediaElement2.Source = null; };
            VideoMediaElement2.MediaOpened += (sender, e) => { VideoMediaElement1.Source = null; };
        }

        public void PlayFile(string videoPath, Action callback)
        {
            if(_callback == null)
            {
                _callback = callback;
            }else
            {
                _callback1 = callback;
            }
         

            if (_video1)
                VideoMediaElement1.Source = new Uri(videoPath);
            else
                VideoMediaElement2.Source = new Uri(videoPath);

            _video1 = !_video1;
        }
        
        public void NullVideos()
        {
            VideoMediaElement1.Source = null;
            VideoMediaElement2.Source = null;
        }

        public void RemoveBackground()
        {
            MainPanel.Children.Remove(_menuBackground);
        }

        public void AddBackground()
        {
            MainPanel.Children.Add(_menuBackground);
        }

        private void MediaEnded(object sender, EventArgs e)
        {
            if (_callback == null)
            {
                _callback1.Invoke();
                _callback1 = null;
            }
            else
            {
                _callback.Invoke();
                _callback = null;
            }
           
        }

        private void DevLogosDone()
        {
            MainPanel.Children.Remove(_devLogos);
            MainPanel.Children.Add(new MainMenu());
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}