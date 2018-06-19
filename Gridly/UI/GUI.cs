using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public abstract class GUI
    {
        public abstract bool IsHovering(Vector2 pos);
        public abstract bool ProcessInput();
        public abstract void Draw(SpriteBatch sb);
    }
}
