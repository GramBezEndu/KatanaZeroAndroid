namespace Engine.MoveStrategies
{
    using Engine.Sprites;
    using Microsoft.Xna.Framework;

    public abstract class Strategy : IComponent
    {
        public Strategy(Enemy e)
        {
            Enemy = e;
        }

        protected Enemy Enemy { get; private set; }

        public abstract void Update(GameTime gameTime);
    }
}
