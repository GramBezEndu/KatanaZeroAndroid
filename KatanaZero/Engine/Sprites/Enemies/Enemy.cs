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

namespace Engine.Sprites
{
    public abstract class Enemy : AnimatedObject, ICollidable
    {
        public Strategy CurrentStrategy;
        protected readonly Player player;
        public virtual MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity { get; set; }
        public Sprite PatrollingSprite { get; set; }
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
                    adjustment = new Vector2(10, -13);
                    PatrollingSprite.SpriteEffects = SpriteEffects.FlipHorizontally;
                    PatrollingSprite.Position = new Vector2(this.Position.X - PatrollingSprite.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                    break;
                case MoveableBodyStates.Idle:
                    //Idle right
                    if (this.SpriteEffects == SpriteEffects.None)
                    {
                        adjustment = new Vector2(-15, 0);
                        PatrollingSprite.SpriteEffects = SpriteEffects.None;
                        PatrollingSprite.Position = new Vector2(this.Position.X + this.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                    }
                    else
                    {
                        adjustment = new Vector2(15, 0);
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
        }
    }
}
