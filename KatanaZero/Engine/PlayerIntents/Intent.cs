using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.PlayerIntents
{
    public abstract class Intent : IPlayerIntent
    {
        public bool Hidden
        {
            get { return DrawableRectangle.Hidden; }
            set { DrawableRectangle.Hidden = value; }
        }
        public Vector2 Position
        {
            get { return DrawableRectangle.Position; }
            set { DrawableRectangle.Position = value; }
        }

        public Vector2 Size
        {
            get { return DrawableRectangle.Size; }
        }

        public Rectangle Rectangle
        {
            get { return DrawableRectangle.Rectangle; }
        }

        public Color Color
        {
            get { return DrawableRectangle.Color; }
            set { DrawableRectangle.Color = value; }
        }

        protected DrawableRectangle DrawableRectangle;
        protected Player player;
        public bool Finished { get; protected set; }
        InputManager inputManager;
        Camera camera;

        public Intent(InputManager im, Camera c, Player p, Rectangle r)
        {
            inputManager = im;
            camera = c;
            player = p;
            DrawableRectangle = new DrawableRectangle(r);
            DrawableRectangle.Color = Color.DarkBlue;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawableRectangle.Draw(gameTime, spriteBatch);
        }

        public abstract void IntentFinished();

        public void Update(GameTime gameTime)
        {
            DrawableRectangle.Update(gameTime);
            if (inputManager.WorldRectnagleWasJustClicked(DrawableRectangle.Rectangle, camera))
            {
                player.AddIntent(this);
                Debug.WriteLine("Intent was clicked!");
            }
        }

        public abstract void UpdateIntent(GameTime gameTime);
        public EventHandler OnFinished { get; set; }
    }
}
