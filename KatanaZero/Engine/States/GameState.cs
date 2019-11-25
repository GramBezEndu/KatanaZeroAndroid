using KatanaZero;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.States
{
    public class GameState : State
    {
        protected SpriteBatch gameBatch;
        protected RenderTarget2D gameLayerRenderTarget;
        protected Camera camera;
        protected Player player;
        protected List<IComponent> gameComponents = new List<IComponent>();
        public GameState(Game1 gameReference) : base(gameReference)
        {
            gameBatch = new SpriteBatch(graphicsDevice);
            gameLayerRenderTarget = new RenderTarget2D(graphicsDevice, (int)game.LogicalSize.X, (int)game.LogicalSize.Y);
            player = new Player(content.Load<Texture2D>("Character/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Character/Map"), inputManager, new Vector2(4f, 4f));
            player.Position = new Vector2(0, gameReference.LogicalSize.Y * (2/3f) - player.Size.Y);
            camera = new Camera(gameReference);
            gameComponents.Add(player);
            gameComponents.Add(camera);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (var c in gameComponents)
                c.Update(gameTime);
        }


        protected override void DrawToScreen()
        {
            gameBatch.Begin(transformMatrix: camera.ViewMatrix/*SpriteSortMode.Immediate, BlendState.Opaque*/);
            gameBatch.Draw(gameLayerRenderTarget, new Rectangle(0, 0, (int)game.WindowSize.X, (int)game.WindowSize.Y), Color.White);
            gameBatch.End();
            //base.DrawToScreen();
        }

        protected override void DrawToRenderTarget(GameTime gameTime)
        {
            graphicsDevice.SetRenderTarget(gameLayerRenderTarget);
            gameBatch.Begin(transformMatrix: camera.ViewMatrix);
            graphicsDevice.Clear(Color.Black);
            player.Draw(gameTime, gameBatch);
            gameBatch.End();
            graphicsDevice.SetRenderTarget(null);
            //base.DrawToRenderTarget(gameTime);
        }
    }
}
