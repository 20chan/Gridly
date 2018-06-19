using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public class TestCaseVisualizer : Clickable
    {
        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
        public int Width { get; set; } = -1;
        public int Height { get; set; } = -1;

        public TestCase Test;

        public TestCaseVisualizer(int x, int y, int width, int height, TestCase tc = null)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Test = tc;
        }

        public TestCaseVisualizer(TestCase tc = null)
        {
            X = 0;
            Width = MainScene.Width;
            Height = 200;
            Y = MainScene.Height - Height;
            Test = tc;
        }

        protected Rectangle GetBounds()
            => new Rectangle(X, Y, Width, Height);

        public override void Draw(SpriteBatch sb)
        {
            sb.FillRectangle(GetBounds(), new Color(Color.SkyBlue, 0.3f));
        }

        protected override bool HandleInputs()
        {
            return IsMousePressing;
        }

        public override bool IsHovering(Vector2 pos)
        {
            return GetBounds().Contains(pos);
        }
    }
}
