namespace KatanaZERO.LightStrategies
{
    using KatanaZERO.States;

    public class ToggleEvenLights : Toggle
    {
        private bool toggleEvenNow;

        public ToggleEvenLights(ClubLights cl)
            : base(cl)
        {
        }

        public override void ToggleLights()
        {
            toggleEvenNow = !toggleEvenNow;
            if (toggleEvenNow)
            {
                TurnOnEvenLights();
            }
            else
            {
                TurnOnOddLights();
            }
        }

        private void TurnOnEvenLights()
        {
            for (int i = 0; i < ClubLights.Lights.Count; i++)
            {
                if (i % 2 == 0)
                {
                    ClubLights.Lights[i].Hidden = false;
                }
                else
                {
                    ClubLights.Lights[i].Hidden = true;
                }
            }
        }

        private void TurnOnOddLights()
        {
            for (int i = 0; i < ClubLights.Lights.Count; i++)
            {
                if (i % 2 == 0)
                {
                    ClubLights.Lights[i].Hidden = true;
                }
                else
                {
                    ClubLights.Lights[i].Hidden = false;
                }
            }
        }
    }
}