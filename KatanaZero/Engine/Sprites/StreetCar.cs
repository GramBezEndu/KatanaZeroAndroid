using System;
using System.Collections.Generic;
using System.Text;
using Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Engine.Sprites
{
    public class StreetCar : Sprite, ICollidable
    {
        public StreetCar(Texture2D t) : base(t)
        {
        }

        public EventHandler OnMapCollision { get; set; }
        public MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity { get; set; }
        float heightTreshold = 18f;

        public Vector2 CollisionSize => new Vector2(113, 45 - heightTreshold);

        public Rectangle CollisionRectangle => new Rectangle((int)Position.X, (int)(Position.Y + heightTreshold), (int)CollisionSize.X, (int)CollisionSize.Y);

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
            if (collider is Player player)
            {
                player.MoveableBodyState = MoveableBodyStates.Dead;
            }
        }

        public void PrepareMove(GameTime gameTime)
        {
            Velocity = new Vector2(-2f, Velocity.Y);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            //spriteBatch.DrawRectangle(CollisionRectangle, Color.Red, 2);
        }
    }
}
