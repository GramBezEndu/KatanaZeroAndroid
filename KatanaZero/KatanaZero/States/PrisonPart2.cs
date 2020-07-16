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
using Engine.States;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;

namespace KatanaZero.States
{
    public class PrisonPart2 : GameState
    {
        //Should be the same time across all prison parts
        public override double LevelTimeInSeconds { get { return 180; } }
        public PrisonPart2(Game1 gameReference, int levelId, bool showLevelTitle) : base(gameReference, levelId, showLevelTitle)
        {
            gameComponents.Add(new Script()
            {
                OnUpdate = EndLevelScript,
            });
        }

        private void EndLevelScript(object sender, EventArgs e)
        {
            if (!GameOver && !Completed)
            {
                if (player.Position.X < 55f && player.Position.Y > 320f)
                {
                    player.Hidden = true;
                    Completed = true;
                }
            }
        }

        public override string LevelName { get { return "PRISON"; } }

        public override void SetPlayerSpawnPoint()
        {
            player.Position = new Vector2(30, 140);
        }

        protected override void LoadMap()
        {
            map = content.Load<TiledMap>("Maps/PrisonPart2/PrisonPart2");
        }

        internal override void RestartLevel()
        {
            game.ChangeState(new PrisonPart1(game, levelId, false));
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(1132, 410 + GameState.UI_BOTTOM_SIZE_Y);
        }

        internal override void SpawnEntitiesBeforePlayer()
        {
            SpawnBottlePickUp(new Vector2(910, 360));
        }

        internal override void SpawnEntitiesAfterPlayer()
        {
            SpawnPatrollingGangster(new Vector2(510, 120), 3f);
            SpawnPatrollingGangster(new Vector2(600, 120), 2.5f, false);

            SpawnPatrollingGangster(new Vector2(200, 340), 2f, false);
            SpawnPatrollingGangster(new Vector2(480, 340), 2.5f);
            SpawnPatrollingGangster(new Vector2(620, 340), 2.5f, false);
            AddGoToArrowDown(new Vector2(46, 336));
        }
    }
}