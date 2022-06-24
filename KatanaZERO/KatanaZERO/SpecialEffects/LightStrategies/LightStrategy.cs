using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Engine;
using KatanaZERO.States;
using Microsoft.Xna.Framework;

namespace KatanaZERO.LightStrategies
{
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