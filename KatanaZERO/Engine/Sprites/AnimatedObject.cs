namespace Engine.Sprites
{
    using System;
    using System.Collections.Generic;
    using Engine.Physics;
    using Engine.SpecialEffects;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Extended.Animations.SpriteSheets;
    using MonoGame.Extended.Sprites;
    using MonoGame.Extended.TextureAtlases;

    public class AnimatedObject : ISprite
    {
        public List<SpecialEffect> SpecialEffects { get; set; } = new List<SpecialEffect>();

        public Vector2 Scale { get; private set; } = Vector2.One;

        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        public bool Hidden { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 DrawingPosition
        {
            get
            {
                if (this is ICollidable collidable)
                {
                    return new Vector2(Position.X - (Size.X - collidable.CollisionSize.X), Position.Y - (Size.Y - collidable.CollisionSize.Y));
                }
                else
                {
                    return Position;
                }
            }
        }

        public virtual Vector2 Size
        {
            get
            {
                return new Vector2(
                    animatedSprite.TextureRegion.Width * Scale.X,
                    animatedSprite.TextureRegion.Height * Scale.Y);
            }
        }

        private readonly MonoGame.Extended.Animations.AnimatedSprite animatedSprite;

        private readonly SpriteSheetAnimationFactory animationFactory;

        private readonly TextureAtlas spriteAtlas;

        public Color Color { get; set; } = Color.White;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)DrawingPosition.X, (int)DrawingPosition.Y, (int)Size.X, (int)Size.Y);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                spriteBatch.Draw(animatedSprite.TextureRegion, DrawingPosition, Color, 0f, new Vector2(0, 0), Scale, SpriteEffects, 0f);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                animatedSprite.Update(gameTime);
                foreach (SpecialEffect effect in SpecialEffects)
                {
                    effect.Update(gameTime);
                }
            }
        }

        public AnimatedObject(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale)
        {
            spriteAtlas = new TextureAtlas("animations", spritesheet, map);
            animationFactory = new SpriteSheetAnimationFactory(spriteAtlas);
            animatedSprite = new MonoGame.Extended.Animations.AnimatedSprite(animationFactory);
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

        public void AddAnimation(string name, MonoGame.Extended.Animations.SpriteSheets.SpriteSheetAnimationData data)
        {
            animationFactory.Add(name, data);
        }

        public void AddSpecialEffect(SpecialEffect effect)
        {
            SpecialEffects.Add(effect);
            effect.AddTarget(this);
        }
    }
}
