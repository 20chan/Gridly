using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace Gridly.Parts
{
    public class Bulb : Part
    {
        private bool shouldActivate;
        private bool activated;
        private static float scale = 0.4f;

        public Bulb(Vector2 pos) : base(pos)
        {
            edgeScale = 1.5f;
            activated = false;
            HaveOutput = false;
        }

        public override void Activate(IConnectable from)
        {
            shouldActivate = true;
        }

        public override void ActivateImmediate()
        {
            activated = true;
        }

        public override Rectangle GetBounds()
        {
            var size = Resources.BulbOnTexture.Bounds.Size.ToVector2() * scale;
            var pos = Position - size / 2;

            return new Rectangle(pos.ToPoint(), size.ToPoint());
        }

        public override void UpdateSynapse()
        {
            activated = false;
        }

        public override void UpdateState()
        {
            if (shouldActivate)
                activated = true;
            shouldActivate = false;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(
                activated ? Resources.BulbOnTexture : Resources.BulbOffTexture,
                position: Position,
                origin: Resources.BulbOffTexture.Bounds.Size.ToVector2() / 2,
                scale: new Vector2(scale),
                color: Color.White,
                layerDepth: .5f);
        }

        public override void Log(StringBuilder sb)
        {
            throw new System.NotImplementedException();
        }

        public override JObject Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(JObject obj, uint[] orgIDs, uint[] newIDs, Part[] parts)
        {
            throw new System.NotImplementedException();
        }
    }
}
