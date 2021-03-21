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

        public PreGame(string path)
        {
            InitializeComponent();

            _path = path;

            BitmapImage bitmapImage = new BitmapImage(new Uri(Path.Combine(path, "bg.png")));
            BgImage.Source = bitmapImage;


            // load the scenario from json file
            using (StreamReader r = new StreamReader(Path.Combine(path, "scenario.json")))
            {
                string json = r.ReadToEnd();
                ScenarioSchema scenario = JsonConvert.DeserializeObject<ScenarioSchema>(json);
                TitleLabel.Content = new TextBlock { Text = scenario.Title, TextWrapping = TextWrapping.Wrap };
                DescriptionLabel.Content = new TextBlock { Text = scenario.Description, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Left };
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
            MainResources.Scores = new List<bool>();
            // life points represent the number of times a user can choose a wrong answer before losing the game
            MainResources.SetLifePoints(1);
            // default branch is A
            MainResources.SetBranch("A");
            // default start position is 1 (A1)
            MainResources.SetPathPosition(1);
            string startingVideo = Path.Combine(_path, "pre.avi");
            GameMenu gameMenu = new GameMenu(_path, MainResources.GetPathPosition(), startingVideo);
            MainResources.MainWindow.MainPanel.Children.Add(gameMenu);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
            MainResources.MainWindow.RemoveBackground();
        }
    }
}