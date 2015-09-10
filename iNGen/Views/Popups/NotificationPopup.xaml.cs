using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;

namespace iNGen.Views.Popups
{
    /// <summary>
    /// Interaction logic for NotificationPopup.xaml
    /// </summary>
    public partial class NotificationPopup
    {
        private bool _isClosing;

        public NotificationPopup()
        {
            InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
        }

        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            _isClosing = true;
        }

        private void FadeOut_OnCompleted(object sender, EventArgs e)
        {
            var pp = (Popup)Parent;
            pp.IsOpen = false;
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_isClosing) return;
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
