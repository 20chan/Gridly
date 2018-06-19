﻿using System.Linq;
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
        Button runBtn;
        
        public StageEditor(string path)
        {
            this.path = path;
            stage = Stage.Load(this, SerializeHelper.LoadFromFile(path));
            statusLabel = new Label(20, 20, "");
            runBtn = new Button(800, 20, 130, 60, "Run");
            MainScene.CurrentGUI.Add(runBtn);
            MainScene.CurrentGUI.Add(statusLabel);
            MainScene.CurrentGUI.Add(stage.Visualizer);
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

            if (IsKeyDown(Keys.F5) || runBtn.IsDown)
            {
                if (!started)
                {
                    started = true;
                    stage.Start();
                    statusLabel.Text = "Running..";
                    runBtn.Text = "Stop";
                }
                else
                {
                    started = false;
                    statusLabel.Text = "Stoped!";
                    runBtn.Text = "Run";
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
                stage.Log();
                if (stage.Succeed)
                {
                    started = false;
                    statusLabel.Text = "Succeed!!";
                    runBtn.Text = "Run";
                }
            }
        }
    }
}
