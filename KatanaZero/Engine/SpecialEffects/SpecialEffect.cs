using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.SpecialEffects
{
    public abstract class SpecialEffect : IComponent
    {
        protected List<IDrawableComponent> targets = new List<IDrawableComponent>();
        public bool Enabled { get; set; } = true;
        public SpecialEffect()
        {

        }

        public virtual void AddTarget(IDrawableComponent target)
        {
            if (!targets.Contains(target))
                targets.Add(target);
        }

        public abstract void Update(GameTime gameTime);
    }
}
