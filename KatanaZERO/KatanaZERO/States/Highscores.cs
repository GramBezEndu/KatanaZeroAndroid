namespace KatanaZERO.States
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Engine;
    using Engine.Controls;
    using Engine.Controls.Buttons;
    using Engine.SpecialEffects;
    using Engine.Sprites;
    using Engine.States;
    using Engine.Storage;
    using KatanaZERO.SpecialEffects;
    using Microsoft.Xna.Framework;

    public class Highscores : State
    {
        private readonly Text currentLevelName;

        private readonly RectangleButton rightLevelButton;

        private readonly RectangleButton leftLevelButton;

        private readonly VerticalNavigationMenu menu;

        private readonly Sprite currentLevelImg;

        private int currentLevelSelected = 0;

        private Text levelTextSmall;

        private Text noDataFound;

        private Text bestTimesText;

        private List<Text> currentHighScores = new List<Text>();

        public Highscores(Game1 gameReference)
            : base(gameReference)
        {
            AddUiComponent(new Sprite(Textures["MainMenu"]));
            AddUiComponent(new Rain());

            RectangleButton backButton = new RectangleButton(InputManager, new Rectangle(0, 0, (int)(Game.LogicalSize.X * 0.5f), (int)Game.LogicalSize.Y / 10), Fonts["Standard"], "BACK")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            backButton.OnClick += (o, e) => Game.ChangeState(new MainMenu(Game));
            menu = new VerticalNavigationMenu(InputManager, new List<IButton>
            {
                backButton,
            });
            menu.Position = new Vector2((Game.LogicalSize.X / 2) - (menu.Size.X / 2), (Game.LogicalSize.Y * 0.925f) - (menu.Size.Y / 2));

            currentLevelName = new Text(Fonts["Standard"], LevelsInfo.LevelInfo[currentLevelSelected].Name);
            int margin = 30;
            currentLevelName.Position = new Vector2(menu.Rectangle.Center.X - (currentLevelName.Size.X / 2), menu.Rectangle.Top - currentLevelName.Size.Y - margin);
            currentLevelName.AddSpecialEffect(new RainbowEffect());

            rightLevelButton = new RectangleButton(InputManager, new Rectangle(0, 0, 80, 80), Fonts["Standard"], ">");
            rightLevelButton.OnClick += IncreaseSelectedLevel;
            rightLevelButton.Position = new Vector2(menu.Rectangle.Right - rightLevelButton.Size.X, currentLevelName.Rectangle.Center.Y - (rightLevelButton.Size.Y / 2));

            leftLevelButton = new RectangleButton(InputManager, new Rectangle(0, 0, 80, 80), Fonts["Standard"], "<");
            leftLevelButton.OnClick += DecreaseSelectedLevel;
            leftLevelButton.Position = new Vector2(menu.Rectangle.Left, currentLevelName.Rectangle.Center.Y - (rightLevelButton.Size.Y / 2));

            DrawableRectangle backgroundMenu = new DrawableRectangle(new Rectangle(0, 0, (int)(menu.Size.X * 1.1f), (int)((menu.Size.Y + currentLevelName.Size.Y + margin) * 1.2f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            backgroundMenu.Position = new Vector2(menu.Position.X - (0.05f * menu.Size.X), currentLevelName.Position.Y - (0.1f * (menu.Size.Y + currentLevelName.Size.Y)));

            currentLevelImg = new Sprite(LevelsInfo.LevelInfo[currentLevelSelected].Texture, new Vector2(0.382f, 0.53f))
            {
                Position = new Vector2(912, 108),
                Origin = new Vector2(380 / 2, 270 / 2),
                Rotation = 0.0872665f,
            };

            CreateJobFolder();
            AddUiComponent(backgroundMenu);
            AddUiComponent(menu);
            AddUiComponent(currentLevelName);
            AddUiComponent(leftLevelButton);
            AddUiComponent(rightLevelButton);
            AddUiComponent(currentLevelImg);

            OnSelectedChange();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (Text score in currentHighScores)
            {
                score.Update(gameTime);
            }
        }

        protected override void DrawToRenderTarget(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(UiLayerRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            UiSpriteBatch.Begin();
            foreach (IComponent c in UiComponents)
            {
                if (c is IDrawableComponent)
                {
                    (c as IDrawableComponent).Draw(gameTime, UiSpriteBatch);
                }
            }

            foreach (Text score in currentHighScores)
            {
                score.Draw(gameTime, UiSpriteBatch);
            }

            UiSpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        private void CreateNoDataFound(Color colorNoData, Vector2 position)
        {
            noDataFound = new Text(Fonts["Small"], "NO DATA FOUND")
            {
                Position = position,
                Color = colorNoData,
                Hidden = true,
            };
        }

        private void CreateBestTimesText(Color color, Vector2 position)
        {
            bestTimesText = new Text(Fonts["Small"], "BEST TIMES:")
            {
                Position = position,
                Color = color,
            };
        }

        private void AddCurrentLevelHighscores()
        {
            double[] bestScores = HighScoresStorage.Instance.GetBestScores(currentLevelSelected);
            currentHighScores = new List<Text>();
            Vector2 position = new Vector2(bestTimesText.Position.X, bestTimesText.Rectangle.Bottom);
            for (int i = 0; i < bestScores.Length; i++)
            {
                double bestScore = bestScores[i];
                Text text = new Text(Fonts["Small"], string.Format("{0}. {1} s", i + 1, Math.Round(bestScore, 2).ToString()))
                {
                    Position = position,
                    Color = Color.Black,
                };
                currentHighScores.Add(text);
                position = new Vector2(position.X, position.Y + text.Size.Y);
            }
        }

        private void DecreaseSelectedLevel(object sender, EventArgs e)
        {
            currentLevelSelected--;
            if (currentLevelSelected < 0)
            {
                currentLevelSelected = LevelsInfo.LevelInfo.Length - 1;
            }

            OnSelectedChange();
        }

        private void IncreaseSelectedLevel(object sender, EventArgs e)
        {
            currentLevelSelected++;
            if (currentLevelSelected > LevelsInfo.LevelInfo.Length - 1)
            {
                currentLevelSelected = 0;
            }

            OnSelectedChange();
        }

        private void OnSelectedChange()
        {
            currentLevelName.Message = LevelsInfo.LevelInfo[currentLevelSelected].Name;
            ManageRainbowEffect();
            levelTextSmall.Message = Regex.Replace(currentLevelName.Message, @"\s+", "\n");

            // Center text
            currentLevelName.Position = new Vector2(menu.Rectangle.Center.X - (currentLevelName.Size.X / 2), currentLevelName.Position.Y);
            currentLevelImg.Texture = LevelsInfo.LevelInfo[currentLevelSelected].Texture;
            AddCurrentLevelHighscores();
            if (currentHighScores.Count == 0)
            {
                noDataFound.Hidden = false;
            }
            else
            {
                noDataFound.Hidden = true;
            }
        }

        private void ManageRainbowEffect()
        {
            // TODO: refactor (we're relying on index 0)
            if (currentLevelSelected == 0)
            {
                currentLevelName.SpecialEffects[0].Enabled = true;
            }
            else
            {
                currentLevelName.SpecialEffects[0].Enabled = false;
                currentLevelName.Color = Color.White;
            }
        }

        private void CreateJobFolder()
        {
            float jobFolderScale = 2f;
            Sprite folderFront = new Sprite(Textures["JobFolderFrontOpen"], new Vector2(jobFolderScale));
            folderFront.Position = new Vector2(Game.LogicalSize.X * 0.15f, Game.LogicalSize.Y * 0.02f);
            Sprite folderBack = new Sprite(Textures["JobFolderBack"], new Vector2(jobFolderScale));
            folderBack.Position = new Vector2(folderFront.Rectangle.Right, folderFront.Position.Y);
            levelTextSmall = new Text(Fonts["Small"], Regex.Replace(currentLevelName.Message, @"\s+", "\n"))
            {
                Color = Color.Gray,
            };
            levelTextSmall.Position = new Vector2(Game.LogicalSize.X * 0.53f, Game.LogicalSize.Y * 0.1f);

            AddUiComponent(folderFront);
            AddUiComponent(folderBack);
            AddUiComponent(levelTextSmall);
            AddCommonHighscoresComponents();
        }

        private void AddCommonHighscoresComponents()
        {
            Color textColor = Color.Black;
            Color colorNoData = Color.DarkRed;

            CreateBestTimesText(textColor, new Vector2(Game.LogicalSize.X * 0.505f, Game.LogicalSize.Y * 0.35f));
            AddUiComponent(bestTimesText);

            Vector2 position = new Vector2(bestTimesText.Position.X, bestTimesText.Rectangle.Bottom);
            CreateNoDataFound(colorNoData, position);
            AddUiComponent(noDataFound);
        }
    }
}