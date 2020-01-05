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
        private Vector2 _velocity;
        protected readonly Player player;
        //public EventHandler OnInteract { get; set; }
        public virtual MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity
        {
            get => _velocity;
            set
            {
                if(value.Y < 0)
                {
                    int x = 5;
                }
                _velocity = value;
            }
        }

        //protected IButton interactionOption;
        //protected InputManager inputManager;
        //TODO: Make states like in player
        //public bool IsDead;
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
    }
}
