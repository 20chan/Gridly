using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static Gridly.Inputs;
using System;

namespace Gridly
{
    public abstract class PartEditor
    {
        public bool Enabled { get; set; }
        protected List<Part> parts;
        private TileMap tilemap;
        EditorState state;
        protected List<PartEditor> childEditors;
        PartEditor aboveEditor;
        PartEditor parentEditor;

        static float synapseInterval = 0.5f;
        static float remainingDelay = 0f;
        static int tickCount = 0;

        Part connectFrom;
        Part dragging;
        Vector2 disconnectFrom;
        Circuit editingCircuit;

        public PartEditor()
        {
            childEditors = new List<PartEditor>();
            parentEditor = null;
            Enabled = true;
            parts = new List<Part>();
            tilemap = new TileMap(parts, 16, 10);
            state = EditorState.IDEAL;
        }

        public void Update()
        {
            UpdatePartInput();
            UpdateByStates();
            UpdateAbove();
            tilemap.UpdatePhysics();
        }

        public void UpdateTick()
        {
            remainingDelay -= (float)Times.TotalElapsedSeconds;

            if (remainingDelay <= 0)
            {
                Tick();
                remainingDelay = synapseInterval;
            }
        }

        private void Tick()
        {
            TickSynapse();
            UpdateTiles();
            foreach (var c in childEditors)
                c.Tick();
        }

        protected virtual void UpdatePartInput()
        {
            if (IsKeyDown(Keys.Space))
                ActivateAt(MousePos);

            if (state == EditorState.OTHER_EDITOR_ABOVE)
                return;

            if (IsRightMouseDown())
                if (state == EditorState.IDEAL)
                    SpawnAt(MousePos, IsKeyPressing(Keys.LeftShift));

            if (IsLeftMouseDown())
                HandleDragConnect();

            if (IsLeftMouseUp())
                EndDragConnect();

            if (IsKeyDown(Keys.Delete))
                DeleteAt(MousePos);

            if (IsKeyDown(Keys.Escape))
                CloseEditor();

            SaveLoadCircuit();
        }

        protected void UpdateByStates()
        {
            if (state == EditorState.NEURON_DISCONNECTING)
                PreviewDisconnects();

            if (state == EditorState.NEURON_DRAGGING)
                dragging.AddForceTo(MousePos, 0.01f);
        }

        public void UpdateTiles()
        {
            tilemap.TileParts();
        }

        public void UpdateAbove()
        {
            if (state == EditorState.OTHER_EDITOR_ABOVE)
                aboveEditor.Update();
        }

        public virtual void TickSynapse()
        {
            foreach (var n in parts)
                n.UpdateSynapse();
            foreach (var n in parts)
                n.UpdateState();
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
            DrawAbove(sb);
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

        private void DrawAbove(SpriteBatch sb)
        {
            if (state == EditorState.OTHER_EDITOR_ABOVE)
            {
                sb.FillRectangle(new Rectangle(0, 0, 1920, 1080),
                    new Color(Color.Black, 0.5f));
                aboveEditor.Draw(sb);
            }
        }

        public void SpawnNeuron(Vector2 pos)
        {
            parts.Add(new Neuron(pos));
        }

        public void SpawnCircuit(Vector2 pos)
        {
            var c = new Circuit(pos);
            parts.Add(c);
            childEditors.Add(c.Editor);
        }

        private void DeletePart(Part n)
        {
            foreach (var neu in parts)
            {
                n.Disconnect(neu);
                neu.Disconnect(n);
            }
            parts.Remove(n);
            if (n is Circuit c)
                childEditors.Remove(c.Editor);
        }

        private void DisconnectIntersection(Vector2 from, Vector2 to)
        {
            foreach (var n in parts)
                n.DisconnectIntersection(from, to);
        }

        protected void ActivateAt(Vector2 pos)
        {
            if (IsPartOnPos(pos, out var n))
            {
                if (n is Circuit c)
                {
                    editingCircuit = c;
                    state = EditorState.OTHER_EDITOR_ABOVE;
                    aboveEditor = c.Editor;
                    aboveEditor.parentEditor = this;
                }
                else
                    n.ActivateImmediate();
            }
        }

        protected void SpawnAt(Vector2 pos, bool isCircuit)
        {
            if (!IsPartOnPos(pos, out var _))
            {
                if (isCircuit)
                    SpawnCircuit(MousePos);
                else
                    SpawnNeuron(MousePos);
            }
        }

        protected void DeleteAt(Vector2 pos)
        {
            if (state == EditorState.IDEAL)
                if (IsPartOnPos(MousePos, out var n))
                    DeletePart(n);
        }

        protected void HandleDragConnect()
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

        protected void EndDragConnect()
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

        protected void PreviewDisconnects()
        {
            foreach (var n in parts)
            {
                n.PreviewDisconnect(disconnectFrom, MousePos);
            }
        }

        protected void CloseEditor()
        {
            if (state == EditorState.IDEAL)
            {
                parentEditor?.AboveClosed();
            }
        }

        protected void AboveClosed()
        {
            editingCircuit = null;
            aboveEditor = null;
            state = EditorState.IDEAL;
        }

        private void SaveLoadCircuit()
        {
            if (IsKeyDown(Keys.S))
                if (IsPartOnPos(MousePos, out var p))
                    if (p is Circuit c)
                    {
                        using (var sw = new StreamWriter("circuit.json"))
                        using (var jw = new JsonTextWriter(sw))
                            c.Serialize().WriteTo(jw);
                    }

            if (IsKeyDown(Keys.L))
            {
                using (var sr = new StreamReader("circuit.json"))
                using (var jr = new JsonTextReader(sr))
                {
                    var c = new Circuit(JObject.Load(jr));
                    c.Position = MousePos;
                    parts.Add(c);
                    childEditors.Add(c.Editor);
                }
            }
        }

        protected bool IsPartOnPos(Vector2 pos, out Part part)
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

        protected abstract JObject Serialize();
        public abstract void Deserialize(JObject arr);
    }
}
