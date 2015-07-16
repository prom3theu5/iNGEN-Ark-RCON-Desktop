using PTK.WPF;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTK.ModelManager
{
    public interface ModelHandler
    {
        void SaveTo(string directory);
        void LoadFrom(string directory);
    }

    public class ModelHandler<T>: ModelHandler where T: new()
    {
        public T Model {get; set;}
        private ModelAttribute ModelAttribute {get; set;}

        public ModelHandler()
        {
            ModelAttribute = Attribute.GetCustomAttribute(typeof(T), typeof(ModelAttribute)) as ModelAttribute;
        }

        public void SaveTo(string directory)
        {
            ProtoBufConvert.SaveTo(Model, String.Format(@"{0}/{1}", directory, Filepath));
        }

        public void LoadFrom(string directory)
        {
            Model = ProtoBufConvert.LoadFrom<T>(String.Format(@"{0}/{1}", directory, Filepath));
        }

        public string Filename
        {
            get
            {
                return ModelAttribute.Filename;
            }
        }

        public string Filepath
        {
            get
            {
                return Filename + ".dat";
            }
        }
    }
}
