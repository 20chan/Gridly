using System.Linq;
using Microsoft.Xna.Framework.Input;

using static Gridly.Inputs;

namespace Gridly
{
    public class StageEditor : DefaultPartEditor
    {
        Stage stage;
        string path;
        public StageEditor(string path)
        {
            this.path = path;
            stage = new Stage();
            stage.Deserialize(this, SerializeHelper.LoadFromFile(path));
        }

        protected override void UpdatePartInput()
        {
            base.UpdatePartInput();

            if (IsKeyDown(Keys.I))
                if (IsPartOnPos(MousePos, out var p))
                    if (p is BasicNeuron n)
                        stage.SetInputNeuron(n);
            if (IsKeyDown(Keys.O))
                if (IsPartOnPos(MousePos, out var p))
                    if (p is BasicNeuron n)
                        stage.SetOutputNeuron(n);

            if (IsKeyPressing(Keys.LeftControl) && IsKeyPressing(Keys.S))
                stage.Serialize(this).WriteToFile(path);
        }

        public Part FromID(uint id)
            => parts.First(p => p.ID == id);
    }
}
