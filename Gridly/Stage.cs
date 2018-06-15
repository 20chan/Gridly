using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Gridly
{
    public class Stage
    {
        public bool Succeed { get; private set; }
        List<BasicNeuron> inputNeurons, outputNeurons;
        TestCase[] cases;
        int elapsedTick = 0;
        int inputLength, outputLength;
        List<bool>[] outputStack;
        int outputCheckIndex;

        int currentTestCase;

        public Stage(params TestCase[] testcases)
        {
            cases = testcases;
            inputNeurons = new List<BasicNeuron>();
            outputNeurons = new List<BasicNeuron>();
        }

        public JObject Serialize(StageEditor editor)
        {
            var obj = editor.Serialize();
            obj["Inputs"] = JArray.FromObject(inputNeurons.Select(n => n.ID));
            obj["Outputs"] = JArray.FromObject(outputNeurons.Select(n => n.ID));
            obj["TestCases"] = JArray.FromObject(cases.Select(c => c.Serialize()));
            return obj;
        }

        public void Deserialize(StageEditor editor, JObject obj)
        {
            cases = obj["TestCases"].Select(t => TestCase.Deserialize(t)).ToArray();
            inputLength = cases[0].Inputs[0].Length;
            outputLength = cases[0].Outputs[0].Length;
            editor.DeserializeParts(JArray.FromObject(obj["Parts"]), out var orgIds, out var newIds);
            foreach (uint i in obj["Inputs"])
                SetInputNeuron(editor.FromID(newIds[Array.IndexOf(orgIds, i)]) as BasicNeuron);
            foreach (uint i in obj["Outputs"])
                SetOutputNeuron(editor.FromID(newIds[Array.IndexOf(orgIds, i)]) as BasicNeuron);

            outputStack = new List<bool>[cases[0].Outputs.Length];
            for (int i = 0; i < cases[0].Outputs.Length; i++)
                outputStack[i] = new List<bool>();
        }

        public void SetInputNeuron(BasicNeuron neuron)
        {
            neuron.SetInputNeuron(inputNeurons.Count);
            inputNeurons.Add(neuron);
        }

        public void SetOutputNeuron(BasicNeuron neuron)
        {
            neuron.SetOutputNeuron(outputNeurons.Count);
            outputNeurons.Add(neuron);
        }
        
        public void Start()
        {
            if (cases[0].Inputs.Length != inputNeurons.Count)
            {
                throw new Exception("입력 뉴런 개수가 다른디..");
            }

            Succeed = false;
            elapsedTick = 0;
            foreach (var stack in outputStack)
                stack.Clear();
            // TODO: 테케 인덱스 설정
            currentTestCase = 0;
            outputCheckIndex = 0;
        }

        public void Tick()
        {
            if (elapsedTick < inputLength)
                for (int i = 0; i < inputNeurons.Count; i++)
                    if (cases[currentTestCase].Inputs[i][elapsedTick])
                        inputNeurons[i].ActivateImmediate();

            for (int i = 0; i < outputStack.Length; i++)
                outputStack[i].Add(outputNeurons[i].Activated);

            bool allCorrect = true;
            var curcase = cases[currentTestCase];
            for (int i = 0; i < outputLength; i++)
            {
                for (int idx = outputCheckIndex; idx < outputLength; idx++)
                {
                    if (outputStack[i][idx] != curcase.Outputs[i][idx - outputCheckIndex])
                    {
                        allCorrect = false;
                        break;
                    }
                }
                if (!allCorrect)
                    break;
            }
            if (allCorrect)
            {
                if (elapsedTick - outputCheckIndex == outputLength)
                {
                    Succeed = true;
                }
            }
            else
            {
                outputCheckIndex = elapsedTick + 1;
            }
            
            elapsedTick++;
        }
    }
}
