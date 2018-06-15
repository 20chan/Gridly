using System.Linq;
using Newtonsoft.Json.Linq;

namespace Gridly
{
    public class DefaultPartEditor : PartEditor
    {
        public override JObject Serialize()
        {
            return new JObject()
            {
                { "Parts", JArray.FromObject(parts.Select(p => p.Serialize())) }
            };
        }

        public override void Deserialize(JObject arr)
        {
            throw new System.NotImplementedException();
        }
    }
}
