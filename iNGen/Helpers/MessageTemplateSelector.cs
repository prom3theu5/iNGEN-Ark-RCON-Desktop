using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace iNGen.Helpers
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        // Template A: Sent under Recieved
        // Template B: Recieved under Sent
        // Template C: Recieved under Recieved
        // Templace D: Sent under Sent

        public DataTemplate A { get; set; }
        public DataTemplate B { get; set; }
        public DataTemplate C { get; set; }
        public DataTemplate D { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var message = item as ChatMessage;

            if (message.Side == MessageSide.Me)
            {
                return message.PrevSide == MessageSide.You ? A : D;
            }
            else
            {
                return message.PrevSide == MessageSide.You ? C : B;
            }
        }
    }
}
