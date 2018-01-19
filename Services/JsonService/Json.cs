using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DP_chan.Services.JsonService
{
    class Json
    {
        private JsonSerializer serializer;

        public Json()
        {
            serializer = new JsonSerializer();

            serializer.Formatting = Formatting.Indented;
        }

        public void SaveProperly(object obj, string filepath)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public static void Save(object o, string filepath, bool noIndent = false)
        {
            Formatting formatting = Formatting.Indented;
            if (noIndent)
                formatting = Formatting.None;

            string jsonObject = JsonConvert.SerializeObject(o, formatting);

            FileHelper.Write(jsonObject, filepath);
        }
        
        public static T Open<T>(string filepath)
        {
            string jsonObject = FileHelper.Read(filepath);

            T obj = JsonConvert.DeserializeObject<T>(jsonObject);

            return obj;
        }

        public static JObject OpenParse(string filepath)
        {
            string jsonObject = FileHelper.Read(filepath);

            JObject obj = JObject.Parse(jsonObject);

            return obj;
        }
    }
}
