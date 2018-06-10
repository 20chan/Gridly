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

        bool paused;
        bool isUIHandledInput;
        Matrix scale;
        PartEditor baseEditor;
        GUIManager guiManager;
        
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

            random = new Random();
            guiManager = new GUIManager();
            scale = Matrix.CreateScale(2);
            baseEditor = new DefaultPartEditor();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Resources.DummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            Resources.DummyTexture.SetData(new[] { Color.White });
            Resources.PartTexture = Content.Load<Texture2D>("Img/Neuron");
            Resources.EdgeTexture = Content.Load<Texture2D>("Img/edge");
            Resources.DefaultFont = Content.Load<SpriteFont>("defaultFont");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            Inputs.Update();
            Times.Update(gameTime);

            isUIHandledInput = guiManager.HandleInput();
            UpdateUIEvent();
            if (!paused)
            {
                baseEditor.UpdateTick();
                baseEditor.Update();
            }
            if (Inputs.IsKeyDown(Keys.S))
                baseEditor.SkipTick();
            if (Inputs.IsKeyDown(Keys.Tab))
                paused = !paused;

            Inputs.LateUpdate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: scale);

            baseEditor.Draw(spriteBatch);
            guiManager.DrawUI(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void UpdateUIEvent()
        {

        }
    }
}
