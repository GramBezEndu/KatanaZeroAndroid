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

namespace Engine.States
{
    public class GameState : State
    {
        protected SpriteBatch gameBatch;
        protected RenderTarget2D gameLayerRenderTarget;
        protected Camera camera;
        protected Player player;
        protected List<IComponent> gameComponents = new List<IComponent>();
        protected List<IComponent> gameCharacters = new List<IComponent>();
        protected List<IComponent> stageClearComponents = new List<IComponent>();
        /// <summary>
        /// Determines where the floor level is (in pixels)
        /// </summary>
        protected int floorLevel;
        public GameState(Game1 gameReference) : base(gameReference)
        {
            gameBatch = new SpriteBatch(graphicsDevice);
            gameLayerRenderTarget = new RenderTarget2D(graphicsDevice, (int)game.LogicalSize.X, (int)game.LogicalSize.Y);
            BuildFloor(commonTextures["Floor"]);
            player = new Player(content.Load<Texture2D>("Character/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Character/Map"), inputManager, new Vector2(3f, 3f));
            player.Position = new Vector2(0, floorLevel - player.Size.Y);
            player.KatanaSlash = new AnimatedObject(content.Load<Texture2D>("Character/Katana/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Character/Katana/Map"), new Vector2(3f, 3f))
            {
                Hidden = true,
            };
            player.KatanaSlash.AddAnimation("Slash", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6 }, frameDuration: 0.05f));
            camera = new Camera(gameReference);
            gameComponents.Add(camera);
            AddStageClearComponents();
        }

        private void AddStageClearComponents()
        {
            var goToArrow = new TextureButton(inputManager, commonTextures["GoArrow"], new Vector2(3f, 3f));
            goToArrow.Position = new Vector2(game.LogicalSize.X - goToArrow.Size.X, floorLevel - 2 * goToArrow.Size.Y);
            goToArrow.OnClick += (o, e) => player.AddIntent(new GoToIntent(player, goToArrow.Rectangle));
            stageClearComponents.Add(goToArrow);

            var goToText = new TextureButton(inputManager, commonTextures["GoText"], new Vector2(2.5f, 2.5f));
            goToText.Position = new Vector2(goToArrow.Position.X + goToArrow.Size.X/2 - goToText.Size.X/2, goToArrow.Position.Y - goToText.Size.Y);
            goToText.OnClick += (o, e) => player.AddIntent(new GoToIntent(player, goToText.Rectangle));
            stageClearComponents.Add(goToText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (var c in gameComponents)
                c.Update(gameTime);
            foreach (var c in gameCharacters)
                c.Update(gameTime);
            CharactersOnFloorLevel();
            if(StageClear())
            {
                foreach (var c in stageClearComponents)
                    c.Update(gameTime);
            }
        }
        protected bool StageClear()
        {
            foreach(var c in gameCharacters)
            {
                if(c is Enemy enemy)
                {
                    if (enemy.IsDead == false)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Make sure that all characters are on the floor level
        /// </summary>
        protected void CharactersOnFloorLevel()
        {
            foreach(var c in gameCharacters)
            {
                if(c is IDrawableComponent drawable)
                {
                    if (drawable.Position.Y != floorLevel - drawable.Size.Y)
                        drawable.Position = new Vector2(drawable.Position.X, floorLevel - drawable.Size.Y);
                }
            }
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
            foreach(var c in gameComponents)
            {
                if (c is IDrawableComponent drawable)
                    drawable.Draw(gameTime, gameBatch);
            }
            foreach (var c in gameCharacters)
            {
                if (c is IDrawableComponent drawable)
                    drawable.Draw(gameTime, gameBatch);
            }
            if (StageClear())
            {
                foreach (var c in stageClearComponents)
                    if(c is IDrawableComponent drawable)
                        drawable.Draw(gameTime, gameBatch);
            }
            gameBatch.End();
            graphicsDevice.SetRenderTarget(null);
            //base.DrawToRenderTarget(gameTime);
        }

        protected void SpawnOfficer(float xPosition, string startingAnimation = "Idle")
        {
            var texture = content.Load<Texture2D>("Enemies/Officer/Spritesheet");
            var map = content.Load < Dictionary < string, Rectangle>>("Enemies/Officer/Map");
            var officer = new Officer(texture, map, new Vector2(3f, 3f), inputManager, graphicsDevice, font, player);
            officer.PlayAnimation(startingAnimation);
            officer.Position = new Vector2(xPosition, floorLevel - officer.Size.Y);
            gameCharacters.Add(officer);
        }

        protected void BuildFloor(Texture2D texture)
        {
            //Note: We can not scale.X on sprite while using this method
            Vector2 textureSize = new Vector2(texture.Width, texture.Height);
            floorLevel = (int)game.LogicalSize.Y - texture.Height;
            int repeats = (int)Math.Ceiling(game.LogicalSize.X / texture.Width);
            for(int i=0;i<repeats;i++)
            {
                gameComponents.Add(new Sprite(texture)
                {
                    Position = new Vector2(i * texture.Width, game.LogicalSize.Y - textureSize.Y)
                });
            }
        }
    }
}
