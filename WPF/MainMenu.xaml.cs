using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPF
{
    public partial class MainMenu : UserControl
    {
        private readonly ScenarioSelect _scenarioSelect;

        public MainMenu()
        {
            InitializeComponent();

            _scenarioSelect = new ScenarioSelect();
            
            MainResources.MainWindow.NullVideos();
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