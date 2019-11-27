using Engine.Input;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Controls.Buttons
{
    public class TextureButton : Sprite, IButton
    {
        private InputManager inputManager;
        public TextureButton(InputManager im, Texture2D t, Vector2 objScale) : base(t, objScale)
        {
            inputManager = im;
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
