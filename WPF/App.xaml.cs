using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App() : base()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            // Get the current directory.
            string path = Directory.GetCurrentDirectory();

            string fileName = $"{path}\\CRASH_REPORT.txt";

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                using (FileStream fs = File.Create(fileName))
                {
                    // error message  
                    Byte[] errorMessage = new UTF8Encoding(true).GetBytes($"-- Message --\n{e.Exception.Message}\n");
                    fs.Write(errorMessage, 0, errorMessage.Length);
                    // error source
                    byte[] source = new UTF8Encoding(true).GetBytes($"-- Source --\n{e.Exception.Source}\n");
                    fs.Write(source, 0, source.Length);
                    // target site
                    byte[] targetsite = new UTF8Encoding(true).GetBytes($"-- Target Site --\n{e.Exception.TargetSite}\n");
                    fs.Write(targetsite, 0, targetsite.Length);

                    byte[] stackTrace = new UTF8Encoding(true).GetBytes($"-- Stack Trace --\n{e.Exception.StackTrace}\n");
                    fs.Write(stackTrace, 0, stackTrace.Length);
                }

            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.ToString());
            }
            MessageBox.Show("Unhandled exception occurred: \n" + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}