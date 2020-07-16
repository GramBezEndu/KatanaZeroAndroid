using System;
using System.Collections.Generic;
using System.Text;
using Engine.SpecialEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Engine
{
    public class DrawableRectangle : IDrawableComponent
    {
        public bool Hidden { get; set; }
        public virtual Vector2 Position
        {
            get
            {
                return new Vector2(rectangle.X, rectangle.Y);
            }
            set
            {
                rectangle = new Rectangle((int)value.X, (int)value.Y, rectangle.Width, rectangle.Height);
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
            protected set
            {
                rectangle = value;
            }
        }

        public Color Color { get; set; } = Color.White;

        public int LineThickness { get; set; } = 1;

        public bool Filled { get; set; } = false;
        public List<SpecialEffect> SpecialEffects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DrawableRectangle(Rectangle rec)
        {
            rectangle = rec;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                if (Filled)
                    spriteBatch.FillRectangle(rectangle, Color, LineThickness);
                else
                    spriteBatch.DrawRectangle(rectangle, Color, LineThickness);
            }
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public void AddSpecialEffect(SpecialEffect effect)
        {
            throw new NotImplementedException();
        }
    }
}
