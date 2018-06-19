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
        private Label[][] inputValueLabels, outputValueLabels;

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
            inputValueLabels = new Label[tc.Inputs.Length][];
            outputValueLabels = new Label[tc.Outputs.Length][];

            var labHeight = 30;
            var labgap = 45;
            for (int i = 0; i < inputLabels.Length; i++)
            {
                inputLabels[i] = new Label(10 + X, 10 + Y + i * labHeight,
                    $"Input {i + 1}:", Alignment.Left);
                inputValueLabels[i] = new Label[tc.Inputs[i].Length];
                for (int j = 0; j < inputValueLabels[i].Length; j++)
                    inputValueLabels[i][j] = new Label(60 + X + j * labgap, 10 + Y + i * labHeight,
                        tc.Inputs[i][j].ToString(), Alignment.Center);
            }
            for (int i = 0; i < outputLabels.Length; i++)
            {
                int x = 10 + Width / 2;
                outputLabels[i] = new Label(x, 10 + Y + i * labHeight,
                    $"Output {i + 1}:", Alignment.Left);
                outputValueLabels[i] = new Label[tc.Outputs[i].Length];
                for (int j = 0; j < outputValueLabels[i].Length; j++)
                    outputValueLabels[i][j] = new Label(70 + x + j * labgap, 10 + Y + i * labHeight,
                        tc.Outputs[i][j].ToString(), Alignment.Center);
            }
        }

        public void UpdateUI()
        {
            foreach (var ls in inputValueLabels)
            {
                if (Tester.ElapsedTick < ls.Length)
                    ls[Tester.ElapsedTick].ForeColor = Color.Blue;
            }

            foreach (var ls in outputValueLabels)
            {
                if (Tester.CorcondanceCount == 0)
                {
                    foreach (var l in ls)
                        l.ForeColor = Color.Black;
                }
                else
                {
                    ls[Tester.CorcondanceCount - 1].ForeColor = Color.Blue;
                }
            }
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

                foreach (var ls in inputValueLabels)
                    foreach (var l in ls)
                        l.Draw(sb);
                foreach (var ls in outputValueLabels)
                    foreach (var l in ls)
                        l.Draw(sb);
            }
        }
    }
}
