using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF
{
    public partial class GameButton : UserControl
    {
        private readonly Action<ButtonSchema> _callback;
        private readonly ButtonSchema _buttonData;
        
        public GameButton(string text, ButtonSchema buttonData, Action<ButtonSchema> callback)
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
