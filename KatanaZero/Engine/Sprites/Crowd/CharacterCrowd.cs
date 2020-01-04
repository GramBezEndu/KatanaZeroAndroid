using Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Sprites.Crowd
{
    public class CharacterCrowd : AnimatedObject, ICollidable
    {
        public CharacterCrowd(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale) : base(spritesheet, map, scale)
        {
        }

        public MoveableBodyStates MoveableBodyState { get; set; }
        public Vector2 Velocity { get; set; }

        public void PrepareMove(GameTime gameTime)
        {

        }
    }
}
