﻿using System;
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
        private Rectangle bound;

        public Label(string text, Alignment alignment = Alignment.Left | Alignment.Top)
        {
            Text = text;
            Alignment = alignment;
        }

        public override void Draw(SpriteBatch sb)
        {
            bound = sb.DrawString(
                Font ?? Resources.DefaultFont,
                Text,
                Alignment,
                new Vector2(),
                ForeColor,
                0);
        }

        public override bool ProcessInput()
        {
            return false;
        }

        public override bool IsHovering(Vector2 pos)
        {
            return bound.Contains(pos);
        }
    }
}
