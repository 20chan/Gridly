using System.Linq;
using Newtonsoft.Json.Linq;

namespace Gridly
{
    public class Stage
    {
        DefaultPartEditor Editor;
        TestCase[] cases;

        public Stage(DefaultPartEditor editor, params TestCase[] testcases)
        {
            Editor = editor;
            cases = testcases;
        }

        public JObject Serialize()
        {
            var obj = Editor.Serialize();
            obj["TestCases"] = JArray.FromObject(cases.Select(c => c.Serialize()));
            return obj;
        }

        public void Deserialize(JObject obj)
        {
            cases = obj["TestCases"].Select(t => TestCase.Deserialize(t)).ToArray();
            Editor.DeserializeParts(JArray.FromObject(obj["Parts"]), out var orgIds, out var newIds);
        }
    }
}
