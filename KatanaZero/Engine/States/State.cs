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
        protected SpriteFont font;
        protected RenderTarget2D uiLayerRenderTarget;
        protected List<IComponent> uiComponents = new List<IComponent>();
        protected SpriteBatch uiSpriteBatch;
        protected Dictionary<string, Texture2D> commonTextures = new Dictionary<string, Texture2D>();
        protected Dictionary<string, Song> songs = new Dictionary<string, Song>();
        public static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

        public void AddUiComponent(IComponent component)
        {
            if (component is IButton)
            {
                throw new Exception("UiComponents list can not directly contain buttons. Did you mean to create a NavigationMenu with list of buttons?");
            }
            else
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
            LoadFont();
            LoadCommonTextures();
            LoadSongs();
            if(sounds.Count == 0)
                LoadSoundEffects();
        }

        private void LoadFont()
        {
            font = content.Load<SpriteFont>("Font");
        }

        private void LoadCommonTextures()
        {
            //TODO: loop through directory and add to dictionary
            commonTextures.Add("MainMenu", content.Load<Texture2D>("Textures/MainMenu"));
            commonTextures.Add("Floor", content.Load<Texture2D>("Textures/Floor"));
            commonTextures.Add("PoliceCar", content.Load<Texture2D>("Textures/PoliceCar"));
            commonTextures.Add("GoArrow", content.Load<Texture2D>("Textures/GoArrow"));
            commonTextures.Add("GoText", content.Load<Texture2D>("Textures/GoText"));
            //DirectoryInfo directoryInfo = new DirectoryInfo(content.RootDirectory + "/Textures/");
            //if (!directoryInfo.Exists)
            //    throw new DirectoryNotFoundException();
            //FileInfo[] files = directoryInfo.GetFiles("*.*");
            //foreach (FileInfo file in files)
            //{
            //    string key = Path.GetFileNameWithoutExtension(file.Name);
            //    commonTextures[key] = content.Load<Texture2D>(Directory.GetCurrentDirectory() + "/Content/Textures/" + key);
            //}
        }

        private void LoadSongs()
        {
            //TODO: loop through directory and add to dictionary
            songs.Add("MainMenu", content.Load<Song>("Songs/MainMenu"));
            songs.Add("Stage1", content.Load<Song>("Songs/Stage1"));
        }

        private void LoadSoundEffects()
        {
            sounds.Add("WeaponSlash", content.Load<SoundEffect>("Sounds/WeaponSlash"));
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
