using Engine.Sprites.Enemies;
using Engine.Sprites;
using KatanaZero;
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
using KatanaZero.States;
using Microsoft.Xna.Framework.Media;
using System.Text.RegularExpressions;
using Engine.Controls;

namespace Engine.States
{
    public abstract class GameState : State
    {
        protected TiledMap map;
        protected TiledMapRenderer mapRenderer;
        protected SpriteBatch mapBatch;
        protected RenderTarget2D mapLayerRenderTarget;
        protected Camera camera;
        protected Player player;
        protected PhysicsManager physicsManager;
        protected List<IComponent> gameComponents = new List<IComponent>();
        //protected List<IComponent> gameCharacters = new List<IComponent>();
        protected List<IComponent> levelCompleteComponents = new List<IComponent>();
        protected List<IComponent> timeIsUpComponents = new List<IComponent>();
        protected List<IComponent> playerSpottedComponents = new List<IComponent>();
        protected List<IComponent> levelTitleComponents = new List<IComponent>();
        protected double levelTimeInSeconds = 120;
        protected GameTimer stageTimer;
        private readonly float timerScale = 2.5f;
        private Sprite timer;
        private bool gameOver;

        public EventHandler OnCompleted { get; set; }
        private bool completed;

        /// <summary>
        /// Determines where the floor level is (in pixels)
        /// </summary>
        protected int floorLevel;

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
                        State.Sounds["LevelFail"].Play();
                        //Hide all intents
                        foreach (var c in gameComponents)
                            if (c is Intent intent)
                                intent.Hidden = true;
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
                    if(completed == true)
                        OnCompleted?.Invoke(this, new EventArgs());
                }
            }
        }

        public GameState(Game1 gameReference, bool showLevelTitle) : base(gameReference)
        {
            mapBatch = new SpriteBatch(graphicsDevice);
            mapLayerRenderTarget = new RenderTarget2D(graphicsDevice, (int)game.LogicalSize.X, (int)game.LogicalSize.Y);
            LoadMap();
            CreateMapRenderer();
            physicsManager = new PhysicsManager();
            CreatePlayer();
            CreateCamera(gameReference);
            AddGameOverComponents();
            AddTimeIsUpComponents();
            AddHud();
            CreateLevelTimer();
            if(showLevelTitle)
                CreateLevelTitleComponents();
            OnCompleted += (o, e) => AddLevelCompleteComponents();
            OnCompleted += (o, e) => ShowStageClearComponents();
            OnCompleted += (o, e) => AddHighscore();
        }

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

            var levelTitle = new Text(fonts["Big"], Regex.Replace(this.GetType().Name, "([a-z])([A-Z])", "$1 $2"));
            levelTitle.Position = new Vector2(game.LogicalSize.X / 2 - levelTitle.Size.X / 2,
                game.LogicalSize.Y / 2 - levelTitle.Size.Y / 2);

            var backgroundText = new DrawableRectangle(new Rectangle(0, 0, (int)(game.LogicalSize.X), (int)(levelTitle.Size.Y * 1.4f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            //TODO: Need refactoring
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

        private void CreateLevelTimer()
        {
            stageTimer = new GameTimer(levelTimeInSeconds)
            {
                OnTimedEvent = (o, e) =>
                {
                    GameOver = true;
                    timer.Hidden = true;
                    ShowTimeIsUpGameOverComponents();
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
            camera = new Camera(gameReference, player);
            gameComponents.Add(camera);
        }

        protected void AddMoveableBody(ICollidable body)
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
            player.Position = new Vector2(10, 375);
            //player.Position = new Vector2(200, 125);
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
            physicsManager.AddMoveableBody(player);
        }

        private void AddLevelCompleteComponents()
        {
            //var goToArrow = new TextureButton(inputManager, commonTextures["GoArrow"], new Vector2(3f, 3f));
            //goToArrow.Position = new Vector2(game.LogicalSize.X - goToArrow.Size.X, floorLevel - 2 * goToArrow.Size.Y);
            //goToArrow.OnClick += (o, e) => player.AddIntent(new GoToIntent(inputManager, camera, player, goToArrow.Rectangle));
            //stageClearComponents.Add(goToArrow);

            //var goToText = new TextureButton(inputManager, commonTextures["GoText"], new Vector2(2.5f, 2.5f));
            //goToText.Position = new Vector2(goToArrow.Position.X + goToArrow.Size.X/2 - goToText.Size.X/2, goToArrow.Position.Y - goToText.Size.Y);
            //goToText.OnClick += (o, e) => player.AddIntent(new GoToIntent(inputManager, camera, player, goToText.Rectangle));
            //stageClearComponents.Add(goToText);
            var levelCompleteText = new Text(fonts["Big"], "LEVEL COMPLETE")
            {
                Hidden = true
            };
            levelCompleteText.Position = new Vector2(game.LogicalSize.X / 2 - levelCompleteText.Size.X / 2, game.LogicalSize.Y / 2 - levelCompleteText.Size.Y / 2);

            var timeText = new Text(fonts["Standard"], String.Format("TIME {0}s.", Math.Round(stageTimer.Interval - stageTimer.CurrentInterval, 2).ToString()))
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
            //TODO: Need refactoring
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
            physicsManager.SetStaticBodies(GetCollisionRectangles());
            physicsManager.Update(gameTime);
            if(PlayerSpotted())
            {
                GameOver = true;
                ShowPlayerSpottedGameOverComponents();
                //player.Hidden = true;
            }
            camera.Update(gameTime);
            mapRenderer.Update(gameTime);
            foreach (var c in gameComponents)
                c.Update(gameTime);
            base.Update(gameTime);
            //base.Update(gameTime);
            //foreach (var c in gameCharacters)
            //    c.Update(gameTime);
            if (StageClear())
            {
                foreach (var c in levelCompleteComponents)
                    c.Update(gameTime);
            }
            if(!GameOver)
                stageTimer?.Update(gameTime);
            UpdateTimerSize();
            if (GameOver)
            {
                player.Color = Color.Red;
                if (inputManager.AnyTapDetected())
                {
                    Type type = this.GetType();
                    game.ChangeState((GameState)Activator.CreateInstance(type, game, false));
                }
            }
        }

        protected abstract void AddHighscore();

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
            timer.Scale = new Vector2((float)(timerScale * (stageTimer.CurrentInterval / levelTimeInSeconds)), timer.Scale.Y);
        }

        protected bool StageClear()
        {
            //foreach(var c in gameCharacters)
            //{
            //    if(c is Enemy enemy)
            //    {
            //        if (enemy.IsDead == false)
            //            return false;
            //    }
            //}
            //return true;
            return false;
        }

        protected override void DrawToScreen()
        {
            mapBatch.Begin(/*transformMatrix: camera.ViewMatrix*/SpriteSortMode.Immediate, BlendState.Opaque);
            mapBatch.Draw(mapLayerRenderTarget, new Rectangle(0, 0, (int)game.WindowSize.X, (int)game.WindowSize.Y), Color.White);
            mapBatch.End();
            base.DrawToScreen();
        }

        protected override void DrawToRenderTarget(GameTime gameTime)
        {
            //graphicsDevice.SetRenderTarget(gameLayerRenderTarget);
            //gameBatch.Begin(transformMatrix: camera.ViewMatrix);
            //graphicsDevice.Clear(Color.Black);
            //foreach(var c in gameComponents)
            //{
            //    if (c is IDrawableComponent drawable)
            //        drawable.Draw(gameTime, gameBatch);
            //}
            //foreach (var c in gameCharacters)
            //{
            //    if (c is IDrawableComponent drawable)
            //        drawable.Draw(gameTime, gameBatch);
            //}
            //if (StageClear())
            //{
            //    foreach (var c in stageClearComponents)
            //        if(c is IDrawableComponent drawable)
            //            drawable.Draw(gameTime, gameBatch);
            //}
            //gameBatch.End();
            //graphicsDevice.SetRenderTarget(null);

            //base.DrawToRenderTarget(gameTime);

            mapBatch.Begin(transformMatrix: camera.ViewMatrix/*SpriteSortMode.Immediate, BlendState.Opaque*/);
            graphicsDevice.SetRenderTarget(mapLayerRenderTarget);
            graphicsDevice.Clear(Color.Black);
            mapRenderer.Draw(camera.ViewMatrix);
            foreach(var c in gameComponents)
            {
                if (c is IDrawableComponent drawable)
                    drawable.Draw(gameTime, mapBatch);
            }
            player.Draw(gameTime, mapBatch);
            mapBatch.End();
            graphicsDevice.SetRenderTarget(null);
            base.DrawToRenderTarget(gameTime);
        }

        protected void SpawnOfficer(Vector2 position, string startingAnimation = "Idle")
        {
            var texture = content.Load<Texture2D>("Enemies/Officer/Spritesheet");
            var map = content.Load < Dictionary < string, Rectangle>>("Enemies/Officer/Map");
            var officer = new Officer(texture, map, new Vector2(1f, 1f), player);
            officer.PlayAnimation(startingAnimation);
            officer.Position = position;
            //gameCharacters.Add(officer);
            //TODO: Finish after rework
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
    }
}
