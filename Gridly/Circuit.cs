using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;

namespace Gridly
{
    public abstract class Circuit : Part
    {
        protected List<IConnectable> connectedInputs;
        protected List<IConnectable> connectedOutputs;

        public Circuit(Vector2 pos) : base(pos)
        {
            connectedInputs = new List<IConnectable>();
            connectedOutputs = new List<IConnectable>();
            BackColor = Color.Purple;
        }

        public Circuit() : this(Vector2.Zero)
        {
            Initialized = false;
        }

        public override void ConnectTo(IConnectable n)
        {
            base.ConnectTo(n);

            connectedOutputs.Add(n);
        }

        public void ConnectFrom(IConnectable from)
        {
            connectedInputs.Add(from);
        }

        public void DisconnectFrom(IConnectable from)
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
            if (idx < connectedOutputs.Count)
                connectedOutputs[idx].Activate(this);
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
    }
}
