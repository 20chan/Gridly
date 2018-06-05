using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gridly
{
    public sealed class MainScene : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MainState state;
        float synapseInterval = 0.5f;
        float remainingDelay = 0f;
        Vector2 curtMousePos, prevMousePos;
        MouseState curMouseState, prevMouseState;
        KeyboardState curKeyState, prevKeyState;
        Matrix scale;

        List<Neuron> neurons;
        Neuron connectFrom;

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
            neurons = new List<Neuron>();
            scale = Matrix.CreateScale(2);
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
                remainingDelay = synapseInterval;
            }

            curMouseState = Mouse.GetState();
            var position = curMouseState.Position;
            var posVec = Vector2.Transform(position.ToVector2(), Matrix.Invert(scale));
            curtMousePos = posVec;
            curKeyState = Keyboard.GetState();

            UpdateNeuronInput();
            
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

            if (state == MainState.NEURON_CONNECTING)
            {
                GUI.DrawLine(spriteBatch, connectFrom.Position, curtMousePos, 1f, Color.Blue);
            }

            foreach (var n in neurons)
                n.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateNeuronInput()
        {
            if (curMouseState.RightButton == ButtonState.Pressed
                && prevMouseState.RightButton == ButtonState.Released)
            {
                if (!IsNeuronOnPos(curtMousePos, out var _))
                    neurons.Add(new Neuron(curtMousePos));
            }

            if (curMouseState.LeftButton == ButtonState.Pressed
                && prevMouseState.LeftButton == ButtonState.Released)
            {
                if (IsNeuronOnPos(curtMousePos, out var n))
                {
                    if (state == MainState.IDEAL)
                    {
                        connectFrom = n;
                        state = MainState.NEURON_CONNECTING;
                    }
                    else if (state == MainState.NEURON_CONNECTING)
                    {
                        connectFrom.ConnectTo(n);
                        connectFrom = null;
                        state = MainState.IDEAL;
                    }
                }
            }

            if (curKeyState.IsKeyDown(Keys.Space)
                && prevKeyState.IsKeyUp(Keys.Space))
            {
                if (IsNeuronOnPos(curtMousePos, out var n))
                {
                    n.ActivateImmediate();
                }
            }
        }

        private void TickSynapse()
        {
            foreach (var n in neurons)
                n.UpdateSynapse();
            foreach (var n in neurons)
                n.UpdateState();
        }
    }
}
