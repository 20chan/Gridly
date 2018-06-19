using System;

namespace Gridly.UI
{
    public class LazyLabel : Label
    {
        public new string Text => TextDelegate();
        public Func<string> TextDelegate { get; set; }

        public LazyLabel(int x, int y, Func<string> text, Alignment alignment = Alignment.Left | Alignment.Top)
            : base(x, y, text(), alignment)
        {
            TextDelegate = text;
        }
    }
}
