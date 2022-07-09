namespace Engine.Physics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public class PhysicsManager : IComponent
    {
        private readonly CollisionManager collisionManager;

        private readonly List<ICollidable> moveableBodies;

        private readonly List<Rectangle> mapCollision;

        public PhysicsManager()
        {
            collisionManager = new CollisionManager();
            moveableBodies = new List<ICollidable>();
            mapCollision = new List<Rectangle>();
        }

        public PhysicsManager(CollisionManager cm)
            : this()
        {
            collisionManager = cm;
        }

        public float Gravity { get; set; } = 1f;

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
            {
                moveableBodies.Remove(c);
            }
            else
            {
                throw new ArgumentException("Body not found");
            }
        }

        public void DeleteStaticBlock(Rectangle r)
        {
            if (mapCollision.Contains(r))
            {
                mapCollision.Remove(r);
            }
            else
            {
                throw new ArgumentException("Block not found");
            }
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
            foreach (ICollidable m in moveableBodies)
            {
                UpdateBodyState(m);
                m.Update(gameTime);
                MoveBody(m);
                ApplyDownForce(m, Gravity);
            }
        }

        public bool Spotted(Player p)
        {
            return collisionManager.Spotted(p);
        }

        private void MoveBody(ICollidable c)
        {
            c.Position += c.Velocity;
            c.Velocity = new Vector2(0f, c.Velocity.Y);
        }

        private void ApplyDownForce(ICollidable c, float downForce)
        {
            c.Velocity = new Vector2(c.Velocity.X, c.Velocity.Y + downForce);
        }

        private void UpdateBodyState(ICollidable c)
        {
            if (c.MovableBodyState != MovableBodyState.Dead)
            {
                if (collisionManager.InAir(c))
                {
                    if (c.Velocity.X > 0f)
                    {
                        c.MovableBodyState = MovableBodyState.InAirRight;
                    }
                    else if (c.Velocity.X < 0f)
                    {
                        c.MovableBodyState = MovableBodyState.InAirLeft;
                    }
                    else
                    {
                        c.MovableBodyState = MovableBodyState.InAir;
                    }
                }
                else if (c.Velocity.X > 0)
                {
                    c.MovableBodyState = MovableBodyState.WalkRight;
                }
                else if (c.Velocity.X < 0)
                {
                    c.MovableBodyState = MovableBodyState.WalkLeft;
                }
                else if (c is Player player)
                {
                    if (collisionManager.InDancingGroup(player))
                    {
                        c.MovableBodyState = MovableBodyState.Dance;
                    }
                    else if (collisionManager.InHidingSpot(player))
                    {
                        c.MovableBodyState = MovableBodyState.Hidden;
                    }
                    else
                    {
                        c.MovableBodyState = MovableBodyState.Idle;
                    }
                }
                else
                {
                    c.MovableBodyState = MovableBodyState.Idle;
                }
            }
        }
    }
}
