using Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Sprites
{
    public class Nitro : Sprite, ICollidable
    {
        public Nitro(Texture2D t) : base(t)
        {
        }

        public Nitro(Texture2D t, Vector2 objScale) : base(t, objScale)
        {
        }

        public EventHandler OnMapCollision { get; set; }
        public MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity { get; set; }

        public Vector2 CollisionSize => this.Size;

        public Rectangle CollisionRectangle => this.Rectangle;

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
            if(!Hidden)
            {
                if (collider is Player player)
                {
                    if (!player.NitroActive)
                    {
                        player.NitroActive = true;
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
