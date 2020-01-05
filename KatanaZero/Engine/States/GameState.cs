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
        /// <summary>
        /// Determines where the floor level is (in pixels)
        /// </summary>
        protected int floorLevel;
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
            player.Position = new Vector2(10, 350/*floorLevel - player.Size.Y*/);
            player.KatanaSlash = new AnimatedObject(content.Load<Texture2D>("Character/Katana/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Character/Katana/Map"), new Vector2(1f, 1f))
            {
                Hidden = true,
            };
            player.KatanaSlash.AddAnimation("Slash", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6 }, frameDuration: 0.05f));
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
            //base.DrawToScreen();
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

        protected void SpawnGangster(Vector2 position)
        {
            var texture = content.Load<Texture2D>("Enemies/Gangster/Spritesheet");
            var map = content.Load<Dictionary<string, Rectangle>>("Enemies/Gangster/Map");
            var gangster = new Gangster(texture, map, new Vector2(1f, 1f), player);
            gangster.Position = position;
            gangster.CurrentStrategy = new PatrollingStrategy(gangster, position.X - 150f, position.X + 150f);
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
