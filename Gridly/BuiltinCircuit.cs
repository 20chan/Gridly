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
        private bool activated = false;

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
            if (activated)
                foreach (var n in connecting)
                    n.Activate(this);
            activated = false;
        }

        public override void UpdateState()
        {
            activated = function(inputStates);
            for (int i = 0; i < inputStates.Count; i++)
                inputStates[i] = false;
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

        public static BuiltinCircuit XorCircuit(Vector2 pos)
            => new BuiltinCircuit(pos, 'X', inputs => inputs.Count() == 2 ? inputs.First() ^ inputs.ElementAt(1) : false);

        public override void Deserialize(JObject obj, uint[] orgIDs, uint[] newIDs, Part[] parts)
        {
            var conns = obj["Connecting"].ToObject<uint[]>();
            foreach (var c in conns)
            {
                uint id = newIDs[Array.IndexOf(orgIDs, c)];
                var part = Match(id);
                ConnectTo(part);
            }

            Initialized = true;

            BasicNeuron Match(uint id)
                => (BasicNeuron)parts.First(p => p.ID == id);
        }

        public override JObject Serialize()
        {
            return new JObject
            {
                { "ID", ID },
                { "Type", 2 },
                { "Position", new JObject { { "x", Position.X }, { "y", Position.Y } } },
                { "Connecting", JArray.FromObject(connecting.Select(c => c.ID)) },
                { "Character", character },
            };
        }

        public static BuiltinCircuit FromChar(Vector2 pos, char c)
        {
            switch (c)
            {
                case 'A':
                    return AndCircuit(pos);
                case 'N':
                    return NotCircuit(pos);
                case 'O':
                    return OrCircuit(pos);
                case 'X':
                    return XorCircuit(pos);
                default:
                    throw new Exception();
            }
        }
    }
}
