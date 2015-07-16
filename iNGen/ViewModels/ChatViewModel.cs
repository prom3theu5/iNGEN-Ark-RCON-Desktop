using PTK.WPF;
using iNGen.Models;
using Ark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.ViewModels
{
    public class ChatViewModel: Notifiable
    {
        public ChatSettings ChatSettings {get; set;}

        public ChatViewModel()
        {
            ChatSettings = App.ModelManager.Get<UserSettings>().ChatSettings;
        }
    }
}