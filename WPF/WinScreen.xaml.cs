using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPF
{
    public partial class WinScreen : UserControl
    {
        public WinScreen(ButtonData buttonData, string videoFilePath)
        {
            InitializeComponent();

            // set default message
            string messageContent = string.IsNullOrEmpty(buttonData.EndScreenMessage) ? "You win :)" : buttonData.EndScreenMessage;
            DisplayMessage.Content = messageContent;

            MainResources.MainWindow.PlayFile(videoFilePath, FadeInScreen);
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
