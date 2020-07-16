using Engine.SpecialEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
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
