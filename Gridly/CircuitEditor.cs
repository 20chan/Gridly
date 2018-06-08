using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static Gridly.Inputs;

namespace Gridly
{
    public class CircuitEditor : PartEditor
    {
        Circuit self;
        List<Neuron> inputNeurons, outputNeurons;

        public CircuitEditor(Circuit c) : base()
        {
            self = c;
            inputNeurons = new List<Neuron>();
            outputNeurons = new List<Neuron>();
        }

        public void ActivateInput(int idx)
        {
            if (inputNeurons.Count > idx)
                inputNeurons[idx].Activate(self);
        }

        public override void TickSynapse()
        {
            base.TickSynapse();

            for (int i = 0; i < outputNeurons.Count; i++)
            {
                if (outputNeurons[i].Activated)
                    self.ActivateOutput(i);
            }
        }

        protected override void UpdatePartInput()
        {
            base.UpdatePartInput();

            if (IsKeyDown(Keys.I))
                if (IsPartOnPos(MousePos, out var p))
                    if (p is Neuron n)
                    {
                        n.DefaultColor = Color.Orange;
                        n.DisplayNumber = true;
                        n.Number = inputNeurons.Count;
                        inputNeurons.Add(n);
                    }
            if (IsKeyDown(Keys.O))
                if (IsPartOnPos(MousePos, out var p))
                    if (p is Neuron n)
                    {
                        n.DefaultColor = Color.LightBlue;
                        n.DisplayNumber = true;
                        n.Number = outputNeurons.Count;
                        outputNeurons.Add(n);
                    }
        }

        internal IEnumerable<uint> GetInputIDs()
            => inputNeurons.Select(n => n.ID);

        internal IEnumerable<uint> GetOutputIDs()
            => outputNeurons.Select(n => n.ID);

        internal IEnumerable<uint> GetPartsIDs()
            => parts.Select(n => n.ID);

        public override void Deserialize(JObject obj)
        {
            DeserializeParts((JArray)obj["Parts"], out var refIds);

            foreach (int n in obj["Inputs"])
                inputNeurons.Add((Neuron)parts[Array.IndexOf(refIds, n)]);
            foreach (int n in obj["Outputs"])
                outputNeurons.Add((Neuron)parts[Array.IndexOf(refIds, n)]);
        }

        public void DeserializeParts(JArray arr, out int[] refIDs)
        {
            refIDs = new int[arr.Count];

            parts.Clear();
            int i = 0;
            foreach (JObject p in arr)
            {
                Part part;
                if ((int)p["Type"] == 0)
                    part = new Circuit();
                else
                    part = new Neuron();

                refIDs[i] = (int)p["ID"];
                parts.Add(part);
                i++;
            }
            for (i = 0; i < parts.Count; i++)
            {
                parts[i].Deserialize((JObject)arr[i], refIDs, parts.ToArray());
            }
        }

        protected override JObject Serialize()
        {
            var res = self.Serialize();
            // 루트 회로의 연결들은 비어있어야 한다
            res["Connecting"] = JArray.FromObject(new int[0]);
            return res;
        }

        public JArray SerializeParts()
        {
            return JArray.FromObject(parts.Select(p => p.Serialize()));
        }
    }
}
