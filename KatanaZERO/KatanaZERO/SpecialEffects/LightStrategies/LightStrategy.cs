namespace KatanaZERO.LightStrategies
{
    using Engine;
    using KatanaZERO.States;
    using Microsoft.Xna.Framework;

    public abstract class LightStrategy : IComponent
    {
        protected ClubLights clubLights;

        public LightStrategy(ClubLights cl)
        {
            clubLights = cl;
        }

        public abstract void Update(GameTime gameTime);
    }
}