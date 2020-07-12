using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Sprites
{
    /// <summary>
    /// Class for creating objects based on single texture
    /// </summary>
    public class Sprite : ISprite
    {
        public Vector2 Scale { get; set; } = Vector2.One;
        public bool Hidden { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size
        {
            get
            {
                return new Vector2(texture.Width * Scale.X, texture.Height * Scale.Y);
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

        protected readonly Texture2D texture;

        public Sprite(Texture2D t, Vector2 objScale)
        {
            texture = t;
            Scale = objScale;
        }

        public Sprite(Texture2D t)
        {
            texture = t;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(!Hidden)
                spriteBatch.Draw(texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects, 0f);
        }

        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
