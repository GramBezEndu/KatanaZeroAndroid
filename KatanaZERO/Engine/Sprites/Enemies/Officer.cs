namespace Engine.Sprites.Enemies
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Extended.Animations.SpriteSheets;

    public class Officer : Enemy
    {
        public Officer(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, Player p)
            : base(spritesheet, map, scale, p)
        {
            AddAnimation("Shoot", new SpriteSheetAnimationData(new int[] { 0, 1, 2 }, frameDuration: 0.1f));
            AddAnimation("DrawGun", new SpriteSheetAnimationData(new int[] { 3, 4, 5, 6, 7, 8, 9, 10 }, frameDuration: 0.1f));
            AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 }, frameDuration: 0.1f));
            AddAnimation("Game", new SpriteSheetAnimationData(new int[] { 32, 33, 34, 35, 36, 37, 38, 39, 40, 41 }, frameDuration: 0.1f));
            AddAnimation("Die", new SpriteSheetAnimationData(new int[] { 32 }));
            PlayAnimation("Idle");
        }

        public override Vector2 CollisionSize { get { return new Vector2(64, 64); } }
    }
}
