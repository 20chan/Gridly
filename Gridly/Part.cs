using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;

namespace Gridly
{
    public abstract class Part : Collidable, IConnectable
    {
        private static uint idCount = 0;
        public uint ID { get; }
        protected float edgeScale = 0.8f;
        private static Vector2 edgeOrigin = Resources.EdgeTexture.Bounds.Size.ToVector2() / 2;
        protected List<IConnectable> connecting;
        protected List<bool> couldDisconnected;
        protected bool selected;
        
        public bool Initialized { get; protected set; }

        public Part(Vector2 pos)
        {
            Position = pos;
            ID = idCount++;

            connecting = new List<IConnectable>();
            couldDisconnected = new List<bool>();
            friction = .1f;
            Initialized = true;
        }
        
        public bool IsCollided(Collidable n)
            => GetBounds().Intersects(n.GetBounds());

        public virtual void ConnectTo(IConnectable n)
        {
            if (!connecting.Contains(n))
            {
                if (n is INeedInput c)
                    c.ConnectFrom(this);
                connecting.Add(n);
                couldDisconnected.Add(false);
            }
            else
            {
                Disconnect(n);
            }
        }

        public virtual void Disconnect(IConnectable n)
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

        public void Select()
        {
            selected = true;
        }

        public void Unselect()
        {
            selected = false;
        }
        
        public abstract void Log(StringBuilder sb);
        public abstract void Activate(IConnectable from);
        public abstract void ActivateImmediate();
        public abstract void UpdateSynapse();
        public abstract void UpdateState();
        public abstract JObject Serialize();
        public abstract void Deserialize(JObject obj, uint[] orgIDs, uint[] newIDs, Part[] parts);
        public virtual void Draw(SpriteBatch sb) { }
        public virtual void DrawBack(SpriteBatch sb)
        {
            if (selected)
            {
                sb.Draw(
                    Resources.EdgeTexture,
                    position: Position,
                    origin: edgeOrigin,
                    scale: new Vector2(edgeScale),
                    color: Color.Green);
            }
        }
    }
}
