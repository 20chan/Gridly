using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static Gridly.Inputs;
using System;
using Gridly.UI;

namespace Gridly
{
    public abstract class PartEditor : Scene
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

        bool connectFromSelects;
        Part connectFrom;
        Part dragFrom;
        Vector2 disconnectFrom;
        Circuit editingCircuit;
        Vector2 selectionFrom;
        List<Part> selectedParts;

        public PartEditor()
        {
            childEditors = new List<PartEditor>();
            parentEditor = null;
            Enabled = true;
            parts = new List<Part>();
            tilemap = new TileMap(parts, 16, 10);
            state = EditorState.IDEAL;
            selectedParts = new List<Part>();
        }

        public override void Update()
        {
            UpdateTick();
            UpdatePartInput();
            UpdateByStates();
            UpdateAbove();
            tilemap.UpdatePhysics();
        }

        public void UpdateTick()
        {
            remainingDelay -= (float)Times.TotalElapsedSeconds;

            if (IsKeyDown(Keys.S))
                SkipTick();

            if (remainingDelay <= 0)
            {
                SkipTick();
            }
        }

        public void SkipTick()
        {
            Tick();
            remainingDelay = synapseInterval;
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
                ActivateSelects(MousePos);

            if (state == EditorState.OTHER_EDITOR_ABOVE)
                return;

            if (!MouseHoverUI)
                if (IsRightMouseDown())
                    if (state == EditorState.IDEAL)
                        SpawnAt(MousePos, IsKeyPressing(Keys.LeftShift));

            if (!MouseHoverUI)
                if (IsLeftMouseDown())
                    HandleDragSelectConnect();

            if (IsLeftMouseUp())
                EndDragConnect();

            if (IsKeyDown(Keys.Delete))
                DeleteSelects(MousePos);

            if (IsKeyDown(Keys.Escape))
                CloseEditor();

            SaveLoadCircuit();
        }

