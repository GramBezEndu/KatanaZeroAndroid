using Engine.SpecialEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Sprites
{
    public interface ISprite : IDrawableComponent
    {
        Vector2 Scale { get; }
        SpriteEffects SpriteEffects { get; set; }
        List<SpecialEffect> SpecialEffects { get; set; }
        void AddSpecialEffect(SpecialEffect effect);
    }
}
