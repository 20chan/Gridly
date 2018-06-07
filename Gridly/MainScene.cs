using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Gridly.UI;

namespace Gridly
{
    public sealed class MainScene : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random random;

        MainState state;
        float synapseInterval = 0.5f;
        float remainingDelay = 0f;
        int tickCount = 0;
        Vector2 curtMousePos, prevMousePos;
        MouseState curMouseState, prevMouseState;
        KeyboardState curKeyState, prevKeyState;
        bool isUIHandledInput;
        Matrix scale;

        TileMap tilemap;
        GUIManager guiManager;
        List<Part> parts;
        Part connectFrom;
        Part dragging;
        Vector2 disconnectFrom;
        Circuit editingCircuit;

        public MainScene()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();

            state = MainState.IDEAL;
            random = new Random();
            parts = new List<Part>();
            guiManager = new GUIManager();
            tilemap = new TileMap(parts, 16, 9);
            scale = Matrix.CreateScale(2);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Resources.DummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            Resources.DummyTexture.SetData(new[] { Color.White });
            Resources.PartTexture = Content.Load<Texture2D>("Img/Neuron");
            Resources.DefaultFont = Content.Load<SpriteFont>("defaultFont");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            remainingDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (remainingDelay <= 0)
            {
                TickSynapse();
                tilemap.TileParts();
                remainingDelay = synapseInterval;
                Console.WriteLine($"Laggy?: {gameTime.IsRunningSlowly}");
            }

            curMouseState = Mouse.GetState();
            var position = curMouseState.Position;
            var posVec = Vector2.Transform(position.ToVector2(), Matrix.Invert(scale));
            curtMousePos = posVec;
            curKeyState = Keyboard.GetState();

            isUIHandledInput = guiManager.HandleInput(curMouseState, curtMousePos);
            UpdateUIEvent();
            UpdatePartInput();
            tilemap.UpdatePhysics();
            if (!state.IsNeuralEditor())
                editingCircuit.UpdateInnerPhysics();

            if (IsKeyDown(Keys.Enter))
            {
                for (int i = 0; i < 100; i++)
                {
                    var x = random.Next(1920);
                    var y = random.Next(1080);
                    SpawnNeuron(new Vector2(x, y));
                }
            }
            
            prevMousePos = posVec;
            prevMouseState = curMouseState;
            prevKeyState = curKeyState;

