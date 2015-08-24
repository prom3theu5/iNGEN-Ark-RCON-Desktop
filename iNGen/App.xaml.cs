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
    public partial class App : Application
    {
        public static ModelManager ModelManager = new ModelManager("UserData");
        public static Rcon ArkRcon = new Rcon();
        public static LogManager LogManager = new LogManager();
        public static EventManager EventManager = new EventManager();
        public static ViewModels.ViewModelLocator Locator { get; private set; }
        private TaskbarIcon tb;

        public App()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            tb = (TaskbarIcon)FindResource("iNGenTaskbarIcon");
            tb.TrayMouseDoubleClick += tb_TrayMouseDoubleClick;
            Locator = new ViewModels.ViewModelLocator();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var filename = Path.Combine(GetApplicationDirectory(), "\\ApplicationError.txt");
            var exception = e.ExceptionObject as Exception;
            if (exception == null) return;
            string errorMessage = $"An unhandled exception occurred: {exception.Message}";
            using (var sw = new StreamWriter(filename, true))
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
            MessageBox.Show("An Error Occurred. Please Report this on GitHub.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Process.Start(filename);
        }

        private string GetApplicationDirectory()
        {
            var exePath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            return Path.GetDirectoryName(exePath);
        }

        void tb_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MetroWindow;

            if (!window.IsVisible) window.Show();
            if (window.WindowState == WindowState.Minimized) window.WindowState = WindowState.Normal;

            window.Show();
            window.Activate();
            window.Topmost = true;
            window.Topmost = false;
            window.Focus();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            tb_TrayMouseDoubleClick(sender, e);
        }
    }
}
