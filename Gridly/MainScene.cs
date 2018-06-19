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
        public static GUIManager CurrentGUI;
        public static readonly int Width = 1920 / 2;
        public static readonly int Height = 1080 / 2;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random random;

        bool paused;
        bool isUIHandledInput;
        Matrix scale;
        StageEditor stageEditor;
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
            CurrentGUI = guiManager;
            scale = Matrix.CreateScale(2);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Resources.DummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            Resources.DummyTexture.SetData(new[] { Color.White });
            Resources.BulbOffTexture = Content.Load<Texture2D>("Img/bulb_off");
            Resources.BulbOnTexture = Content.Load<Texture2D>("Img/bulb_on");
            Resources.PartTexture = Content.Load<Texture2D>("Img/Neuron");
            Resources.EdgeTexture = Content.Load<Texture2D>("Img/edge");
            Resources.DefaultFont = Content.Load<SpriteFont>("defaultFont");

            stageEditor = new StageEditor("stage.json");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            Inputs.Update();
            Times.Update(gameTime);

            isUIHandledInput = guiManager.HandleInput(out var anyUIHovering);
            Inputs.UIHandledMouse = isUIHandledInput;
            Inputs.MouseHoverUI = anyUIHovering;
            UpdateUIEvent();
            if (!paused)
            {
                stageEditor.UpdateTick();
                stageEditor.Update();
            }
            if (Inputs.IsKeyDown(Keys.S))
                stageEditor.SkipTick();
            if (Inputs.IsKeyDown(Keys.Tab))
                paused = !paused;

            Inputs.LateUpdate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: scale);

            stageEditor.Draw(spriteBatch);
            guiManager.DrawUI(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void UpdateUIEvent()
        {

        }
    }
}
