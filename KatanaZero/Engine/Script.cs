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
        public void Update(GameTime gameTime)
        {
            OnUpdate?.Invoke(this, new EventArgs());
        }
    }
}
