namespace Engine.Physics
{
    using System;
    using Microsoft.Xna.Framework;

    public enum MovableBodyState
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
        event EventHandler OnMapCollision;

        MovableBodyState MovableBodyState { get; set; }

        Vector2 Velocity { get; set; }

        Vector2 CollisionSize { get; }

        Rectangle CollisionRectangle { get; }

        void PrepareMove(GameTime gameTime);

        void NotifyHorizontalCollision(GameTime gameTime, object collider);

        void InvokeOnMapCollision(object sender, EventArgs args);
    }
}
