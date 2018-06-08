using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace Gridly
{
    public class Neuron : Part
    {
        public bool Activated { get; private set; }
        public Color DefaultColor { get; set; }
        private bool shouldActivate = false;

        public bool DisplayNumber { get; set; }
        public int Number { get; set; }

        public Neuron(Vector2 pos) : base(pos)
        {
            Activated = false;
            DefaultColor = Color.White;
            DisplayNumber = false;
        }

        public Neuron() : this(Vector2.Zero)
        {
            Initialized = false;
        }

        public override void UpdateSynapse()
        {
            if (Activated)
                foreach (var n in connecting)
                    n.Activate(this);
            Activated = false;
        }

        public override void UpdateState()
        {
            if (shouldActivate)
            {
                Activated = true;
                shouldActivate = false;
            }
        }

        public override void DrawSynapse(SpriteBatch sb)
        {
            for (int i = 0; i < connecting.Count; i++)
            {
                var n = connecting[i];
                sb.DrawLine(Position, n.Position, 2f,
                    couldDisconnected[i]
                    ? Color.Red
                    : n is Circuit ? Color.Orange : Color.White, 0.5f);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            BackColor = Activated ? Color.Blue : DefaultColor;
            base.Draw(sb);
            DrawNumber(sb);
        }

        public void DrawNumber(SpriteBatch sb)
        {
            if (DisplayNumber)
            {
                sb.DrawString(Resources.DefaultFont,
                    Number.ToString(),
                    new Vector2(Position.X + 10, Position.Y),
                    Color.Black);
            }
        }

        public override void Log(StringBuilder sb)
        {
            sb.AppendLine($"Neuron {ID}");
            sb.AppendLine($"  Activated: {Activated}");
            sb.AppendLine($"  ReadyActivate: {shouldActivate}");
            var cs = connecting.Count == 0
                ? "None" : string.Join(", ", connecting.Select(c => c.ID));
            sb.AppendLine($"  Connected Neurons: {cs}");
            sb.AppendLine();
        }

        public override void Activate(IConnectable from)
        {
            shouldActivate = true;
        }

        public override void ActivateImmediate()
        {
            Activated = true;
        }

        public override void Deserialize(JObject obj, uint[] orgIDs, uint[] newIDs, Part[] parts)
        {
            Activated = (bool)obj["Activated"];
            var conns = obj["Connecting"].ToObject<uint[]>();
            foreach (var c in conns)
            {
                uint id = newIDs[Array.IndexOf(orgIDs, c)];
                var part = parts.First(p => p.ID == id);
                ConnectTo(part);
            }

            Initialized = true;
        }

        public override JObject Serialize()
        {
            return new JObject
            {
                { "ID", ID },
                { "Type", 0 },
                { "Activated", Activated },
                { "Position", new JObject { { "x", Position.X }, { "y", Position.Y } } },
                { "Connecting", JArray.FromObject(connecting.Select(c => c.ID)) }
            };
        }
    }
}
