using Newtonsoft.Json.Linq;

namespace Gridly
{
    public class TestCase
    {
        public readonly bool[][] Inputs, Outputs;

        public TestCase(bool[][] inputs, bool[][] outputs)
        {
            Inputs = inputs;
            Outputs = outputs;
        }

        public JObject Serialize()
        {
            return new JObject()
            {
                { "input", JArray.FromObject(Inputs) },
                { "output", JArray.FromObject(Outputs) }
            };
        }

        public static TestCase Deserialize(JToken obj)
        {
            var input = obj["input"].ToObject<bool[][]>();
            var output = obj["output"].ToObject<bool[][]>();
            return new TestCase(input, output);
        }
    }
}
