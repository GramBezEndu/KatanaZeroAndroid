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
using Engine.PlayerIntents;
using Engine.Sprites;
using Engine.Sprites.Crowd;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace KatanaZero.States
{
    public class Club : GameState
    {
        public Club(Game1 gameReference) : base(gameReference)
        {
            game.PlaySong(songs["Club"]);
            //SpawnPoliceCar();
            //SpawnOfficer(game.LogicalSize.X *0.68f, "Game");
            //gameCharacters.Add(player);
            SpawnCrowdGroupOne();
            SpawnCrowdGroupTwo();
            SpawnCrowdGroupThree();
            SpawnCrowdGroupFour();
            SpawnCrowdGroupFive();
        }

        protected override void LoadMap()
        {
            map = content.Load<TiledMap>("Maps/Club");
        }

        private void SpawnPoliceCar()
        {
            var car = new Sprite(commonTextures["PoliceCar"], new Vector2(3f, 3f));
            car.Position = new Vector2(game.LogicalSize.X/2, floorLevel - car.Size.Y);
            gameComponents.Add(car);
        }

        private void SpawnGirl1(Vector2 position, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            Girl1 girl = new Girl1(content.Load<Texture2D>("Crowd/Girl1/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Crowd/Girl1/Map"), Vector2.One);
            girl.Position = position;
            girl.SpriteEffects = spriteEffects;
            this.AddMoveableBody(girl);
        }

        private void SpawnGirl2(Vector2 position, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            Girl2 girl = new Girl2(content.Load<Texture2D>("Crowd/Girl2/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Crowd/Girl2/Map"), Vector2.One);
            girl.Position = position;
            girl.SpriteEffects = spriteEffects;
            this.AddMoveableBody(girl);
        }

        private void SpawnGuy1(Vector2 position, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            Guy1 guy = new Guy1(content.Load<Texture2D>("Crowd/Guy1/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Crowd/Guy1/Map"), Vector2.One);
            guy.Position = position;
            guy.SpriteEffects = spriteEffects;
            this.AddMoveableBody(guy);
        }

        private void SpawnGuy2(Vector2 position, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            Guy2 guy = new Guy2(content.Load<Texture2D>("Crowd/Guy2/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Crowd/Guy2/Map"), Vector2.One);
            guy.Position = position;
            guy.SpriteEffects = spriteEffects;
            this.AddMoveableBody(guy);
        }

        private void SpawnCrowdGroupOne()
        {
            SpawnGirl1(new Vector2(240, 350));
            SpawnGuy2(new Vector2(255, 350));
            SpawnGuy1(new Vector2(275, 350));
            SpawnGirl1(new Vector2(290, 350), SpriteEffects.FlipHorizontally);
            SpawnGirl1(new Vector2(303, 350), SpriteEffects.FlipHorizontally);

            Rectangle intentRectangle = new Rectangle(240, 400, 95, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void SpawnCrowdGroupTwo()
        {
            SpawnGirl2(new Vector2(470, 350));
            SpawnGirl2(new Vector2(495, 350), SpriteEffects.FlipHorizontally);

            Rectangle intentRectangle = new Rectangle(475, 400, 50, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void SpawnCrowdGroupThree()
        {
            SpawnGuy2(new Vector2(570, 350));
            SpawnGirl1(new Vector2(590, 350));

            Rectangle intentRectangle = new Rectangle(570, 400, 50, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void SpawnCrowdGroupFour()
        {
            SpawnGirl2(new Vector2(675, 350), SpriteEffects.FlipHorizontally);
            SpawnGirl1(new Vector2(695, 350));

            Rectangle intentRectangle = new Rectangle(675, 400, 50, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void SpawnCrowdGroupFive()
        {
            SpawnGuy2(new Vector2(880, 350));
            SpawnGuy2(new Vector2(900, 350));

            Rectangle intentRectangle = new Rectangle(880, 400, 50, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }
    }
}