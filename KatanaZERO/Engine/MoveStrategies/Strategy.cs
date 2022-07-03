namespace Engine.MoveStrategies
{
    using Engine.Sprites;
    using Microsoft.Xna.Framework;

    public abstract class Strategy : IComponent
    {
        protected Enemy enemy;

        public Strategy(Enemy e)
        {
            enemy = e;
        }

        public abstract void Update(GameTime gameTime);
    }
}
