namespace Engine.Sprites
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public interface ISprite : IDrawableComponent
    {
        Vector2 Scale { get; }

        SpriteEffects SpriteEffects { get; set; }
    }
}
