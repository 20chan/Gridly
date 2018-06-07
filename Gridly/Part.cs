using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly
{
    public abstract class Part : Collidable, IConnectable
    {
        private static uint idCount = 0;
        public uint ID { get; }
        protected static Vector2 Origin = Resources.PartTexture.Bounds.Size.ToVector2() / 2;

        protected Color BackColor;
        protected List<IConnectable> connecting;
        protected List<bool> couldDisconnected;

        public Part(Vector2 pos)
        {
            Position = pos;
            ID = idCount++;

            BackColor = Color.White;
            connecting = new List<IConnectable>();
            couldDisconnected = new List<bool>();
            friction = .1f;
        }
        
        public override Rectangle GetBounds()
            => new Rectangle((Position - Origin).ToPoint(), Resources.PartTexture.Bounds.Size);

        public bool IsCollided(Collidable n)
            => GetBounds().Intersects(n.GetBounds());

        public virtual void ConnectTo(IConnectable n)
        {
            if (!connecting.Contains(n))
            {
                if (n is Circuit c)
                    c.ConnectFrom(this);
                connecting.Add(n);
                couldDisconnected.Add(false);
            }
        }

        public void Disconnect(IConnectable n)
        {
            if (connecting.Contains(n))
            {
                if (n is Circuit c)
                    c.DisconnectFrom(this);
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

        public abstract void Log(StringBuilder sb);
        public abstract void Activate(IConnectable from);
        public abstract void ActivateImmediate();
        public abstract void UpdateSynapse();
        public abstract void UpdateState();
        public virtual void DrawSynapse(SpriteBatch sb) { }
        public virtual void DrawUpperSynapse(SpriteBatch sb)
        {
            foreach (var n in connecting)
            {
                var ratio80 = new Vector2(
                    .2f * Position.X + .8f * n.Position.X,
                    .2f * Position.Y + .8f * n.Position.Y);
                sb.DrawLine(ratio80, n.Position, 2f, Color.Gray, 0.3f);
            }
        }
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(
                Resources.PartTexture,
                position: Position,
                origin: Origin,
                color: BackColor,
                layerDepth: .5f);
        }
    }
}
