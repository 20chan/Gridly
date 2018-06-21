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
        public static readonly Rectangle Bound = new Rectangle(0, 0, Width, Height);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random random;

        bool paused;
        bool isUIHandledInput;
        Matrix scale;
        Scene curScene;
        GUIManager guiManager;

        static MainScene mainScene;
        
        public MainScene()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            mainScene = this;
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

            // LoadScene(new StageEditor(@"Stages\and_or.json"));
            LoadScene(new StageSelector());
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            Inputs.Update();
            if (!IsActive)
                paused = true;
            if (!paused)
            {
                Times.Update(gameTime);

                isUIHandledInput = guiManager.HandleInput(out var anyUIHovering);
                Inputs.UIHandledMouse = isUIHandledInput;
                Inputs.MouseHoverUI = anyUIHovering;

                curScene.Update();
            }
            if (Inputs.IsKeyDown(Keys.Tab))
                paused = !paused;

            Inputs.LateUpdate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(133, 182, 203));

            spriteBatch.Begin(transformMatrix: scale);
            
            curScene.Draw(spriteBatch);
            guiManager.DrawUI(spriteBatch);

            if (paused)
            {
                spriteBatch.FillRectangle(Bound, new Color(Color.Black, 0.7f));
                spriteBatch.DrawString(Resources.DefaultFont, "PAUSED", Alignment.Center, Bound, Color.White, 0);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public static void LoadScene(Scene scene)
        {
            mainScene.curScene?.OnUnload();
            scene.OnLoad();
            mainScene.curScene = scene;
        }
    }
}
