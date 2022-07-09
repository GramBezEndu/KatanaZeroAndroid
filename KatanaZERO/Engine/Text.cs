namespace Engine
{
    using System;
    using System.Collections.Generic;
    using Engine.SpecialEffects;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Text : IDrawableComponent
    {
        private string message;

        public Text(SpriteFont f, string msg, Vector2 scale)
        {
            Font = f;
            Message = msg;
            Scale = scale;
        }

        public Text(SpriteFont f, string msg)
        {
            Font = f;
            Message = msg;
        }

        public bool Hidden { get; set; }

        public Vector2 Position { get; set; }

        public SpriteFont Font { get; private set; }

        public string Message
        {
            get => message;
            set
            {
                message = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                if (Font != null && message != null)
                {
                    return new Vector2(Font.MeasureString(message).X * Scale.X, Font.MeasureString(message).Y * Scale.Y);
                }
                else
                {
                    return Vector2.Zero;
                }
            }
        }

        public Vector2 Scale { get; set; } = new Vector2(1f, 1f);

        public Color Color { get; set; } = Color.White;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            }
        }

        public List<SpecialEffect> SpecialEffects { get; set; } = new List<SpecialEffect>();

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                spriteBatch.DrawString(Font, Message, Position, Color, 0f, new Vector2(0, 0), Scale, SpriteEffects.None, 0f);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (SpecialEffect effect in SpecialEffects)
            {
                effect.Update(gameTime);
            }
        }

        public void AddSpecialEffect(SpecialEffect effect)
        {
            SpecialEffects.Add(effect);
            effect.AddTarget(this);
        }
    }
}
