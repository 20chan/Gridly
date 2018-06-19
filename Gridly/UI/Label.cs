using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public class Label : GUI
    {
        public string Text { get; set; }
        public Alignment Alignment { get; set; }
        public Color ForeColor { get; set; } = Color.Black;
        public SpriteFont Font { get; set; } = null;
        public Rectangle Bound { get; protected set; }

        public int X { get; set; }
        public int Y { get; set; }

        public Label(int x, int y, string text, Alignment alignment = Alignment.Left | Alignment.Top)
        {
            X = x;
            Y = y;
            Text = text;
            Alignment = alignment;
        }

        public override void Draw(SpriteBatch sb)
        {
            Bound = sb.DrawString(
                Font ?? Resources.DefaultFont,
                Text,
                Alignment,
                new Vector2(X, Y),
                ForeColor,
                0);
        }

        public override bool ProcessInput()
        {
            return false;
        }

        public override bool IsHovering(Vector2 pos)
        {
            return Bound.Contains(pos);
        }
    }
}
