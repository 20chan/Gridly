using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly.UI
{
    public abstract class ButtonBase : Clickable
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

        public bool IsDown => IsMouseDown;
        public bool IsUp => IsMouseUp;
        public bool IsPressing => IsMousePressing;
        
        protected override bool HandleInputs()
        {
            return IsPressing;
        }
    }
}
