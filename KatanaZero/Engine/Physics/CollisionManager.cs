using Engine.Sprites;
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
        List<Rectangle> mapCollision;
        List<Sprite> hidingSpots;

        public void Update(GameTime gameTime)
        {
            foreach (var c in collidableBodies)
                c.PrepareMove(gameTime);
            foreach (var c in collidableBodies)
            {
                foreach (var s in mapCollision)
                {
                    CheckHorizontal(gameTime, c, s);
                    CheckVertical(gameTime, c, s);
                    CheckDiagonal(gameTime, c, s);
                }
            }
        }

        private void CheckDiagonal(GameTime gameTime, ICollidable c, Rectangle s)
        {
            if (c.Velocity.X == 0)
                return;
            Rectangle moved = new Rectangle(
                (int)(c.Position.X + c.Velocity.X),
                (int)(c.Position.Y + c.Velocity.Y),
                (int)c.CollisionSize.X,
                (int)c.CollisionSize.Y
                );
            if (ShareXCoordinate(c.CollisionRectangle, s) || ShareYCoordinate(c.CollisionRectangle, s))
                return;
            if (moved.Intersects(s))
            {
                float distanceX = GetHorizontalDistance(c.CollisionRectangle, s);
                float distanceY = GetVerticalDistance(c.CollisionRectangle, s);
                float velocityProportion = c.Velocity.X / c.Velocity.Y;

                if (distanceX == 0 && distanceY == 0)
                {
                    c.Velocity = new Vector2(0, c.Velocity.Y);
                    return;
                }
                if (distanceY == 0)
                {
                    c.Velocity = new Vector2(distanceX, distanceX / velocityProportion);
                    return;
                }
                float distanceProportion = distanceX / distanceY;
                if (Math.Abs(velocityProportion) < Math.Abs(distanceProportion))
                {
                    c.Velocity = new Vector2(distanceX, distanceX / velocityProportion);
                    return;
                }
                else
                {
                    c.Velocity = new Vector2(distanceY * velocityProportion, distanceY);
                    return;
                }
            }
        }

        private int GetVerticalDistance(Rectangle c, Rectangle r)
        {
            if (GetDistanceBeneath(c, r) >= 0)
                return GetDistanceBeneath(c, r);
            return -GetDistanceAbove(c, r);
        }

        private int GetHorizontalDistance(Rectangle c, Rectangle r)
        {
            if (GetDistanceToTheRight(c, r) >= 0)
                return GetDistanceToTheRight(c, r);
            return -GetDistanceToTheLeft(c, r);
        }

        private void CheckHorizontal(GameTime gameTime, ICollidable c, Rectangle s)
        {
            if (ShareYCoordinate(c.CollisionRectangle, s))
            {
                if (c.Velocity.X > 0)
                {
                    float distanceX = GetDistanceToTheRight(c.CollisionRectangle, s);
                    if (distanceX >= 0 && distanceX < c.Velocity.X)
                    {
                        c.Velocity = new Vector2(distanceX, c.Velocity.Y);
                    }
                }
                else if (c.Velocity.X < 0)
                {
                    float distanceX = GetDistanceToTheLeft(c.CollisionRectangle, s);
                    if (distanceX >= 0 && distanceX < -c.Velocity.X)
                    {
                        c.Velocity = new Vector2(-distanceX, c.Velocity.Y);
                    }
                }
            }
        }

        private void CheckVertical(GameTime gameTime, ICollidable c, Rectangle s)
        {
            if (ShareXCoordinate(c.CollisionRectangle, s))
            {
                if (c.Velocity.Y > 0)
                {
                    float distanceY = GetDistanceBeneath(c.CollisionRectangle, s);
                    if (distanceY >= 0 && distanceY < c.Velocity.Y)
                    {
                        c.Velocity = new Vector2(c.Velocity.X, distanceY);
                    }
                }
                else if (c.Velocity.Y < 0)
                {
                    float distanceY = GetDistanceAbove(c.CollisionRectangle, s);
                    if (distanceY >= 0 && distanceY < -c.Velocity.Y)
                    {
                        c.Velocity = new Vector2(c.Velocity.X, -distanceY);
                    }
                }
            }
        }

        private bool ShareXCoordinate(Rectangle c, Rectangle r)
        {
            return c.Left < r.Right && c.Right > r.Left;
        }

        private bool ShareXCoordinateClosed(Rectangle c, Rectangle r)
        {
            return c.Left <= r.Right && c.Right >= r.Left;
        }

        private bool ShareYCoordinate(Rectangle c, Rectangle r)
        {
            return c.Top < r.Bottom && c.Bottom > r.Top;
        }

        private int GetDistanceBeneath(Rectangle c, Rectangle r)
        {
            return r.Top - c.Bottom;
        }
        private int GetDistanceAbove(Rectangle c, Rectangle r)
        {
            return c.Top - r.Bottom;
        }
        private int GetDistanceToTheRight(Rectangle c, Rectangle r)
        {
            return r.Left - c.Right;
        }
        private int GetDistanceToTheLeft(Rectangle c, Rectangle r)
        {
            return c.Left - r.Right;
        }

        public void SetCollisionBodies(List<ICollidable> collidables)
        {
            collidableBodies = collidables;
        }

        public void SetMapCollision(List<Rectangle> rectangles)
        {
            mapCollision = rectangles;
        }

        public void SetHidingSpots(List<Sprite> hidingObstacles)
        {
            hidingSpots = hidingObstacles;
        }

        public bool InAir(ICollidable c)
        {
            foreach (var s in mapCollision)
            {
                if (ShareXCoordinate(c.CollisionRectangle, s))
                {
                    if (GetDistanceBeneath(c.CollisionRectangle, s) == 0)
                        return false;
                }
            }
            return true;
        }

        public bool InDancingGroup(Player p)
        {
            foreach(var body in collidableBodies)
            {
                if (body == p)
                    continue;
                if (body is CharacterCrowd && p.CollisionRectangle.Intersects(body.CollisionRectangle))
                    return true;
            }
            return false;
        }

        public bool InHidingSpot(Player p)
        {
            foreach (var hidingSpot in hidingSpots)
            {
                if (p.CollisionRectangle.Intersects(hidingSpot.Rectangle))
                    return true;
            }
            return false;
        }

        public bool Spotted(Player p)
        {
            foreach(var body in collidableBodies)
            {
                if (p == body || !(body is Enemy enemy))
                    continue;
                else if (enemy.PatrollingSprite.Rectangle.Intersects(p.CollisionRectangle))
                {
                    if(p.MoveableBodyState != MoveableBodyStates.Dance && p.MoveableBodyState != MoveableBodyStates.Hidden)
                    {
                        enemy.PatrollingSprite.Color = Color.Red * 0.7f;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
