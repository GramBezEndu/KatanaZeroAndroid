using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
//using System.Diagnostics;
using Engine.States;
using KatanaZero.States;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace KatanaZero
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        State currentState;
        State nextState;
        Song currentSong;
        public InputManager InputManager { get; private set; }
        public Vector2 LogicalSize { get; } = new Vector2(1280, 720);
        public Vector2 WindowSize
        {
            get
            {
                return new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            }
        }

        public Vector2 Scale
        {
            get
            {
                return WindowSize / LogicalSize;
            }
        }

        public void ChangeState(State state)
        {
            nextState = state;
        }

        public void PlaySong(Song s)
        {
            //If parameter is null we stop playing music
            if (s == null)
            {
                currentSong = null;
                MediaPlayer.Stop();
                return;
            }
            //Regular case: stop playing old song and start playing new song
            currentSong = s;
            MediaPlayer.Stop();
            MediaPlayer.Play(s);
        }

        public bool IsThisSongPlaying(Song song)
        {
            if (currentSong == song)
                return true;
            else
                return false;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            SoundEffect.MasterVolume = 1f;
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.IsRepeating = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            InputManager = new InputManager(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            currentState = new MainMenu(this);
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            TouchPanel.DisplayWidth = (int)WindowSize.X;
            TouchPanel.DisplayHeight = (int)WindowSize.Y;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                TouchPanel.EnableMouseTouchPoint = true;
            }
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            LevelsInfo.Init(this, Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.ChangeState(new MainMenu(this));
            //Handle changing states
            if (nextState != null)
            {
                //Remember to dispose any unmanaged resources
                if(currentState != null)
                    currentState.Dispose();
                currentState = nextState;
                nextState = null;
            }
            InputManager.Update(gameTime);
            //Update
            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            currentState.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
