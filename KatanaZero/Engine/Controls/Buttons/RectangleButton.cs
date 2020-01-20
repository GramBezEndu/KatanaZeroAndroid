using System;
using System.Collections.Generic;
using System.Text;
using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Controls.Buttons
{
    public class RectangleButton : DrawableRectangle, IButton
    {
        private Text message;
        private readonly InputManager inputManager;

        public RectangleButton(InputManager im, Rectangle rec, SpriteFont f, string msg) : base(rec)
        {
            inputManager = im;
            message = new Text(f, msg);
            message.Position = new Vector2(Position.X + this.Size.X / 2 - message.Size.X / 2,
                Position.Y + this.Size.Y / 2 - message.Size.Y / 2);
            OnClick += (o, e) => Engine.States.State.Sounds["OptionSelect"].Play();
        }

        public override Vector2 Position 
        { 
            get
            {
                return new Vector2(Rectangle.X, Rectangle.Y);
            }
            set
            {
                //set new rectangle
                Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);
                if (message != null)
                {
                    message.Position = new Vector2(Position.X + this.Size.X / 2 - message.Size.X / 2,
                        Position.Y + this.Size.Y / 2 - message.Size.Y / 2);
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            message?.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (inputManager.RectangleWasJustClicked(this.Rectangle))
                OnClick?.Invoke(this, new EventArgs());
            message?.Update(gameTime);
        }

        public EventHandler OnClick { get; set; }
    }
}
