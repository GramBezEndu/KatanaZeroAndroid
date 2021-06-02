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
        int fireLane = 1;
        public HelicopterScript(Helicopter h)
        {
            helicopter = h;
            phases = new GameTimer[17];
            phases[0] = new GameTimer(3.9f);
            phases[0].OnTimedEvent = (o, e) => Advance();

            phases[1] = new GameTimer(2.1f);
            phases[1].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[2] = new GameTimer(0.15f);
            phases[2].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[3] = new GameTimer(0.15f);
            phases[3].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[4] = new GameTimer(0.15f);
            phases[4].OnTimedEvent = (o, e) => AdvanceAndFire();

            phases[5] = new GameTimer(0.25f);
            phases[5].OnTimedEvent = (o, e) => Advance();

            phases[6] = new GameTimer(1f);
            phases[6].OnTimedEvent = (o, e) => Advance();

            phases[7] = new GameTimer(0.15f);
            phases[7].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[8] = new GameTimer(0.15f);
            phases[8].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[9] = new GameTimer(0.15f);
            phases[9].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[10] = new GameTimer(0.15f);
            phases[10].OnTimedEvent = (o, e) => AdvanceAndFire();

            phases[11] = new GameTimer(0.25f);
            phases[11].OnTimedEvent = (o, e) => Advance();
            phases[12] = new GameTimer(1f);
            phases[12].OnTimedEvent = (o, e) => Advance();

            phases[13] = new GameTimer(0.15f);
            phases[13].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[14] = new GameTimer(0.15f);
            phases[14].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[15] = new GameTimer(0.15f);
            phases[15].OnTimedEvent = (o, e) => AdvanceAndFire();
            phases[16] = new GameTimer(0.15f);
            phases[16].OnTimedEvent = (o, e) => AdvanceAndFire();
        }

        private void AdvanceAndFire()
        {
            helicopter.Fire(fireLane);
            fireLane += 1;
            if (fireLane > 4)
                fireLane = 1;
            Advance();
        }

        private void Advance()
        {
            currentPhase++;
        }

        public void Update(GameTime gameTime)
        {
            if (helicopter.Hidden)
                return;
            if (currentPhase >= 0 && currentPhase < phases.Length)
                phases[currentPhase].Update(gameTime);
            switch (currentPhase)
            {
                case 0:
                    helicopter.RotationBack = false;
                    helicopter.Velocity = new Vector2(14f, helicopter.Velocity.Y);
                    break;
                case 1:
                case 5:
                case 11:
                    helicopter.RotationBack = true;
                    helicopter.Velocity = new Vector2(5f, helicopter.Velocity.Y);
                    break;
                default:
                    helicopter.RotationBack = true;
                    helicopter.Velocity = new Vector2(9f, helicopter.Velocity.Y);
                    break;
            }
        }
    }
}
