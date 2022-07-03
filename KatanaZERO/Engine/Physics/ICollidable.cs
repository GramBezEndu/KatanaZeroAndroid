namespace Engine.Physics
{
    using System;
    using Microsoft.Xna.Framework;

    public enum MoveableBodyStates
    {
        Idle,
        WalkRight,
        WalkLeft,
        InAir,
        InAirRight,
        InAirLeft,
        Attack,
        Dance,
        Hidden,
        Dead,
    }

    public interface ICollidable : IDrawableComponent
    {
        EventHandler OnMapCollision { get; set; }

        MoveableBodyStates MoveableBodyState { get; set; }

        Vector2 Velocity { get; set; }

        Vector2 CollisionSize { get; }

        Rectangle CollisionRectangle { get; }

        void PrepareMove(GameTime gameTime);

        void NotifyHorizontalCollision(GameTime gameTime, object collider);
    }
}
