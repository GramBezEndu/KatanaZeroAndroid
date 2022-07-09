namespace Engine.Sprites
{
    using System;
    using System.Collections.Generic;
    using Engine.MoveStrategies;
    using Engine.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class Enemy : AnimatedObject, ICollidable
    {
        private readonly Player player;

        public Enemy(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, Player p)
            : base(spritesheet, map, scale)
        {
            player = p;
        }

        public event EventHandler OnMapCollision;

        public Strategy CurrentStrategy { get; set; }

        public virtual MovableBodyState MovableBodyState { get; set; }

        public Vector2 Velocity { get; set; }

        public Sprite PatrollingSprite { get; set; }

        public Sprite QuestionMark { get; set; }

        public abstract Vector2 CollisionSize { get; }

        public Rectangle CollisionRectangle => new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y);

        public void Die()
        {
            PlayAnimation("Die");
        }

        public virtual void PrepareMove(GameTime gameTime)
        {
            CurrentStrategy?.Update(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (PatrollingSprite != null)
            {
                PatrollingSprite.Update(gameTime);

                // Adjust beam position based on current animation (state)
                AdjustPatrollBeamPosition();
            }

            if (QuestionMark != null)
            {
                QuestionMark.Update(gameTime);
                AdjustQuestionMarkPosition();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            PatrollingSprite?.Draw(gameTime, spriteBatch);
            QuestionMark?.Draw(gameTime, spriteBatch);
        }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
        }

        public void InvokeOnMapCollision(object sender, EventArgs args)
        {
            OnMapCollision?.Invoke(sender, args);
        }

        private void AdjustQuestionMarkPosition()
        {
            Vector2 adjustment;
            switch (MovableBodyState)
            {
                case MovableBodyState.WalkRight:
                    adjustment = new Vector2(12, -13);
                    QuestionMark.Position = new Vector2(Position.X + adjustment.X, Position.Y + adjustment.Y);
                    break;
                case MovableBodyState.WalkLeft:
                    adjustment = new Vector2(7, -15);
                    QuestionMark.Position = new Vector2(Position.X + adjustment.X, Position.Y + adjustment.Y);
                    break;
                case MovableBodyState.InAir:
                case MovableBodyState.InAirRight:
                case MovableBodyState.InAirLeft:
                case MovableBodyState.Idle:
                    // Idle right
                    if (SpriteEffects == SpriteEffects.None)
                    {
                        adjustment = new Vector2(2, -11);
                        QuestionMark.Position = new Vector2(Position.X + adjustment.X, Position.Y + adjustment.Y);
                    }
                    else
                    {
                        adjustment = new Vector2(1, -12);
                        QuestionMark.Position = new Vector2(Position.X + adjustment.X, Position.Y + adjustment.Y);
                    }

                    break;
            }
        }

        private void AdjustPatrollBeamPosition()
        {
            Vector2 adjustment;
            switch (MovableBodyState)
            {
                case MovableBodyState.WalkRight:
                    adjustment = new Vector2(-10, -13);
                    PatrollingSprite.SpriteEffects = SpriteEffects.None;
                    PatrollingSprite.Position = new Vector2(Position.X + Size.X + adjustment.X, Position.Y + adjustment.Y);
                    break;
                case MovableBodyState.WalkLeft:
                    adjustment = new Vector2(5, -15);
                    PatrollingSprite.SpriteEffects = SpriteEffects.FlipHorizontally;
                    PatrollingSprite.Position = new Vector2(Position.X - PatrollingSprite.Size.X + adjustment.X, Position.Y + adjustment.Y);
                    break;
                case MovableBodyState.InAir:
                case MovableBodyState.InAirRight:
                case MovableBodyState.InAirLeft:
                case MovableBodyState.Idle:
                    // Idle right
                    if (SpriteEffects == SpriteEffects.None)
                    {
                        adjustment = new Vector2(-35, -11);
                        PatrollingSprite.SpriteEffects = SpriteEffects.None;
                        PatrollingSprite.Position = new Vector2(Position.X + Size.X + adjustment.X, Position.Y + adjustment.Y);
                    }
                    else
                    {
                        adjustment = new Vector2(-2, -12);
                        PatrollingSprite.SpriteEffects = SpriteEffects.FlipHorizontally;
                        PatrollingSprite.Position = new Vector2(Position.X - PatrollingSprite.Size.X + adjustment.X, Position.Y + adjustment.Y);
                    }

                    break;
            }
        }
    }
}
