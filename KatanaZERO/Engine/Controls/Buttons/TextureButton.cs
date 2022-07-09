namespace Engine.Controls.Buttons
{
    using System;
    using Engine.Input;
    using Engine.Sprites;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class TextureButton : Sprite, IButton
    {
        private readonly InputManager inputManager;

        public TextureButton(InputManager im, Texture2D t, Vector2 objScale)
            : base(t, objScale)
        {
            inputManager = im;
            OnClick += (o, e) => Engine.States.GameState.Sounds["OptionSelect"].Play();
        }

        public EventHandler OnClick { get; set; }

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
    }
}
