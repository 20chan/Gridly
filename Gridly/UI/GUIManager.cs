using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public class GUIManager
    {
        private List<GUI> uis;
        private List<GUIManager> layers;

        public GUIManager()
        {
            uis = new List<GUI>();
            layers = new List<GUIManager>();
        }

        public void Add(GUI gui)
        {
            uis.Add(gui);
        }

        public void Attach(GUIManager child)
        {
            if (layers.Contains(child)) return;
            layers.Add(child);
        }

        public void Dettach(GUIManager child)
        {
            layers.Remove(child);
        }

        public void DrawUI(SpriteBatch sb)
        {
            foreach (var layer in layers)
                layer.DrawUI(sb);
            foreach (var ui in uis)
                ui.Draw(sb);
        }

        /// <summary>
        /// Returns if mouse event was handled
        /// </summary>
        public bool HandleInput(out bool anyoneHovering)
        {
            anyoneHovering = false;
            foreach (var layer in layers)
            {
                var handled = layer.HandleInput(out var hov);
                if (hov)
                    anyoneHovering = true;
                if (handled)
                    return true;
            }
            foreach (var ui in uis)
            {
                if (ui.IsHovering(Inputs.MousePos))
                    anyoneHovering = true;
                if (ui.ProcessInput())
                    return true;
            }
            return false;
        }
    }
}
