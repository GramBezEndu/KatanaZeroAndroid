using System;
using System.Collections.Generic;
using System.Text;
using Engine.MoveStrategies;
using Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;

namespace Engine.Sprites.Enemies
{
    public class Gangster : Enemy
    {
        public Gangster(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, Player p) : base(spritesheet, map, scale, p)
        {
            AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, frameDuration: 0.1f));
            AddAnimation("Walk", new SpriteSheetAnimationData(new int[] { 8, 9, 10, 11, 12, 13, 14, 15 }, frameDuration: 0.1f));
            PlayAnimation("Idle");
        }

        private MoveableBodyStates _moveableBodyState;

        public override MoveableBodyStates MoveableBodyState
        {
            get => _moveableBodyState;
            set
            {
                if (_moveableBodyState != value)
                {
                    _moveableBodyState = value;
                    switch (value)
                    {
                        case MoveableBodyStates.Idle:
                            PlayAnimation("Idle");
                            break;
                        case MoveableBodyStates.WalkRight:
                            SpriteEffects = SpriteEffects.None;
                            PlayAnimation("Walk");
                            break;
                        case MoveableBodyStates.WalkLeft:
                            SpriteEffects = SpriteEffects.FlipHorizontally;
                            PlayAnimation("Walk");
                            break;
                    }
                }
            }
        }
    }
}
