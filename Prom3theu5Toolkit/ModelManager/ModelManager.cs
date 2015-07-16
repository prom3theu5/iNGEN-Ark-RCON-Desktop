using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTK.ModelManager
{
    public class ModelManager
    {
        public Dictionary<Type, ModelHandler> Models {get; private set;}
        public string ModelDirectory {get; set;}

        public ModelManager(string modelDirectory)
        {
            ModelDirectory = modelDirectory;
            Models = new Dictionary<Type, ModelHandler>();
        }

        public T Get<T>() where T: new()
        {
            ModelHandler model;
            
            if(!Models.TryGetValue(typeof(T), out model))
            {
                model = new ModelHandler<T>();
                model.LoadFrom(ModelDirectory);
                Models[typeof(T)] = model;
            }

            return (model as ModelHandler<T>).Model;
        }

        public void Save<T>()
        {
            ModelHandler model;
            if(Models.TryGetValue(typeof(T), out model))
                model.SaveTo(ModelDirectory);
        }

        public void SaveAll()
        {
            foreach(var model in Models.Values)
            {
                model.SaveTo(ModelDirectory);
            }
        }
    }
}
