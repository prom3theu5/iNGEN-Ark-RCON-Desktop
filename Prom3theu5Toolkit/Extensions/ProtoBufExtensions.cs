using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuf
{
    public static class ProtoBufConvert
    {
        public static T LoadFrom<T>(string filepath) where T: new()
        {
            if(File.Exists(filepath))
                using(var stream = File.OpenRead(filepath))
                    return Serializer.Deserialize<T>(stream);
            else
                return new T();
        }

        public static void SaveTo<T>(T item, string filepath)
        {
            string pathDirectory = Path.GetDirectoryName(filepath);

            if(!Directory.Exists(pathDirectory))
                Directory.CreateDirectory(pathDirectory);

            using(var stream = File.Open(filepath, FileMode.Create))
            {
                Serializer.Serialize(stream, item);
            }
        }
    }
}
