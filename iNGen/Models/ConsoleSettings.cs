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
    public class ConsoleSettings: Notifiable
    {
        private bool mIsAutoScrollEnabled;
        private bool mIsTextWrappingEnabled;
        private bool mIsTimestampingEnabled;
        private bool mIsNotificationsEnabled;
        private string mNotificationString;
        private List<string> mNotificationWords;
        private bool mIsFlashWindowNotificationEnabled;
        private bool mIsCustomServerConsoleNameEnabled;
        private string mCustomServerConsoleName;
        private bool mIsLoggingEnabled;
        private bool mIsLogTimestampingEnabled;

        [ProtoMember(1)]
        public bool IsAutoScrollEnabled
        {
            get { return mIsAutoScrollEnabled; }
            set { SetField(ref mIsAutoScrollEnabled, value); }
        }

        [ProtoMember(2)]
        public bool IsTextWrappingEnabled
        {
            get { return mIsTextWrappingEnabled; }
            set { SetField(ref mIsTextWrappingEnabled, value); }
        }

        [ProtoMember(3)]
        public bool IsTimestampingEnabled
        {
            get { return mIsTimestampingEnabled; }
            set { SetField(ref mIsTimestampingEnabled, value); }
        }

        [ProtoMember(4)]
        public bool IsNotificationsEnabled
        {
            get { return mIsNotificationsEnabled; }
            set { SetField(ref mIsNotificationsEnabled, value); }
        }

        [ProtoMember(5)]
        public string NotificationString
        {
            get { return mNotificationString; }
            set
            {
                SetField(ref mNotificationString, value);
                NotificationWords = value.Split(';').ToList();
            }
        }

        public List<string> NotificationWords
        {
            get { return mNotificationWords; }
            private set { SetField(ref mNotificationWords, value); }
        }

        [ProtoMember(6)]
        public bool IsFlashWindowNotificationEnabled
        {
            get { return mIsFlashWindowNotificationEnabled; }
            set { SetField(ref mIsFlashWindowNotificationEnabled, value); }
        }

        [ProtoMember(7)]
        public bool IsCustomServerConsoleNameEnabled
        {
            get { return mIsCustomServerConsoleNameEnabled; }
            set { SetField(ref mIsCustomServerConsoleNameEnabled, value); }
        }

        [ProtoMember(8)]
        public string CustomServerConsoleName
        {
            get { return mCustomServerConsoleName; }
            set { SetField(ref mCustomServerConsoleName, value); }
        }
        
        [ProtoMember(9)]
        public bool IsLoggingEnabled
        {
            get { return mIsLoggingEnabled; }
            set { SetField(ref mIsLoggingEnabled, value); }
        }
        
        [ProtoMember(10)]
        public bool IsLogTimestampingEnabled
        {
            get { return mIsLogTimestampingEnabled; }
            set { SetField(ref mIsLogTimestampingEnabled, value); }
        }
    }
}
