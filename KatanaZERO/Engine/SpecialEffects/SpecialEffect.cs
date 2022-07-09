namespace Engine.SpecialEffects
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public abstract class SpecialEffect : IComponent
    {
        public SpecialEffect()
        {
        }

        public bool Enabled { get; set; } = true;

        public List<IDrawableComponent> Targets { get; private set; } = new List<IDrawableComponent>();

        public virtual void AddTarget(IDrawableComponent target)
        {
            if (!Targets.Contains(target))
            {
                Targets.Add(target);
            }
        }

        public abstract void Update(GameTime gameTime);
    }
}
