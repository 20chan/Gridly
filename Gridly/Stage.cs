using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Gridly
{
    public class Stage
    {
        List<uint> inputNeurons, outputNeurons;
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
            inputNeurons = new List<uint>();
            outputNeurons = new List<uint>();
            foreach (uint i in obj["Inputs"])
                SetInputNeuron(editor.FromID(newIds[Array.IndexOf(orgIds, i)]) as BasicNeuron);
            foreach (uint i in obj["Outputs"])
                SetOutputNeuron(editor.FromID(newIds[Array.IndexOf(orgIds, i)]) as BasicNeuron);
        }

        public void SetInputNeuron(BasicNeuron neuron)
        {
            neuron.SetInputNeuron(inputNeurons.Count);
            inputNeurons.Add(neuron.ID);
        }

        public void SetOutputNeuron(BasicNeuron neuron)
        {
            neuron.SetOutputNeuron(outputNeurons.Count);
            outputNeurons.Add(neuron.ID);
        }
    }
}
