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
        MouseState prevMouseState;

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

            var mouse = Mouse.GetState();

            if (mouse.RightButton == ButtonState.Pressed
                && prevMouseState.RightButton == ButtonState.Released)
            {
                if (!IsNeuronOnMouse(out var _))
                    neurons.Add(new Neuron(mouse.Position.ToVector2()));
            }

            if (mouse.LeftButton == ButtonState.Pressed
                && prevMouseState.LeftButton == ButtonState.Released)
            {
                if (IsNeuronOnMouse(out var n))
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

            prevMouseState = mouse;

            base.Update(gameTime);
        }

        private bool IsNeuronOnMouse(out Neuron neuron)
        {
            var mouse = Mouse.GetState();
            foreach (var n in neurons)
            {
                if (n.GetBounds().Contains(mouse.Position))
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

            spriteBatch.Begin();

            if (state == MainState.NEURON_CONNECTING)
            {
                GUI.DrawLine(spriteBatch, connectFrom.Position, Mouse.GetState().Position.ToVector2(), 1f, Color.Blue);
            }

            foreach (var n in neurons)
                n.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void TickSynapse()
        {

        }
    }
}
