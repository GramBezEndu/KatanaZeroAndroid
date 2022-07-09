namespace KatanaZERO.States
{
    using System;
    using Engine;
    using Engine.States;
    using Microsoft.Xna.Framework;
    using MonoGame.Extended.Tiled;

    public class PrisonPart2 : GameState
    {
        public PrisonPart2(Game1 gameReference, int levelId, bool showLevelTitle, StageData stageData = null)
            : base(gameReference, levelId, showLevelTitle, stageData)
        {
            GameComponents.Add(new Script()
            {
                OnUpdate = EndLevelScript,
            });
        }

        public override string LevelName => "PRISON";

        // Should be the same time across all prison parts
        public override double LevelTimeInSeconds => 180;

        public override void SetPlayerSpawnPoint()
        {
            Player.Position = new Vector2(30, 140);
        }

        internal override void RestartLevel(StageData stageData)
        {
            Game.ChangeState(new PrisonPart1(Game, LevelId, false, stageData));
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(1132, 410 + UiBottomHeight);
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

        protected override void LoadMap()
        {
            Map = Content.Load<TiledMap>("Maps/PrisonPart2/PrisonPart2");
        }

        private void EndLevelScript(object sender, EventArgs e)
        {
            if (!GameOver && !Completed)
            {
                if (Player.Position.X < 55f && Player.Position.Y > 320f)
                {
                    Completed = true;
                }
            }
        }
    }
}