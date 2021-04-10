using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WPF
{
    public partial class MainMenu : UserControl
    {
        private readonly ScenarioSelect _scenarioSelect;
        private readonly GameSettingsScreen _gameSettings;

        public MainMenu()
        {
            InitializeComponent();

            _scenarioSelect = new ScenarioSelect();
            _gameSettings = new GameSettingsScreen(this);

            BitmapImage bitmapImage = new BitmapImage(new Uri(Path.Combine("file:///", Directory.GetCurrentDirectory(), @"Assets\game_logo.png")));
            MainMenuImage1.Source = bitmapImage;
            BitmapImage bitmapImage2 = new BitmapImage(new Uri(Path.Combine("file:///", Directory.GetCurrentDirectory(), @"Assets\mainmenu_bg.png")));
            MainMenuBGImage.ImageSource = bitmapImage2;

            StartAnimateBGImages();


            MainResources.MainWindow.NullVideos();
        }

        private void StartAnimateBGImages()
        {
            DoubleAnimation animation;
            animation = new DoubleAnimation();
            animation.To = 0.9;
            animation.From = 1;
            animation.Duration = new Duration(TimeSpan.FromSeconds(5));
            animation.AutoReverse = false;
            animation.Completed += Animation_Completed;
            MainMenuBGImage.BeginAnimation(ImageBrush.OpacityProperty, animation);
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            // fade out the image and fade in the new one
            DoubleAnimation animation;
            animation = new DoubleAnimation();
            animation.To = 0;
            animation.From = 0.9;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.AutoReverse = false;
            animation.Completed += ShowNextBGImage;
            MainMenuBGImage.BeginAnimation(ImageBrush.OpacityProperty, animation);
        }

        private bool _swapImage = false;
        private void ShowNextBGImage(object sender, EventArgs e)
        {
            if (_swapImage)
            {
                MainMenuBGImage.ImageSource = new BitmapImage(new Uri(Path.Combine("file:///", Directory.GetCurrentDirectory(), @"Assets\mainmenu_bg.png")));
                _swapImage = false;
            } else
            {
                _swapImage = true;
                MainMenuBGImage.ImageSource = new BitmapImage(new Uri(Path.Combine("file:///", Directory.GetCurrentDirectory(), @"Assets\mainmenu_bg2.png")));
            }
          
            // fade out the image and fade in the new one
            DoubleAnimation animation;
            animation = new DoubleAnimation();
            animation.To = 1;
            animation.From = 0;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.AutoReverse = false;
            animation.Completed += ResetAnimation;
            MainMenuBGImage.BeginAnimation(ImageBrush.OpacityProperty, animation);
        }

        private void ResetAnimation(object sender, EventArgs e)
        {
           
            StartAnimateBGImages();
        }

        private void OnPlayClicked(object sender, RoutedEventArgs e)
        {
            MainResources.MainWindow.MainPanel.Children.Add(_scenarioSelect);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
        }

        private void OnExitClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            MainResources.MainWindow.MainPanel.Children.Add(_gameSettings);
           MainResources.MainWindow.MainPanel.Children.Remove(this);
        }

        private void button_mainMenu_MouseEnter(object sender, MouseEventArgs e)

        {

            //ScenariosButton.Content = "Mouse Enter Event";

          


        }

        private void button_mainMenu_MouseLeave(object sender, MouseEventArgs e)

        {

            //ScenariosButton.Content = "Scenarios";

        }


    }
}