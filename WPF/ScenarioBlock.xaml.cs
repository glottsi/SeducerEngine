using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPF
{
    public partial class ScenarioBlock : UserControl
    {
        private readonly string _path;
        private readonly Action<string> _loadPreGame;
        
        public ScenarioBlock(string path, Action<string> loadPreGame)
        {
            InitializeComponent();

            _path = path;
            _loadPreGame = loadPreGame;

            BitmapImage bitmapImage = new BitmapImage(new Uri(Path.Combine(path, "bg.png")));
            BgImage.Source = bitmapImage;

            // load the scenario from json file
            using (StreamReader r = new StreamReader(Path.Combine(path, "scenario.json")))
            {
                string json = r.ReadToEnd();
                ScenarioSchema scenario = JsonConvert.DeserializeObject<ScenarioSchema>(json);
                TitleLabel.Content = new TextBlock { Text = scenario.Title, TextWrapping = TextWrapping.Wrap };
                SubTitleLabel.Content = new TextBlock { Text = scenario.Subtitle, TextWrapping = TextWrapping.Wrap };
            }
        }


        private void OnClick(object sender, RoutedEventArgs e)
        {
            _loadPreGame.Invoke(_path);
        }
    }
}