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

        protected void SetInput(Neuron n)
        {
            n.DefaultColor = Color.Orange;
            n.DisplayNumber = true;
            n.Number = inputNeurons.Count;
            inputNeurons.Add(n);
        }

        protected void SetOutput(Neuron n)
        {
            n.DefaultColor = Color.LightBlue;
            n.DisplayNumber = true;
            n.Number = outputNeurons.Count;
            outputNeurons.Add(n);
        }

        protected override void UpdatePartInput()
        {
            base.UpdatePartInput();

            if (IsKeyDown(Keys.I))
                if (IsPartOnPos(MousePos, out var p))
                    if (p is Neuron n)
                        SetInput(n);
            if (IsKeyDown(Keys.O))
                if (IsPartOnPos(MousePos, out var p))
                    if (p is Neuron n)
                        SetOutput(n);
        }

        internal IEnumerable<uint> GetInputIDs()
            => inputNeurons.Select(n => n.ID);

        internal IEnumerable<uint> GetOutputIDs()
            => outputNeurons.Select(n => n.ID);

        internal IEnumerable<uint> GetPartsIDs()
            => parts.Select(n => n.ID);

        public override void Deserialize(JObject obj)
        {
            DeserializeParts((JArray)obj["Parts"], out var refIds, out var newIds);

            foreach (uint n in obj["Inputs"])
                SetInput(Match(newIds[Array.IndexOf(refIds, n)]));
            foreach (uint n in obj["Outputs"])
                SetOutput(Match(newIds[Array.IndexOf(refIds, n)]));

            Neuron Match(uint id)
                => (Neuron)parts.First(p => p.ID == id);
        }

        public void DeserializeParts(JArray arr, out uint[] refIDs, out uint[] newIDs)
        {
            refIDs = new uint[arr.Count];
            newIDs = new uint[arr.Count];

            parts.Clear();
            int i = 0;
            foreach (JObject p in arr)
            {
                Part part;
                if ((int)p["Type"] == 0)
                    part = new Neuron();
                else
                    part = new Circuit();

                part.Position = new Vector2(800, 500);
                refIDs[i] = (uint)p["ID"];
                newIDs[i] = part.ID;
                parts.Add(part);
                i++;
            }
            for (i = 0; i < parts.Count; i++)
            {
                parts[i].Deserialize((JObject)arr[i], refIDs, newIDs, parts.ToArray());
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
