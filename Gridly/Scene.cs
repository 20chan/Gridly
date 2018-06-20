using Microsoft.Xna.Framework.Graphics;
using Gridly.UI;

namespace Gridly
{
    public abstract class Scene
    {
        public GUIManager SceneGUI { get; protected set; } = new GUIManager();

        public abstract void Update();
        public abstract void Draw(SpriteBatch sb);

        public virtual void OnLoad()
        {
            MainScene.CurrentGUI.Attach(SceneGUI);
        }

        public virtual void OnUnload()
        {
            MainScene.CurrentGUI.Dettach(SceneGUI);
        }
    }
}
