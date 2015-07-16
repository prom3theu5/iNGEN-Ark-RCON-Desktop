using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTK.ModelManager
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelAttribute: Attribute
    {
        public string Filename {get;set;}

        public ModelAttribute(string filename)
        {
            Filename = filename;
        }
    }
}
