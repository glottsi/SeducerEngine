using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace WPF
{
    /// <summary>
    /// Interaction logic for GameSettingsScreen.xaml
    /// </summary>
    public partial class GameSettingsScreen : UserControl
    {
        private readonly MainMenu _prevMenu;

        public GameSettingsScreen(MainMenu prevMenu)
        {
            InitializeComponent();
            _prevMenu = prevMenu;
            // load the settings from json file (currently unused)
            string settingsPath = Path.Combine("file:///", Directory.GetCurrentDirectory(), @"GameSettings.json");
          
           /* using (StreamReader r = new StreamReader(settingsPath))
            {
                string json = r.ReadToEnd();
                GameSettings settings = JsonConvert.DeserializeObject<GameSettings>(json);

                // handle/parse settings 

                _settings = settings;
            }*/
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            MainResources.MainWindow.MainPanel.Children.Add(_prevMenu);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
        }

    }
}

