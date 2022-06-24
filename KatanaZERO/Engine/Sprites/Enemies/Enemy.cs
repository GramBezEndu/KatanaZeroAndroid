using System;
using System.Collections.Generic;
using System.Text;
using Engine.Controls.Buttons;
using Engine.Input;
using Engine.MoveStrategies;
using Engine.Physics;
using Engine.PlayerIntents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Engine.Sprites
{
    public abstract class Enemy : AnimatedObject, ICollidable
    {
        public Strategy CurrentStrategy;
        protected readonly Player player;
        public virtual MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity { get; set; }
        public Sprite PatrollingSprite { get; set; }
        public Sprite QuestionMark { get; set; }

        public abstract Vector2 CollisionSize { get; }
        public Rectangle CollisionRectangle { get { return new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y); } }

        public EventHandler OnMapCollision { get; set; }

        public Enemy(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, Player p) : base(spritesheet, map, scale)
        {
            player = p;
        }

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
            if(PatrollingSprite != null)
            {
                PatrollingSprite.Update(gameTime);
                //Adjust beam position based on current animation (state)
                AdjustPatrollBeamPosition();
            }
            if(QuestionMark != null)
            {
                QuestionMark.Update(gameTime);
                AdjustQuestionMarkPosition();
            }
        }

        private void AdjustQuestionMarkPosition()
        {
            Vector2 adjustment;
            switch (MoveableBodyState)
            {
                case MoveableBodyStates.WalkRight:
                    adjustment = new Vector2(12, -13);
                    QuestionMark.Position = new Vector2(this.Position.X + adjustment.X, this.Position.Y + adjustment.Y);
                    break;
                case MoveableBodyStates.WalkLeft:
                    adjustment = new Vector2(7, -15);
                    QuestionMark.Position = new Vector2(this.Position.X + adjustment.X, this.Position.Y + adjustment.Y);
                    break;
                case MoveableBodyStates.InAir:
                case MoveableBodyStates.InAirRight:
                case MoveableBodyStates.InAirLeft:
                case MoveableBodyStates.Idle:
                    //Idle right
                    if (this.SpriteEffects == SpriteEffects.None)
                    {
                        adjustment = new Vector2(2, -11);
                        QuestionMark.Position = new Vector2(this.Position.X + adjustment.X, this.Position.Y + adjustment.Y);
                    }
                    else
                    {
                        adjustment = new Vector2(1, -12);
                        QuestionMark.Position = new Vector2(this.Position.X + adjustment.X, this.Position.Y + adjustment.Y);
                    }
                    break;
            }
        }

        private void AdjustPatrollBeamPosition()
        {
            Vector2 adjustment;
            switch (MoveableBodyState)
            {
                case MoveableBodyStates.WalkRight:
                    adjustment = new Vector2(-10, -13);
                    PatrollingSprite.SpriteEffects = SpriteEffects.None;
                    PatrollingSprite.Position = new Vector2(this.Position.X + this.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                    break;
                case MoveableBodyStates.WalkLeft:
                    adjustment = new Vector2(5, -15);
                    PatrollingSprite.SpriteEffects = SpriteEffects.FlipHorizontally;
                    PatrollingSprite.Position = new Vector2(this.Position.X - PatrollingSprite.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                    break;
                case MoveableBodyStates.InAir:
                case MoveableBodyStates.InAirRight:
                case MoveableBodyStates.InAirLeft:
                case MoveableBodyStates.Idle:
                    //Idle right
                    if (this.SpriteEffects == SpriteEffects.None)
                    {
                        adjustment = new Vector2(-35, -11);
                        PatrollingSprite.SpriteEffects = SpriteEffects.None;
                        PatrollingSprite.Position = new Vector2(this.Position.X + this.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                    }
                    else
                    {
                        adjustment = new Vector2(-2, -12);
                        PatrollingSprite.SpriteEffects = SpriteEffects.FlipHorizontally;
                        PatrollingSprite.Position = new Vector2(this.Position.X - PatrollingSprite.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                    }
                    break;
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
    }
}
