namespace Engine
{
    using System.Collections.Generic;
    using Engine.SpecialEffects;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public interface IDrawableComponent : IComponent
    {
        bool Hidden { get; set; }

        Vector2 Position { get; set; }

        Vector2 Size { get; }

        Rectangle Rectangle { get; }

        Color Color { get; set; }

        List<SpecialEffect> SpecialEffects { get; set; }

        void AddSpecialEffect(SpecialEffect effect);

        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
