using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Gridly.UI;

namespace Gridly
{
    public class Stage
    {
        public bool Succeed { get; private set; }
        List<BasicNeuron> inputNeurons, outputNeurons;
        TestCase[] cases;
        int inputLength, outputLength;

        Tester curTester;
        List<bool>[] outputStack;
        int elapsedTick;
        int outputCheckIndex;

        public TestCaseVisualizer Visualizer { get; private set; }

        int currentTestCase;
        TestCase curCase => cases[currentTestCase];

        protected Stage(params TestCase[] testcases)
        {
            cases = testcases;
            inputNeurons = new List<BasicNeuron>();
            outputNeurons = new List<BasicNeuron>();

            Visualizer = new TestCaseVisualizer();
        }

        public JObject Serialize(StageEditor editor)
        {
            var obj = editor.Serialize();
            obj["Inputs"] = JArray.FromObject(inputNeurons.Select(n => n.ID));
            obj["Outputs"] = JArray.FromObject(outputNeurons.Select(n => n.ID));
            obj["TestCases"] = JArray.FromObject(cases.Select(c => c.Serialize()));
            return obj;
        }

        protected void Deserialize(StageEditor editor, JObject obj)
        {
            cases = obj["TestCases"].Select(t => TestCase.Deserialize(t)).ToArray();
            inputLength = curCase.Inputs[0].Length;
            outputLength = curCase.Outputs[0].Length;
            editor.DeserializeParts(JArray.FromObject(obj["Parts"]), out var orgIds, out var newIds);
            foreach (uint i in obj["Inputs"])
                SetInputNeuron(editor.FromID(newIds[Array.IndexOf(orgIds, i)]) as BasicNeuron);
            foreach (uint i in obj["Outputs"])
                SetOutputNeuron(editor.FromID(newIds[Array.IndexOf(orgIds, i)]) as BasicNeuron);

            outputStack = new List<bool>[curCase.Outputs.Length];
            for (int i = 0; i < curCase.Outputs.Length; i++)
                outputStack[i] = new List<bool>();

            curTester = new Tester(curCase);
            Visualizer.Tester = curTester;
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

        public void Start(int index = 0)
        {
            currentTestCase = index;

            if (curCase.Inputs.Length != inputNeurons.Count)
            {
                throw new Exception("입력 뉴런 개수가 다른디..");
            }

            Succeed = false;
            elapsedTick = -1;
            foreach (var stack in outputStack)
                stack.Clear();
            outputCheckIndex = 0;
            inputLength = curCase.Inputs[0].Length;
            outputLength = curCase.Outputs[0].Length;
            curTester.TestCase = curCase;
            curTester.Reset();

            Visualizer.Tester = curTester;
        }

        public void Tick()
        {
            elapsedTick++;
            if (elapsedTick < inputLength)
                for (int i = 0; i < inputNeurons.Count; i++)
                    if (curTester.Inputs[i][elapsedTick])
                        inputNeurons[i].ActivateImmediate();

            for (int i = 0; i < outputStack.Length; i++)
                outputStack[i].Add(outputNeurons[i].Activated);

            bool allCorrect = true;
            for (int i = 0; i < outputStack.Length; i++)
            {
                for (int idx = outputCheckIndex; idx < outputStack[i].Count; idx++)
                {
                    if (outputStack[i][idx] != curTester.Outputs[i][idx - outputCheckIndex])
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
                var cnt = outputStack[0].Count - outputCheckIndex;
                curTester.CorcondanceCount = cnt;
                if (cnt == outputLength)
                {
                    if (currentTestCase + 1 == cases.Length)
                    {
                        curTester.Succeed = true;
                        Succeed = true;
                    }
                    else
                    {
                        currentTestCase++;
                        Visualizer.UpdateUI();
                        Start(currentTestCase);
                        return;
                    }
                }
            }
            else
            {
                // 최대의 outputCheckIndex를 찾기
                int i = outputCheckIndex;
                for (; i < outputStack[0].Count; i++)
                {
                    allCorrect = true;
                    for (int j = 0; j < outputStack.Length; j++)
                    {
                        for (int idx = i; idx < outputStack[j].Count; idx++)
                        {
                            if (outputStack[j][idx] != curTester.Outputs[j][idx - i])
                            {
                                allCorrect = false;
                                break;
                            }
                        }
                        if (!allCorrect)
                            break;
                    }
                    if (allCorrect)
                        break;
                }
                outputCheckIndex = i;
                curTester.CorcondanceCount = outputStack[0].Count - outputCheckIndex;
            }
            curTester.ElapsedTick = elapsedTick;
            Visualizer.UpdateUI();
        }

        public void Log()
        {
            Console.WriteLine($"Expected outputs:");
            for (int i = 0; i < curCase.Outputs.Length; i++)
                Console.WriteLine($"{i}: {string.Join(",", curCase.Outputs[i])}");

            Console.WriteLine($"Actual outputs stacks:");
            for (int i = 0; i < curCase.Outputs.Length; i++)
                Console.WriteLine($"{i}: {string.Join(",", outputStack[i].Skip(outputCheckIndex))}");
            Console.WriteLine();
        }

        public static Stage Load(StageEditor editor, JObject obj)
        {
            var stage = new Stage();
            stage.Deserialize(editor, obj);
            return stage;
        }

        public static Stage New(StageEditor editor)
        {
            var stage = new Stage();
            return stage;
        }
    }
}
