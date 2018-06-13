using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly
{
    public abstract class Neuron : Part
    {
        protected static Vector2 Origin = Resources.PartTexture.Bounds.Size.ToVector2() / 2;
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
    }
}
