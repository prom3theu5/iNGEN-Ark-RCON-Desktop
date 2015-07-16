using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.Models
{
   public class BroadcastMessage
   {
       public string MessageTitle { get; set; }
       public string MessageText { get; set; }
       public bool ReBroadcast { get; set; }
       public int RebroadcastDelay { get; set; }
   }
}
