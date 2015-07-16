using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTK.ModelManager;
using ProtoBuf;

namespace iNGen.Models
{
    [Model("UserSettings")]
    [ProtoContract]
    public class UserSettings
    {

        [ProtoMember(1)]
        public ConsoleSettings ConsoleSettings {get; set;}
        
        [ProtoMember(2)]
        public GeneralSettings GeneralSettings {get; set;}
    
        [ProtoMember(3)]
        public ChatSettings ChatSettings { get; set; }
        
        public UserSettings()
        {
            ConsoleSettings = new ConsoleSettings();
            GeneralSettings = new GeneralSettings();
            ChatSettings = new ChatSettings();
        }
    }
}

