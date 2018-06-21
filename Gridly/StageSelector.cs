using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gridly.UI;

namespace Gridly
{
    public class StageSelector : Scene
    {
        Button[] btns;
        Button editorBtn;
        public StageSelector() : base()
        {
            int init_x = 30, init_y = 40;
            int gap_y = 20, gap_x = 20;
            int width = 200, height = 80;
            var files = new DirectoryInfo("Stages").GetFiles("*.json");
            var iter = CoordIter();
            btns = new Button[files.Length];
            int i = 0;
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                iter.MoveNext();
                var coord = iter.Current;
                var b = new Button(coord.X, coord.Y, width, height, name)
                {
                    ClickStyle = ClickStyle.Popup
                };
                SceneGUI.Add(b);
                btns[i++] = b;
            }
            iter.MoveNext();
            var c = iter.Current;
            editorBtn = new Button(c.X, c.Y, width, height, "+")
            {
                ClickStyle = ClickStyle.Popup
            };
            SceneGUI.Add(editorBtn);

            IEnumerator<Point> CoordIter()
            {
                for (int y = init_y; ; y += height + gap_y)
                {
                    for (int x = 1; x <= 4; x++)
                        yield return new Point(init_x + (x-1) * width + x * gap_x, y);
                }
            }
        }

        public override void Update()
        {
            foreach (var b in btns)
            {
                if (b.IsDown)
                {
                    MainScene.LoadScene(new StageEditor($@"Stages\{b.Text}.json"));
                }
            }

            if (editorBtn.IsDown)
            {
                MainScene.LoadScene(new StageEditor());
            }
        }

        public override void Draw(SpriteBatch sb)
        {

        }
    }
}
