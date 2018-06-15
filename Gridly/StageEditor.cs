using System.Linq;
using Microsoft.Xna.Framework.Input;
using Gridly.UI;

using static Gridly.Inputs;

namespace Gridly
{
    public class StageEditor : DefaultPartEditor
    {
        Stage stage;
        bool started = false;
        string path;
        Label statusLabel;
        
        public StageEditor(string path)
        {
            this.path = path;
            stage = new Stage();
            stage.Deserialize(this, SerializeHelper.LoadFromFile(path));
            statusLabel = new Label("");
            MainScene.CurrentGUI.Add(statusLabel);
        }

        protected override void UpdatePartInput()
        {
            if (!started)
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
            }

            if (IsKeyDown(Keys.F5))
            {
                if (!started)
                {
                    started = true;
                    stage.Start();
                    statusLabel.Text = "Running..";
                }
                else
                {
                    started = false;
                    statusLabel.Text = "Stoped!";
                }
            }

            if (IsKeyPressing(Keys.LeftControl) && IsKeyPressing(Keys.S))
                stage.Serialize(this).WriteToFile(path);
        }

        public Part FromID(uint id)
            => parts.First(p => p.ID == id);

        public override void TickSynapse()
        {
            base.TickSynapse();
            if (started)
            {
                stage.Tick();
                if (stage.Succeed)
                {
                    started = false;
                    statusLabel.Text = "Succeed!!";
                }
            }
        }
    }
}
