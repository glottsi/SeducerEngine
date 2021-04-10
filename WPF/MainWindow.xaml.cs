using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF
{
    public partial class MainWindow
    {
        private readonly DevLogos _devLogos;

        private readonly PressAnyKeyScreen _pressAnyKeyScreen;
        private bool _showingStartScreen = false;

        private Action _callback;
        private Action _callback1;
        private bool _video1 = true;

        private MediaPlayer _BGMplayer;
        private MediaPlayer _startSoundPlayer;
        private MediaPlayer _playGameSound;

        private readonly MenuBackground _menuBackground;

        private bool _gameStarted = false;

        public MainWindow()
        {
            InitializeComponent();

            MainResources.MainWindow = this;

            initializeMediaPlayers();

            _pressAnyKeyScreen = new PressAnyKeyScreen();
            _menuBackground = new MenuBackground();
            _devLogos = new DevLogos(DevLogosDone);

            MainPanel.Children.Add(_menuBackground);
            MainPanel.Children.Add(_devLogos);

            VideoMediaElement1.MediaOpened += (sender, e) => { VideoMediaElement2.Source = null; };
            VideoMediaElement2.MediaOpened += (sender, e) => { VideoMediaElement1.Source = null; };
        }

        private void initializeMediaPlayers()
        {
            // initialize players
            _startSoundPlayer = new MediaPlayer();
            _BGMplayer = new MediaPlayer();
            _playGameSound = new MediaPlayer();
            // load sound files
            _startSoundPlayer.Open(new Uri(Path.Combine("file:///", Directory.GetCurrentDirectory(), @"Assets\button1.mp3")));
            _BGMplayer.Open(new Uri(Path.Combine("file:///", Directory.GetCurrentDirectory(), @"Assets\bgm.mp3")));
            _playGameSound.Open(new Uri(Path.Combine("file:///", Directory.GetCurrentDirectory(), @"Assets\button2.mp3")));
            // adjust settings

            _BGMplayer.Volume = 0.1; // 1 is full volume, default 0.5
            _BGMplayer.MediaEnded += _BGMplayer_MediaEnded; // callback invoked after bgm finishes playing (resets it)
            _startSoundPlayer.Volume = 0.3;
            _playGameSound.Volume = 0.3;
        }

        public void ResetToMainMenu(UIElement prevScreen)
        {
            AddBackground();
            ResetAndPlayBGM();
            MainPanel.Children.Add(new MainMenu());
            MainPanel.Children.Remove(prevScreen);
        }

        public void ResetAndPlayBGM()
        {
            _gameStarted = false;
            _BGMplayer.Position = TimeSpan.Zero;
            _playGameSound.Stop();
            _playGameSound.Position = TimeSpan.Zero;
            _BGMplayer.Play();
        }

        private void _BGMplayer_MediaEnded(object sender, EventArgs e)
        {
            // if the music ends, replay it
            ResetAndPlayBGM();
        }

        public void PlayFile(string videoPath, Action callback)
        {
            if (!_gameStarted)
            {
                // stop bgm
                _BGMplayer.Stop();
                // play game start sound
                _playGameSound.Play();
                _gameStarted = true;
            }
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
            _showingStartScreen = true;
            _BGMplayer.Play();
            MainPanel.Children.Remove(_devLogos);
            MainPanel.Children.Add(_pressAnyKeyScreen);
        }

        private void ShowMainMenu(object sender, EventArgs e)
        {
            MainPanel.Children.Remove(_pressAnyKeyScreen);
            MainPanel.Children.Add(new MainMenu());
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (_showingStartScreen)
            {
                _showingStartScreen = false;
                // play sound effect when button pressed
             
                _startSoundPlayer.Play();
                // flash the text to indicate that game is starting
                _pressAnyKeyScreen.Flash_Text(ShowMainMenu);
            } else if (e.Key == Key.Escape)
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