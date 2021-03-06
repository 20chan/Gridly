﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using static Gridly.Inputs;

namespace Gridly
{
    public class CircuitEditor : PartEditor
    {
        Circuit self;
        List<BasicNeuron> inputNeurons, outputNeurons;

        public CircuitEditor(Circuit c) : base()
        {
            self = c;
            inputNeurons = new List<BasicNeuron>();
            outputNeurons = new List<BasicNeuron>();
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

        protected void SetInput(BasicNeuron n)
        {
            n.SetInputNeuron(inputNeurons.Count);
            inputNeurons.Add(n);
        }

        protected void SetOutput(BasicNeuron n)
        {
            n.SetOutputNeuron(outputNeurons.Count);
            outputNeurons.Add(n);
        }

        protected override void UpdatePartInput()
        {
            base.UpdatePartInput();

            if (IsKeyDown(Keys.I))
                if (IsPartOnPos(MousePos, out var p))
                    if (p is BasicNeuron n)
                        SetInput(n);
            if (IsKeyDown(Keys.O))
                if (IsPartOnPos(MousePos, out var p))
                    if (p is BasicNeuron n)
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

            BasicNeuron Match(uint id)
                => (BasicNeuron)parts.First(p => p.ID == id);
        }

        public override JObject Serialize()
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
