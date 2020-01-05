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
        //public EventHandler OnInteract { get; set; }
        public virtual MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity { get; set; }

        //protected IButton interactionOption;
        //protected InputManager inputManager;
        //TODO: Make states like in player
        //public bool IsDead;
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
                if(MoveableBodyState == MoveableBodyStates.WalkRight)
                {
                    Vector2 adjustment = new Vector2(-10, -13);
                    PatrollingSprite.SpriteEffects = SpriteEffects.None;
                    PatrollingSprite.Position = new Vector2(this.Position.X + this.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                }
                else if(MoveableBodyState == MoveableBodyStates.WalkLeft)
                {
                    Vector2 adjustment = new Vector2(10, -13);
                    PatrollingSprite.SpriteEffects = SpriteEffects.FlipHorizontally;
                    PatrollingSprite.Position = new Vector2(this.Position.X - PatrollingSprite.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                }
                //Idle right
                else if(MoveableBodyState == MoveableBodyStates.Idle && this.SpriteEffects == SpriteEffects.None)
                {
                    Vector2 adjustment = new Vector2(-15, 0);
                    PatrollingSprite.SpriteEffects = SpriteEffects.None;
                    PatrollingSprite.Position = new Vector2(this.Position.X + this.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                }
                //Idle left
                else if(MoveableBodyState == MoveableBodyStates.Idle && this.SpriteEffects == SpriteEffects.FlipHorizontally)
                {
                    Vector2 adjustment = new Vector2(15, 0);
                    PatrollingSprite.SpriteEffects = SpriteEffects.FlipHorizontally;
                    PatrollingSprite.Position = new Vector2(this.Position.X - PatrollingSprite.Size.X + adjustment.X, this.Position.Y + adjustment.Y);
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            PatrollingSprite?.Draw(gameTime, spriteBatch);
        }
    }
}
