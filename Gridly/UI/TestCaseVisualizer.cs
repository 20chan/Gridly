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

        private Tester _test;
        public Tester Tester
        {
            get => _test;
            set => AdjustTC(value);
        }

        private Label[] inputLabels, outputLabels;

        public TestCaseVisualizer(int x, int y, int width, int height, Tester tc = null)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Tester = tc;
        }

        public TestCaseVisualizer(Tester tc = null)
        {
            X = 0;
            Width = MainScene.Width;
            Height = 150;
            Y = MainScene.Height - Height;
            Tester = tc;
        }

        private void AdjustTC(Tester tc)
        {
            _test = tc;
            if (tc?.TestCase == null) return;
            inputLabels = new Label[tc.Inputs.Length];
            outputLabels = new Label[tc.Outputs.Length];
            var labHeight = 30;
            for (int i = 0; i < inputLabels.Length; i++)
                inputLabels[i] = new LazyLabel(10 + X, 10 + Y + i * labHeight, 
                    () => $"Input {i + 1}: {string.Join(",", tc.Inputs[i])}", Alignment.Left);

            for (int i = 0; i < outputLabels.Length; i++)
                outputLabels[i] = new LazyLabel(10 + Width / 2, 10 + Y + i * labHeight,
                    () => $"Output {i + 1}: {string.Join(",", tc.Outputs[i])}", Alignment.Left);
        }

        protected Rectangle GetBounds()
            => new Rectangle(X, Y, Width, Height);

        protected override bool HandleInputs()
        {
            return IsMousePressing;
        }

        public override bool IsHovering(Vector2 pos)
        {
            return GetBounds().Contains(pos);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.FillRectangle(GetBounds(), new Color(Color.Black, 0.3f));

            if (Tester != null)
            {
                foreach (var l in inputLabels)
                    l.Draw(sb);
                foreach (var l in outputLabels)
                    l.Draw(sb);
            }
        }
    }
}