        protected void UpdateByStates()
        {
            if (state == EditorState.NEURON_DISCONNECTING)
                PreviewDisconnects();

            if (state == EditorState.NEURON_DRAGGING)
                foreach (var p in selectedParts)
                    p.AddForce(MousePos - dragFrom.Position, 0.01f);

            if (state == EditorState.NEURON_SELECT_DRAGGING)
                SelectSelection();
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

        public override void Draw(SpriteBatch sb)
        {
            foreach (var n in parts)
                n.DrawBack(sb);
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
                if (connectFromSelects)
                    foreach (var p in selectedParts)
                        sb.DrawLine(p.Position, MousePos, 1f, Color.White);
                else
                    sb.DrawLine(connectFrom.Position, MousePos, 1f, Color.White);
            }
            else if (state == EditorState.NEURON_DISCONNECTING)
            {
                sb.DrawLine(disconnectFrom, MousePos, 1f, Color.Red);
            }
            else if (state == EditorState.NEURON_SELECT_DRAGGING)
            {
                sb.DrawRectangle(UIHelper.RectFromTwo(selectionFrom, MousePos), 2, Color.Green);
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
            parts.Add(new BasicNeuron(pos));
        }

        public void SpawnCircuit(Vector2 pos)
        {
            var c = new EditableCircuit(pos);
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
            if (n is EditableCircuit c)
                childEditors.Remove(c.Editor);
        }

        private void DisconnectIntersection(Vector2 from, Vector2 to)
        {
            foreach (var n in parts)
                n.DisconnectIntersection(from, to);
        }

        protected void ActivateSelects(Vector2 pos)
        {
            if (IsPartOnPos(pos, out var n))
            {
                if (n is EditableCircuit c)
                {
                    editingCircuit = c;
                    state = EditorState.OTHER_EDITOR_ABOVE;
                    aboveEditor = c.Editor;
                    aboveEditor.parentEditor = this;
                }
                else
                {
                    if (!selectedParts.Contains(n))
                        n.ActivateImmediate();
                    else
                        foreach (var s in selectedParts)
                            s.ActivateImmediate();
                }
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

        protected void DeleteSelects(Vector2 pos)
        {
            if (state == EditorState.IDEAL)
                if (IsPartOnPos(MousePos, out var n))
                {
                    if (!selectedParts.Contains(n))
                        DeletePart(n);
                    else
                    {
                        foreach (var p in selectedParts)
                            DeletePart(p);
                        UnselectAll();
                    }
                }
        }

        protected void Select(Part p)
        {
            selectedParts.Add(p);
            p.Select();
        }

        protected void Unselect(Part p)
        {
            selectedParts.Remove(p);
            p.Unselect();
        }

        protected void UnselectAll()
        {
            foreach (var p in parts)
                p.Unselect();
            selectedParts.Clear();
        }

        protected void HandleDragSelectConnect()
        {
            if (IsPartOnPos(MousePos, out var n))
            {
                if (state == EditorState.IDEAL)
                {
                    if (IsKeyPressing(Keys.LeftShift))
                    {
                        if (n.HaveOutput)
                        {
                            connectFromSelects = selectedParts.Contains(n);
                            connectFrom = n;
                            state = EditorState.NEURON_CONNECTING;
                        }
                    }
                    else
                    {
                        if (!selectedParts.Contains(n))
                        {
                            UnselectAll();
                            Select(n);
                        }
                        dragFrom = n;
                        state = EditorState.NEURON_DRAGGING;
                    }
                }
                else if (state == EditorState.NEURON_CONNECTING)
                {
                    if (connectFromSelects)
                        foreach (var p in selectedParts)
                            p.ConnectTo(n);
                    else
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
            else
            {
                if (state == EditorState.NEURON_CONNECTING)
                {
                    connectFrom = null;
                    state = EditorState.IDEAL;
                }
                else if (state == EditorState.IDEAL)
                {
                    UnselectAll();
                    selectionFrom = MousePos;
                    state = EditorState.NEURON_SELECT_DRAGGING;
                }
            }
        }

        protected void EndDragConnect()
        {
            if (state == EditorState.NEURON_DRAGGING)
            {
                state = EditorState.IDEAL;
            }
            else if (state == EditorState.NEURON_DISCONNECTING)
            {
                DisconnectIntersection(disconnectFrom, MousePos);
                state = EditorState.IDEAL;
            }
            else if (state == EditorState.NEURON_SELECT_DRAGGING)
            {
                SelectSelection();
                state = EditorState.IDEAL;
            }
        }

        private void SelectSelection()
        {
            UnselectAll();
            var bound = UIHelper.RectFromTwo(selectionFrom, MousePos);

            foreach (var p in parts)
                if (bound.Contains(p.Position))
                    Select(p);
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
            if (state == EditorState.IDEAL && parentEditor != null)
            {
                parentEditor.AboveClosed();
            }
            else
                MainScene.LoadScene(new StageSelector());
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
                    var loaded = JObject.Load(jr);
                    Circuit c = null;
                    if ((int)loaded["Type"] == 1)
                    {
                        c = new EditableCircuit(loaded);
                        c.Position = MousePos;
                        childEditors.Add((c as EditableCircuit).Editor);
                    }
                    else
                    {
                        var ch = loaded["Character"].ToObject<char>();
                        c = BuiltinCircuit.FromChar(MousePos, ch);
                    }
                    parts.Add(c);
                }
            }

            if (!IsPartOnPos(MousePos, out var _))
            {
                if (IsKeyDown(Keys.A))
                    parts.Add(BuiltinCircuit.AndCircuit(MousePos));
                if (IsKeyDown(Keys.N))
                    parts.Add(BuiltinCircuit.NotCircuit(MousePos));
                if (IsKeyDown(Keys.O))
                    parts.Add(BuiltinCircuit.OrCircuit(MousePos));
                if (IsKeyDown(Keys.R))
                    parts.Add(BuiltinCircuit.NorCircuit(MousePos));
                if (IsKeyDown(Keys.X))
                    parts.Add(BuiltinCircuit.XorCircuit(MousePos));
                if (IsKeyDown(Keys.B))
                    parts.Add(new Parts.Bulb(MousePos));
                if (IsKeyDown(Keys.T))
                    parts.Add(new Parts.ToggleSwitch(MousePos));
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
        
        public void DeserializeParts(JArray arr, out uint[] refIDs, out uint[] newIDs)
        {
            refIDs = new uint[arr.Count];
            newIDs = new uint[arr.Count];

            parts.Clear();
            int i = 0;
            foreach (JObject p in arr)
            {
                Part part = null;
                var type = (int)p["Type"];
                if (type == 2)
                {
                    var ch = p["Character"].ToObject<char>();
                    part = BuiltinCircuit.FromChar(Vector2.Zero, ch);
                }
                else if (type == 0)
                    part = new BasicNeuron();
                else if (type == 1)
                    part = new EditableCircuit();
                else
                    throw new Exception();

                var pos = p["Position"];
                part.Position = new Vector2((float)pos["x"], (float)pos["y"]);
                refIDs[i] = (uint)p["ID"];
                newIDs[i] = part.ID;
                parts.Add(part);
                i++;
            }
            for (i = 0; i < parts.Count; i++)
            {
                parts[i].Deserialize((JObject)arr[i], refIDs, newIDs, parts.ToArray());
                if (parts[i] is EditableCircuit c)
                    childEditors.Add(c.Editor);
            }
        }

        public abstract JObject Serialize();
        public abstract void Deserialize(JObject arr);
    }
}
