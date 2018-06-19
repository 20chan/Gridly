using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public class Button : ButtonBase, ISquareUI
    {
        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;

        public Button(int x, int y, int width, int height, string text = "")
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Text = text;
        }

        protected Rectangle GetBounds()
            => new Rectangle(X, Y, Width, Height);

        protected Rectangle GetBorderBounds()
            => ExpandRect(GetBounds(), Border);

        private Rectangle ExpandRect(Rectangle r, int value)
            => new Rectangle(r.X - value, r.Y - value, r.Width + value * 2, r.Height + value * 2);

        public override bool IsHovering(Vector2 pos)
            => GetBounds().Contains(pos);

        public override void Draw(SpriteBatch sb)
        {
            if (ClickStyle == ClickStyle.None)
                DrawNormal();
            else if (ClickStyle == ClickStyle.Popup)
            {
                if (IsMousePressing)
                    DrawExpanded(-3);
                else if (IsMouseHover)
                    DrawExpanded(5);
                else
                    DrawNormal();
            }
            else if (ClickStyle == ClickStyle.Flat)
            {
                Color back = BackColor;
                if (IsMousePressing)
                    back = MouseDownBackColor;
                else if (IsMouseHover)
                    back = MouseOverBackColor;

                sb.FillRectangle(GetBorderBounds(), BorderColor);
                sb.FillRectangle(GetBounds(), back);
                sb.DrawString(Font ?? Resources.DefaultFont, Text, TextAlignment, GetBounds(), TextColor, 0f);
            }

            void DrawNormal()
            {
                sb.FillRectangle(GetBorderBounds(), BorderColor);
                sb.FillRectangle(GetBounds(), BackColor);
                sb.DrawString(Font ?? Resources.DefaultFont, Text, TextAlignment, GetBounds(), TextColor, 0f);
            }
            void DrawExpanded(int val)
            {
                var border = ExpandRect(GetBorderBounds(), val);
                var rect = ExpandRect(GetBounds(), val);
                sb.FillRectangle(border, BorderColor);
                sb.FillRectangle(rect, BackColor); sb.DrawString(Font ?? Resources.DefaultFont, Text, TextAlignment, rect, TextColor, 0f);
            }
        }
    }
}
