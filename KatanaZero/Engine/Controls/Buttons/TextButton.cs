using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Input;

namespace Engine.Controls.Buttons
{
    public class TextButton : Text, IButton
    {
        protected InputManager inputManager;

        public TextButton(InputManager im, SpriteFont f, string msg, Vector2 scale) : base(f, msg, scale)
        {
            inputManager = im;
            OnClick += (o, e) => Engine.States.State.Sounds["OptionSelect"].Play();
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
                if (inputManager.RectangleWasJustClicked(this.Rectangle))
                {
                    OnClick?.Invoke(this, new EventArgs());
                }
            }
        }

        public EventHandler OnClick { get; set; }
    }
}
