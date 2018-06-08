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

        public override void Deserialize(JObject arr)
        {

        }

        public override JObject Serialize()
            => self.Serialize();

        public JArray SerializeParts()
        {
            return JArray.FromObject(parts.Select(p => p.Serialize()));
        }
    }
}
