using Engine.Sprites.Enemies;
using Engine.Sprites;
using KatanaZERO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Engine.Controls.Buttons;
using Engine.PlayerIntents;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Engine.Physics;
using Engine.MoveStrategies;
using PlatformerEngine.Timers;
using System.Diagnostics;
using KatanaZERO.States;
using Microsoft.Xna.Framework.Media;
using System.Text.RegularExpressions;
using Engine.Controls;
using System.Linq;
using Microsoft.Xna.Framework.Input.Touch;
using Engine.States;
using Engine.SpecialEffects;
using Engine.Storage;

namespace Engine.States
{
    public abstract class GameState : State
    {
        protected TiledMap map;
        protected Vector2 mapSize;
        protected TiledMapRenderer mapRenderer;
        protected SpriteBatch mapBatch;
        protected RenderTarget2D mapLayerRenderTarget;
        public Camera Camera { get; protected set; }
        protected Player player;
        protected PhysicsManager physicsManager;
        protected List<IComponent> gameComponents = new List<IComponent>();
        protected List<IComponent> levelCompleteComponents = new List<IComponent>();
        protected List<IComponent> timeIsUpComponents = new List<IComponent>();
        protected List<IComponent> playerSpottedComponents = new List<IComponent>();
        protected List<IComponent> genericLevelFailure = new List<IComponent>();
        protected List<IComponent> levelTitleComponents = new List<IComponent>();
        protected List<IComponent> uiBottom = new List<IComponent>();
        public virtual double LevelTimeInSeconds { get { return 120; } }
        public GameTimer StageTimer;
        private readonly float timerScale = 2.5f;
        private Sprite timer;
        private bool gameOver;
        public Color AmbientColor = Color.White;
        protected int levelId;

        protected List<IDrawableComponent> pickUpComponents = new List<IDrawableComponent>();

        protected RectangleButton throwButton;
        protected IButton weaponSlotButton;
        protected int lastBottleThrowTapId = -1;
        private Sprite bottleSprite;
        public const float UI_BOTTOM_SIZE_Y = 50f;

        public event EventHandler OnCompleted;
        private bool completed;

        /// <summary>
        /// Determines where the floor level is (in pixels)
        /// </summary>
        protected virtual int FloorLevel { get; }

        public abstract string LevelName { get; }

