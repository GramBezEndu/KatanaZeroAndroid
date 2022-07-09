namespace Engine.SpecialEffects
{
    using System;
    using Microsoft.Xna.Framework;

    public class RainbowEffect : SpecialEffect
    {
        private float currentHue = 0f;

        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                foreach (IDrawableComponent target in Targets)
                {
                    target.Color = MonoGame.Extended.ColorHelper.FromHsl(currentHue, 0.5f, 0.5f);
                }

                currentHue += 0.015f;
                if (currentHue >= 1f)
                {
                    currentHue = 0f;
                }
            }
        }
    }
}
