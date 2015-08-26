using PTK.WPF;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.Models
{
    [ProtoContract]
    public class GeneralSettings: Notifiable
    {
        private bool mStartOnWindows;
        private bool mKeepWindowTopmost;
        private bool mMinimizeToSystemTray;
        private bool mCloseToSystemTray;
        private bool mAutoReconnect;
        private bool mReconnectEveryFiveMinutes;
        private bool mAutoStartChat;
        private string mSteamAPIKey;

        [ProtoMember(1)]
        public bool StartOnWindows
        {
            get { return mStartOnWindows; }
            set { SetField(ref mStartOnWindows, value); }
        }
        
        [ProtoMember(2)]
        public bool KeepWindowTopmost
        {
            get { return mKeepWindowTopmost; }
            set { SetField(ref mKeepWindowTopmost, value); }
        }
        
        [ProtoMember(3)]
        public bool MinimizeToSystemTray
        {
            get { return mMinimizeToSystemTray; }
            set { SetField(ref mMinimizeToSystemTray, value); }
        }
        
        [ProtoMember(4)]
        public bool CloseToSystemTray
        {
            get { return mCloseToSystemTray; }
            set { SetField(ref mCloseToSystemTray, value); }
        }

        [ProtoMember(5)]
        public bool IsAutoReconnectEnabled
        {
            get { return mAutoReconnect; }
            set { SetField(ref mAutoReconnect, value); }
        }

        [ProtoMember(6)]
        public bool DoReconnectEveryFiveMinutes
        {
            get { return mReconnectEveryFiveMinutes; }
            set { SetField(ref mReconnectEveryFiveMinutes, value); }
        }

        [ProtoMember(7)]
        public bool AutoStartChat
        {
            get { return mAutoStartChat; }
            set { SetField(ref mAutoStartChat, value); }
        }

        [ProtoMember(8)]
        public string SteamApiKey
        {
            get { return mSteamAPIKey; }
            set { SetField(ref mSteamAPIKey, value); }
        }

    }
}
