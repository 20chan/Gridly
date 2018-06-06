using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public abstract class GUI
    {
        public abstract bool HandleInputs(MouseState state, Vector2 position);
        public abstract void Draw(SpriteBatch sb);
    }
}
