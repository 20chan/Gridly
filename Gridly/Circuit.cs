using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;

namespace Gridly
{
    public class Circuit : Part
    {
        private List<IConnectable> connectedInputs;
        private List<IConnectable> connectedOutputs;
        public CircuitEditor Editor { get; private set; }

        public Circuit(Vector2 pos) : base(pos)
        {
            connectedInputs = new List<IConnectable>();
            connectedOutputs = new List<IConnectable>();
            Editor = new CircuitEditor(this);
            BackColor = Color.Purple;
        }

        public Circuit(JObject obj) : this(Vector2.Zero)
        {
            Editor.Deserialize(obj);
        }

        public Circuit() : this(Vector2.Zero)
        {
            Initialized = false;
        }

        public override void ConnectTo(IConnectable n)
        {
            base.ConnectTo(n);

            connectedOutputs.Add(n);
        }

        public void ConnectFrom(IConnectable from)
        {
            connectedInputs.Add(from);
        }

        public void DisconnectFrom(IConnectable from)
        {
            // if (connectedInputs.Contains(from))
            connectedInputs.Remove(from);
        }

        public override void Activate(IConnectable from)
        {
            var idx = connectedInputs.IndexOf(from);
            Editor.ActivateInput(idx);
        }

        /// <summary>
        /// NOT USED
        /// </summary>
        public override void ActivateImmediate() { }

        public override void Log(StringBuilder sb)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            // Editor.TickSynapse();
        }

        public override void UpdateSynapse()
        {

        }

        public void ActivateOutput(int idx)
        {
            if (idx < connectedOutputs.Count)
                connectedOutputs[idx].Activate(this);
        }

        public override void DrawSynapse(SpriteBatch sb)
        {
            for (int i = 0; i < connecting.Count; i++)
            {
                var n = connecting[i];
                sb.DrawLine(Position, n.Position, 2f,
                    couldDisconnected[i]
                    ? Color.Red
                    : n is Circuit ? Color.Orange : Color.LightBlue, 0.5f);
            }
        }

        public override void Deserialize(JObject obj, uint[] orgIDs, uint[] newIDs, Part[] parts)
        {
            var conns = obj["Connecting"].ToObject<uint[]>();
            foreach (var c in conns)
            {
                uint id = newIDs[Array.IndexOf(orgIDs, c)];
                connecting.Add(parts.First(p => p.ID == id));
                couldDisconnected.Add(false);
            }
            Editor.Deserialize(obj);
            Initialized = true;
        }

        public override JObject Serialize()
        {
            return new JObject
            {
                { "ID", ID },
                { "Type", 1 },
                { "Connecting", JArray.FromObject(connecting.Select(c => c.ID)) },
                { "Inputs", JArray.FromObject(Editor.GetInputIDs()) },
                { "Outputs", JArray.FromObject(Editor.GetOutputIDs()) },
                { "Parts", Editor.SerializeParts()},
            };
        }
    }
}
