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

            MainResources.MainWindow.PlayFile(buttonData.VideoFilename[0], FadeInScreen);
        }
        
        private void FadeInScreen()
        {
            Storyboard fadeIn = FindResource("FadeInStoryboard") as Storyboard;
            Storyboard.SetTarget(fadeIn, MainGrid);
            fadeIn.Begin();
            IsEnabled = true;
        }

        private void ResetToMainMenu()
        {
            MainResources.MainWindow.ResetToMainMenu(this);
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            ResetToMainMenu();
        }
    }
}
