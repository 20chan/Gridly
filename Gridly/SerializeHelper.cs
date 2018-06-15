using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gridly
{
    public static class SerializeHelper
    {
        public static void WriteToFile(this JObject obj, string path)
        {
            using (var sw = new StreamWriter(path))
            using (var jw = new JsonTextWriter(sw))
                obj.WriteTo(jw);
        }

        public static JObject LoadFromFile(string path)
        {
            using (var sr = new StreamReader(path))
            using (var jr = new JsonTextReader(sr))
                return JObject.Load(jr);
        }
    }
}
