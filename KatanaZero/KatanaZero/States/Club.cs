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
            SpawnCrowdGroupOne();
            SpawnCrowdGroupTwo();
            SpawnCrowdGroupThree();
            SpawnCrowdGroupFour();
            SpawnCrowdGroupFive();
            SpawnCrowdGroupSix();

            SpawnPatrollingGangster(new Vector2(475, 350));
            SpawnPatrollingGangster(new Vector2(650, 350), 4.5f, false);
            SpawnPatrollingGangster(new Vector2(920, 350), 6.5f);

            TeleportToSecondFloor();
            SpawnCrowdGroupSeven();
            SpawnCrowdGroupEight();
            SpawnCrowdGroupNine();
            SpawnCrowdGroupTen();
            AddEndLevelIntent();

            SpawnPatrollingGangster(new Vector2(860, 220));
            SpawnPatrollingGangster(new Vector2(650, 220), 4.5f, false);
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
            SpawnGuy2(new Vector2(870, 350));
            SpawnGuy2(new Vector2(890, 350));

            Rectangle intentRectangle = new Rectangle(870, 400, 50, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void SpawnCrowdGroupSix()
        {
            SpawnGirl1(new Vector2(1000, 350));
            SpawnGuy2(new Vector2(1015, 350));
            SpawnGuy1(new Vector2(1035, 350));
            SpawnGirl1(new Vector2(1050, 350), SpriteEffects.FlipHorizontally);
            SpawnGirl1(new Vector2(1063, 350), SpriteEffects.FlipHorizontally);

            Rectangle intentRectangle = new Rectangle(1000, 400, 95, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void TeleportToSecondFloor()
        {
            Rectangle intentRectangle = new Rectangle(1215, 400, 35, 50);
            TeleportIntent teleportIntent = new TeleportIntent(inputManager, camera, player, intentRectangle, new Vector2(1215, 220))
            {
                OnFinished = (o, e) => camera.MultiplierOriginX = 0.75f
            };
            gameComponents.Add(teleportIntent);
        }

        private void SpawnCrowdGroupSeven()
        {
            SpawnGirl1(new Vector2(940, 220));
            SpawnGirl2(new Vector2(950, 220));
            SpawnGirl1(new Vector2(980, 220));

            Rectangle intentRectangle = new Rectangle(940, 210, 70, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void SpawnCrowdGroupEight()
        {
            SpawnGirl1(new Vector2(690, 220));
            SpawnGuy2(new Vector2(705, 220));
            SpawnGuy1(new Vector2(725, 220));
            SpawnGirl1(new Vector2(740, 220), SpriteEffects.FlipHorizontally);
            SpawnGirl1(new Vector2(753, 220), SpriteEffects.FlipHorizontally);

            Rectangle intentRectangle = new Rectangle(690, 210, 95, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void SpawnCrowdGroupNine()
        {
            SpawnGuy2(new Vector2(580, 220));
            SpawnGuy1(new Vector2(560, 220));

            Rectangle intentRectangle = new Rectangle(560, 210, 50, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void SpawnCrowdGroupTen()
        {
            SpawnGirl2(new Vector2(365, 220), SpriteEffects.FlipHorizontally);
            SpawnGirl1(new Vector2(340, 220));

            Rectangle intentRectangle = new Rectangle(340, 210, 50, 50);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle);
            gameComponents.Add(goToIntent);
        }

        private void AddEndLevelIntent()
        {
            Rectangle intentRectangle = new Rectangle(222, 208, 37, 49);
            GoToIntent goToIntent = new GoToIntent(inputManager, camera, player, intentRectangle)
            {
                OnFinished = (o,e) => game.ChangeState(new MainMenu(game))
            };
            gameComponents.Add(goToIntent);
        }
    }
}