using System;
using System.Collections.Generic;
using System.Text;
using Engine.Physics;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Engine.Sprites
{
    public class BottlePickUp : Sprite, ICollidable
    {
        private readonly GameState gameState;
        public PickUpArrow PickUpArrow;
        public BottlePickUp(Texture2D t) : base(t)
        {
        }

        public BottlePickUp(GameState state, Texture2D t, Vector2 objScale) : base(t, objScale)
        {
            gameState = state;
        }

        public EventHandler OnMapCollision { get; set; }
        public MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity { get; set; }

        public Vector2 CollisionSize { get { return new Vector2(10, 21); } }

        public Rectangle CollisionRectangle { get { return new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y); } }

        public void PrepareMove(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                PickUpArrow?.Draw(gameTime, spriteBatch);
                base.Draw(gameTime, spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                base.Update(gameTime);
                PickUpArrow?.Update(gameTime);
            }
        }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
            if (collider is Player player)
            {
                if (player.Hidden || player.HasBottle || this.Hidden)
                {
                    return;
                }
                else
                {
                    State.Sounds["PickUp"].Play();
                    gameState.PickUpBottle();
                    player.HasBottle = true;
                    this.Hidden = true;
                }
            }
        }
    }
}
