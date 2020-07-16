using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Engine;
using Engine.Sprites;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;

namespace KatanaZero.States
{
    public class PrisonPart1 : GameState
    {
        protected override int FloorLevel { get { return 320; } }
        //Should be the same time across all prison parts
        public override double LevelTimeInSeconds { get { return 180; } }
        public PrisonPart1(Game1 gameReference, int levelId, bool showLevelTitle) : base(gameReference, levelId, showLevelTitle)
        {
            AmbientColor = new Color(150, 150, 150);
            game.PlaySong(content.Load<Song>("Songs/Prison"));

            AddGoToArrowRight(new Vector2(1460, 310));
            gameComponents.Add(new Script()
            {
                OnUpdate = AutoRun,
            });
        }

        private void AutoRun(object sender, EventArgs e)
        {
            if (!GameOver)
            {
                if (player.Position.X > 1400f)
                {
                    player.ResetIntent();
                    player.StepRight();
                }
                if (player.Position.X > 1500f)
                {
                    var nextStage = new PrisonPart2(game, this.levelId, false);
                    //Setup stage timer manually
                    nextStage.StageTimer.CurrentInterval = this.StageTimer.CurrentInterval;
                    //Change camera origin
                    nextStage.Camera.MultiplierOriginX = 0.5f;

                    game.ChangeState(nextStage);
                    (sender as Script).Enabled = false;
                }
            }
        }

        public override string LevelName { get { return "PRISON"; } }

        protected override List<Rectangle> CreateHidingSpots()
        {
            List<Rectangle> hidingSpots = new List<Rectangle>()
            {
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle6"), 180f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle3"), 400f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle4"), 430f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle5"), 600f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle5"), 650f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle5"), 700f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle3"), 950f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle1"), 985f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle1"), 1100f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle2"), 1160f).Rectangle,
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle7"), 1300f).Rectangle,
            };
            hidingSpots.AddRange(base.ReadHidingSpotsFromMap());
            return hidingSpots;
        }

        public override void SetPlayerSpawnPoint()
        {
            player.Position = new Vector2(10, 270);
        }

        protected override void AddHighscore()
        {

        }

        protected override void LoadMap()
        {
            map = content.Load<TiledMap>("Maps/PrisonPart1/PrisonPart1");
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(1480, 464 + GameState.UI_BOTTOM_SIZE_Y);
        }

        private Sprite SpawnObstacle(Texture2D texture, float posX)
        {
            var obstacle = new Sprite(texture);
            obstacle.Color = Color.Black;
            obstacle.Position = new Vector2(posX, FloorLevel - obstacle.Size.Y);
            gameComponents.Add(obstacle);
            return obstacle;
        }

        internal override void RestartLevel()
        {
            game.ChangeState(new PrisonPart1(game, levelId, false));
        }

        internal override void SpawnEntitiesAfterPlayer()
        {
            SpawnPatrollingGangster(new Vector2(510, 200), 3f);
            SpawnPatrollingGangster(new Vector2(500, 200), 2.5f, false);
            SpawnPatrollingGangster(new Vector2(800, 200), 2.5f);
            SpawnPatrollingGangster(new Vector2(1120, 200), 2f);
            SpawnPatrollingGangster(new Vector2(1200, 200), 3f, false);
        }
    }
}