namespace Engine.States
{
    using System;
    using System.Collections.Generic;
    using Engine.Input;
    using KatanaZERO;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;

    public abstract class State : IComponent, IDisposable
    {
        public State(Game1 gameReference)
        {
            Game = gameReference;
            GraphicsDevice = Game.GraphicsDevice;
            UiSpriteBatch = new SpriteBatch(GraphicsDevice);
            InputManager = Game.InputManager;
            Content = Game.Content;
            CreateRenderTarget();
            LoadFonts();
            LoadCommonTextures();
            LoadSongs();
            if (Sounds.Count == 0)
            {
                LoadSoundEffects();
            }
        }

        public static Dictionary<string, SoundEffect> Sounds { get; private set; } = new Dictionary<string, SoundEffect>();

        protected Game1 Game { get; private set; }

        protected ContentManager Content { get; private set; }

        protected Dictionary<string, SpriteFont> Fonts { get; private set; } = new Dictionary<string, SpriteFont>();

        protected RenderTarget2D UiLayerRenderTarget { get; private set; }

        protected List<IComponent> UiComponents { get; private set; } = new List<IComponent>();

        protected SpriteBatch UiSpriteBatch { get; private set; }

        protected Dictionary<string, Texture2D> Textures { get; private set; } = new Dictionary<string, Texture2D>();

        protected InputManager InputManager { get; private set; }

        protected GraphicsDevice GraphicsDevice { get; private set; }

        protected Dictionary<string, Song> Songs { get; private set; } = new Dictionary<string, Song>();

        public void AddUiComponent(IComponent component)
        {
            UiComponents.Add(component);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (IComponent c in UiComponents)
            {
                c.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            DrawToRenderTarget(gameTime);
            DrawToScreen();
        }

        public void Dispose()
        {
            if (UiLayerRenderTarget != null && !UiLayerRenderTarget.IsDisposed)
            {
                UiLayerRenderTarget.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void DrawToScreen()
        {
            UiSpriteBatch.Begin();
            UiSpriteBatch.Draw(UiLayerRenderTarget, new Rectangle(0, 0, (int)Game.WindowSize.X, (int)Game.WindowSize.Y), Color.White);
            UiSpriteBatch.End();
        }

        protected virtual void DrawToRenderTarget(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(UiLayerRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            UiSpriteBatch.Begin();
            foreach (IComponent c in UiComponents)
            {
                if (c is IDrawableComponent drawableComponent)
                {
                    drawableComponent.Draw(gameTime, UiSpriteBatch);
                }
            }

            UiSpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        private void LoadFonts()
        {
            Fonts["Standard"] = Content.Load<SpriteFont>("Font");
            Fonts["Small"] = Content.Load<SpriteFont>("FontSmall");
            Fonts["Big"] = Content.Load<SpriteFont>("FontBig");
            Fonts["XirodMedium"] = Content.Load<SpriteFont>("XirodMedium");
        }

        private void LoadCommonTextures()
        {
            Textures.Add("MainMenu", Content.Load<Texture2D>("Textures/MainMenu"));
            Textures.Add("Fence", Content.Load<Texture2D>("Textures/Fence"));
            Textures.Add("Floor", Content.Load<Texture2D>("Textures/Floor"));
            Textures.Add("PoliceCar", Content.Load<Texture2D>("Textures/PoliceCar"));
            Textures.Add("GoArrow", Content.Load<Texture2D>("Textures/GoArrow"));
            Textures.Add("GoText", Content.Load<Texture2D>("Textures/GoText"));
            Textures.Add("HudTimer", Content.Load<Texture2D>("Textures/HudTimer"));
            Textures.Add("Timer", Content.Load<Texture2D>("Textures/Timer"));
            Textures.Add("Hud", Content.Load<Texture2D>("Textures/Hud"));
            Textures.Add("JobFolderBack", Content.Load<Texture2D>("Textures/JobFolderBack"));
            Textures.Add("JobFolderFrontOpen", Content.Load<Texture2D>("Textures/JobFolderFrontOpen"));
            Textures.Add("JobFolderFrontClosed", Content.Load<Texture2D>("Textures/JobFolderFrontClosed"));
        }

        private void LoadSongs()
        {
            Songs.Add("MainMenu", Content.Load<Song>("Songs/MainMenu"));
            Songs.Add("Stage1", Content.Load<Song>("Songs/Stage1"));
            Songs.Add("Club", Content.Load<Song>("Songs/Club"));
            Songs.Add("BikeEscape", Content.Load<Song>("Songs/BikeEscape"));
            Songs.Add("BikeEscapeBoss", Content.Load<Song>("Songs/BikeEscapeBoss"));
        }

        private void LoadSoundEffects()
        {
            Sounds.Add("WeaponSlash", Content.Load<SoundEffect>("Sounds/WeaponSlash"));
            Sounds.Add("OptionSelect", Content.Load<SoundEffect>("Sounds/OptionSelect"));
            Sounds.Add("LevelFail", Content.Load<SoundEffect>("Sounds/LevelFail"));
            Sounds.Add("BottleThrow", Content.Load<SoundEffect>("Sounds/BottleThrow"));
            Sounds.Add("GlassBreak", Content.Load<SoundEffect>("Sounds/GlassBreak"));
            Sounds.Add("PickUp", Content.Load<SoundEffect>("Sounds/KeycardPickUp"));
            Sounds.Add("Fire", Content.Load<SoundEffect>("Sounds/Fire"));
            Sounds.Add("StageClear", Content.Load<SoundEffect>("Sounds/StageClear"));
            Sounds.Add("SpeedingVehicle", Content.Load<SoundEffect>("Sounds/SpeedingVehicle"));
            Sounds.Add("SpeedingVehicle2", Content.Load<SoundEffect>("Sounds/SpeedingVehicle2"));
        }

        private void CreateRenderTarget()
        {
            UiLayerRenderTarget = new RenderTarget2D(GraphicsDevice, (int)Game.LogicalSize.X, (int)Game.LogicalSize.Y);
        }
    }
}
