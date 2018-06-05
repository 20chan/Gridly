using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly
{
    public class Neuron : Collidable
    {
        public readonly uint ID;
        private static Vector2 Origin = Resources.NeuronTexture.Bounds.Size.ToVector2() / 2;

        private List<Neuron> connecting;
        private List<bool> couldDisconnected;

        public bool Activated { get; private set; }
        private bool shouldActivate = false;

        public Neuron(Vector2 pos, uint id)
        {
            ID = id;
            Position = pos;
            connecting = new List<Neuron>();
            couldDisconnected = new List<bool>();
            Activated = false;
            friction = .1f;
        }

        public void UpdateSynapse()
        {
            if (Activated)
                foreach (var n in connecting)
                    n.Activate();
            Activated = false;
        }

        public void UpdateState()
        {
            if (shouldActivate)
            {
                Activated = true;
                shouldActivate = false;
            }
        }

        public override Rectangle GetBounds()
            => new Rectangle((Position - Origin).ToPoint(), Resources.NeuronTexture.Bounds.Size);

        public void DrawSynapse(SpriteBatch sb)
        {
            for (int i = 0; i < connecting.Count; i++)
            {
                var n = connecting[i];
                GUI.DrawLine(sb, Position, n.Position, 2f,
                    couldDisconnected[i] ? Color.Yellow : Color.Blue, 0.5f);
            }
        }

        public void DrawUpperSynapse(SpriteBatch sb)
        {
            foreach (var n in connecting)
            {
                var ratio80 = new Vector2(
                    .2f * Position.X + .8f * n.Position.X,
                    .2f * Position.Y + .8f * n.Position.Y);
                GUI.DrawLine(sb, ratio80, n.Position, 2f, Color.Gray, 0.3f);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(
                Resources.NeuronTexture,
                position: Position,
                origin: Origin,
                color: Activated ? Color.Blue : Color.White,
                layerDepth: .5f);
        }

        public void ConnectTo(Neuron n)
        {
            if (!connecting.Contains(n))
            {
                connecting.Add(n);
                couldDisconnected.Add(false);
            }
        }

        public void Disconnect(Neuron n)
        {
            if (connecting.Contains(n))
            {
                var idx = connecting.IndexOf(n);
                connecting.RemoveAt(idx);
                couldDisconnected.RemoveAt(idx);
            }
        }

        public void DisconnectIntersection(Vector2 p1, Vector2 p2)
        {
            var clone = connecting.ToArray();
            foreach (var n in clone)
                if (Geometry.IsTwoSegmentsInstersect(Position, n.Position, p1, p2))
                    Disconnect(n);
        }

        public void PreviewDisconnect(Vector2 p1, Vector2 p2)
        {
            for (int i = 0; i < connecting.Count; i++)
                couldDisconnected[i] = Geometry.IsTwoSegmentsInstersect(Position, connecting[i].Position, p1, p2);
        }

        public void Activate()
        {
            shouldActivate = true;
        }

        public void ActivateImmediate()
        {
            Activated = true;
        }

        public bool IsCollided(Neuron n)
            => GetBounds().Intersects(n.GetBounds());

        public void Log(StringBuilder sb)
        {
            sb.AppendLine($"Neuron {ID}");
            sb.AppendLine($"  Activated: {Activated}");
            sb.AppendLine($"  ReadyActivate: {shouldActivate}");
            var cs = connecting.Count == 0
                ? "None" : string.Join(", ", connecting.Select(c => c.ID));
            sb.AppendLine($"  Connected Neurons: {cs}");
            sb.AppendLine();
        }
    }
}
