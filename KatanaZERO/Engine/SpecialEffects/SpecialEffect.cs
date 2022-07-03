namespace Engine.SpecialEffects
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

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
            {
                targets.Add(target);
            }
        }

        public abstract void Update(GameTime gameTime);
    }
}
