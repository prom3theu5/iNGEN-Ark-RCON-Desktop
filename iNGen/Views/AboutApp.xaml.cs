using MahApps.Metro.Controls;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iNGen.Views
{
    public partial class AboutApp : MetroWindow
    {
        public UpdateInfo UpdateInformation { get; set; }
        public string ReleaseNotes { get; set; }
        public string CurrentVersion { get; set; }
        public string NewVersion { get; set; }
        public string ChangeLogSource { get; set; }

        public AboutApp(UpdateInfo _updateInfo, string Changes)
        {
            InitializeComponent();
            UpdateInformation = _updateInfo;
            CurrentVersion = UpdateInformation.CurrentlyInstalledVersion.Version.ToString();
            NewVersion = UpdateInformation.FutureReleaseEntry.Version.ToString();
            ChangeLogSource = Changes;
            this.DataContext = this;
            Loaded += AppUpdate_Loaded;
            if (NewVersion != CurrentVersion)
            {
                UpdateButton.IsEnabled = true;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
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

        private void TwitterButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://www.twitter.com/prom3theu5"));
        }

        private void GooglePlusButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://plus.google.com/+DaveSekulaProm3theu5/"));
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            string url = "";
            string business = "dave.sekula@gmail.com";
            string description = "iNGen%20ArkRcon%20Donation";
            string country = "GB";
            string currency = "GBP";

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + country +
                "&item_name=" + description +
                "&currency_code=" + currency +
                "&bn=" + "PP%2dDonationsBF";
            System.Diagnostics.Process.Start(url);
        }

    }
}
