using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Input;
using KatanaZero;
using Microsoft.Xna.Framework.Media;
using Engine.Controls.Buttons;
using Microsoft.Xna.Framework.Audio;

namespace Engine.States
{
    public abstract class State : IComponent, IDisposable
    {
        protected readonly Game1 game;
        protected readonly InputManager inputManager;
        protected readonly GraphicsDevice graphicsDevice;
        protected readonly ContentManager content;
        protected Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();
        protected RenderTarget2D uiLayerRenderTarget;
        protected List<IComponent> uiComponents = new List<IComponent>();
        protected SpriteBatch uiSpriteBatch;
        protected Dictionary<string, Texture2D> commonTextures = new Dictionary<string, Texture2D>();
        protected Dictionary<string, Song> songs = new Dictionary<string, Song>();
        public static Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();

        public void AddUiComponent(IComponent component)
        {
            uiComponents.Add(component);
        }

        public State(Game1 gameReference)
        {
            game = gameReference;
            graphicsDevice = game.GraphicsDevice;
            uiSpriteBatch = new SpriteBatch(graphicsDevice);
            inputManager = game.InputManager;
            content = game.Content;
            CreateRenderTarget();
            LoadFonts();
            LoadCommonTextures();
            LoadSongs();
            if(Sounds.Count == 0)
                LoadSoundEffects();
        }

        private void LoadFonts()
        {
            fonts["Standard"] = content.Load<SpriteFont>("Font");
            fonts["Small"] = content.Load<SpriteFont>("FontSmall");
            fonts["Big"] = content.Load<SpriteFont>("FontBig");
        }

        private void LoadCommonTextures()
        {
            commonTextures.Add("MainMenu", content.Load<Texture2D>("Textures/MainMenu"));
            commonTextures.Add("Fence", content.Load<Texture2D>("Textures/Fence"));
            commonTextures.Add("Floor", content.Load<Texture2D>("Textures/Floor"));
            commonTextures.Add("PoliceCar", content.Load<Texture2D>("Textures/PoliceCar"));
            commonTextures.Add("GoArrow", content.Load<Texture2D>("Textures/GoArrow"));
            commonTextures.Add("GoText", content.Load<Texture2D>("Textures/GoText"));
            commonTextures.Add("HudTimer", content.Load<Texture2D>("Textures/HudTimer"));
            commonTextures.Add("Timer", content.Load<Texture2D>("Textures/Timer"));
            commonTextures.Add("Hud", content.Load<Texture2D>("Textures/Hud"));
            commonTextures.Add("JobFolderBack", content.Load<Texture2D>("Textures/JobFolderBack"));
            commonTextures.Add("JobFolderFrontOpen", content.Load<Texture2D>("Textures/JobFolderFrontOpen"));
            commonTextures.Add("JobFolderFrontClosed", content.Load<Texture2D>("Textures/JobFolderFrontClosed"));
        }

        private void LoadSongs()
        {
            songs.Add("MainMenu", content.Load<Song>("Songs/MainMenu"));
            songs.Add("Stage1", content.Load<Song>("Songs/Stage1"));
            songs.Add("Club", content.Load<Song>("Songs/Club"));
        }

        private void LoadSoundEffects()
        {
            Sounds.Add("WeaponSlash", content.Load<SoundEffect>("Sounds/WeaponSlash"));
            Sounds.Add("OptionSelect", content.Load<SoundEffect>("Sounds/OptionSelect"));
            Sounds.Add("LevelFail", content.Load<SoundEffect>("Sounds/LevelFail"));
            Sounds.Add("BottleThrow", content.Load<SoundEffect>("Sounds/BottleThrow"));
            Sounds.Add("GlassBreak", content.Load<SoundEffect>("Sounds/GlassBreak"));
        }

        private void CreateRenderTarget()
        {
            uiLayerRenderTarget = new RenderTarget2D(graphicsDevice, (int)game.LogicalSize.X, (int)game.LogicalSize.Y);
        }

        public void Draw(GameTime gameTime)
        {
            DrawToRenderTarget(gameTime);
            DrawToScreen();
        }

        protected virtual void DrawToScreen()
        {
            uiSpriteBatch.Begin();
            uiSpriteBatch.Draw(uiLayerRenderTarget, new Rectangle(0, 0, (int)game.WindowSize.X, (int)game.WindowSize.Y), Color.White);
            uiSpriteBatch.End();
        }

        protected virtual void DrawToRenderTarget(GameTime gameTime)
        {
            graphicsDevice.SetRenderTarget(uiLayerRenderTarget);
            graphicsDevice.Clear(Color.Transparent);
            uiSpriteBatch.Begin();
            foreach (var c in uiComponents)
            {
                if (c is IDrawableComponent)
                    (c as IDrawableComponent).Draw(gameTime, uiSpriteBatch);
            }
            uiSpriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var c in uiComponents)
                c.Update(gameTime);
        }

        public void Dispose()
        {
            if(uiLayerRenderTarget != null && !(uiLayerRenderTarget.IsDisposed))
            {
                uiLayerRenderTarget.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}
