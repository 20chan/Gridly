using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Gridly
{
    public class Stage
    {
        uint[] inputNeurons, outputNeurons;
        TestCase[] cases;

        public Stage(params TestCase[] testcases)
        {
            cases = testcases;
        }

        public JObject Serialize(StageEditor editor)
        {
            var obj = editor.Serialize();
            obj["Inputs"] = JArray.FromObject(inputNeurons);
            obj["Outputs"] = JArray.FromObject(outputNeurons);
            obj["TestCases"] = JArray.FromObject(cases.Select(c => c.Serialize()));
            return obj;
        }

        public void Deserialize(StageEditor editor, JObject obj)
        {
            cases = obj["TestCases"].Select(t => TestCase.Deserialize(t)).ToArray();
            editor.DeserializeParts(JArray.FromObject(obj["Parts"]), out var orgIds, out var newIds);
            inputNeurons = (from int i in obj["Inputs"]
                            let idx = Array.IndexOf(orgIds, i)
                            select newIds[idx]).ToArray();
            outputNeurons = (from int i in obj["Outputs"]
                             let idx = Array.IndexOf(orgIds, i)
                             select newIds[idx]).ToArray();
        }
    }
}
