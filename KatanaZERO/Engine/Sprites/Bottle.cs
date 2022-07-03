namespace Engine.Sprites
{
    using System;
    using Engine.Physics;
    using Engine.States;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using PlatformerEngine.Timers;

    public class Bottle : Sprite, ICollidable
    {
        private readonly bool throwingLeft;

        private readonly float velocityIncrement = 0.8f;

        private readonly GameTimer increaseVelocityTimer;

        private float velocityX = 8f;

        public Bottle(Texture2D t, Vector2 scale, bool throwingLeft)
            : base(t, scale)
        {
            increaseVelocityTimer = new GameTimer(0.05f)
            {
                OnTimedEvent = IncreaseVelocity,
            };
            this.throwingLeft = throwingLeft;
            GameState.Sounds["BottleThrow"].Play();
            OnMapCollision += BreakBottle;
        }

        private void BreakBottle(object sender, EventArgs e)
        {
            if (!Hidden)
            {
                GameState.Sounds["GlassBreak"].Play();
                Hidden = true;
                (sender as CollisionManager).BottleBreak(Position);
            }
        }

        private void IncreaseVelocity(object sender, EventArgs e)
        {
            velocityX += velocityIncrement;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Hidden)
            {
                base.Update(gameTime);
                increaseVelocityTimer.Update(gameTime);
            }
        }

        public MoveableBodyStates MoveableBodyState { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 CollisionSize { get { return new Vector2(5, 10); } }

        public Rectangle CollisionRectangle { get { return new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y); } }

        public EventHandler OnMapCollision { get; set; }

        public void PrepareMove(GameTime gameTime)
        {
            if (throwingLeft)
            {
                Velocity = new Vector2(-velocityX, 0.55f);
            }
            else
            {
                Velocity = new Vector2(velocityX, 0.55f);
            }
        }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
        }
    }
}
