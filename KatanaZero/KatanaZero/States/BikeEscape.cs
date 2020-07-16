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
using Engine.Physics;
using Engine.Sprites;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace KatanaZero.States
{
    public class BikeEscape : GameState
    {
        Script bossScript;
        public override double LevelTimeInSeconds => 90;
        public BikeEscape(Game1 gameReference, int levelId, bool showLevelTitle) : base(gameReference, levelId, showLevelTitle)
        {
            game.PlaySong(songs["BikeEscape"]);
            Camera.CameraMode = Engine.Camera.CameraModes.ConstantVelocity;
            player.OnBike = true;

            bossScript = new Script()
            {
                OnUpdate = (o, e) => BossInitiate()
            };
            gameComponents.Add(bossScript);
        }

        protected override void CreatePhysicsManager()
        {
            physicsManager = new PhysicsManager(new SideScrollCollisionManager());
            physicsManager.GRAVITY = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            System.Diagnostics.Debug.WriteLine("C: {0} P: {1}", Camera.Position, player.Position);
        }

        private void BossInitiate()
        {
            if (Camera.Position.X > 32000f)
            {
                if (!GameOver)
                {
                    game.PlaySong(songs["BikeEscapeBoss"]);
                    bossScript.Enabled = false;
                }
            }
        }

        public override string LevelName => "TEST";

        public override void SetPlayerSpawnPoint()
        {
            player.Position = new Vector2(100, 220);
        }

        protected override void LoadMap()
        {
            map = content.Load<TiledMap>("Maps/Escape/Escape");
        }

        internal override void RestartLevel()
        {
            //
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(57570, 334);
        }

        internal override void SpawnEntitiesAfterPlayer()
        {
            SpawnCar(4200f, 1);
            SpawnCar(5200f, 3);
            SpawnCar(7200f, 1);
            SpawnCar(7300f, 3);
            SpawnCar(12000f, 2);
            SpawnCar(12000f, 3);
            SpawnCar(14300f, 1);
            SpawnCar(14000f, 3);
            SpawnCar(15100f, 2);
            SpawnCar(15400f, 1);
            SpawnCar(19100f, 1);
            SpawnCar(19700f, 2);
            SpawnCar(20120f, 2);
            SpawnCar(20300f, 3);
            SpawnCar(20380f, 1);
            SpawnCar(24120f, 1);
            SpawnCar(24700f, 2);
            //SpawnCar(new Vector2(3000, 170));
            //SpawnCar(new Vector2(5000, 210));
            //SpawnCar(new Vector2(18000, 180));
        }

        private void SpawnCar(float posX, int lane)
        {
            float posY = 160f;
            switch (lane)
            {
                case 1:
                    posY = 160f;
                    break;
                case 2:
                    posY = 200f;
                    break;
                case 3:
                    posY = 240f;
                    break;
            };
            var car = new StreetCar(content.Load<Texture2D>("Textures/StreetCar"))
            {
                Position = new Vector2(posX, posY),
            };
            gameComponents.Add(car);
            physicsManager.AddMoveableBody(car);
        }
    }
}