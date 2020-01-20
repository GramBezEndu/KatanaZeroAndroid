using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Engine.Sprites
{
    public class AnimatedObject : ISprite
    {
        public Vector2 Scale { get; private set; } = Vector2.One;
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
        public bool Hidden { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size
        {
            get
            {
                return new Vector2(animatedSprite.TextureRegion.Width * Scale.X,
                    animatedSprite.TextureRegion.Height * Scale.Y);
            }
        }
        private readonly AnimatedSprite animatedSprite;
        private readonly SpriteSheetAnimationFactory animationFactory;
        private readonly TextureAtlas spriteAtlas;
        public Color Color { get; set; } = Color.White;
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(!Hidden)
                spriteBatch.Draw(animatedSprite.TextureRegion, Position, Color, 0f, new Vector2(0, 0), Scale, SpriteEffects, 0f);
        }

        public virtual void Update(GameTime gameTime)
        {
            if(!Hidden)
                animatedSprite.Update(gameTime);
        }

        public AnimatedObject(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale)
        {
            spriteAtlas = new TextureAtlas("animations", spritesheet, map);
            animationFactory = new SpriteSheetAnimationFactory(spriteAtlas);
            animatedSprite = new AnimatedSprite(animationFactory);
            Scale = scale;
        }

        /// <summary>
        /// Calls Play on member animatedSprite
        /// </summary>
        /// <param name="name"></param>
        /// <param name="onCompleted"></param>
        public void PlayAnimation(string name, Action onCompleted = null)
        {
            animatedSprite.Play(name, onCompleted);
        }

        public void AddAnimation(string name, SpriteSheetAnimationData data)
        {
            animationFactory.Add(name, data);
        }
    }
}
