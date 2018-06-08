using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace Gridly
{
    public class BuiltinCircuit : Circuit
    {
        private Func<IEnumerable<bool>, bool> function;
        private List<bool> inputStates;

        private BuiltinCircuit(Vector2 pos, Func<IEnumerable<bool>, bool> f) : base(pos)
        {
            function = f;
            inputStates = new List<bool>();
        }

        public override void Activate(IConnectable from)
        {
            var idx = connectedInputs.IndexOf(from);
            inputStates[idx] = true;
        }

        public override void ConnectFrom(IConnectable from)
        {
            base.ConnectFrom(from);
            inputStates.Add(false);
        }

        public override void DisconnectFrom(IConnectable from)
        {
            inputStates.RemoveAt(connectedInputs.IndexOf(from));
            base.DisconnectFrom(from);
        }

        public override void UpdateSynapse()
        {
            if (function(inputStates))
                foreach (var n in connectedOutputs)
                    n.Activate(this);
            for (int i = 0; i < inputStates.Count; i++)
                inputStates[i] = false;
        }

        public override void UpdateState()
        {
        }

        public override void Deserialize(JObject obj, uint[] orgIDs, uint[] newIDs, Part[] parts)
        {
            throw new System.NotImplementedException();
        }

        public override JObject Serialize()
        {
            throw new System.NotImplementedException();
        }

        public static BuiltinCircuit AndCircuit(Vector2 pos)
            => new BuiltinCircuit(pos, inputs => inputs.All(i => i));
    }
}
