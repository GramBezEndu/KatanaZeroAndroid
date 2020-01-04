using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Engine
{
    public class DrawableRectangle : IDrawableComponent
    {
        public bool Hidden { get; set; }
        public Vector2 Position
        {
            get
            {
                return new Vector2(rectangle.X, rectangle.Y);
            }
            set
            {
                rectangle = new Rectangle((int)Position.X, (int)Position.X, rectangle.Width, rectangle.Height);
            }
        }

        public Vector2 Size
        {
            get
            {
                return Rectangle.Size.ToVector2();
            }
        }

        private Rectangle rectangle;

        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
        }

        public Color Color { get; set; } = Color.White;

        public float LineThickness { get; set; } = 1;

        public DrawableRectangle(Rectangle rec)
        {
            rectangle = rec;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                spriteBatch.DrawRectangle(rectangle, Color, LineThickness);
            }
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
