using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace Gridly.Parts
{
    public class ToggleSwitch : Part
    {
        bool activated = false;
        bool gotSignal = false;

        public ToggleSwitch(Vector2 pos) : base(pos)
        {

        }

        public override void Activate(IConnectable from)
        {
            gotSignal = true;
        }

        public override void ActivateImmediate()
        {
            activated = !activated;
        }

        public override void Deserialize(JObject obj, uint[] orgIDs, uint[] newIDs, Part[] parts)
        {
            throw new NotImplementedException();
        }

        public override Rectangle GetBounds()
        {
            var size = 10;
            return new Rectangle((int)Position.X - size, (int)Position.Y - size, size*2, size*2);
        }

        public override void Log(StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public override JObject Serialize()
        {
            throw new NotImplementedException();
        }
        
        public override void UpdateSynapse()
        {
            if (activated)
                foreach (var n in connecting)
                    n.Activate(this);
        }

        public override void UpdateState()
        {
            if (gotSignal)
                activated = !activated;
            gotSignal = false;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.FillRectangle(GetBounds(), activated ? Color.Blue : Color.White);
            sb.DrawRectangle(GetBounds(), 1, Color.Black);
        }
    }
}
