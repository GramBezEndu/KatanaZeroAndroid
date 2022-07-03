namespace KatanaZERO.States
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Engine;
    using Engine.Physics;
    using Engine.PlayerIntents;
    using Engine.Sprites;
    using Engine.Sprites.Enemies;
    using Engine.States;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input.Touch;
    using MonoGame.Extended.Tiled;

    public class BikeEscape : GameState
    {
        private readonly Script bossIntiate;
        private readonly Helicopter helicopter;
        private TrafficManager trafficManager;
        private readonly AnimatedObject lightPillar;

        public override double LevelTimeInSeconds => 90;

        private readonly int currentLane = 1;
        //const float firstLaneMiddle = 170f;
        //const float secondLaneMiddle = 210f;
        //const float thirdLaneMiddle = 250f;
        public BikeEscape(Game1 gameReference, int levelId, bool showLevelTitle, StageData stageData = null)
            : base(gameReference, levelId, showLevelTitle, stageData)
        {
            game.PlaySong(songs["BikeEscape"]);
            Camera.CameraMode = Engine.Camera.CameraModes.ConstantVelocity;
            player.OnBike = true;

            bossIntiate = new Script()
            {
                OnUpdate = (o, e) => BossInitiate(),
            };
            gameComponents.Add(bossIntiate);
            InitializeTrafficManager();
            gameComponents.Add(trafficManager);

            helicopter = new Helicopter(this, content, content.Load<Texture2D>("Enemies/Helicopter/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Enemies/Helicopter/Map"), new Vector2(1f, 1f), player);
            helicopter.Hidden = true;
            gameComponents.Add(helicopter);
            physicsManager.AddMoveableBody(helicopter);

            lightPillar = new AnimatedObject(content.Load<Texture2D>("Textures/LightPillar/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Textures/LightPillar/Map"), new Vector2(1f, 1f));
            lightPillar.Color = Color.LightSkyBlue * 0.6f;
            lightPillar.Hidden = true;
            lightPillar.AddAnimation("Idle", new MonoGame.Extended.Animations.SpriteSheets.SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, frameDuration: 0.1f));
            lightPillar.PlayAnimation("Idle");
            AddUiComponent(lightPillar);

            Script lightScript = new Script()
            {
                OnUpdate = (o, e) =>
                {
                    if (player.HasIntent())
                    {
                        lightPillar.Hidden = false;
                    }
                    else
                    {
                        lightPillar.Hidden = true;
                    }
                },
            };
            AddUiComponent(lightScript);

            OnGameOver += new EventHandler(StopCameraMovement);
        }

        private void InitializeTrafficManager()
        {
            trafficManager = new TrafficManager(game, this, player, Camera, content);
            foreach (StreetCar car in trafficManager.Cars)
            {
                gameComponents.Add(car);
                physicsManager.AddMoveableBody(car);
            }

            foreach (BikeEnemy enemy in trafficManager.Enemies)
            {
                gameComponents.Add(enemy);
                physicsManager.AddMoveableBody(enemy);
            }

            foreach (ICollidable item in trafficManager.Items)
            {
                gameComponents.Add(item);
                physicsManager.AddMoveableBody(item);
            }

            foreach (AnimatedObject notification in trafficManager.TrafficWarnings)
            {
                uiComponents.Add(notification);
            }

            foreach (AnimatedObject notification in trafficManager.EnemyWarnings)
            {
                uiComponents.Add(notification);
            }

            foreach (AnimatedObject notification in trafficManager.ItemNotifications)
            {
                uiComponents.Add(notification);
            }
        }

        private void StopCameraMovement(object sender, EventArgs e)
        {
            Camera.CameraMode = Camera.CameraModes.Static;
        }

        protected override void PlayerClick()
        {
            foreach (TouchLocation touch in inputManager.CurrentTouchCollection.Where(x => x.State == TouchLocationState.Pressed || x.State == TouchLocationState.Moved))
            {
                // We clicked to throw the bottle
                if ((inputManager.RectangleWasJustClicked(weaponSlotButton.Rectangle) && !weaponSlotButton.Hidden) ||
                    (inputManager.RectangleWasJustClicked(throwButton.Rectangle) && !throwButton.Hidden))
                {
                    lastBottleThrowTapId = touch.Id;
                    continue;
                }

                // We clicked to move (if it's the same ID as last bottle throw then player did not intend to move)
                else if (touch.Id != lastBottleThrowTapId)
                {
                    AddPlayerGoToIntent(touch);
                }
            }
        }

        override internal void RestartLevel(StageData stageData)
        {
            game.ChangeState(new BikeEscape(game, levelId, false, stageData));
        }

        protected override void AddPlayerGoToIntent(TouchLocation touch)
        {
            float roadBeginningY = 165f;
            float roadEndY = 335f - player.CollisionSize.Y;

            Vector2 posBeginning = new Vector2(player.CollisionSize.X, Camera.WorldToScreen(new Vector2(0f, roadBeginningY)).Y);
            Vector2 sizeOnScreen = new Vector2((game.LogicalSize.X - 2 * posBeginning.X) * game.Scale.X, game.WindowSize.Y - posBeginning.Y);

            Rectangle roadArea = new Rectangle(
                (int)posBeginning.X,
                (int)posBeginning.Y,
                (int)sizeOnScreen.X,
                (int)sizeOnScreen.Y);
            if (roadArea.Contains(touch.Position))
            {
                float beginningY = roadArea.Y + (35f * game.Scale.Y);
                float posY = beginningY;
                if (touch.Position.Y > posBeginning.Y + 0.666f * sizeOnScreen.Y)
                {
                    if (currentLane != 2)
                    {
                        posY = beginningY + 3.4f * (65f * game.Scale.Y);
                    }
                }
                else if (touch.Position.Y > posBeginning.Y + 0.333f * sizeOnScreen.Y)
                {
                    posY = beginningY + 2 * (65f * game.Scale.Y);
                    if (currentLane != 1)
                    {
                        posY = beginningY + 2f * (65f * game.Scale.Y);
                    }
                }
                else
                {
                    if (currentLane != 0)
                    {
                        posY = beginningY;
                    }
                }

                GoToOnScreenIntent newIntent = new GoToOnScreenIntent(inputManager, Camera, player, new Vector2(touch.Position.X, posY));
                player.AddIntent(newIntent);
                lightPillar.Position = new Vector2(touch.Position.X - lightPillar.Size.X / 2, posY - lightPillar.Size.Y / 2);
            }
        }

        protected override void CreatePhysicsManager()
        {
            physicsManager = new PhysicsManager(new SideScrollCollisionManager());
            physicsManager.GRAVITY = 0f;
        }

        private void BossInitiate()
        {
            if (Camera.Position.X > 32000f/*1000f*/)
            {
                if (!GameOver)
                {
                    game.PlaySong(songs["BikeEscapeBoss"]);
                    helicopter.Position = new Vector2(32000f - 300f/*700f*/, 80f);
                    helicopter.Hidden = false;
                    bossIntiate.Enabled = false;
                }
            }
        }

        public override string LevelName => "BIKE ESCAPE";

        public override void SetPlayerSpawnPoint()
        {
            player.Position = new Vector2(100, 230);
        }

        protected override void LoadMap()
        {
            map = content.Load<TiledMap>("Maps/Escape/Escape");
        }

        override internal Vector2 SetMapSize()
        {
            return new Vector2(57570, 334);
        }
    }
}