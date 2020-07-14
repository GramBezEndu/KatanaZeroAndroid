using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.States;

namespace Engine
{
    public class Script : IComponent
    {
        public EventHandler OnUpdate { get; set; }
        public bool Enabled { get; set; } = true;
        public void Update(GameTime gameTime)
        {
            if (Enabled)
                OnUpdate?.Invoke(this, new EventArgs());
        }
    }
}
