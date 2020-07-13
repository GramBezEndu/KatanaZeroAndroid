using Engine;
using Engine.SpecialEffects;
using Engine.Sprites;
using Engine.Sprites.Crowd;
using Engine.States;
using Engine.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;

namespace KatanaZero.States
{
    public class ClubNeon : GameState
    {
        Rectangle doorToSecondFloor;
        Rectangle doorLevelEnd;

        public override string LevelName { get { return "CLUB NEON"; } }

        public ClubNeon(Game1 gameReference, bool showLevelTitle) : base(gameReference, showLevelTitle)
        {
            game.PlaySong(songs["Club"]);
            gameComponents.Add(new ClubLights());
            SpawnCrowdGroupOne();
            SpawnCrowdGroupTwo();
            SpawnCrowdGroupThree();
            SpawnCrowdGroupFour();
            SpawnCrowdGroupFive();
            SpawnCrowdGroupSix();

            SpawnPatrollingGangster(new Vector2(475, 350));
            SpawnPatrollingGangster(new Vector2(650, 350), 4.5f, false);
            SpawnPatrollingGangster(new Vector2(920, 350), 6.5f);

            AddDoorToSecondFloor();
            SpawnCrowdGroupSeven();
            SpawnCrowdGroupEight();
            SpawnCrowdGroupNine();
            SpawnCrowdGroupTen();
            AddEndLevelDoor();

            SpawnPatrollingGangster(new Vector2(860, 218));
            SpawnPatrollingGangster(new Vector2(650, 218), 4.5f, false);
        }

        private void TeleportToSecondFloor(object sender, EventArgs e)
        {
            if (!GameOver)
            {
                if (player.Rectangle.Intersects(doorToSecondFloor))
                {
                    player.Position = new Vector2(1215, 220);
                    player.Velocity = new Vector2(0, player.Velocity.Y);
                    player.ResetIntent();
                    camera.MultiplierOriginX = 0.75f;
                    player.SpriteEffects = SpriteEffects.FlipHorizontally;
                }
            }
        }

        private void CheckLevelEnd(object sender, EventArgs e)
        {
            if (!GameOver)
            {
                if (player.Rectangle.Intersects(doorLevelEnd))
                {
                    Completed = true;
                }
            }
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
        }

        private void SpawnCrowdGroupTwo()
        {
            SpawnGirl2(new Vector2(470, 350));
            SpawnGirl2(new Vector2(495, 350), SpriteEffects.FlipHorizontally);
        }

        private void SpawnCrowdGroupThree()
        {
            SpawnGuy2(new Vector2(570, 350));
            SpawnGirl1(new Vector2(590, 350));
        }

        private void SpawnCrowdGroupFour()
        {
            SpawnGirl2(new Vector2(675, 350), SpriteEffects.FlipHorizontally);
            SpawnGirl1(new Vector2(695, 350));
        }

        private void SpawnCrowdGroupFive()
        {
            SpawnGuy2(new Vector2(870, 350));
            SpawnGuy2(new Vector2(890, 350));
        }

        private void SpawnCrowdGroupSix()
        {
            SpawnGirl1(new Vector2(1000, 350));
            SpawnGuy2(new Vector2(1015, 350));
            SpawnGuy1(new Vector2(1035, 350));
            SpawnGirl1(new Vector2(1050, 350), SpriteEffects.FlipHorizontally);
            SpawnGirl1(new Vector2(1063, 350), SpriteEffects.FlipHorizontally);
        }

        private void AddDoorToSecondFloor()
        {
            doorToSecondFloor = new Rectangle(1215, 400, 35, 50);
            var arrowTexture = content.Load<Texture2D>("Textures/GoArrow");
            var arrow = new Sprite(arrowTexture)
            {
                Rotation = 1.5708f,
                Origin = new Vector2(arrowTexture.Width / 2, arrowTexture.Height / 2),
                SpriteEffects = SpriteEffects.FlipVertically,
            };
            arrow.Position = new Vector2(doorToSecondFloor.Center.X, doorToSecondFloor.Y - arrow.Size.Y);
            arrow.AddSpecialEffect(new JumpingEffect());
            gameComponents.Add(arrow);
            gameComponents.Add(new Script()
            {
                OnUpdate = TeleportToSecondFloor,
            });
        }

        private void SpawnCrowdGroupSeven()
        {
            SpawnGirl1(new Vector2(940, 215));
            SpawnGirl2(new Vector2(950, 215));
            SpawnGirl1(new Vector2(980, 215));
        }

        private void SpawnCrowdGroupEight()
        {
            SpawnGirl1(new Vector2(690, 215));
            SpawnGuy2(new Vector2(705, 215));
            SpawnGuy1(new Vector2(725, 215));
            SpawnGirl1(new Vector2(740, 215), SpriteEffects.FlipHorizontally);
            SpawnGirl1(new Vector2(753, 215), SpriteEffects.FlipHorizontally);
        }

        private void SpawnCrowdGroupNine()
        {
            SpawnGuy2(new Vector2(580, 215));
            SpawnGuy1(new Vector2(560, 215));
        }

        private void SpawnCrowdGroupTen()
        {
            SpawnGirl2(new Vector2(365, 215), SpriteEffects.FlipHorizontally);
            SpawnGirl1(new Vector2(340, 215));
        }

        private void AddEndLevelDoor()
        {
            doorLevelEnd = new Rectangle(222, 208, 37, 49);
            var arrowTexture = content.Load<Texture2D>("Textures/GoArrow");
            var arrow = new Sprite(arrowTexture)
            {
                Rotation = 1.5708f,
                Origin = new Vector2(arrowTexture.Width / 2, arrowTexture.Height / 2),
                SpriteEffects = SpriteEffects.FlipVertically,
            };
            arrow.Position = new Vector2(doorLevelEnd.Center.X, doorLevelEnd.Y - arrow.Size.Y);
            arrow.AddSpecialEffect(new JumpingEffect());
            gameComponents.Add(arrow);
            gameComponents.Add(new Script()
            {
                OnUpdate = CheckLevelEnd,
            });
        }

        protected override void AddHighscore()
        {
            HighScoresStorage.Instance.AddTime(new ClubNeonScore(stageTimer.Interval - stageTimer.CurrentInterval));
        }

        internal override Vector2 SetMapSize()
        {
            return new Vector2(1295, 464);
        }

        public override void SetPlayerSpawnPoint()
        {
            player.Position = new Vector2(10, 375);
        }
    }
}