namespace KatanaZERO.States
{
    using System;
    using System.Collections.Generic;
    using Engine;
    using Engine.Sprites;
    using Engine.States;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using MonoGame.Extended.Tiled;

    public class PrisonPart1 : GameState
    {
        public PrisonPart1(Game1 gameReference, int levelId, bool showLevelTitle, StageData stageData = null)
            : base(gameReference, levelId, showLevelTitle, stageData)
        {
            AmbientColor = new Color(150, 150, 150);
            Game.PlaySong(Content.Load<Song>("Songs/Prison"));

            AddGoToArrowRight(new Vector2(1460, 310));
            GameComponents.Add(new Script()
            {
                OnUpdate = AutoRun,
            });
        }

        // Should be the same time across all prison parts
        public override double LevelTimeInSeconds => 180;

        public override string LevelName => "PRISON";

        protected override int FloorLevel => 320;

        public override void SetPlayerSpawnPoint()
        {
            Player.Position = new Vector2(35, 270);
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(1480, 464);
        }

        internal override void RestartLevel(StageData stageData)
        {
            Game.ChangeState(new PrisonPart1(Game, LevelId, false, stageData));
        }

        internal override void SpawnEntitiesAfterPlayer()
        {
            SpawnPatrollingGangster(new Vector2(510, 200), 3f);
            SpawnPatrollingGangster(new Vector2(500, 200), 2.5f, false);
            SpawnPatrollingGangster(new Vector2(800, 200), 2.5f);
            SpawnPatrollingGangster(new Vector2(1120, 200), 2f);
            SpawnPatrollingGangster(new Vector2(1200, 200), 3f, false);
        }

        protected override List<Rectangle> CreateHidingSpots()
        {
            List<Rectangle> hidingSpots = new List<Rectangle>()
            {
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle6"), 180f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle3"), 400f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle4"), 430f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle5"), 600f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle5"), 650f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle5"), 700f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle3"), 950f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle1"), 985f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle1"), 1100f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle2"), 1160f).Rectangle,
                SpawnObstacle(Content.Load<Texture2D>("Obstacles/Obstacle7"), 1300f).Rectangle,
            };
            hidingSpots.AddRange(ReadHidingSpotsFromMap());
            return hidingSpots;
        }

        protected override void AddHighscore()
        {
        }

        protected override void LoadMap()
        {
            Map = Content.Load<TiledMap>("Maps/PrisonPart1/PrisonPart1");
        }

        private Sprite SpawnObstacle(Texture2D texture, float posX)
        {
            Sprite obstacle = new Sprite(texture)
            {
                Color = Color.Black,
            };
            obstacle.Position = new Vector2(posX, FloorLevel - obstacle.Size.Y);
            GameComponents.Add(obstacle);
            return obstacle;
        }

        private void AutoRun(object sender, EventArgs e)
        {
            if (!GameOver)
            {
                if (Player.Position.X > 1400f)
                {
                    Player.ResetIntent();
                    Player.MoveRight();
                }

                if (Player.Position.X > 1500f)
                {
                    PrisonPart2 nextStage = new PrisonPart2(Game, LevelId, false);

                    // Setup stage timer manually
                    nextStage.StageTimer.CurrentInterval = StageTimer.CurrentInterval;

                    // Change camera origin
                    nextStage.Camera.MultiplierOriginX = 0.5f;

                    Game.ChangeState(nextStage);
                    (sender as Script).Enabled = false;
                }
            }
        }
    }
}