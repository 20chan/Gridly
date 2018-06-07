using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly
{
    public class Neuron : Part
    {
        public bool Activated { get; private set; }
        private bool shouldActivate = false;

        public Neuron(Vector2 pos) : base(pos)
        {
            Activated = false;
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
                    ? Color.Yellow
                    : n is Circuit ? Color.Orange : Color.White, 0.5f);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(
                Resources.PartTexture,
                position: Position,
                origin: Origin,
                color: Activated ? Color.Blue : Color.White,
                layerDepth: .5f);
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
    }
}
