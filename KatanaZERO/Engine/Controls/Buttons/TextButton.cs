namespace Engine.Controls.Buttons
{
    using System;
    using Engine.Input;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class TextButton : Text, IButton
    {
        protected InputManager inputManager;

        public TextButton(InputManager im, SpriteFont f, string msg, Vector2 scale) : base(f, msg, scale)
        {
            inputManager = im;
            OnClick += (o, e) => Engine.States.GameState.Sounds["OptionSelect"].Play();
        }

        public TextButton(InputManager im, SpriteFont f, string msg) : base(f, msg)
        {
            inputManager = im;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                spriteBatch.DrawString(font, Message, Position, Color.White, 0f, new Vector2(0, 0), Scale, SpriteEffects.None, 0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                if (inputManager.RectangleWasJustClicked(Rectangle))
                {
                    OnClick?.Invoke(this, new EventArgs());
                }
            }
        }

        public EventHandler OnClick { get; set; }
    }
}
