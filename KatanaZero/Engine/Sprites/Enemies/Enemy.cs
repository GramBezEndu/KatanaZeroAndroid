using System;
using System.Collections.Generic;
using System.Text;
using Engine.Controls.Buttons;
using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Sprites
{
    public abstract class Enemy : AnimatedObject, IInteractable
    {
        public EventHandler OnInteract { get; set; }
        protected IButton interactionOption;
        protected InputManager inputManager;
        protected Texture2D rectangleTexture;
        protected GraphicsDevice graphicsDevice;
        public Enemy(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, InputManager im, GraphicsDevice gd, SpriteFont f) : base(spritesheet, map, scale)
        {
            inputManager = im;
            graphicsDevice = gd;
            interactionOption = new TextButton(im, f, "KILL")
            {
                Hidden = true,
            };
            OnInteract += (o, e) =>
            {
                interactionOption.Hidden = !(interactionOption.Hidden);
            };
        }

        public override void Update(GameTime gameTime)
        {
            if(!Hidden)
            {
                base.Update(gameTime);
                if (inputManager.RectangleWasJustClicked(this.Rectangle))
                {
                    OnInteract?.Invoke(this, new EventArgs());
                }
                //TODO: Do it only on size changed
                SetSpriteRectangle();
                interactionOption.Position = new Vector2(Position.X + Size.X, Position.Y);
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(!Hidden)
            {
                base.Draw(gameTime, spriteBatch);
                spriteBatch.Draw(rectangleTexture, Position, Color.White);
                interactionOption.Draw(gameTime, spriteBatch);
            }
        }
        private void SetSpriteRectangle()
        {
            var data = new List<Color>();
            int thickness = 4;

            for (int y = 0; y < Size.Y; y++)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    if (y < thickness || // On the top
                        x < thickness || // On the left
                        (y <= Size.Y - 1 && y >= Size.Y - thickness) || // on the bottom
                        (x <= Size.X - 1 && x >= Size.X - thickness)) // on the right
                    {
                        data.Add(Color.Red);
                    }
                    else
                    {
                        data.Add(Color.Transparent);
                    }
                }
            }

            rectangleTexture = new Texture2D(graphicsDevice, (int)Size.X, (int)Size.Y);
            rectangleTexture.SetData<Color>(data.ToArray());
        }
    }
}
