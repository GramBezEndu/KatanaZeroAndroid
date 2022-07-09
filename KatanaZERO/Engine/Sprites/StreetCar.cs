namespace Engine.Sprites
{
    using System;
    using Engine.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class StreetCar : Sprite, ICollidable
    {
        private readonly float heightTreshold = 18f;

        public StreetCar(Texture2D t)
            : base(t)
        {
        }

        public event EventHandler OnMapCollision;

        public MovableBodyState MovableBodyState { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 CollisionSize => new Vector2(113, 45 - heightTreshold);

        public Rectangle CollisionRectangle => new Rectangle((int)Position.X, (int)(Position.Y + heightTreshold), (int)CollisionSize.X, (int)CollisionSize.Y);

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
            if (collider is Player player)
            {
                player.MovableBodyState = MovableBodyState.Dead;
            }
        }

        public void PrepareMove(GameTime gameTime)
        {
            Velocity = new Vector2(-2f, Velocity.Y);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        public void InvokeOnMapCollision(object sender, EventArgs args)
        {
            OnMapCollision?.Invoke(sender, args);
        }
    }
}
