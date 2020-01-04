using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Engine.Physics
{
    public class PhysicsManager : IComponent
    {
        private readonly CollisionManager collisionManager;
        private readonly List<ICollidable> moveableBodies;
        private readonly List<Rectangle> staticBodies;
        const float GRAVITY = 1f;

        public PhysicsManager()
        {
            collisionManager = new CollisionManager();
            moveableBodies = new List<ICollidable>();
            staticBodies = new List<Rectangle>();
        }

        public void AddMoveableBody(ICollidable c)
        {
            moveableBodies.Add(c);
        }

        public void AddStaticBody(Rectangle r)
        {
            staticBodies.Add(r);
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
            if (staticBodies.Contains(r))
                staticBodies.Remove(r);
            else
                throw new ArgumentException("Block not found");
        }

        public void SetStaticBodies(List<Rectangle> rectangles)
        {
            staticBodies.Clear();
            rectangles.ForEach((item) => staticBodies.Add(item));
            collisionManager.SetStaticBodies(staticBodies);
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
            if (c.Velocity.X > 0)
            {
                c.Velocity = new Vector2(c.Velocity.X - 1f, c.Velocity.Y);
                if (c.Velocity.X < 0)
                    c.Velocity = new Vector2(0, c.Velocity.Y);
            }
            else if (c.Velocity.X < 0)
            {
                c.Velocity = new Vector2(c.Velocity.X + 1f, c.Velocity.Y);
                if (c.Velocity.X > 0)
                    c.Velocity = new Vector2(0, c.Velocity.Y);
            }
        }

        private void ApplyDownForce(ICollidable c, float downForce)
        {
            c.Velocity = new Vector2(c.Velocity.X, c.Velocity.Y + downForce);
        }

        private void UpdateBodyState(ICollidable c)
        {
            if (c.Velocity.X > 0 && c.Velocity.Y != 0)
            {
                c.MoveableBodyState = MoveableBodyStates.InAirRight;
            }
            else if (c.Velocity.X < 0 && c.Velocity.Y != 0)
            {
                c.MoveableBodyState = MoveableBodyStates.InAirLeft;
            }
            else if (c.Velocity.Y != 0)
            {
                c.MoveableBodyState = MoveableBodyStates.InAir;
            }
            else if (c.Velocity.X > 0)
            {
                if (collisionManager.InAir(c))
                    c.MoveableBodyState = MoveableBodyStates.InAirRight;
                else
                    c.MoveableBodyState = MoveableBodyStates.WalkRight;
            }
            else if (c.Velocity.X < 0)
            {
                if (collisionManager.InAir(c))
                    c.MoveableBodyState = MoveableBodyStates.InAirLeft;
                else
                    c.MoveableBodyState = MoveableBodyStates.WalkLeft;
            }
            else
            {
                if (collisionManager.InAir(c))
                    c.MoveableBodyState = MoveableBodyStates.InAir;
                else
                    c.MoveableBodyState = MoveableBodyStates.Idle;
            }
        }
    }
}
