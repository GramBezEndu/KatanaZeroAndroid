namespace Engine.Sprites.Crowd
{
    using System;
    using System.Collections.Generic;
    using Engine.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class CharacterCrowd : AnimatedObject, ICollidable
    {
        public CharacterCrowd(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale)
            : base(spritesheet, map, scale)
        {
        }

        public event EventHandler OnMapCollision;

        public MovableBodyState MovableBodyState { get; set; }

        public Vector2 Velocity { get; set; }

        public abstract Vector2 CollisionSize { get; }

        public Rectangle CollisionRectangle => new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y);

        public void InvokeOnMapCollision(object sender, EventArgs args)
        {
            OnMapCollision?.Invoke(sender, args);
        }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
        }

        public void PrepareMove(GameTime gameTime)
        {
        }
    }
}