            base.Update(gameTime);
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: scale);

            foreach (var n in parts)
                n.DrawSynapse(spriteBatch);
            foreach (var n in parts)
                n.DrawUpperSynapse(spriteBatch);
            foreach (var n in parts)
                n.Draw(spriteBatch);
            DrawPreviews();
            if (MainState.INNER_CIRCUIT_IDEAL <= state
                && state <= MainState.INNER_CIRCUIT_DISCONNECTING)
            {
                spriteBatch.FillRectangle(
                    new Rectangle(0, 0, 1920, 1080),
                    new Color(Color.Black, .5f));

                editingCircuit.DrawInner(spriteBatch);
                DrawCircuitPreview();
            }
            guiManager.DrawUI(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawPreviews()
        {
            if (state == MainState.NEURON_CONNECTING)
            {
                spriteBatch.DrawLine(connectFrom.Position, curtMousePos, 1f, Color.White);
            }
            else if (state == MainState.NEURON_DISCONNECTING)
            {
                spriteBatch.DrawLine(disconnectFrom, curtMousePos, 1f, Color.Red);
            }
        }

        private void DrawCircuitPreview()
        {
            if (state == MainState.INNER_CIRCUIT_CONNECTING)
                spriteBatch.DrawLine(connectFrom.Position, curtMousePos, 1f, Color.White);
        }

        private string Log()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var n in parts)
                n.Log(sb);
            return sb.ToString();
        }

        private void UpdateUIEvent()
        {

        }

        private void UpdatePartInput()
        {
            if (IsRightMouseDown())
            {
                if (state == MainState.IDEAL)
                {
                    if (!IsPartOnPos(curtMousePos, out var _))
                    {
                        if (IsKeyPressing(Keys.LeftShift))
                            SpawnCircuit(curtMousePos);
                        else
                            SpawnNeuron(curtMousePos);
                    }
                }
                else if (state == MainState.INNER_CIRCUIT_IDEAL)
                {
                    if (!editingCircuit.IsPartOnPos(curtMousePos, out var _))
                        editingCircuit.Add(new Neuron(curtMousePos));
                }
            }

            if (IsLeftMouseDown())
            {
                if (state.IsNeuralEditor())
                {
                    if (IsPartOnPos(curtMousePos, out var n))
                    {
                        if (state == MainState.IDEAL)
                        {
                            if (IsKeyPressing(Keys.LeftShift))
                            {
                                connectFrom = n;
                                state = MainState.NEURON_CONNECTING;
                            }
                            else
                            {
                                dragging = n;
                                state = MainState.NEURON_DRAGGING;
                            }
                        }
                        else if (state == MainState.NEURON_CONNECTING)
                        {
                            connectFrom.ConnectTo(n);
                            connectFrom = null;
                            state = MainState.IDEAL;
                        }
                    }
                    else if (IsKeyPressing(Keys.LeftControl))
                    {
                        disconnectFrom = curtMousePos;
                        state = MainState.NEURON_DISCONNECTING;
                    }
                }
                else
                {
                    if (editingCircuit.IsPartOnPos(curtMousePos, out var n))
                    {
                        if (state == MainState.INNER_CIRCUIT_IDEAL)
                        {
                            if (IsKeyPressing(Keys.LeftShift))
                            {
                                connectFrom = n;
                                state = MainState.INNER_CIRCUIT_CONNECTING;
                            }
                            else
                            {
                                dragging = n;
                                state = MainState.INNER_CIRCUIT_DRAGGING;
                            }
                        }
                        else if (state == MainState.INNER_CIRCUIT_CONNECTING)
                        {
                            connectFrom.ConnectTo(n);
                            connectFrom = null;
                            state = MainState.INNER_CIRCUIT_IDEAL;
                        }
                    }
                }
            }

            if (state == MainState.NEURON_DRAGGING || state == MainState.INNER_CIRCUIT_DRAGGING)
            {
                dragging.AddForceTo(curtMousePos, 0.01f);
            }

            if (state == MainState.NEURON_DISCONNECTING)
            {
                foreach (var n in parts)
                {
                    n.PreviewDisconnect(disconnectFrom, curtMousePos);
                }
            }
            
            if (IsLeftMouseUp())
            {
                if (state == MainState.NEURON_DRAGGING)
                {
                    dragging = null;
                    state = MainState.IDEAL;
                }
                else if (state == MainState.NEURON_DISCONNECTING)
                {
                    DisconnectIntersection(disconnectFrom, curtMousePos);
                    state = MainState.IDEAL;
                }
                else if (state == MainState.INNER_CIRCUIT_DRAGGING)
                {
                    dragging = null;
                    state = MainState.INNER_CIRCUIT_IDEAL;
                }
            }

            if (IsKeyDown(Keys.Space))
            {
                if (IsPartOnPos(curtMousePos, out var n))
                {
                    if (n is Circuit c)
                    {
                        editingCircuit = c;
                        state = MainState.INNER_CIRCUIT_IDEAL;
                    }
                    n.ActivateImmediate();
                }
            }

            if (IsKeyDown(Keys.Delete))
            {
                if (state == MainState.IDEAL)
                    if (IsPartOnPos(curtMousePos, out var n))
                        DeletePart(n);
            }

            if (IsKeyDown(Keys.Escape))
                if (state == MainState.INNER_CIRCUIT_IDEAL)
                    state = MainState.IDEAL;

            if (state == MainState.INNER_CIRCUIT_IDEAL)
            {
                if (IsKeyDown(Keys.I))
                    if (editingCircuit.IsPartOnPos(curtMousePos, out var n))
                        editingCircuit.SetInput(n as Neuron);
                if (IsKeyDown(Keys.O))
                    if (editingCircuit.IsPartOnPos(curtMousePos, out var n))
                        editingCircuit.SetOutput(n as Neuron);
            }
        }

        private void SpawnNeuron(Vector2 position)
        {
            parts.Add(new Neuron(position));
        }

        private void SpawnCircuit(Vector2 position)
        {
            parts.Add(new Circuit(position));
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

        private void TickSynapse()
        {
            foreach (var n in parts)
                n.UpdateSynapse();
            foreach (var n in parts)
                n.UpdateState();
            //Console.WriteLine($"After Tick {tickCount++}");
            //Console.Write(Log());
        }

        #region Input

        private bool IsKeyDown(Keys key)
            => curKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key);

        private bool IsKeyPressing(Keys key)
            => curKeyState.IsKeyDown(key);

        private bool IsKeyUp(Keys key)
            => curKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key);

        private bool IsLeftMouseDown()
            => !isUIHandledInput
            && curMouseState.LeftButton == ButtonState.Pressed
            && prevMouseState.LeftButton == ButtonState.Released;

        private bool IsLeftMousePressing()
            => curMouseState.LeftButton == ButtonState.Pressed;

        private bool IsLeftMouseUp()
            => curMouseState.LeftButton == ButtonState.Released
            && prevMouseState.LeftButton == ButtonState.Pressed;

        private bool IsRightMouseDown()
            => curMouseState.RightButton == ButtonState.Pressed
            && prevMouseState.RightButton == ButtonState.Released;

        private bool IsRightMousePressing()
            => curMouseState.RightButton == ButtonState.Pressed;

        private bool IsRightMouseUp()
            => curMouseState.RightButton == ButtonState.Released
            && prevMouseState.RightButton == ButtonState.Pressed;

        #endregion
    }
}
