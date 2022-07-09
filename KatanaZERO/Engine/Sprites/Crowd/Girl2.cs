namespace Engine.Sprites.Crowd
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Extended.Animations.SpriteSheets;

    public class Girl2 : CharacterCrowd
    {
        public Girl2(Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale)
            : base(spritesheet, map, scale)
        {
            AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 0, 1, 4, 5, 6, 7, 8, 9, 10, 11, 2, 3 }, frameDuration: 0.1f));
            PlayAnimation("Idle");
        }

        public override Vector2 CollisionSize => new Vector2(30, 40);
    }
}
