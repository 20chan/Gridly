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
        Matrix camScale;

        float synapseInterval = 0.5f;
        float remainingDelay = 0f;
        MouseState prevMouseState;

        List<Neuron> neurons;

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

            camScale = Matrix.CreateScale(2);
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

            if (mouse.LeftButton == ButtonState.Pressed
                && prevMouseState.LeftButton == ButtonState.Released)
                neurons.Add(new Neuron(mouse.Position.ToVector2() / 2));

            prevMouseState = mouse;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: camScale);
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
