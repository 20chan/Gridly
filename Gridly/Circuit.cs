using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;

namespace Gridly
{
    public abstract class Circuit : Neuron, INeedInput
    {
        protected List<IConnectable> connectedInputs;

        public Circuit(Vector2 pos) : base(pos)
        {
            connectedInputs = new List<IConnectable>();
            BackColor = Color.Purple;
        }

        public virtual void ConnectFrom(IConnectable from)
        {
            connectedInputs.Add(from);
        }

        public virtual void DisconnectFrom(IConnectable from)
        {
            // if (connectedInputs.Contains(from))
            connectedInputs.Remove(from);
        }

        /// <summary>
        /// NOT USED
        /// </summary>
        public override void ActivateImmediate() { }

        public override void Log(StringBuilder sb)
        {
            throw new System.NotImplementedException();
        }
        
        public void ActivateOutput(int idx)
        {
            if (idx < connecting.Count)
                connecting[idx].Activate(this);
        }

        public override void DrawSynapse(SpriteBatch sb)
        {
            for (int i = 0; i < connecting.Count; i++)
            {
                var n = connecting[i];
                sb.DrawLine(Position, n.Position, 2f,
                    couldDisconnected[i]
                    ? Color.Red
                    : n is Circuit ? Color.Orange : Color.LightBlue, 0.5f);
            }
        }

        List<IConnectable> INeedInput.ConnectedInputs
            => connectedInputs;
    }
}
