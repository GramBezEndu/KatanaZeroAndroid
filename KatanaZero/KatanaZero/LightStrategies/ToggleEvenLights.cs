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
using KatanaZero.States;
using Microsoft.Xna.Framework;
using PlatformerEngine.Timers;

namespace KatanaZero.LightStrategies
{
    public class ToggleEvenLights : Toggle
    {
        bool toggleEvenNow;
        public ToggleEvenLights(ClubLights cl) : base(cl)
        {
        }

        public override void ToggleLights()
        {
            toggleEvenNow = !toggleEvenNow;
            if (toggleEvenNow)
                TurnOnEvenLights();
            else
                TurnOnOddLights();
        }

        private void TurnOnEvenLights()
        {
            for (int i = 0; i < clubLights.Lights.Count; i++)
            {
                if (i % 2 == 0)
                    clubLights.Lights[i].Hidden = false;
                else
                    clubLights.Lights[i].Hidden = true;
            }
        }

        private void TurnOnOddLights()
        {
            for (int i = 0; i < clubLights.Lights.Count; i++)
            {
                if (i % 2 == 0)
                    clubLights.Lights[i].Hidden = true;
                else
                    clubLights.Lights[i].Hidden = false;
            }
        }
    }
}