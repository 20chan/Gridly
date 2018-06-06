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
        List<Neuron> neurons;
        Neuron connectFrom;
        Neuron dragging;
        Vector2 disconnectFrom;

        Button btnTest;

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
            neurons = new List<Neuron>();
            guiManager = new GUIManager();
            tilemap = new TileMap(neurons, 16, 9);
            scale = Matrix.CreateScale(2);

            btnTest = new Button(10, 10, 250, 100)
            {
                BackColor = Color.Gray
            };
            guiManager.Add(btnTest);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Resources.DummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            Resources.DummyTexture.SetData(new[] { Color.White });
            Resources.NeuronTexture = Content.Load<Texture2D>("Img/Neuron");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            remainingDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (remainingDelay <= 0)
            {
                TickSynapse();
                tilemap.TileNeurons();
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
            UpdateNeuronInput();
            tilemap.UpdatePhysics();

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
        
        private bool IsNeuronOnPos(Vector2 pos, out Neuron neuron)
        {
            foreach (var n in neurons)
            {
                if (n.GetBounds().Contains(pos))
                {
                    neuron = n;
                    return true;
                }
            }
            neuron = null;
            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: scale);

            guiManager.DrawUI(spriteBatch);
            DrawPreviews();
            foreach (var n in neurons)
                n.DrawSynapse(spriteBatch);
            foreach (var n in neurons)
                n.DrawUpperSynapse(spriteBatch);
            foreach (var n in neurons)
                n.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawPreviews()
        {
            if (state == MainState.NEURON_CONNECTING)
            {
                spriteBatch.DrawLine(connectFrom.Position, curtMousePos, 1f, Color.Blue);
            }
            else if (state == MainState.NEURON_DISCONNECTING)
            {
                spriteBatch.DrawLine(disconnectFrom, curtMousePos, 1f, Color.Red);
            }
        }

        private string Log()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var n in neurons)
                n.Log(sb);
            return sb.ToString();
        }

        private void UpdateUIEvent()
        {
            if (btnTest.IsDown)
            {
                btnTest.X += 10;
            }
        }

        private void UpdateNeuronInput()
        {
            if (IsRightMouseDown())
            {
                if (!IsNeuronOnPos(curtMousePos, out var _))
                    SpawnNeuron(curtMousePos);
            }

            if (IsLeftMouseDown())
            {
                if (IsNeuronOnPos(curtMousePos, out var n))
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

            if (state == MainState.NEURON_DRAGGING)
            {
                dragging.AddForceTo(curtMousePos, 0.01f);
            }

            if (state == MainState.NEURON_DISCONNECTING)
            {
                foreach (var n in neurons)
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
            }

            if (IsKeyDown(Keys.Space))
            {
                if (IsNeuronOnPos(curtMousePos, out var n))
                {
                    n.ActivateImmediate();
                }
            }

            if (IsKeyDown(Keys.Delete))
                if (state == MainState.IDEAL)
                    if (IsNeuronOnPos(curtMousePos, out var n))
                        DeleteNeuron(n);
        }

        private void SpawnNeuron(Vector2 position)
        {
            neurons.Add(new Neuron(position, (uint)neurons.Count));
        }

        private void DeleteNeuron(Neuron n)
        {
            foreach (var neu in neurons)
            {
                n.Disconnect(neu);
                neu.Disconnect(n);
            }
            neurons.Remove(n);
        }

        private void DisconnectIntersection(Vector2 from, Vector2 to)
        {
            foreach (var n in neurons)
                n.DisconnectIntersection(from, to);
        }

        private void TickSynapse()
        {
            foreach (var n in neurons)
                n.UpdateSynapse();
            foreach (var n in neurons)
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
