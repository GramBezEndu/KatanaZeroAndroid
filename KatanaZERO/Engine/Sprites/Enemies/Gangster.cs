namespace Engine.Sprites.Enemies
{
    using System.Collections.Generic;
    using Engine.Physics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Extended.Animations.SpriteSheets;

    public class Gangster : Enemy
    {
        private MovableBodyState movableBodyState;

        public Gangster(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, Player p)
            : base(spritesheet, map, scale, p)
        {
            AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, frameDuration: 0.05f));
            AddAnimation("Walk", new SpriteSheetAnimationData(new int[] { 8, 9, 10, 11, 12, 13, 14, 15 }, frameDuration: 0.05f));
            PlayAnimation("Idle");
        }

        public override MovableBodyState MovableBodyState
        {
            get => movableBodyState;
            set
            {
                if (movableBodyState != value)
                {
                    movableBodyState = value;
                    switch (value)
                    {
                        case MovableBodyState.InAir:
                        case MovableBodyState.InAirRight:
                        case MovableBodyState.InAirLeft:
                        case MovableBodyState.Idle:
                            PlayAnimation("Idle");
                            break;
                        case MovableBodyState.WalkRight:
                            SpriteEffects = SpriteEffects.None;
                            PlayAnimation("Walk");
                            break;
                        case MovableBodyState.WalkLeft:
                            SpriteEffects = SpriteEffects.FlipHorizontally;
                            PlayAnimation("Walk");
                            break;
                    }
                }
            }
        }

        public override Vector2 CollisionSize => new Vector2(30, 38);
    }
}
