using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public class GUIManager
    {
        private List<GUI> uis;
        public GUIManager()
        {
            uis = new List<GUI>();
        }

        public void Add(GUI gui)
        {
            uis.Add(gui);
        }

        public void DrawUI(SpriteBatch sb)
        {
            foreach (var ui in uis)
                ui.Draw(sb);
        }

        /// <summary>
        /// Returns if mouse event was handled
        /// </summary>
        public bool HandleInput()
        {
            foreach (var ui in uis)
            {
                if (ui.ProcessInput())
                    return true;
            }
            return false;
        }
    }
}
