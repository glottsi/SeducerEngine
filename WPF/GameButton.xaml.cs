using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF
{
    public partial class GameButton : UserControl
    {
        private readonly Action<ButtonData> _callback;
        private readonly ButtonData _buttonData;
        
        public GameButton(string text, ButtonData buttonData, Action<ButtonData> callback)
        {
            InitializeComponent();
            _buttonData = buttonData;
           
            _callback = callback;
         
            ButtonLabel.Content = text;
            Button.Click += OnClicked;
        }

        private void OnClicked(object sender, RoutedEventArgs e)
        {
            _callback.Invoke(_buttonData);
        }
    }
}
