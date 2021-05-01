using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPF
{
    public partial class LoseScreen : UserControl
    {
        public LoseScreen(ButtonSchema buttonData)
        {
            InitializeComponent();
            // set default message
            string messageContent = string.IsNullOrEmpty(buttonData.EndScreenMessage) ? "You lose :(" : buttonData.EndScreenMessage;
            LoseMessage.Content = messageContent;

            // disable buttons
            SetButtonsEnabled(false);
          
            MainResources.MainWindow.PlayFile(buttonData.VideoFilename[0], FadeInScreen);
        }

        private void SetButtonsEnabled(bool enabled)
        {
            QuitButton.IsEnabled = enabled;
            RetryButton.IsEnabled = enabled;
        }
        
        private void FadeInScreen()
        {
            Storyboard fadeIn = FindResource("FadeInStoryboard") as Storyboard;
            Storyboard.SetTarget(fadeIn, MainGrid);
            fadeIn.Begin();
            IsEnabled = true;
            // re-enable the buttons
            SetButtonsEnabled(true);
        }

        private void ResetToMainMenu()
        {
            MainResources.MainWindow.ResetToMainMenu(this);
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            ResetToMainMenu();
        }
        private void RetryButtonClicked(object sender, RoutedEventArgs e)
        {
            // create new GameMenu instance with previous game state
            RestorePoint load = MainResources.RestorePreviousState();
            DebugFunctions.DEBUG_output_list_of_videos("LOADED RESTORE POINT", load.LastChoice);
            GameMenu replayChoices = new GameMenu(load.LastChoice);
            MainResources.MainWindow.MainPanel.Children.Add(replayChoices);
            MainResources.MainWindow.MainPanel.Children.Remove(this);
        }
      
    }
}
