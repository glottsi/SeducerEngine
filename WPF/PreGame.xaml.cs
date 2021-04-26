using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPF
{
    public partial class PreGame : UserControl
    {
        private readonly string _path;
        private readonly ScenarioSettings _settings;

        private readonly string _introVideo;

        public PreGame(string path)
        {
            InitializeComponent();

            _path = path;

            // load the scenario from json file
            using (StreamReader r = new StreamReader(Path.Combine(_path, "scenario.json")))
            {

                string json = r.ReadToEnd();
                ScenarioSchema scenario = JsonConvert.DeserializeObject<ScenarioSchema>(json);
                // set the title of the scenario
                TitleLabel.Content = new TextBlock { Text = scenario.Title, TextWrapping = TextWrapping.Wrap };
                // set description text
                DescriptionLabel.Content = new TextBlock { Text = scenario.Description, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Left };
                // load the image
                BitmapImage bitmapImage = new BitmapImage(new Uri(Path.Combine(_path, scenario.Image)));
                BgImage.Source = bitmapImage;
                // prepare the scenario's settings to be loaded
                _settings = scenario.Settings;
                _introVideo = scenario.IntroVideo;
               
            }
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            ScenarioSelect scenarioSelect = new ScenarioSelect();
            MainResources.MainWindow.MainPanel.Children.Add(scenarioSelect);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
        }

        private void PlayButtonClicked(object sender, RoutedEventArgs e)
        {
            // sets base path file location (root directory)
            MainResources.SetRootDirectory(_path);

            // confirm the scenario, loads the settings into mainresources
            MainResources.SetHP(_settings.StartingHP);
            MainResources.SetBranch(_settings.StartingBranch);
            MainResources.SetPathPosition(_settings.StartingPathPosition);
            MainResources.SetPoints(_settings.StartingPoints);
      
            // sets the folder path where we store all the ending videos
            MainResources.SetEndingPathRoot(Path.Combine(_path, "endings"));

            ButtonSchema buttonData = new ButtonSchema()
            {
                VideoFilename = new List<string>(){ Path.Combine(_path, _introVideo) },
                Endings = new List<Ending>()
            };

            GameMenu gameMenu = new GameMenu(buttonData);
            MainResources.MainWindow.MainPanel.Children.Add(gameMenu);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
            MainResources.MainWindow.RemoveBackground();
        }
    }
}