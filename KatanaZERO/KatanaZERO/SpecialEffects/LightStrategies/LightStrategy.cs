namespace KatanaZERO.LightStrategies
{
    using Engine;
    using KatanaZERO.States;
    using Microsoft.Xna.Framework;

    public abstract class LightStrategy : IComponent
    {
        public LightStrategy(ClubLights cl)
        {
            ClubLights = cl;
        }

        protected ClubLights ClubLights { get; private set; }

        public abstract void Update(GameTime gameTime);
    }
}