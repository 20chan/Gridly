using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly
{
    public class Neuron
    {
        public Vector2 Position { get; private set; }
        private static Vector2 Origin = Resources.NeuronTexture.Bounds.Size.ToVector2() / 2;

        public Neuron(Vector2 pos)
            => Position = pos;

        public void UpdateSynapse()
        {

        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Resources.NeuronTexture, position: Position, origin: Origin, color: Color.White);
        }
    }
}
