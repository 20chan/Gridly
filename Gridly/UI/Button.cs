using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public class Button : Clickable, ISquareUI
    {
        public Color BackColor { get; set; }

        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;

        public bool IsDown => IsMouseDown;
        public bool Pressing { get; private set; }

        public Button(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        protected Rectangle GetBounds()
            => new Rectangle(X, Y, Width, Height);

        protected override bool IsHovering(Vector2 pos)
            => GetBounds().Contains(pos);

        protected override bool HandleInputs()
        {
            if (IsMouseDown)
                Pressing = true;
            if (IsMouseUp)
                Pressing = false;

            return Pressing;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.FillRectangle(GetBounds(), BackColor);
        }
    }
}
