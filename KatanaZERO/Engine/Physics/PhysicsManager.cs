using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engine.Sprites;
using Microsoft.Xna.Framework;

namespace Engine.Physics
{
    public class PhysicsManager : IComponent
    {
        private readonly CollisionManager collisionManager;
        private readonly List<ICollidable> moveableBodies;
        private readonly List<Rectangle> mapCollision;
        public float GRAVITY = 1f;

        public PhysicsManager()
        {
            collisionManager = new CollisionManager();
            moveableBodies = new List<ICollidable>();
            mapCollision = new List<Rectangle>();
        }

        public PhysicsManager(CollisionManager cm) : this()
        {
            collisionManager = cm;
        }

        public void AddMoveableBody(ICollidable c)
        {
            moveableBodies.Add(c);
        }

        public void AddStaticBody(Rectangle r)
        {
            mapCollision.Add(r);
        }

        public void DeleteBody(ICollidable c)
        {
            if (moveableBodies.Contains(c))
                moveableBodies.Remove(c);
            else
                throw new ArgumentException("Body not found");
        }

        public void DeleteStaticBlock(Rectangle r)
        {
            if (mapCollision.Contains(r))
                mapCollision.Remove(r);
            else
                throw new ArgumentException("Block not found");
        }

        public void SetMapCollision(List<Rectangle> rectangles)
        {
            mapCollision.Clear();
            rectangles.ForEach((item) => mapCollision.Add(item));
            collisionManager.SetMapCollision(mapCollision);
        }

        public void SetHidingSpots(List<Rectangle> hidingObstacles)
        {
            collisionManager.SetHidingSpots(hidingObstacles);
        }

        public void Update(GameTime gameTime)
        {
            collisionManager.SetCollisionBodies(moveableBodies);
            collisionManager.Update(gameTime);
            foreach (var m in moveableBodies)
            {
                UpdateBodyState(m);
                m.Update(gameTime);
                MoveBody(m);
                ApplyDownForce(m, GRAVITY);
            }
        }

        private void MoveBody(ICollidable c)
        {
            c.Position += c.Velocity;
            c.Velocity = new Vector2(0f, c.Velocity.Y);
            //if (c.Velocity.X > 0)
            //{
                //c.Velocity = new Vector2(c.Velocity.X - 1f, c.Velocity.Y);
                //if (c.Velocity.X < 0)
                //    c.Velocity = new Vector2(0, c.Velocity.Y);
            //}
            //else if (c.Velocity.X < 0)
            //{
            //    c.Velocity = new Vector2(c.Velocity.X + 1f, c.Velocity.Y);
            //    if (c.Velocity.X > 0)
            //        c.Velocity = new Vector2(0, c.Velocity.Y);
            //}
        }

        private void ApplyDownForce(ICollidable c, float downForce)
        {
            c.Velocity = new Vector2(c.Velocity.X, c.Velocity.Y + downForce);
        }

        private void UpdateBodyState(ICollidable c)
        {
            if (c.MoveableBodyState != MoveableBodyStates.Dead)
            {
                if (collisionManager.InAir(c))
                {
                    if (c.Velocity.X > 0f)
                    {
                        c.MoveableBodyState = MoveableBodyStates.InAirRight;
                    }
                    else if (c.Velocity.X < 0f)
                    {
                        c.MoveableBodyState = MoveableBodyStates.InAirLeft;
                    }
                    else
                    {
                        c.MoveableBodyState = MoveableBodyStates.InAir;
                    }
                }
                else if (c.Velocity.X > 0)
                {
                    c.MoveableBodyState = MoveableBodyStates.WalkRight;
                }
                else if (c.Velocity.X < 0)
                {
                    c.MoveableBodyState = MoveableBodyStates.WalkLeft;
                }
                else if (c is Player player)
                {
                    if (collisionManager.InDancingGroup(player))
                        c.MoveableBodyState = MoveableBodyStates.Dance;
                    else if (collisionManager.InHidingSpot(player))
                        c.MoveableBodyState = MoveableBodyStates.Hidden;
                    else
                        c.MoveableBodyState = MoveableBodyStates.Idle;
                }
                else
                {
                    c.MoveableBodyState = MoveableBodyStates.Idle;
                }
            }
        }

        public bool Spotted(Player p)
        {
            return collisionManager.Spotted(p);
        }
    }
}
