using MahApps.Metro.Controls;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iNGen.Views
{
    public partial class AppUpdate : MetroWindow
    {
        public UpdateInfo UpdateInformation { get; set; }
        public string ReleaseNotes { get; set; }
        public string CurrentVersion { get; set; }
        public string NewVersion { get; set; }
        public string ChangeLogSource { get; set; }

        public AppUpdate(UpdateInfo _updateInfo, string Changes)
        {
            InitializeComponent();
            UpdateInformation = _updateInfo;
            CurrentVersion = UpdateInformation.CurrentlyInstalledVersion.Version.ToString();
            NewVersion = UpdateInformation.FutureReleaseEntry.Version.ToString();
            ChangeLogSource = Changes;
            this.DataContext = this;
            Loaded += AppUpdate_Loaded;
        }

        void AppUpdate_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLog.NavigateToString(ChangeLogSource);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
