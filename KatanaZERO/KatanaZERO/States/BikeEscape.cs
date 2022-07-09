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

        private readonly AnimatedObject lightPillar;

        private readonly int currentLane = 1;

        private TrafficManager trafficManager;

        public BikeEscape(Game1 gameReference, int levelId, bool showLevelTitle, StageData stageData = null)
            : base(gameReference, levelId, showLevelTitle, stageData)
        {
            Game.PlaySong(Songs["BikeEscape"]);
            Camera.CameraMode = CameraModes.ConstantVelocity;
            Player.OnBike = true;

            bossIntiate = new Script()
            {
                OnUpdate = (o, e) => BossInitiate(),
            };
            GameComponents.Add(bossIntiate);
            InitializeTrafficManager();
            GameComponents.Add(trafficManager);

            helicopter = new Helicopter(this, Content, Content.Load<Texture2D>("Enemies/Helicopter/Spritesheet"), Content.Load<Dictionary<string, Rectangle>>("Enemies/Helicopter/Map"), new Vector2(1f, 1f), Player);
            helicopter.Hidden = true;
            GameComponents.Add(helicopter);
            PhysicsManager.AddMoveableBody(helicopter);

            lightPillar = new AnimatedObject(Content.Load<Texture2D>("Textures/LightPillar/Spritesheet"), Content.Load<Dictionary<string, Rectangle>>("Textures/LightPillar/Map"), new Vector2(1f, 1f));
            lightPillar.Color = Color.LightSkyBlue * 0.6f;
            lightPillar.Hidden = true;
            lightPillar.AddAnimation("Idle", new MonoGame.Extended.Animations.SpriteSheets.SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, frameDuration: 0.1f));
            lightPillar.PlayAnimation("Idle");
            AddUiComponent(lightPillar);

            Script lightScript = new Script()
            {
                OnUpdate = (o, e) =>
                {
                    if (Player.HasIntent())
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

        public override double LevelTimeInSeconds => 90;

        public override string LevelName => "BIKE ESCAPE";

        public override void SetPlayerSpawnPoint()
        {
            Player.Position = new Vector2(100, 230);
        }

        internal override void RestartLevel(StageData stageData)
        {
            Game.ChangeState(new BikeEscape(Game, LevelId, false, stageData));
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(57570, 334);
        }

        protected override void AddPlayerGoToIntent(TouchLocation touch)
        {
            float roadBeginningY = 165f;
            float roadEndY = 335f - Player.CollisionSize.Y;

            Vector2 posBeginning = new Vector2(Player.CollisionSize.X, Camera.WorldToScreen(new Vector2(0f, roadBeginningY)).Y);
            Vector2 sizeOnScreen = new Vector2((Game.LogicalSize.X - (2 * posBeginning.X)) * Game.Scale.X, Game.WindowSize.Y - posBeginning.Y);

            Rectangle roadArea = new Rectangle(
                (int)posBeginning.X,
                (int)posBeginning.Y,
                (int)sizeOnScreen.X,
                (int)sizeOnScreen.Y);
            if (roadArea.Contains(touch.Position))
            {
                float beginningY = roadArea.Y + (35f * Game.Scale.Y);
                float posY = beginningY;
                if (touch.Position.Y > posBeginning.Y + (0.666f * sizeOnScreen.Y))
                {
                    if (currentLane != 2)
                    {
                        posY = beginningY + (3.4f * (65f * Game.Scale.Y));
                    }
                }
                else if (touch.Position.Y > posBeginning.Y + (0.333f * sizeOnScreen.Y))
                {
                    posY = beginningY + (2 * (65f * Game.Scale.Y));
                    if (currentLane != 1)
                    {
                        posY = beginningY + (2f * (65f * Game.Scale.Y));
                    }
                }
                else
                {
                    if (currentLane != 0)
                    {
                        posY = beginningY;
                    }
                }

                GoToOnScreenIntent newIntent = new GoToOnScreenIntent(InputManager, Camera, Player, new Vector2(touch.Position.X, posY));
                Player.AddIntent(newIntent);
                lightPillar.Position = new Vector2(touch.Position.X - (lightPillar.Size.X / 2), posY - (lightPillar.Size.Y / 2));
            }
        }

        protected override void PlayerClick()
        {
            foreach (TouchLocation touch in InputManager.CurrentTouchCollection.Where(x => x.State == TouchLocationState.Pressed || x.State == TouchLocationState.Moved))
            {
                // We clicked to throw the bottle
                if ((InputManager.RectangleWasJustClicked(WeaponSlotButton.Rectangle) && !WeaponSlotButton.Hidden) ||
                    (InputManager.RectangleWasJustClicked(ThrowButton.Rectangle) && !ThrowButton.Hidden))
                {
                    LastBottleThrowTapId = touch.Id;
                    continue;
                }

                // We clicked to move (if it's the same ID as last bottle throw then player did not intend to move)
                else if (touch.Id != LastBottleThrowTapId)
                {
                    AddPlayerGoToIntent(touch);
                }
            }
        }

        protected override void CreatePhysicsManager()
        {
            PhysicsManager = new PhysicsManager(new SideScrollCollisionManager());
            PhysicsManager.Gravity = 0f;
        }

        protected override void LoadMap()
        {
            Map = Content.Load<TiledMap>("Maps/Escape/Escape");
        }

        private void InitializeTrafficManager()
        {
            trafficManager = new TrafficManager(Game, this, Player, Camera, Content);
            foreach (StreetCar car in trafficManager.Cars)
            {
                GameComponents.Add(car);
                PhysicsManager.AddMoveableBody(car);
            }

            foreach (BikeEnemy enemy in trafficManager.Enemies)
            {
                GameComponents.Add(enemy);
                PhysicsManager.AddMoveableBody(enemy);
            }

            foreach (ICollidable item in trafficManager.Items)
            {
                GameComponents.Add(item);
                PhysicsManager.AddMoveableBody(item);
            }

            foreach (AnimatedObject notification in trafficManager.TrafficWarnings)
            {
                UiComponents.Add(notification);
            }

            foreach (AnimatedObject notification in trafficManager.EnemyWarnings)
            {
                UiComponents.Add(notification);
            }

            foreach (AnimatedObject notification in trafficManager.ItemNotifications)
            {
                UiComponents.Add(notification);
            }
        }

        private void StopCameraMovement(object sender, EventArgs e)
        {
            Camera.CameraMode = CameraModes.Static;
        }

        private void BossInitiate()
        {
            if (Camera.Position.X > 32000f)
            {
                if (!GameOver)
                {
                    Game.PlaySong(Songs["BikeEscapeBoss"]);
                    helicopter.Position = new Vector2(32000f - 300f, 80f);
                    helicopter.Hidden = false;
                    bossIntiate.Enabled = false;
                }
            }
        }
    }
}