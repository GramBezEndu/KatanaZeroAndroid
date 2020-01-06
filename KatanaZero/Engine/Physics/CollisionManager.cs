using Engine.Sprites.Crowd;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Engine.Physics
{
    public class CollisionManager : IComponent
    {
        List<ICollidable> collidableBodies;
        List<Rectangle> staticBodies;

        public void Update(GameTime gameTime)
        {
            foreach (var c in collidableBodies)
                c.PrepareMove(gameTime);
            foreach (var c in collidableBodies)
            {
                foreach (var s in staticBodies)
                {
                    if (IsTouchingRight(c, s))
                    {
                        float distanceX = c.Rectangle.Left - s.Right;
                        c.Velocity = new Vector2(-distanceX, c.Velocity.Y);
                    }
                    else if (IsTouchingLeft(c, s))
                    {
                        float distanceX = s.Left - c.Rectangle.Right;
                        c.Velocity = new Vector2(distanceX, c.Velocity.Y);
                    }
                    if (IsTouchingBottom(c, s))
                    {
                        float distanceY = c.Rectangle.Top - s.Bottom;
                        //c.Velocity = new Vector2(c.Velocity.X, -distanceY);

                        //Is in the block -> adjust position
                        c.Velocity = new Vector2(c.Velocity.X, 0f);
                        c.Position = new Vector2(c.Position.X, c.Position.Y - distanceY);
                    }
                    if (IsTouchingTop(c, s))
                    {
                        float distanceY = s.Top - c.Rectangle.Bottom;
                        //c.Velocity = new Vector2(c.Velocity.X, distanceY);

                        //Is in the block -> adjust position
                        c.Velocity = new Vector2(c.Velocity.X, 0f);
                        c.Position = new Vector2(c.Position.X, c.Position.Y + distanceY);
                    }
                }
            }
        }

        public void SetCollisionBodies(List<ICollidable> collidables)
        {
            collidableBodies = collidables;
        }

        public void SetStaticBodies(List<Rectangle> rectangles)
        {
            staticBodies = rectangles;
        }

        private bool IsTouchingLeft(ICollidable c, Rectangle r)
        {
            return c.Rectangle.Right + c.Velocity.X > r.Left &&
              c.Rectangle.Left < r.Left &&
              c.Rectangle.Bottom > r.Top &&
              c.Rectangle.Top < r.Bottom;
        }

        private bool IsTouchingRight(ICollidable c, Rectangle r)
        {
            return c.Rectangle.Left + c.Velocity.X < r.Right &&
              c.Rectangle.Right > r.Right &&
              c.Rectangle.Bottom > r.Top &&
              c.Rectangle.Top < r.Bottom;
        }

        private bool IsTouchingTop(ICollidable c, Rectangle r)
        {
            return c.Rectangle.Bottom + c.Velocity.Y > r.Top &&
              c.Rectangle.Top < r.Top &&
              c.Rectangle.Right > r.Left &&
              c.Rectangle.Left < r.Right;
        }

        private bool IsTouchingBottom(ICollidable c, Rectangle r)
        {
            return (c.Rectangle.Top + c.Velocity.Y < r.Bottom &&
              c.Rectangle.Bottom > r.Bottom &&
              c.Rectangle.Right > r.Left &&
              c.Rectangle.Left < r.Right);
        }

        public bool InAir(ICollidable c)
        {
            Rectangle collidableEnlarged = new Rectangle(
                (int)c.Position.X,
                (int)c.Position.Y,
                (int)c.Size.X,
                (int)c.Size.Y + 1);
            foreach (var s in staticBodies)
            {
                if (collidableEnlarged.Intersects(s))
                    return false;
            }
            return true;
        }

        public bool InDancingGroup(Player p)
        {
            foreach(var body in collidableBodies)
            {
                if (body == p)
                    continue;
                if (body is CharacterCrowd && p.Rectangle.Intersects(body.Rectangle))
                    return true;
            }
            return false;
        }
    }
}
