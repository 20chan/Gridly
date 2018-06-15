using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gridly.UI;

namespace Gridly
{
    public static class UIHelper
    {
        public static Rectangle RectFromTwo(Vector2 p1, Vector2 p2)
        {
            int x1 = (int)Math.Min(p1.X, p2.X);
            int x2 = (int)Math.Max(p1.X, p2.X);
            int y1 = (int)Math.Min(p1.Y, p2.Y);
            int y2 = (int)Math.Max(p1.Y, p2.Y);
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static void DrawLine(this SpriteBatch sb, Vector2 p1, Vector2 p2, float border, Color color, float layerDepth = 0)
        {
            float angle = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            float length = Vector2.Distance(p1, p2);
            var scale = new Vector2(length, border);
            var _angle = angle + MathHelper.ToRadians(90);
            var _x = Math.Cos(_angle) * 1;
            var _y = Math.Sin(_angle) * 1;
            var _p1 = p1 - new Vector2((float)_x, (float)_y) * border * 0.5f;
            sb.Draw(Resources.DummyTexture, _p1, null, color, angle, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }

        public static void FillRectangle(this SpriteBatch sb, Rectangle rect, Color color)
            => sb.Draw(Resources.DummyTexture, rect, color);

        public static void DrawRectangle(this SpriteBatch sb, Rectangle rect, int border, Color color)
        {
            sb.Draw(Resources.DummyTexture, new Rectangle(rect.X, rect.Y, border, rect.Height + border), color);
            sb.Draw(Resources.DummyTexture, new Rectangle(rect.X, rect.Y, rect.Width + border, border), color);
            sb.Draw(Resources.DummyTexture, new Rectangle(rect.X + rect.Width, rect.Y, border, rect.Height + border), color);
            sb.Draw(Resources.DummyTexture, new Rectangle(rect.X, rect.Y + rect.Height, rect.Width + border, border), color);
        }

        public static void DrawString(this SpriteBatch sb, SpriteFont font, string text, Alignment align, Vector2 pos, Color color, float rotation)
            => DrawString(sb, font, text, align, new Rectangle(pos.ToPoint(), font.MeasureString(text).ToPoint()), color, rotation);

        public static void DrawString(this SpriteBatch sb, SpriteFont font, string text, Alignment align, Rectangle bound, Color color, float rotation)
        {
            Vector2 size = font.MeasureString(text);
            Point pos = bound.Center;
            Vector2 origin = size * 0.5f;

            if (align.HasFlag(Alignment.Left))
                origin.X += bound.Width / 2 - size.X / 2;

            if (align.HasFlag(Alignment.Right))
                origin.X -= bound.Width / 2 - size.X / 2;

            if (align.HasFlag(Alignment.Top))
                origin.Y += bound.Height / 2 - size.Y / 2;

            if (align.HasFlag(Alignment.Bottom))
                origin.Y -= bound.Height / 2 - size.Y / 2;

            sb.DrawString(font, text, new Vector2(pos.X, pos.Y), color, rotation, origin, 1, SpriteEffects.None, 0);
        }
    }
}
