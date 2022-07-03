namespace Engine
{
    using System;
    using Microsoft.Xna.Framework;

    public class Script : IComponent
    {
        public EventHandler OnUpdate { get; set; }

        public bool Enabled { get; set; } = true;

        public void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                OnUpdate?.Invoke(this, new EventArgs());
            }
        }
    }
}