        protected bool GameOver
        {
            get => gameOver;
            set
            {
                if(gameOver != value)
                {
                    gameOver = value;
                    if (gameOver == true)
                    {
                        MediaPlayer.Stop();
                        GameState.Sounds["LevelFail"].Play();
                        player.ResetIntent();
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
                if(completed != value)
                {
                    completed = value;
                    player.ResetIntent();
                    player.Hidden = true;
                    if(completed == true)
                    {
                        State.Sounds["StageClear"].Play();
                        OnCompleted?.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        public EventHandler OnGameOver { get; protected set; }

        public GameState(Game1 gameReference, int levelId, bool showLevelTitle, StageData stageData = null) : base(gameReference)
        {
            this.levelId = levelId;
            CreatePickUpComponents();
            CreateBottleThrowUI();
            mapBatch = new SpriteBatch(graphicsDevice);
            mapLayerRenderTarget = new RenderTarget2D(graphicsDevice, (int)game.LogicalSize.X, (int)game.LogicalSize.Y);
            if (stageData == null)
            {
                LoadMap();
                CreateMapRenderer();
            }
            else
            {
                map = stageData.Map;
                mapRenderer = stageData.MapRenderer;
            }
            mapSize = SetMapSize();
            CreatePhysicsManager();

            var hidingSpots = CreateHidingSpots();
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
                CreateLevelTitleComponents();
            OnCompleted += (o, e) => AddLevelCompleteComponents();
            OnCompleted += (o, e) => ShowStageClearComponents();
            OnCompleted += (o, e) => AddHighscore();
        }

        protected virtual void CreatePhysicsManager()
        {
            physicsManager = new PhysicsManager();
        }

        private void CreatePickUpComponents()
        {
            var pickedUpText = new Text(fonts["XirodMedium"], "PICKED UP BOTTLE")
            {
                Hidden = true,
            };
            pickedUpText.Position = new Vector2(game.LogicalSize.X / 2 - pickedUpText.Size.X / 2, game.LogicalSize.Y * 0.3f);
            Vector2 enlarged = new Vector2(20, 20);
            var rect = new DrawableRectangle(new Rectangle(0, 0, (int)(pickedUpText.Size.X + enlarged.X), (int)(pickedUpText.Size.Y + enlarged.Y)))
            {
                Hidden = true,
                Filled = true,
                Color = Color.Black,
            };
            rect.Position = new Vector2(pickedUpText.Position.X - enlarged.X / 2, pickedUpText.Position.Y - enlarged.Y / 2);
            pickUpComponents.Add(rect);
            pickUpComponents.Add(pickedUpText);

            uiComponents.AddRange(pickUpComponents);
        }

        private void CreateBottleThrowUI()
        {
            throwButton = new RectangleButton(inputManager, new Rectangle(0, 0, 240, 72), fonts["Standard"], "THROW")
            {
                OnClick = (o, e) => ThrowBottle(),
                Filled = true,
                Color = Color.Black * 0.1f,
                Hidden = true,
            };
            throwButton.Message.Color = /*Color.HotPink*/new Color(255, 97, 236);
            throwButton.Position = new Vector2(game.LogicalSize.X - throwButton.Size.X, game.LogicalSize.Y - throwButton.Size.Y);

            weaponSlotButton = new TextureButton(inputManager, content.Load<Texture2D>("Textures/HudWeapon"), new Vector2(3f, 3f))
            {
                OnClick = (o, e) => ThrowBottle(),
                Hidden = true,
            };
            weaponSlotButton.Position = new Vector2(throwButton.Rectangle.Left - weaponSlotButton.Size.X - 8, throwButton.Rectangle.Center.Y - weaponSlotButton.Size.Y / 2);

            bottleSprite = new Sprite(content.Load<Texture2D>("Textures/Bottle"), new Vector2(1.3f, 1.3f))
            {
                Hidden = true,
            };
            bottleSprite.Position = new Vector2(weaponSlotButton.Rectangle.Center.X - bottleSprite.Size.X / 2, weaponSlotButton.Rectangle.Center.Y - bottleSprite.Size.Y / 2);

            var hudBackground = new Sprite(content.Load<Texture2D>("Textures/HudBottom"), new Vector2(3f, 3.8f))
            {
                Hidden = true,
            };
            hudBackground.Position = new Vector2(weaponSlotButton.Position.X - 15f, game.LogicalSize.Y - hudBackground.Size.Y);

            uiBottom.Add(hudBackground);
            uiBottom.Add(throwButton);
            uiBottom.Add(bottleSprite);
            uiBottom.Add(weaponSlotButton);

            uiComponents.AddRange(uiBottom);
        }

        internal virtual void SpawnEntitiesBeforePlayer() { }
        internal virtual void SpawnEntitiesAfterPlayer() { }

        public void SetHidingSpots(List<Rectangle> hidingSpots)
        {
            physicsManager.SetHidingSpots(hidingSpots);
        }

        protected virtual List<Rectangle> CreateHidingSpots()
        {
            return ReadHidingSpotsFromMap();
        }

        internal abstract Vector2 SetMapSize();

        private void CreateLevelTitleComponents()
        {
            var fadeTimer = new GameTimer(0.1f)
            {
                Enabled = false,
                OnTimedEvent = (o, e) =>
                {
                    //Fade out effect
                    foreach (var c in levelTitleComponents)
                    {
                        if (c is IDrawableComponent drawable)
                        {
                            drawable.Color = drawable.Color * 0.8f;
                        }
                    }
                }
            };
            var startFadeTimer = new GameTimer(3f)
            {
                //After 3 seconds activate timer that will do fade out effect
                OnTimedEvent = (o, e) =>
                {
                    fadeTimer.Enabled = true;
                }
            };
            levelTitleComponents.Add(startFadeTimer);
            levelTitleComponents.Add(fadeTimer);

            var levelTitle = new Text(fonts["Big"], LevelName);
            levelTitle.Position = new Vector2(game.LogicalSize.X / 2 - levelTitle.Size.X / 2,
                game.LogicalSize.Y / 2 - levelTitle.Size.Y / 2);

            var backgroundText = new DrawableRectangle(new Rectangle(0, 0, (int)(game.LogicalSize.X), (int)(levelTitle.Size.Y * 1.4f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            backgroundText.Position = new Vector2(0, levelTitle.Position.Y - 0.2f * levelTitle.Size.Y);

            levelTitleComponents.Add(backgroundText);
            levelTitleComponents.Add(levelTitle);

            foreach (var c in levelTitleComponents)
                AddUiComponent(c);
        }

        private void ShowStageClearComponents()
        {
            foreach (var c in levelCompleteComponents)
                if (c is IDrawableComponent drawable)
                    drawable.Hidden = false;
        }

        private void AddTimeIsUpComponents()
        {
            var textColor = Color.LightBlue;
            var font = fonts["Small"];
            var needToBeFaster = new Text(font, "I need to be faster.")
            {
                Color = textColor,
            };
            needToBeFaster.Position = new Vector2(game.LogicalSize.X / 2 - needToBeFaster.Size.X / 2, game.LogicalSize.Y / 2 - needToBeFaster.Size.Y / 2);
            needToBeFaster.Hidden = true;

            var restartText = new Text(font, "(Shake phone or tap to restart)")
            {
                Color = textColor,
            };
            restartText.Position = new Vector2(game.LogicalSize.X / 2 - restartText.Size.X / 2, needToBeFaster.Position.Y + needToBeFaster.Size.Y);
            restartText.Hidden = true;

            timeIsUpComponents.Add(needToBeFaster);
            timeIsUpComponents.Add(restartText);

            foreach (var c in timeIsUpComponents)
                AddUiComponent(c);
        }

        private void AddGameOverComponents()
        {
            var textColor = Color.LightBlue;
            var font = fonts["Small"];
            var cantBeSeen = new Text(font, "I can't be seen.")
            {
                Color = textColor,
            };
            cantBeSeen.Position = new Vector2(game.LogicalSize.X / 2 - cantBeSeen.Size.X / 2, game.LogicalSize.Y / 2 - cantBeSeen.Size.Y / 2);
            cantBeSeen.Hidden = true;

            var moreCareful = new Text(font, "I need to be more careful.")
            {
                Color = textColor,
            };
            moreCareful.Position = new Vector2(game.LogicalSize.X / 2 - moreCareful.Size.X / 2, cantBeSeen.Position.Y + cantBeSeen.Size.Y);
            moreCareful.Hidden = true;

            var restartText = new Text(font, "(Shake phone or tap to restart)")
            {
                Color = textColor,
            };
            restartText.Position = new Vector2(game.LogicalSize.X / 2 - restartText.Size.X / 2, moreCareful.Position.Y + moreCareful.Size.Y);
            restartText.Hidden = true;

            playerSpottedComponents.Add(cantBeSeen);
            playerSpottedComponents.Add(moreCareful);
            playerSpottedComponents.Add(restartText);
            foreach (var c in playerSpottedComponents)
                AddUiComponent(c);
        }

        private void AddGenericLevelFailureComponents()
        {
            var textColor = Color.LightBlue;
            var font = fonts["Small"];
            var thatWontWork = new Text(font, "That won't work.")
            {
                Color = textColor,
            };
            thatWontWork.Position = new Vector2(game.LogicalSize.X / 2 - thatWontWork.Size.X / 2, game.LogicalSize.Y / 2 - thatWontWork.Size.Y / 2);
            thatWontWork.Hidden = true;

            var restartText = new Text(font, "(Shake phone or tap to restart)")
            {
                Color = textColor,
            };
            restartText.Position = new Vector2(game.LogicalSize.X / 2 - restartText.Size.X / 2, thatWontWork.Position.Y + thatWontWork.Size.Y);
            restartText.Hidden = true;

            genericLevelFailure.Add(thatWontWork);
            genericLevelFailure.Add(restartText);

            foreach (var c in genericLevelFailure)
                AddUiComponent(c);
        }

        private void CreateLevelTimer()
        {
            StageTimer = new GameTimer(LevelTimeInSeconds)
            {
                OnTimedEvent = (o, e) =>
                {
                    if (!GameOver)
                    {
                        GameOver = true;
                        ShowTimeIsUpGameOverComponents();
                    }
                    timer.Hidden = true;
                }
            };
        }

        private void AddHud()
        {
            var hud = new Sprite(commonTextures["Hud"], new Vector2(2f, 3f));
            var hudTimer = new Sprite(commonTextures["HudTimer"], new Vector2(timerScale, timerScale));
            hudTimer.Position = new Vector2(game.LogicalSize.X / 2 - hudTimer.Size.X / 2, 0.01f * game.LogicalSize.Y);
            Vector2 timerAdjustments = new Vector2(20, 5);
            timer = new Sprite(commonTextures["Timer"], new Vector2(timerScale, timerScale));
            timer.Position = new Vector2(game.LogicalSize.X / 2 - timer.Size.X / 2 + timerAdjustments.X, 0.01f * game.LogicalSize.Y + timerAdjustments.Y);
            AddUiComponent(hud);
            AddUiComponent(hudTimer);
            AddUiComponent(timer);
        }

        private void CreateCamera(Game1 gameReference)
        {
            Camera = new Camera(gameReference, mapSize, player);
        }

        public void AddGameComponent(IComponent c)
        {
            gameComponents.Add(c);
        }

        public void AddMoveableBody(ICollidable body)
        {
            gameComponents.Add(body);
            physicsManager.AddMoveableBody(body);
        }

        protected abstract void LoadMap();
        protected void CreateMapRenderer()
        {
            mapRenderer = new TiledMapRenderer(graphicsDevice, map);
        }

        private void CreatePlayer()
        {
            player = new Player(content.Load<Texture2D>("Character/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Character/Map"), inputManager, new Vector2(1f, 1f));
            SetPlayerSpawnPoint();
            player.KatanaSlash = new AnimatedObject(content.Load<Texture2D>("Character/Katana/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Character/Katana/Map"), new Vector2(1f, 1f))
            {
                Hidden = true,
            };
            player.KatanaSlash.AddAnimation("Slash", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6 }, frameDuration: 0.05f));
            player.HiddenNotification = new AnimatedObject(content.Load<Texture2D>("Character/Hidden/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Character/Hidden/Map"), new Vector2(1f, 1f))
            {
                Hidden = true,
                Color = Color.OrangeRed
            };
            player.HiddenNotification.AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 0, 1 }, frameDuration: 0.2f));
            gameComponents.Add(player);
            physicsManager.AddMoveableBody(player);
        }

        public abstract void SetPlayerSpawnPoint();

        private void AddLevelCompleteComponents()
        {
            var levelCompleteText = new Text(fonts["Big"], "LEVEL COMPLETE")
            {
                Hidden = true
            };
            levelCompleteText.Position = new Vector2(game.LogicalSize.X / 2 - levelCompleteText.Size.X / 2, game.LogicalSize.Y / 2 - levelCompleteText.Size.Y / 2);

            var timeText = new Text(fonts["Standard"], String.Format("TIME {0}s.", Math.Round(StageTimer.Interval - StageTimer.CurrentInterval, 2).ToString()))
            {
                Hidden = true
            };
            timeText.Position = new Vector2(game.LogicalSize.X / 2 - timeText.Size.X / 2, levelCompleteText.Position.Y + levelCompleteText.Size.Y);

            var backButton = new RectangleButton(inputManager, new Rectangle(0, 0, (int)(game.LogicalSize.X * 0.5f), (int)game.LogicalSize.Y / 10), fonts["Standard"], "BACK")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            backButton.OnClick += (o, e) => game.ChangeState(new MainMenu(game));
            var menu = new VerticalNavigationMenu(inputManager, new List<IButton>
            {
                backButton,
            });
            menu.Position = new Vector2(game.LogicalSize.X / 2 - menu.Size.X / 2, game.LogicalSize.Y * (0.925f) - menu.Size.Y / 2);
            var backgroundMenu = new DrawableRectangle(new Rectangle(0, 0, (int)(game.LogicalSize.X), (int)(menu.Size.Y * 1.5f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            backgroundMenu.Position = new Vector2(0, menu.Position.Y - 0.25f * menu.Size.Y);

            levelCompleteComponents.Add(levelCompleteText);
            levelCompleteComponents.Add(timeText);
            levelCompleteComponents.Add(backgroundMenu);
            levelCompleteComponents.Add(menu);

            foreach (var c in levelCompleteComponents)
                AddUiComponent(c);
        }

        public override void Update(GameTime gameTime)
        {
            physicsManager.SetMapCollision(GetCollisionRectangles());
            physicsManager.Update(gameTime);
            if (!GameOver && !Completed)
                PlayerClick();
            ManageBottomHudVisibility();
            if (!GameOver)
            {
                if (PlayerSpotted())
                {
                    GameOver = true;
                    ShowPlayerSpottedGameOverComponents();
                }
                if (player.MoveableBodyState == MoveableBodyStates.Dead)
                {
                    GameOver = true;
                    ShowGenericLevelFailComponents();
                }
            }
            Camera.Update(gameTime);
            mapRenderer.Update(gameTime);
            if (!GameOver && !Completed)
                StageTimer?.Update(gameTime);
            //TODO: Refactor
            //ICollidable are being updated in physics manager (avoids updating twice)
            foreach (var c in gameComponents.Where(x => !(x is ICollidable)))
                c.Update(gameTime);
            UpdateTimerSize();
            if (GameOver)
            {
                player.Color = Color.Red;
                if (inputManager.AnyTapDetected() || inputManager.ShakeDetected())
                {
                    RestartLevel(new StageData()
                    {
                        Map = this.map,
                        MapRenderer = this.mapRenderer,
                    });
                }
            }
            base.Update(gameTime);
        }

        private void ShowGenericLevelFailComponents()
        {
            foreach (var c in genericLevelFailure)
                if (c is IDrawableComponent drawable)
                    drawable.Hidden = false;
        }

        private void ManageBottomHudVisibility()
        {
            if (player.HasBottle)
            {
                foreach (var c in uiBottom)
                {
                    if (c is IDrawableComponent drawable)
                    {
                        drawable.Hidden = false;
                    }
                }
            }
            else
            {
                foreach (var c in uiBottom)
                {
                    if (c is IDrawableComponent drawable)
                    {
                        drawable.Hidden = true;
                    }
                }
            }
        }

        internal abstract void RestartLevel(StageData stageData = null);

        protected virtual void PlayerClick()
        {
            bool foundMovement = false;
            foreach (var touch in inputManager.CurrentTouchCollection.Where(x => x.State == TouchLocationState.Pressed || x.State == TouchLocationState.Moved))
            {
                //We clicked to throw the bottle
                if ((inputManager.RectangleWasJustClicked(weaponSlotButton.Rectangle) && !weaponSlotButton.Hidden) ||
                    (inputManager.RectangleWasJustClicked(throwButton.Rectangle) && !throwButton.Hidden))
                {
                    lastBottleThrowTapId = touch.Id;
                    continue;
                }
                //We clicked to move (if it's the same ID as last bottle throw then player did not intend to move)
                else if (touch.Id != lastBottleThrowTapId)
                {
                    AddPlayerGoToIntent(touch);
                    foundMovement = true;
                }
            }
            if (!foundMovement)
                player.ResetIntent();
        }

        protected virtual void AddPlayerGoToIntent(TouchLocation touch)
        {
            var screenRectLeft = new Rectangle(0, 0, (int)(game.WindowSize.X / 2), (int)game.WindowSize.Y);
            if (screenRectLeft.Contains(touch.Position))
                player.AddIntent(new GoLeft(inputManager, Camera, player));
            else
                player.AddIntent(new GoRight(inputManager, Camera, player));
        }

        private void ThrowBottle()
        {
            if (player.HasBottle && !Completed && !GameOver)
            {
                var texture = content.Load<Texture2D>("Textures/Bottle");
                bool throwingLeft = player.SpriteEffects == SpriteEffects.None ? false : true;
                var bottle = new Bottle(texture, new Vector2(0.5f, 0.5f), throwingLeft)
                {
                    Position = throwingLeft ? new Vector2(player.Rectangle.Left, player.Position.Y + 10) : new Vector2(player.Rectangle.Right, player.Position.Y + 10),
                    Origin = new Vector2(texture.Width / 2, texture.Height / 2),
                    Rotation = 0.349066f,
                };
                physicsManager.AddMoveableBody(bottle);
                gameComponents.Add(bottle);
                player.HasBottle = false;
            }
        }

        protected virtual void AddHighscore()
        {
            HighScoresStorage.Instance.AddTime(new Score(levelId, StageTimer.Interval - StageTimer.CurrentInterval));
        }

        private void ShowTimeIsUpGameOverComponents()
        {
            foreach (var c in timeIsUpComponents)
                if (c is IDrawableComponent drawable)
                    drawable.Hidden = false;
        }

        private void ShowPlayerSpottedGameOverComponents()
        {
            foreach (var c in playerSpottedComponents)
                if (c is IDrawableComponent drawable)
                    drawable.Hidden = false;
        }

        protected bool PlayerSpotted()
        {
            return physicsManager.Spotted(player);
        }

        private void UpdateTimerSize()
        {
            timer.Scale = new Vector2((float)(timerScale * (StageTimer.CurrentInterval / LevelTimeInSeconds)), timer.Scale.Y);
        }

        protected override void DrawToScreen()
        {
            mapBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            mapBatch.Draw(mapLayerRenderTarget, new Rectangle(0, 0, (int)game.WindowSize.X, (int)game.WindowSize.Y), AmbientColor);
            mapBatch.End();
            base.DrawToScreen();
        }

        protected override void DrawToRenderTarget(GameTime gameTime)
        {
            mapBatch.Begin(transformMatrix: Camera.ViewMatrix);
            graphicsDevice.SetRenderTarget(mapLayerRenderTarget);
            //Important -> removes weird lines
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            graphicsDevice.Clear(Color.Black);
            mapRenderer.Draw(Camera.ViewMatrix);
            foreach(var c in gameComponents)
            {
                if (c is IDrawableComponent drawable)
                    drawable.Draw(gameTime, mapBatch);
            }
            mapBatch.End();
            graphicsDevice.SetRenderTarget(null);
            base.DrawToRenderTarget(gameTime);
        }

        protected void SpawnPatrollingGangster(Vector2 position, float idleTimeSeconds = 3.5f, bool startPatrollingToleft = true)
        {
            var texture = content.Load<Texture2D>("Enemies/Gangster/Spritesheet");
            var map = content.Load<Dictionary<string, Rectangle>>("Enemies/Gangster/Map");
            var gangster = new Gangster(texture, map, new Vector2(1f, 1f), player);
            gangster.Position = position;
            gangster.CurrentStrategy = new PatrollingStrategy(gangster, position.X - 150f, position.X + 150f, idleTimeSeconds, startPatrollingToleft);
            gangster.PatrollingSprite = new Sprite(content.Load<Texture2D>("Enemies/Triangle"), new Vector2(0.5f, 0.8f))
            {
                Color = Color.DarkGreen * 0.5f,
            };
            gangster.QuestionMark = new Sprite(content.Load<Texture2D>("Enemies/Questionmark"))
            {
                Hidden = true,
            };
            gameComponents.Add(gangster);
            physicsManager.AddMoveableBody(gangster);
        }

        private List<Rectangle> GetCollisionRectangles()
        {
            TiledMapObjectLayer collisionLayer = map.GetLayer<TiledMapObjectLayer>("Collision");
            TiledMapObject[] collisionObjects = collisionLayer.Objects;
            List<Rectangle> rectangles = new List<Rectangle>();
            foreach (var collisionObject in collisionObjects)
            {
                rectangles.Add(new Rectangle((int)collisionObject.Position.X, (int)collisionObject.Position.Y, (int)collisionObject.Size.Width, (int)collisionObject.Size.Height));
            }
            return rectangles;
        }

        protected List<Rectangle> ReadHidingSpotsFromMap()
        {
            TiledMapObjectLayer collisionLayer = map.GetLayer<TiledMapObjectLayer>("HidingSpots");
            TiledMapObject[] collisionObjects = collisionLayer.Objects;
            List<Rectangle> rectangles = new List<Rectangle>();
            foreach (var collisionObject in collisionObjects)
            {
                rectangles.Add(new Rectangle((int)collisionObject.Position.X, (int)collisionObject.Position.Y, (int)collisionObject.Size.Width, (int)collisionObject.Size.Height));
            }
            return rectangles;
        }

        protected void AddGoToArrowDown(Vector2 position)
        {
            var arrowTexture = content.Load<Texture2D>("Textures/GoArrow");
            var arrow = new Sprite(arrowTexture)
            {
                Rotation = 1.5708f,
                Origin = new Vector2(arrowTexture.Width / 2, arrowTexture.Height / 2),
            };
            arrow.Position = new Vector2(position.X, position.Y - arrow.Size.Y);
            arrow.AddSpecialEffect(new JumpingEffect());
            gameComponents.Add(arrow);
        }

        protected void AddGoToArrowRight(Vector2 position)
        {
            var arrowTexture = content.Load<Texture2D>("Textures/GoArrow");
            var arrow = new Sprite(arrowTexture)
            {
                Origin = new Vector2(arrowTexture.Width / 2, arrowTexture.Height / 2),
            };
            arrow.Position = new Vector2(position.X, position.Y - arrow.Size.Y);
            arrow.AddSpecialEffect(new JumpingEffect());
            gameComponents.Add(arrow);
        }

        protected void SpawnBottlePickUp(Vector2 position)
        {
            var bottle = new BottlePickUp(this, content.Load<Texture2D>("Textures/Bottle"), new Vector2(0.5f, 0.5f))
            {
                Position = position,
            };

            var pickUpArrow = new PickUpArrow(content.Load<Texture2D>("PickUpArrow/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("PickUpArrow/Map"), Vector2.One);
            pickUpArrow.Position = new Vector2(bottle.CollisionRectangle.Center.X - pickUpArrow.Size.X / 2 + 2f, bottle.Position.Y - pickUpArrow.Size.Y - 3);
            pickUpArrow.AddSpecialEffect(new JumpingEffect());
            bottle.PickUpArrow = pickUpArrow;

            gameComponents.Add(bottle);
            physicsManager.AddMoveableBody(bottle);
        }

        public void PickUpBottle()
        {
            foreach (var c in pickUpComponents)
                c.Hidden = false;
            var hideTimer = new GameTimer(3f);
            hideTimer.OnTimedEvent = (o, e) =>
            {
                foreach (var c in pickUpComponents)
                    c.Hidden = true;
                hideTimer.Enabled = false;
            };
            gameComponents.Add(hideTimer);
        }
    }
}
