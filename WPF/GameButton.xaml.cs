using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF
{
    public partial class GameButton : UserControl
    {
        private readonly Action<ButtonType, StoryPath, string> _callback;
        private readonly ButtonType _buttonType;
        private readonly string _videoPath;
        private readonly StoryPath _storyPath;
        
        public GameButton(string text, ButtonType buttonType, StoryPath branch, string videoPath, Action<ButtonType, StoryPath, string> callback)
        {
            InitializeComponent();

            _buttonType = buttonType;
            _videoPath = videoPath;
            _callback = callback;
            _storyPath = branch;

            ButtonLabel.Content = text;
            Button.Click += OnClicked;
        }

        private void OnClicked(object sender, RoutedEventArgs e)
        {
            _callback.Invoke(_buttonType, _storyPath, _videoPath);
        }
    }
}
