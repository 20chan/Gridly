using Microsoft.Xna.Framework.Graphics;

namespace Gridly
{
    public interface IScene
    {
        void Update();
        void Draw(SpriteBatch sb);
    }
}
