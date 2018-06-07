using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gridly
{
    public class Circuit : Part
    {
        private List<IConnectable> connectedInputs;
        private List<IConnectable> connectedOutputs;
        private InnerCircuit inner;

        public Circuit(Vector2 pos) : base(pos)
        {
            connectedInputs = new List<IConnectable>();
            connectedOutputs = new List<IConnectable>();
            inner = new InnerCircuit();
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

        public override void Activate(IConnectable from)
        {
            var idx = connectedInputs.IndexOf(from);
            inner.ActivateInput(idx);
        }

        /// <summary>
        /// NOT USED
        /// </summary>
        public override void ActivateImmediate() { }

        public override void Log(StringBuilder sb)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            inner.Tick();
        }

        public override void UpdateSynapse()
        {
            for (int i = 0; i < connectedOutputs.Count; i++)
            {
                if (inner.IsOutputActivated(i))
                    // ActivateImmediate 가 더 맞을 거 같은데
                    connectedOutputs[i].Activate(this);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(
                   Resources.PartTexture,
                   position: Position,
                   origin: Origin,
                   color: Color.Purple,
                   layerDepth: .5f);
        }

        public override void DrawSynapse(SpriteBatch sb)
        {
            for (int i = 0; i < connecting.Count; i++)
            {
                var n = connecting[i];
                sb.DrawLine(Position, n.Position, 2f,
                    couldDisconnected[i]
                    ? Color.Yellow
                    : n is Circuit ? Color.Orange : Color.Blue, 0.5f);
            }
        }

        class InnerCircuit
        {
            private List<Part> parts;
            private List<Neuron> inputNeurons;
            private List<Neuron> outputNeurons;

            public InnerCircuit()
            {
                parts = new List<Part>();
                inputNeurons = new List<Neuron>();
                outputNeurons = new List<Neuron>();
            }

            public void Tick()
            {
                foreach (var n in parts)
                    n.UpdateSynapse();
                foreach (var n in parts)
                    n.UpdateState();
            }

            public void ActivateInput(int idx)
            {
                if (inputNeurons.Count > idx)
                    inputNeurons[idx].ActivateImmediate();
            }

            public bool IsOutputActivated(int idx)
            {
                if (outputNeurons.Count > idx)
                    return outputNeurons[idx].Activated;
                return false;
            }
        }
    }
}
