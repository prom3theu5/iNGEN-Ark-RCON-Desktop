using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.Models
{
    public enum MessageSide
    {
        Me,
        You
    }

    public class ChatMessage
    {
        public ChatMessage()
        {
            Timestamp = DateTime.Now;
        }

        public string SenderName { get; set; }

        public string Text { get; set; }

        public DateTime Timestamp { get; set; }

        public MessageSide Side { get; set; }
        public MessageSide PrevSide { get; set; }
    }
}
