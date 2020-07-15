using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.SpecialEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Sprites
{
    /// <summary>
    /// Class for creating objects based on single texture
    /// </summary>
    public class Sprite : ISprite
    {
        private List<SpecialEffect> specialEffects = new List<SpecialEffect>();
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

        public Texture2D Texture;

        public Sprite(Texture2D t, Vector2 objScale)
        {
            Texture = t;
            Scale = objScale;
        }

        public Sprite(Texture2D t)
        {
            Texture = t;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(!Hidden)
                spriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects, 0f);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                foreach(var effect in specialEffects)
                {
                    effect.Update(gameTime);
                }
            }
        }

        public void AddSpecialEffect(SpecialEffect effect)
        {
            specialEffects.Add(effect);
            effect.AddTarget(this);
        }
    }
}
