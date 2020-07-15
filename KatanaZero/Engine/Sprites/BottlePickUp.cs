using System;
using System.Collections.Generic;
using System.Text;
using Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Engine.Sprites
{
    public class BottlePickUp : Sprite, ICollidable
    {
        public BottlePickUp(Texture2D t) : base(t)
        {
        }

        public BottlePickUp(Texture2D t, Vector2 objScale) : base(t, objScale)
        {
        }

        public EventHandler OnMapCollision { get; set; }
        public MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity { get; set; }

        public Vector2 CollisionSize { get { return new Vector2(10, 21); } }

        public Rectangle CollisionRectangle { get { return new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y); } }

        public void PrepareMove(GameTime gameTime)
        {
            
        }
    }
}
