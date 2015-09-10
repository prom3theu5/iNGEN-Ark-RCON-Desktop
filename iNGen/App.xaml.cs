using PTK.ModelManager;
using Ark;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro.Controls;

namespace iNGen
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static ModelManager ModelManager = new ModelManager("UserData");
        public static Rcon ArkRcon = new Rcon();
        public static LogManager LogManager = new LogManager();
        public static EventManager EventManager = new EventManager();
        public static ViewModels.ViewModelLocator Locator { get; private set; }
        public static TaskbarIcon Tb;

        public App()
        {
            InitializeComponent();
            Debug.WriteLine(GetErrorFile());
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Tb = (TaskbarIcon)FindResource("iNGenTaskbarIcon");
            if (Tb != null) Tb.TrayMouseDoubleClick += tb_TrayMouseDoubleClick;
            Locator = new ViewModels.ViewModelLocator();
        }

        private static string GetErrorFile()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\iNGenError.txt";
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
            var exception = e.ExceptionObject as Exception;
            if (exception == null) return;
            LogErrorMessage(exception);
        }

        public static void LogErrorMessage(Exception exception)
        {
            string errorMessage = $"An unhandled exception occurred: {exception.Message}";
            using (var sw = new StreamWriter(GetErrorFile(), true))
            {
                sw.WriteLine($"-----Application Exception Logged At: {DateTime.Now}-----");
                sw.WriteLine("Application Error Message:");
                sw.WriteLine(errorMessage);
                sw.WriteLine(Environment.NewLine);
                sw.WriteLine("Error Stack Trace:");
                sw.WriteLine(exception.StackTrace);
                sw.WriteLine("-----END OF ERROR-----");
                sw.WriteLine(Environment.NewLine);
            }
            MessageBox.Show("An Error Occurred. Please Report this on GitHub. There is a log on your desktop.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        static void tb_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var window = Current.MainWindow as MetroWindow;

            if (window != null && !window.IsVisible) window.Show();
            if (window != null && window.WindowState == WindowState.Minimized) window.WindowState = WindowState.Normal;

            if (window == null) return;
            window.Show();
            window.Activate();
            window.Topmost = true;
            window.Topmost = false;
            window.Focus();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            tb_TrayMouseDoubleClick(sender, e);
        }
    }
}
