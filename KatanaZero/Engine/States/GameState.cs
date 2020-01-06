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
        protected List<IComponent> stageClearComponents = new List<IComponent>();
        protected List<IComponent> timeIsUpComponents = new List<IComponent>();
        protected List<IComponent> playerSpottedComponents = new List<IComponent>();
        protected double levelTimeInSeconds = 100;
        protected GameTimer stageTimer;
        private readonly float timerScale = 2.5f;
        private Sprite timer;
        private bool gameOver;

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
                        //Hide all intents
                        foreach (var c in gameComponents)
                            if (c is Intent intent)
                                intent.Hidden = true;
                    }
                }
            }
        }

        public GameState(Game1 gameReference) : base(gameReference)
        {
            mapBatch = new SpriteBatch(graphicsDevice);
            mapLayerRenderTarget = new RenderTarget2D(graphicsDevice, (int)game.LogicalSize.X, (int)game.LogicalSize.Y);
            LoadMap();
            CreateMapRenderer();
            physicsManager = new PhysicsManager();
            CreatePlayer();
            CreateCamera(gameReference);
            AddStageClearComponents();
            AddGameOverComponents();
            AddTimeIsUpComponents();
            AddHud();
            CreateLevelTimer();
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
            player.Position = new Vector2(10, 375/*floorLevel - player.Size.Y*/);
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

        private void AddStageClearComponents()
        {
            var goToArrow = new TextureButton(inputManager, commonTextures["GoArrow"], new Vector2(3f, 3f));
            goToArrow.Position = new Vector2(game.LogicalSize.X - goToArrow.Size.X, floorLevel - 2 * goToArrow.Size.Y);
            goToArrow.OnClick += (o, e) => player.AddIntent(new GoToIntent(inputManager, camera, player, goToArrow.Rectangle));
            stageClearComponents.Add(goToArrow);

            var goToText = new TextureButton(inputManager, commonTextures["GoText"], new Vector2(2.5f, 2.5f));
            goToText.Position = new Vector2(goToArrow.Position.X + goToArrow.Size.X/2 - goToText.Size.X/2, goToArrow.Position.Y - goToText.Size.Y);
            goToText.OnClick += (o, e) => player.AddIntent(new GoToIntent(inputManager, camera, player, goToText.Rectangle));
            stageClearComponents.Add(goToText);
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
                foreach (var c in stageClearComponents)
                    c.Update(gameTime);
            }
            stageTimer?.Update(gameTime);
            UpdateTimerSize();
            if(GameOver)
            {
                player.Color = Color.Red;
                if (inputManager.AnyTapDetected())
                {
                    Type type = this.GetType();
                    game.ChangeState((GameState)Activator.CreateInstance(type, game));
                }
            }
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
