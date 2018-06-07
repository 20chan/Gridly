using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public abstract class GUI
    {
        public abstract bool ProcessInput();
        public abstract void Draw(SpriteBatch sb);
    }
}
