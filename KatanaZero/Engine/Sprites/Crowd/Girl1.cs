using Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Sprites.Crowd
{
    public class Girl1 : CharacterCrowd
    {
        public Girl1(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale) : base(spritesheet, map, scale)
        {
            AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 0, 1, 4, 5, 6, 7, 8, 9, 10, 11, 2, 3 }, frameDuration: 0.2f));
            PlayAnimation("Idle");
        }
    }
}
