using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public class Button : Clickable, ISquareUI
    {
        private int _border = 0;
        public int Border
        {
            get => _border;
            set => _border = Math.Max(0, value);
        }
        public Color BorderColor { get; set; } = Color.Black;
        public Color BackColor { get; set; } = Color.White;
        public Color TextColor { get; set; } = Color.Black;
        public Alignment TextAlignment { get; set; } = 0;
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public ClickStyle ClickStyle { get; set; } = 0;

        public Color MouseOverBackColor;
        public Color MouseDownBackColor;

        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;

        public bool IsDown => IsMouseDown;
        public bool IsUp => IsMouseUp;
        public bool Pressing { get; private set; }

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
