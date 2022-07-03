namespace Engine.Controls.Buttons
{
    using System;
    using Engine.Input;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class RectangleButton : DrawableRectangle, IButton
    {
        public Text Message { get; private set; }

        private readonly InputManager inputManager;

        public RectangleButton(InputManager im, Rectangle rec, SpriteFont f, string msg)
            : base(rec)
        {
            inputManager = im;
            Message = new Text(f, msg);
            Message.Position = new Vector2(
                Position.X + Size.X / 2 - Message.Size.X / 2,
                Position.Y + Size.Y / 2 - Message.Size.Y / 2);
            OnClick += (o, e) => Engine.States.GameState.Sounds["OptionSelect"].Play();
        }

        public override Vector2 Position
        {
            get
            {
                return new Vector2(Rectangle.X, Rectangle.Y);
            }

            set
            {
                // set new rectangle
                Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);
                if (Message != null)
                {
                    Message.Position = new Vector2(
                        Position.X + Size.X / 2 - Message.Size.X / 2,
                        Position.Y + Size.Y / 2 - Message.Size.Y / 2);
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                base.Draw(gameTime, spriteBatch);
                Message?.Draw(gameTime, spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                base.Update(gameTime);
                if (inputManager.RectangleWasJustClicked(Rectangle))
                {
                    OnClick?.Invoke(this, new EventArgs());
                }

                Message?.Update(gameTime);
            }
        }

        public EventHandler OnClick { get; set; }
    }
}
