using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPF
{
    public partial class WinScreen : UserControl
    {
        private readonly Storyboard _fadeIn;
        public WinScreen(string videoPath)
        {
            InitializeComponent();

            _fadeIn = FindResource("FadeInStoryboard") as Storyboard;

            Storyboard.SetTarget(_fadeIn, MainGrid);

            MainResources.MainWindow.PlayFile(videoPath, PrevVideoDone);
        }
        
        private void PrevVideoDone()
        {
            _fadeIn.Begin();
            IsEnabled = true;
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            MainResources.MainWindow.AddBackground();
            MainResources.MainWindow.MainPanel.Children.Add(new MainMenu());
            MainResources.MainWindow.MainPanel.Children.Remove(this);
        }
    }
}
