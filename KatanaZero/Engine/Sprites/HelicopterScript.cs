using System;
using System.Collections.Generic;
using System.Text;
using Engine.Sprites.Enemies;
using Microsoft.Xna.Framework;
using PlatformerEngine.Timers;

namespace Engine.Sprites
{
    public class HelicopterScript : IComponent
    {
        Helicopter helicopter;
        GameTimer[] phases;
        int currentPhase = 0;
        public HelicopterScript(Helicopter h)
        {
            helicopter = h;
            phases = new GameTimer[5];
            phases[0] = new GameTimer(5.5f);
            phases[0].OnTimedEvent = (o, e) => Advance();
            phases[1] = new GameTimer(2.1f);
            phases[1].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[2] = new GameTimer(0.15f);
            phases[2].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[3] = new GameTimer(0.15f);
            phases[3].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[4] = new GameTimer(0.15f);
            phases[4].OnTimedEvent = (o, e) => AdvanceAndFire();
        }

        private void AdvanceAndFire()
        {
            helicopter.Fire(currentPhase);
            Advance();
        }

        private void Advance()
        {
            currentPhase++;
        }

        public void Update(GameTime gameTime)
        {
            if (currentPhase >= 0 && currentPhase < phases.Length)
                phases[currentPhase].Update(gameTime);
            switch (currentPhase)
            {
                default:
                case 0:
                    helicopter.RotationBack = false;
                    helicopter.Velocity = new Vector2(14f, helicopter.Velocity.Y);
                    break;
                case 1:
                    helicopter.RotationBack = true;
                    helicopter.Velocity = new Vector2(5f, helicopter.Velocity.Y);
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    helicopter.RotationBack = true;
                    helicopter.Velocity = new Vector2(9f, helicopter.Velocity.Y);
                    break;
            }
        }
    }
}
