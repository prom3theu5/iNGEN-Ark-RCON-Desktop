using iNGen.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PTK.Extensions;
using System.Threading;
using PTK.Utils;
using iNGen.Models;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using Ark;
using GalaSoft.MvvmLight.Messaging;

namespace iNGen.Views
{
    public partial class ChatView : UserControl
    {
        public ChatViewModel ViewModel { get { return DataContext as ChatViewModel; } }

        #region Chat
        public ObservableCollection<ChatMessage> messages;
        private Storyboard scrollViewerStoryboard;
        private DoubleAnimation scrollViewerScrollToEndAnim;

        #region VerticalOffset DP
        private DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset",
          typeof(double), typeof(ChatView), new PropertyMetadata(0.0, OnVerticalOffsetChanged));

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatView chat = d as ChatView;
            chat.OnVerticalOffsetChanged(e);
        }

        private void OnVerticalOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            ConversationScrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        #endregion
        #endregion

        public ChatView()
        {
            InitializeComponent();

            scrollViewerScrollToEndAnim = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new SineEase()
            };
            Storyboard.SetTarget(scrollViewerScrollToEndAnim, this);
            Storyboard.SetTargetProperty(scrollViewerScrollToEndAnim, new PropertyPath(VerticalOffsetProperty));
            scrollViewerStoryboard = new Storyboard();

            scrollViewerStoryboard.Children.Add(scrollViewerScrollToEndAnim);
            ViewModel.View = this;
        }

        public void ScrollConversationToEnd()
        {
            scrollViewerScrollToEndAnim.From = ConversationScrollViewer.VerticalOffset;
            scrollViewerScrollToEndAnim.To = ConversationContentContainer.ActualHeight;
            scrollViewerStoryboard.Begin();
        }

    }
}
