using Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Sprites.Crowd
{
    public abstract class CharacterCrowd : AnimatedObject, ICollidable
    {
        public CharacterCrowd(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale) : base(spritesheet, map, scale)
        {
        }

        public MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity { get; set; }

        public abstract Vector2 CollisionSize { get; }

        public Rectangle CollisionRectangle { get { return new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y); } }

        public EventHandler OnMapCollision { get; set; }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
            
        }

        public void PrepareMove(GameTime gameTime)
        {

        }
    }
}
