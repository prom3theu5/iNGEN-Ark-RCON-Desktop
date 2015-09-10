using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace iNGen.Models
{
    [ProtoContract]
    public class ChatSettings : ViewModelBase
    {
        [ProtoMember(1)]
        public bool IsAutoScrollEnabled { get; set; }

        [ProtoMember(2)]
        public bool IsTextWrappingEnabled { get; set; }

        [ProtoMember(3)]
        public bool IsTimestampingEnabled { get; set; }

        [ProtoMember(4)]
        public bool IsNotificationsEnabled { get; set; }

        [ProtoMember(5)]
        public string NotificationString { get; set; }

        public List<string> NotificationWords => NotificationString.Split(';').ToList();

        [ProtoMember(6)]
        public bool IsFlashWindowNotificationEnabled { get; set; }

        [ProtoMember(7)]
        public bool IsCustomServerConsoleNameEnabled { get; set; }

        [ProtoMember(8)]
        public string CustomServerConsoleName { get; set; }

        [ProtoMember(9)]
        public bool IsLoggingEnabled { get; set; }

        [ProtoMember(10)]
        public bool IsLogTimestampingEnabled { get; set; }

        [ProtoMember(11)]
        public bool IsNotificationSoundFileEnabled { get; set; }

        [ProtoMember(12)]
        public string NotificationSoundFile { get; set; }
    }
}