﻿namespace Engine.Sprites
{
    using System.Collections.Generic;
    using Engine.SpecialEffects;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Class for creating objects based on single texture.
    /// </summary>
    public class Sprite : ISprite
    {
        public Sprite(Texture2D t, Vector2 objScale)
        {
            Texture = t;
            Scale = objScale;
        }

        public Sprite(Texture2D t)
        {
            Texture = t;
        }

        public List<SpecialEffect> SpecialEffects { get; set; } = new List<SpecialEffect>();

        public Vector2 Scale { get; set; } = Vector2.One;

        public bool Hidden { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size
        {
            get
            {
                return new Vector2(Texture.Width * Scale.X, Texture.Height * Scale.Y);
            }
        }

        public Color Color { get; set; } = Color.White;

        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        public Vector2 Origin { get; set; } = Vector2.Zero;

        public float Rotation { get; set; } = 0f;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            }
        }

        public Texture2D Texture { get; set; }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                spriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects, 0f);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                foreach (SpecialEffect effect in SpecialEffects)
                {
                    effect.Update(gameTime);
                }
            }
        }

        public void AddSpecialEffect(SpecialEffect effect)
        {
            SpecialEffects.Add(effect);
            effect.AddTarget(this);
        }
    }
}
