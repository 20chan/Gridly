using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly
{
    public abstract class Neuron : Part
    {
        protected static Vector2 Origin = Resources.PartTexture.Bounds.Size.ToVector2() / 2;
        private static float edgeScale = 0.2f;
        private static Vector2 edgeOrigin = Resources.EdgeTexture.Bounds.Size.ToVector2() / 2;
        protected Color BackColor;

        public Neuron(Vector2 pos) : base(pos)
        {
            BackColor = Color.White;
        }

        public override Rectangle GetBounds()
            => new Rectangle((Position - Origin).ToPoint(), Resources.PartTexture.Bounds.Size);
        
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(
                Resources.PartTexture,
                position: Position,
                origin: Origin,
                color: BackColor,
                layerDepth: .5f);
        }
        public virtual void DrawBack(SpriteBatch sb)
        {
            if (selected)
            {
                sb.Draw(
                    Resources.EdgeTexture,
                    position: Position,
                    origin: edgeOrigin,
                    //scale: new Vector2(edgeScale),
                    color: Color.Green);
            }
        }
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
    }
}
