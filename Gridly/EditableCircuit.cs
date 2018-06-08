using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace Gridly
{
    public class EditableCircuit : Circuit
    {
        public CircuitEditor Editor { get; private set; }

        public EditableCircuit(Vector2 pos) : base(pos)
        {
            Editor = new CircuitEditor(this);
        }

        public EditableCircuit() : base()
        {

        }

        public EditableCircuit(JObject obj) : this(Vector2.Zero)
        {
            Editor.Deserialize(obj);
        }

        public override void Activate(IConnectable from)
        {
            var idx = connectedInputs.IndexOf(from);
            Editor.ActivateInput(idx);
        }

        public override void UpdateState()
        {

        }

        public override void UpdateSynapse()
        {

        }

        public override void Deserialize(JObject obj, uint[] orgIDs, uint[] newIDs, Part[] parts)
        {
            var conns = obj["Connecting"].ToObject<uint[]>();
            foreach (var c in conns)
            {
                uint id = newIDs[Array.IndexOf(orgIDs, c)];
                var part = parts.First(p => p.ID == id);
                ConnectTo(part);
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
                { "Position", new JObject { { "x", Position.X }, { "y", Position.Y } } },
                { "Connecting", JArray.FromObject(connecting.Select(c => c.ID)) },
                { "Inputs", JArray.FromObject(Editor.GetInputIDs()) },
                { "Outputs", JArray.FromObject(Editor.GetOutputIDs()) },
                { "Parts", Editor.SerializeParts()},
            };
        }
    }
}
