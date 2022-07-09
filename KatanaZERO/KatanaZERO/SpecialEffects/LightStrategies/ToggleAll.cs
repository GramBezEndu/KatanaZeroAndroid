namespace KatanaZERO.LightStrategies
{
    using KatanaZERO.States;

    public class ToggleAll : Toggle
    {
        private bool turnOnNow;

        public ToggleAll(ClubLights cl)
            : base(cl)
        {
        }

        public override void ToggleLights()
        {
            turnOnNow = !turnOnNow;
            if (turnOnNow)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        private void TurnOn()
        {
            foreach (Engine.DrawableRectangle l in ClubLights.Lights)
            {
                l.Hidden = false;
            }
        }

        private void TurnOff()
        {
            foreach (Engine.DrawableRectangle l in ClubLights.Lights)
            {
                l.Hidden = true;
            }
        }
    }
}