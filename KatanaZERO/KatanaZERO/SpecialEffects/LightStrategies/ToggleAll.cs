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
using KatanaZERO.States;
using Microsoft.Xna.Framework;
using PlatformerEngine.Timers;

namespace KatanaZERO.LightStrategies
{
    public class ToggleAll : Toggle
    {
        bool turnOnNow;
        public ToggleAll(ClubLights cl) : base(cl)
        {
        }

        public override void ToggleLights()
        {
            turnOnNow = !turnOnNow;
            if (turnOnNow)
                TurnOn();
            else
                TurnOff();
        }

        private void TurnOn()
        {
            foreach (var l in clubLights.Lights)
                l.Hidden = false;
        }

        private void TurnOff()
        {
            foreach (var l in clubLights.Lights)
                l.Hidden = true;
        }
    }
}