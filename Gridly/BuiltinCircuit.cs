using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Gridly.UI;

namespace Gridly
{
    public class BuiltinCircuit : Circuit
    {
        private Func<IEnumerable<bool>, bool> function;
        private List<bool> inputStates;
        private char character;

        private BuiltinCircuit(Vector2 pos, char ch, Func<IEnumerable<bool>, bool> f) : base(pos)
        {
            function = f;
            character = ch;
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
            Console.WriteLine($"BUILTIN {{{character}}}");
            Console.WriteLine(string.Join(",", inputStates));
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

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.DrawString(Resources.DefaultFont, character.ToString(), Alignment.Center, GetBounds(), Color.White, 0);
        }

        public static BuiltinCircuit AndCircuit(Vector2 pos)
            => new BuiltinCircuit(pos, 'A', inputs => inputs.All(i => i));

        public static BuiltinCircuit NotCircuit(Vector2 pos)
            => new BuiltinCircuit(pos, 'N', inputs => inputs.Count() > 0 ? !inputs.First() : true);

        public static BuiltinCircuit OrCircuit(Vector2 pos)
            => new BuiltinCircuit(pos, 'O', inputs => inputs.Any(i => i));
    }
}
