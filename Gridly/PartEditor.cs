using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static Gridly.Inputs;

namespace Gridly
{
    public abstract class PartEditor
    {
        public bool Enabled { get; set; }
        private List<Part> parts;
        private TileMap tilemap;
        EditorState state;

        Part connectFrom;
        Part dragging;
        Vector2 disconnectFrom;

        public PartEditor()
        {
            Enabled = true;
            parts = new List<Part>();
            tilemap = new TileMap(parts, 16, 10);
            state = EditorState.IDEAL;
        }

        public void Update()
        {
            UpdatePartInput();
            tilemap.UpdatePhysics();
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var n in parts)
                n.DrawSynapse(sb);
            foreach (var n in parts)
                n.DrawUpperSynapse(sb);
            foreach (var n in parts)
                n.Draw(sb);
            DrawPreviews(sb);
        }

        private void DrawPreviews(SpriteBatch sb)
        {
            if (state == EditorState.NEURON_CONNECTING)
            {
                sb.DrawLine(connectFrom.Position, MousePos, 1f, Color.White);
            }
            else if (state == EditorState.NEURON_DISCONNECTING)
            {
                sb.DrawLine(disconnectFrom, MousePos, 1f, Color.Red);
            }
        }

        public void SpawnNeuron(Vector2 pos)
        {
            parts.Add(new Neuron(pos));
        }

        public void SpawnCircuit(Vector2 pos)
        {
            parts.Add(new Circuit(pos));
        }

        private void DeletePart(Part n)
        {
            foreach (var neu in parts)
            {
                n.Disconnect(neu);
                neu.Disconnect(n);
            }
            parts.Remove(n);
        }

        private void DisconnectIntersection(Vector2 from, Vector2 to)
        {
            foreach (var n in parts)
                n.DisconnectIntersection(from, to);
        }

        public void TickSynapse()
        {
            foreach (var n in parts)
                n.UpdateSynapse();
            foreach (var n in parts)
                n.UpdateState();
        }

        private void UpdatePartInput()
        {
            if (IsRightMouseDown())
            {
                if (state == EditorState.IDEAL)
                {
                    if (!IsPartOnPos(MousePos, out var _))
                    {
                        if (IsKeyPressing(Keys.LeftShift))
                            SpawnCircuit(MousePos);
                        else
                            SpawnNeuron(MousePos);
                    }
                }
            }

            if (IsLeftMouseDown())
            {
                if (IsPartOnPos(MousePos, out var n))
                {
                    if (state == EditorState.IDEAL)
                    {
                        if (IsKeyPressing(Keys.LeftShift))
                        {
                            connectFrom = n;
                            state = EditorState.NEURON_CONNECTING;
                        }
                        else
                        {
                            dragging = n;
                            state = EditorState.NEURON_DRAGGING;
                        }
                    }
                    else if (state == EditorState.NEURON_CONNECTING)
                    {
                        connectFrom.ConnectTo(n);
                        connectFrom = null;
                        state = EditorState.IDEAL;
                    }
                }
                else if (IsKeyPressing(Keys.LeftControl))
                {
                    disconnectFrom = MousePos;
                    state = EditorState.NEURON_DISCONNECTING;
                }
            }

            if (state == EditorState.NEURON_DRAGGING)
            {
                dragging.AddForceTo(MousePos, 0.01f);
            }

            if (state == EditorState.NEURON_DISCONNECTING)
            {
                foreach (var n in parts)
                {
                    n.PreviewDisconnect(disconnectFrom, MousePos);
                }
            }

            if (IsLeftMouseUp())
            {
                if (state == EditorState.NEURON_DRAGGING)
                {
                    dragging = null;
                    state = EditorState.IDEAL;
                }
                else if (state == EditorState.NEURON_DISCONNECTING)
                {
                    DisconnectIntersection(disconnectFrom, MousePos);
                    state = EditorState.IDEAL;
                }
            }

            if (IsKeyDown(Keys.Space))
            {
                if (IsPartOnPos(MousePos, out var n))
                {
                    n.ActivateImmediate();
                }
            }

            if (IsKeyDown(Keys.Delete))
            {
                if (state == EditorState.IDEAL)
                    if (IsPartOnPos(MousePos, out var n))
                        DeletePart(n);
            }
        }

        private bool IsPartOnPos(Vector2 pos, out Part part)
        {
            foreach (var n in parts)
            {
                if (n.GetBounds().Contains(pos))
                {
                    part = n;
                    return true;
                }
            }
            part = null;
            return false;
        }
    }
}
