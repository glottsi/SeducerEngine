using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WPF
{
    public partial class PressAnyKeyScreen : UserControl
    {
        
        public PressAnyKeyScreen()
        {
            InitializeComponent();
            var imgBrush = new ImageBrush();

            imgBrush.ImageSource = new BitmapImage(new Uri(Path.Combine("file:///", Directory.GetCurrentDirectory(), @"Assets\press_any_key_screen.jpg")));
            SplashScreenGrid.Background = imgBrush;
          
        }

        public void Screen_Loaded(object sender, RoutedEventArgs e)
        {
            ColorAnimation animation;
            animation = new ColorAnimation();
            animation.To = Colors.Transparent;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.AutoReverse = true;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            PressAnyKeyTextBlockBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        public void Flash_Text(EventHandler callback)
        {
           
            ColorAnimation animation;
            animation = new ColorAnimation();
            animation.To = Colors.Transparent;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.05));
            animation.AutoReverse = true;
            animation.RepeatBehavior = new RepeatBehavior(3);
            animation.Completed += callback;
            PressAnyKeyTextBlockBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }



    }
}
