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
        public PrisonPart1(Game1 gameReference, bool showLevelTitle) : base(gameReference, showLevelTitle)
        {
            AmbientColor = new Color(150, 150, 150);
            game.PlaySong(content.Load<Song>("Songs/Prison"));

            SpawnPatrollingGangster(new Vector2(510, 200), 3f);
            SpawnPatrollingGangster(new Vector2(500, 200), 2.5f, false);
            //SpawnPatrollingGangster(new Vector2(800, 200), 2f);
            SpawnPatrollingGangster(new Vector2(800, 200), 2.5f);
            SpawnPatrollingGangster(new Vector2(1120, 200), 2f);
            SpawnPatrollingGangster(new Vector2(1200, 200), 3f, false);

            AddGoToArrowRight(new Vector2(1460, 310));
            gameComponents.Add(new Script()
            {
                OnUpdate = AutoRun,
            });
        }

        private void AutoRun(object sender, EventArgs e)
        {
            if(player.Position.X > 1400f)
            {
                player.ResetIntent();
                player.MoveRight();
            }
            if(player.Position.X > 1500f)
            {
                var nextStage = new PrisonPart2(game, false);
                //Transfer stage timer and level time
                nextStage.LevelTimeInSeconds = this.LevelTimeInSeconds;
                nextStage.StageTimer = this.StageTimer;

                game.ChangeState(nextStage);
                (sender as Script).Enabled = false;
            }
        }

        public override string LevelName { get { return "PRISON"; } }

        protected override List<Sprite> CreateHidingSpots()
        {
            return new List<Sprite>()
            {
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle6"), 180f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle3"), 400f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle4"), 430f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle5"), 600f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle5"), 650f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle5"), 700f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle3"), 950f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle1"), 985f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle1"), 1100f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle2"), 1160f),
                SpawnObstacle(content.Load<Texture2D>("Obstacles/Obstacle7"), 1300f),
            };
        }

        public override void SetPlayerSpawnPoint()
        {
            player.Position = new Vector2(10, 220);
        }

        protected override void AddHighscore()
        {
            //throw new NotImplementedException();
        }

        protected override void LoadMap()
        {
            map = content.Load<TiledMap>("Maps/PrisonPart1/PrisonPart1");
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(1480, 464);
        }

        private Sprite SpawnObstacle(Texture2D texture, float posX)
        {
            var obstacle = new Sprite(texture);
            obstacle.Color = Color.Black;
            obstacle.Position = new Vector2(posX, FloorLevel - obstacle.Size.Y);
            return obstacle;
        }
    }
}