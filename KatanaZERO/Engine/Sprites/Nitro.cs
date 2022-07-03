namespace Engine.Sprites
{
    using System;
    using Engine.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Nitro : Sprite, ICollidable
    {
        public Nitro(Texture2D t)
            : base(t)
        {
        }

        public Nitro(Texture2D t, Vector2 objScale)
            : base(t, objScale)
        {
        }

        public EventHandler OnMapCollision { get; set; }

        public MoveableBodyStates MoveableBodyState { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 CollisionSize => Size;

        public Rectangle CollisionRectangle => Rectangle;

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
            if (!Hidden)
            {
                if (collider is Player player)
                {
                    if (!player.NitroActive)
                    {
                        player.NitroActive = true;
                        Engine.States.GameState.Sounds["PickUp"].Play();
                        Hidden = true;
                    }
                }
            }
        }

        public void PrepareMove(GameTime gameTime)
        {
        }
    }
}
