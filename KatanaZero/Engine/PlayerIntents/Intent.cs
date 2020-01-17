using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

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

        public void ResetIntent()
        {
            Finished = false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(!Hidden)
                DrawableRectangle.Draw(gameTime, spriteBatch);
        }

        public abstract void IntentFinished();

        public void Update(GameTime gameTime)
        {
            if(!Hidden)
            {
                DrawableRectangle.Update(gameTime);
                if (inputManager.WorldRectnagleWasJustClicked(DrawableRectangle.Rectangle, camera))
                {
                    //Player is not on the same level -> you can not start this intent
                    if (player.Rectangle.Top > DrawableRectangle.Rectangle.Bottom || player.Rectangle.Bottom < DrawableRectangle.Rectangle.Top)
                        return;
                    else
                        player.AddIntent(this);
                }
            }
        }

        public abstract void UpdateIntent(GameTime gameTime);
        public EventHandler OnFinished { get; set; }
    }
}
