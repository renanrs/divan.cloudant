
using System.IO;
using System.Text;

namespace Divan.Cloudant
{
    internal static class JsonSerializer
    {
        public static string Serialize<T>(T model) where T : class
        {
            var jsonSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

            string objJson = string.Empty;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                jsonSerializer.WriteObject(memoryStream, model);
                memoryStream.Position = 0;
                using (var reader = new StreamReader(memoryStream))
                {
                    objJson = reader.ReadToEnd();
                }
            }

            return objJson;
        }
              

        public static object Deserialize<T>(string json) where T: class
        {
            object jsonObject;
            var jsonSerializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
            using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                memoryStream.Position = 0;
                jsonObject = jsonSerializer.ReadObject(memoryStream);
            }

            return jsonObject;
        }
    }
}
