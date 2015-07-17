using PTK.ModelManager;
using Ark;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            tb = (TaskbarIcon)FindResource("iNGenTaskbarIcon");
            tb.TrayMouseDoubleClick += tb_TrayMouseDoubleClick;
            Locator = new ViewModels.ViewModelLocator();
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
