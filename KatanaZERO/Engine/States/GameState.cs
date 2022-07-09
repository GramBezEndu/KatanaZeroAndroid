namespace Engine.States
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Engine.Controls;
    using Engine.Controls.Buttons;
    using Engine.MoveStrategies;
    using Engine.Physics;
    using Engine.PlayerIntents;
    using Engine.SpecialEffects;
    using Engine.Sprites;
    using Engine.Sprites.Enemies;
    using Engine.Storage;
    using KatanaZERO;
    using KatanaZERO.States;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input.Touch;
    using Microsoft.Xna.Framework.Media;
    using MonoGame.Extended.Animations.SpriteSheets;
    using MonoGame.Extended.Tiled;
    using MonoGame.Extended.Tiled.Renderers;
    using PlatformerEngine.Timers;

    public abstract class GameState : State
    {
        private readonly float timerScale = 2.5f;

        private readonly SpriteBatch mapBatch;

        private readonly RenderTarget2D mapLayerRenderTarget;

        private readonly List<IComponent> levelCompleteComponents = new List<IComponent>();

        private readonly List<IComponent> timeIsUpComponents = new List<IComponent>();

        private readonly List<IComponent> playerSpottedComponents = new List<IComponent>();

        private readonly List<IComponent> genericLevelFailure = new List<IComponent>();

        private readonly List<IComponent> levelTitleComponents = new List<IComponent>();

        private readonly List<IComponent> uiBottom = new List<IComponent>();

        private readonly List<IDrawableComponent> pickUpComponents = new List<IDrawableComponent>();

        private Sprite timer;

        private bool gameOver;

        private TiledMapRenderer mapRenderer;

        private bool completed;

        private Sprite bottleSprite;

        public GameState(Game1 gameReference, int levelId, bool showLevelTitle, StageData stageData = null)
            : base(gameReference)
        {
            LevelId = levelId;
            CreatePickUpComponents();
            CreateBottleThrowUI();
            mapBatch = new SpriteBatch(GraphicsDevice);
            mapLayerRenderTarget = new RenderTarget2D(GraphicsDevice, (int)Game.LogicalSize.X, (int)Game.LogicalSize.Y);
            if (stageData == null)
            {
                LoadMap();
                CreateMapRenderer();
            }
            else
            {
                Map = stageData.Map;
                mapRenderer = stageData.MapRenderer;
            }

            MapSize = SetMapSize();
            CreatePhysicsManager();

            List<Rectangle> hidingSpots = CreateHidingSpots();
            SetHidingSpots(hidingSpots);
            SpawnEntitiesBeforePlayer();
            CreatePlayer();
            SpawnEntitiesAfterPlayer();
            CreateCamera(gameReference);
            AddGameOverComponents();
            AddTimeIsUpComponents();
            AddGenericLevelFailureComponents();
            AddHud();
            CreateLevelTimer();
            if (showLevelTitle)
            {
                CreateLevelTitleComponents();
            }

            OnCompleted += (o, e) => AddLevelCompleteComponents();
            OnCompleted += (o, e) => ShowStageClearComponents();
            OnCompleted += (o, e) => AddHighscore();
        }

        public event EventHandler OnCompleted;

        public abstract string LevelName { get; }

        public virtual double LevelTimeInSeconds => 120;

        public EventHandler OnGameOver { get; protected set; }

        public Camera Camera { get; protected set; }

        public GameTimer StageTimer { get; private set; }

        public Color AmbientColor { get; set; } = Color.White;

        public float UiBottomHeight => 50f;

        protected int LevelId { get; private set; }

        protected RectangleButton ThrowButton { get; private set; }

        protected IButton WeaponSlotButton { get; private set; }

        protected int LastBottleThrowTapId { get; set; } = -1;

        protected TiledMap Map { get; set; }

        protected Vector2 MapSize { get; private set; }

        protected Player Player { get; private set; }

        protected PhysicsManager PhysicsManager { get; set; }

        protected List<IComponent> GameComponents { get; private set; } = new List<IComponent>();

        /// <summary>
        /// Determines where the floor level is (in pixels)
        /// </summary>
        protected virtual int FloorLevel { get; }

        protected bool GameOver
        {
            get => gameOver;
            set
            {
                if (gameOver != value)
                {
                    gameOver = value;
                    if (gameOver == true)
                    {
                        MediaPlayer.Stop();
                        GameState.Sounds["LevelFail"].Play();
                        Player.ResetIntent();
                        OnGameOver?.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        protected bool Completed
        {
            get => completed;
            set
            {
                if (completed != value)
                {
                    completed = value;
                    Player.ResetIntent();
                    Player.Hidden = true;
                    if (completed == true)
                    {
                        State.Sounds["StageClear"].Play();
                        OnCompleted?.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        public void AddGameComponent(IComponent c)
        {
            GameComponents.Add(c);
        }

        public void AddMoveableBody(ICollidable body)
        {
            GameComponents.Add(body);
            PhysicsManager.AddMoveableBody(body);
        }

        public void PickUpBottle()
        {
            foreach (IDrawableComponent c in pickUpComponents)
            {
                c.Hidden = false;
            }

            GameTimer hideTimer = new GameTimer(3f);
            hideTimer.OnTimedEvent += (o, e) =>
            {
                foreach (IDrawableComponent c in pickUpComponents)
                {
                    c.Hidden = true;
                }

                hideTimer.Enabled = false;
            };
            GameComponents.Add(hideTimer);
        }

        public void SetHidingSpots(List<Rectangle> hidingSpots)
        {
            PhysicsManager.SetHidingSpots(hidingSpots);
        }

        public abstract void SetPlayerSpawnPoint();

        public override void Update(GameTime gameTime)
        {
            PhysicsManager.SetMapCollision(GetCollisionRectangles());
            PhysicsManager.Update(gameTime);
            if (!GameOver && !Completed)
            {
                PlayerClick();
            }

            ManageBottomHudVisibility();
            if (!GameOver)
            {
                if (PlayerSpotted())
                {
                    GameOver = true;
                    ShowPlayerSpottedGameOverComponents();
                }

                if (Player.MovableBodyState == MovableBodyState.Dead)
                {
                    GameOver = true;
                    ShowGenericLevelFailComponents();
                }
            }

            Camera.Update(gameTime);
            mapRenderer.Update(gameTime);
            if (!GameOver && !Completed)
            {
                StageTimer?.Update(gameTime);
            }

            // TODO: Refactor
            // ICollidable are being updated in physics manager (avoids updating twice)
            foreach (IComponent c in GameComponents.Where(x => !(x is ICollidable)))
            {
                c.Update(gameTime);
            }

            UpdateTimerSize();
            if (GameOver)
            {
                Player.Color = Color.Red;
                if (InputManager.AnyTapDetected() || InputManager.ShakeDetected())
                {
                    RestartLevel(new StageData()
                    {
                        Map = Map,
                        MapRenderer = mapRenderer,
                    });
                }
            }

            base.Update(gameTime);
        }

        internal abstract void RestartLevel(StageData stageData = null);

        internal abstract Vector2 SetMapSize();

        internal virtual void SpawnEntitiesBeforePlayer()
        {
        }

        internal virtual void SpawnEntitiesAfterPlayer()
        {
        }

        protected virtual void PlayerClick()
        {
            bool foundMovement = false;
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
                    foundMovement = true;
                }
            }

            if (!foundMovement)
            {
                Player.ResetIntent();
            }
        }

        protected virtual void CreatePhysicsManager()
        {
            PhysicsManager = new PhysicsManager();
        }

        protected virtual List<Rectangle> CreateHidingSpots()
        {
            return ReadHidingSpotsFromMap();
        }

        protected virtual void AddPlayerGoToIntent(TouchLocation touch)
        {
            Rectangle screenRectLeft = new Rectangle(0, 0, (int)(Game.WindowSize.X / 2), (int)Game.WindowSize.Y);
            if (screenRectLeft.Contains(touch.Position))
            {
                Player.AddIntent(new GoLeft(InputManager, Camera, Player));
            }
            else
            {
                Player.AddIntent(new GoRight(InputManager, Camera, Player));
            }
        }

        protected virtual void AddHighscore()
        {
            HighScoresStorage.Instance.AddTime(new Score(LevelId, StageTimer.Interval - StageTimer.CurrentInterval));
        }

        protected bool PlayerSpotted()
        {
            return PhysicsManager.Spotted(Player);
        }

        protected abstract void LoadMap();

        protected void CreateMapRenderer()
        {
            mapRenderer = new TiledMapRenderer(GraphicsDevice, Map);
        }

        protected override void DrawToScreen()
        {
            mapBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            mapBatch.Draw(mapLayerRenderTarget, new Rectangle(0, 0, (int)Game.WindowSize.X, (int)Game.WindowSize.Y), AmbientColor);
            mapBatch.End();
            base.DrawToScreen();
        }

        protected override void DrawToRenderTarget(GameTime gameTime)
        {
            mapBatch.Begin(transformMatrix: Camera.ViewMatrix);
            GraphicsDevice.SetRenderTarget(mapLayerRenderTarget);

            // Important -> removes weird lines
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.Clear(Color.Black);
            mapRenderer.Draw(Camera.ViewMatrix);
            foreach (IComponent c in GameComponents)
            {
                if (c is IDrawableComponent drawable)
                {
                    drawable.Draw(gameTime, mapBatch);
                }
            }

            mapBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            base.DrawToRenderTarget(gameTime);
        }

        protected void SpawnPatrollingGangster(Vector2 position, float idleTimeSeconds = 3.5f, bool startPatrollingToleft = true)
        {
            Texture2D texture = Content.Load<Texture2D>("Enemies/Gangster/Spritesheet");
            Dictionary<string, Rectangle> map = Content.Load<Dictionary<string, Rectangle>>("Enemies/Gangster/Map");
            Gangster gangster = new Gangster(texture, map, new Vector2(1f, 1f), Player)
            {
                Position = position,
            };
            gangster.CurrentStrategy = new PatrollingStrategy(gangster, position.X - 150f, position.X + 150f, idleTimeSeconds, startPatrollingToleft);
            gangster.PatrollingSprite = new Sprite(Content.Load<Texture2D>("Enemies/Triangle"), new Vector2(0.5f, 0.8f))
            {
                Color = Color.DarkGreen * 0.5f,
            };
            gangster.QuestionMark = new Sprite(Content.Load<Texture2D>("Enemies/Questionmark"))
            {
                Hidden = true,
            };
            GameComponents.Add(gangster);
            PhysicsManager.AddMoveableBody(gangster);
        }

        protected List<Rectangle> ReadHidingSpotsFromMap()
        {
            TiledMapObjectLayer collisionLayer = Map.GetLayer<TiledMapObjectLayer>("HidingSpots");
            TiledMapObject[] collisionObjects = collisionLayer.Objects;
            List<Rectangle> rectangles = new List<Rectangle>();
            foreach (TiledMapObject collisionObject in collisionObjects)
            {
                rectangles.Add(new Rectangle((int)collisionObject.Position.X, (int)collisionObject.Position.Y, (int)collisionObject.Size.Width, (int)collisionObject.Size.Height));
            }

            return rectangles;
        }

        protected void AddGoToArrowDown(Vector2 position)
        {
            Texture2D arrowTexture = Content.Load<Texture2D>("Textures/GoArrow");
            Sprite arrow = new Sprite(arrowTexture)
            {
                Rotation = 1.5708f,
                Origin = new Vector2(arrowTexture.Width / 2, arrowTexture.Height / 2),
            };
            arrow.Position = new Vector2(position.X, position.Y - arrow.Size.Y);
            arrow.AddSpecialEffect(new JumpingEffect());
            GameComponents.Add(arrow);
        }

        protected void AddGoToArrowRight(Vector2 position)
        {
            Texture2D arrowTexture = Content.Load<Texture2D>("Textures/GoArrow");
            Sprite arrow = new Sprite(arrowTexture)
            {
                Origin = new Vector2(arrowTexture.Width / 2, arrowTexture.Height / 2),
            };
            arrow.Position = new Vector2(position.X, position.Y - arrow.Size.Y);
            arrow.AddSpecialEffect(new JumpingEffect());
            GameComponents.Add(arrow);
        }

        protected void SpawnBottlePickUp(Vector2 position)
        {
            BottlePickUp bottle = new BottlePickUp(this, Content.Load<Texture2D>("Textures/Bottle"), new Vector2(0.5f, 0.5f))
            {
                Position = position,
            };

            PickUpArrow pickUpArrow = new PickUpArrow(Content.Load<Texture2D>("PickUpArrow/Spritesheet"), Content.Load<Dictionary<string, Rectangle>>("PickUpArrow/Map"), Vector2.One);
            pickUpArrow.Position = new Vector2(bottle.CollisionRectangle.Center.X - (pickUpArrow.Size.X / 2) + 2f, bottle.Position.Y - pickUpArrow.Size.Y - 3);
            pickUpArrow.AddSpecialEffect(new JumpingEffect());
            bottle.PickUpArrow = pickUpArrow;

            GameComponents.Add(bottle);
            PhysicsManager.AddMoveableBody(bottle);
        }

        private List<Rectangle> GetCollisionRectangles()
        {
            TiledMapObjectLayer collisionLayer = Map.GetLayer<TiledMapObjectLayer>("Collision");
            TiledMapObject[] collisionObjects = collisionLayer.Objects;
            List<Rectangle> rectangles = new List<Rectangle>();
            foreach (TiledMapObject collisionObject in collisionObjects)
            {
                rectangles.Add(new Rectangle((int)collisionObject.Position.X, (int)collisionObject.Position.Y, (int)collisionObject.Size.Width, (int)collisionObject.Size.Height));
            }

            return rectangles;
        }

        private void UpdateTimerSize()
        {
            timer.Scale = new Vector2((float)(timerScale * (StageTimer.CurrentInterval / LevelTimeInSeconds)), timer.Scale.Y);
        }

        private void ShowTimeIsUpGameOverComponents()
        {
            foreach (IComponent c in timeIsUpComponents)
            {
                if (c is IDrawableComponent drawable)
                {
                    drawable.Hidden = false;
                }
            }
        }

        private void ShowPlayerSpottedGameOverComponents()
        {
            foreach (IComponent c in playerSpottedComponents)
            {
                if (c is IDrawableComponent drawable)
                {
                    drawable.Hidden = false;
                }
            }
        }

        private void CreateLevelTimer()
        {
            StageTimer = new GameTimer(LevelTimeInSeconds);
            StageTimer.OnTimedEvent += (o, e) =>
            {
                if (!GameOver)
                {
                    GameOver = true;
                    ShowTimeIsUpGameOverComponents();
                }

                timer.Hidden = true;
            };
        }

        private void AddHud()
        {
            Sprite hud = new Sprite(Textures["Hud"], new Vector2(2f, 3f));
            Sprite hudTimer = new Sprite(Textures["HudTimer"], new Vector2(timerScale, timerScale));
            hudTimer.Position = new Vector2((Game.LogicalSize.X / 2) - (hudTimer.Size.X / 2), 0.01f * Game.LogicalSize.Y);
            Vector2 timerAdjustments = new Vector2(20, 5);
            timer = new Sprite(Textures["Timer"], new Vector2(timerScale, timerScale));
            timer.Position = new Vector2((Game.LogicalSize.X / 2) - (timer.Size.X / 2) + timerAdjustments.X, (0.01f * Game.LogicalSize.Y) + timerAdjustments.Y);
            AddUiComponent(hud);
            AddUiComponent(hudTimer);
            AddUiComponent(timer);
        }

        private void CreateCamera(Game1 gameReference)
        {
            Camera = new Camera(gameReference, MapSize, Player);
        }

        private void ThrowBottle()
        {
            if (Player.HasBottle && !Completed && !GameOver)
            {
                Texture2D texture = Content.Load<Texture2D>("Textures/Bottle");
                bool throwingLeft = Player.SpriteEffects != SpriteEffects.None;
                Bottle bottle = new Bottle(texture, new Vector2(0.5f, 0.5f), throwingLeft)
                {
                    Position = throwingLeft ? new Vector2(Player.Rectangle.Left, Player.Position.Y + 10) : new Vector2(Player.Rectangle.Right, Player.Position.Y + 10),
                    Origin = new Vector2(texture.Width / 2, texture.Height / 2),
                    Rotation = 0.349066f,
                };
                PhysicsManager.AddMoveableBody(bottle);
                GameComponents.Add(bottle);
                Player.HasBottle = false;
            }
        }

        private void AddLevelCompleteComponents()
        {
            Text levelCompleteText = new Text(Fonts["Big"], "LEVEL COMPLETE")
            {
                Hidden = true,
            };
            levelCompleteText.Position = new Vector2((Game.LogicalSize.X / 2) - (levelCompleteText.Size.X / 2), (Game.LogicalSize.Y / 2) - (levelCompleteText.Size.Y / 2));

            Text timeText = new Text(Fonts["Standard"], string.Format("TIME {0}s.", Math.Round(StageTimer.Interval - StageTimer.CurrentInterval, 2).ToString()))
            {
                Hidden = true,
            };
            timeText.Position = new Vector2((Game.LogicalSize.X / 2) - (timeText.Size.X / 2), levelCompleteText.Position.Y + levelCompleteText.Size.Y);

            RectangleButton backButton = new RectangleButton(InputManager, new Rectangle(0, 0, (int)(Game.LogicalSize.X * 0.5f), (int)Game.LogicalSize.Y / 10), Fonts["Standard"], "BACK")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            backButton.OnClick += (o, e) => Game.ChangeState(new MainMenu(Game));
            VerticalNavigationMenu menu = new VerticalNavigationMenu(InputManager, new List<IButton>
            {
                backButton,
            });
            menu.Position = new Vector2((Game.LogicalSize.X / 2) - (menu.Size.X / 2), (Game.LogicalSize.Y * 0.925f) - (menu.Size.Y / 2));
            DrawableRectangle backgroundMenu = new DrawableRectangle(new Rectangle(0, 0, (int)Game.LogicalSize.X, (int)(menu.Size.Y * 1.5f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            backgroundMenu.Position = new Vector2(0, menu.Position.Y - (0.25f * menu.Size.Y));

            levelCompleteComponents.Add(levelCompleteText);
            levelCompleteComponents.Add(timeText);
            levelCompleteComponents.Add(backgroundMenu);
            levelCompleteComponents.Add(menu);

            foreach (IComponent c in levelCompleteComponents)
            {
                AddUiComponent(c);
            }
        }

        private void ShowGenericLevelFailComponents()
        {
            foreach (IComponent c in genericLevelFailure)
            {
                if (c is IDrawableComponent drawable)
                {
                    drawable.Hidden = false;
                }
            }
        }

        private void ManageBottomHudVisibility()
        {
            if (Player.HasBottle)
            {
                foreach (IComponent c in uiBottom)
                {
                    if (c is IDrawableComponent drawable)
                    {
                        drawable.Hidden = false;
                    }
                }
            }
            else
            {
                foreach (IComponent c in uiBottom)
                {
                    if (c is IDrawableComponent drawable)
                    {
                        drawable.Hidden = true;
                    }
                }
            }
        }

        private void CreatePlayer()
        {
            Player = new Player(Content.Load<Texture2D>("Character/Spritesheet"), Content.Load<Dictionary<string, Rectangle>>("Character/Map"), new Vector2(1f, 1f));
            SetPlayerSpawnPoint();
            Player.KatanaSlash = new AnimatedObject(Content.Load<Texture2D>("Character/Katana/Spritesheet"), Content.Load<Dictionary<string, Rectangle>>("Character/Katana/Map"), new Vector2(1f, 1f))
            {
                Hidden = true,
            };
            Player.KatanaSlash.AddAnimation("Slash", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6 }, frameDuration: 0.05f));
            Player.HiddenNotification = new AnimatedObject(Content.Load<Texture2D>("Character/Hidden/Spritesheet"), Content.Load<Dictionary<string, Rectangle>>("Character/Hidden/Map"), new Vector2(1f, 1f))
            {
                Hidden = true,
                Color = Color.OrangeRed,
            };
            Player.HiddenNotification.AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 0, 1 }, frameDuration: 0.2f));
            GameComponents.Add(Player);
            PhysicsManager.AddMoveableBody(Player);
        }

        private void CreateBottleThrowUI()
        {
            ThrowButton = new RectangleButton(InputManager, new Rectangle(0, 0, 240, 72), Fonts["Standard"], "THROW")
            {
                OnClick = (o, e) => ThrowBottle(),
                Filled = true,
                Color = Color.Black * 0.1f,
                Hidden = true,
            };
            ThrowButton.Message.Color = new Color(255, 97, 236);
            ThrowButton.Position = new Vector2(Game.LogicalSize.X - ThrowButton.Size.X, Game.LogicalSize.Y - ThrowButton.Size.Y);

            WeaponSlotButton = new TextureButton(InputManager, Content.Load<Texture2D>("Textures/HudWeapon"), new Vector2(3f, 3f))
            {
                OnClick = (o, e) => ThrowBottle(),
                Hidden = true,
            };
            WeaponSlotButton.Position = new Vector2(ThrowButton.Rectangle.Left - WeaponSlotButton.Size.X - 8, ThrowButton.Rectangle.Center.Y - (WeaponSlotButton.Size.Y / 2));

            bottleSprite = new Sprite(Content.Load<Texture2D>("Textures/Bottle"), new Vector2(1.3f, 1.3f))
            {
                Hidden = true,
            };
            bottleSprite.Position = new Vector2(WeaponSlotButton.Rectangle.Center.X - (bottleSprite.Size.X / 2), WeaponSlotButton.Rectangle.Center.Y - (bottleSprite.Size.Y / 2));

            Sprite hudBackground = new Sprite(Content.Load<Texture2D>("Textures/HudBottom"), new Vector2(3f, 3.8f))
            {
                Hidden = true,
            };
            hudBackground.Position = new Vector2(WeaponSlotButton.Position.X - 15f, Game.LogicalSize.Y - hudBackground.Size.Y);

            uiBottom.Add(hudBackground);
            uiBottom.Add(ThrowButton);
            uiBottom.Add(bottleSprite);
            uiBottom.Add(WeaponSlotButton);

            UiComponents.AddRange(uiBottom);
        }

        private void CreatePickUpComponents()
        {
            Text pickedUpText = new Text(Fonts["XirodMedium"], "PICKED UP BOTTLE")
            {
                Hidden = true,
            };
            pickedUpText.Position = new Vector2((Game.LogicalSize.X / 2) - (pickedUpText.Size.X / 2), Game.LogicalSize.Y * 0.3f);
            Vector2 enlarged = new Vector2(20, 20);
            DrawableRectangle rect = new DrawableRectangle(new Rectangle(0, 0, (int)(pickedUpText.Size.X + enlarged.X), (int)(pickedUpText.Size.Y + enlarged.Y)))
            {
                Hidden = true,
                Filled = true,
                Color = Color.Black,
            };
            rect.Position = new Vector2(pickedUpText.Position.X - (enlarged.X / 2), pickedUpText.Position.Y - (enlarged.Y / 2));
            pickUpComponents.Add(rect);
            pickUpComponents.Add(pickedUpText);

            UiComponents.AddRange(pickUpComponents);
        }

        private void ShowStageClearComponents()
        {
            foreach (IComponent c in levelCompleteComponents)
            {
                if (c is IDrawableComponent drawable)
                {
                    drawable.Hidden = false;
                }
            }
        }

        private void AddTimeIsUpComponents()
        {
            Color textColor = Color.LightBlue;
            SpriteFont font = Fonts["Small"];
            Text needToBeFaster = new Text(font, "I need to be faster.")
            {
                Color = textColor,
            };
            needToBeFaster.Position = new Vector2((Game.LogicalSize.X / 2) - (needToBeFaster.Size.X / 2), (Game.LogicalSize.Y / 2) - (needToBeFaster.Size.Y / 2));
            needToBeFaster.Hidden = true;

            Text restartText = new Text(font, "(Shake phone or tap to restart)")
            {
                Color = textColor,
            };
            restartText.Position = new Vector2((Game.LogicalSize.X / 2) - (restartText.Size.X / 2), needToBeFaster.Position.Y + needToBeFaster.Size.Y);
            restartText.Hidden = true;

            timeIsUpComponents.Add(needToBeFaster);
            timeIsUpComponents.Add(restartText);

            foreach (IComponent c in timeIsUpComponents)
            {
                AddUiComponent(c);
            }
        }

        private void AddGameOverComponents()
        {
            Color textColor = Color.LightBlue;
            SpriteFont font = Fonts["Small"];
            Text cantBeSeen = new Text(font, "I can't be seen.")
            {
                Color = textColor,
            };
            cantBeSeen.Position = new Vector2((Game.LogicalSize.X / 2) - (cantBeSeen.Size.X / 2), (Game.LogicalSize.Y / 2) - (cantBeSeen.Size.Y / 2));
            cantBeSeen.Hidden = true;

            Text moreCareful = new Text(font, "I need to be more careful.")
            {
                Color = textColor,
            };
            moreCareful.Position = new Vector2((Game.LogicalSize.X / 2) - (moreCareful.Size.X / 2), cantBeSeen.Position.Y + cantBeSeen.Size.Y);
            moreCareful.Hidden = true;

            Text restartText = new Text(font, "(Shake phone or tap to restart)")
            {
                Color = textColor,
            };
            restartText.Position = new Vector2((Game.LogicalSize.X / 2) - (restartText.Size.X / 2), moreCareful.Position.Y + moreCareful.Size.Y);
            restartText.Hidden = true;

            playerSpottedComponents.Add(cantBeSeen);
            playerSpottedComponents.Add(moreCareful);
            playerSpottedComponents.Add(restartText);
            foreach (IComponent c in playerSpottedComponents)
            {
                AddUiComponent(c);
            }
        }

        private void AddGenericLevelFailureComponents()
        {
            Color textColor = Color.LightBlue;
            SpriteFont font = Fonts["Small"];
            Text thatWontWork = new Text(font, "That won't work.")
            {
                Color = textColor,
            };
            thatWontWork.Position = new Vector2((Game.LogicalSize.X / 2) - (thatWontWork.Size.X / 2), (Game.LogicalSize.Y / 2) - (thatWontWork.Size.Y / 2));
            thatWontWork.Hidden = true;

            Text restartText = new Text(font, "(Shake phone or tap to restart)")
            {
                Color = textColor,
            };
            restartText.Position = new Vector2((Game.LogicalSize.X / 2) - (restartText.Size.X / 2), thatWontWork.Position.Y + thatWontWork.Size.Y);
            restartText.Hidden = true;

            genericLevelFailure.Add(thatWontWork);
            genericLevelFailure.Add(restartText);

            foreach (IComponent c in genericLevelFailure)
            {
                AddUiComponent(c);
            }
        }

        private void CreateLevelTitleComponents()
        {
            GameTimer fadeTimer = new GameTimer(0.1f)
            {
                Enabled = false,
            };
            fadeTimer.OnTimedEvent += (o, e) =>
            {
                // Fade out effect
                foreach (IComponent c in levelTitleComponents)
                {
                    if (c is IDrawableComponent drawable)
                    {
                        drawable.Color *= 0.8f;
                    }
                }
            };
            GameTimer startFadeTimer = new GameTimer(3f);

            // After 3 seconds activate timer that will do fade out effect
            startFadeTimer.OnTimedEvent += (o, e) => fadeTimer.Enabled = true;
            levelTitleComponents.Add(startFadeTimer);
            levelTitleComponents.Add(fadeTimer);

            Text levelTitle = new Text(Fonts["Big"], LevelName);
            levelTitle.Position = new Vector2(
                (Game.LogicalSize.X / 2) - (levelTitle.Size.X / 2),
                (Game.LogicalSize.Y / 2) - (levelTitle.Size.Y / 2));

            DrawableRectangle backgroundText = new DrawableRectangle(new Rectangle(0, 0, (int)Game.LogicalSize.X, (int)(levelTitle.Size.Y * 1.4f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            backgroundText.Position = new Vector2(0, levelTitle.Position.Y - (0.2f * levelTitle.Size.Y));

            levelTitleComponents.Add(backgroundText);
            levelTitleComponents.Add(levelTitle);

            foreach (IComponent c in levelTitleComponents)
            {
                AddUiComponent(c);
            }
        }
    }
}
