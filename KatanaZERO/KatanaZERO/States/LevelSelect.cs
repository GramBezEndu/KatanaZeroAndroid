namespace KatanaZERO.States
{
    using System;
    using System.Collections.Generic;
    using Engine;
    using Engine.Controls;
    using Engine.Controls.Buttons;
    using Engine.SpecialEffects;
    using Engine.Sprites;
    using Engine.States;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class LevelSelect : State
    {
        private readonly Text currentLevelName;

        private readonly Sprite currentLevelImg;

        private readonly RectangleButton rightLevelButton;

        private readonly RectangleButton leftLevelButton;

        private readonly VerticalNavigationMenu menu;

        private int currentLevelSelected = 0;

        public LevelSelect(Game1 gameReference)
            : base(gameReference)
        {
            AddUiComponent(new Sprite(Content.Load<Texture2D>("Textures/LevelSelect/LevelSelectBackground")));
            currentLevelImg = new Sprite(LevelsInfo.LevelInfo[currentLevelSelected].Texture)
            {
                Position = new Vector2(455, 45),
            };
            AddUiComponent(currentLevelImg);

            AddUiComponent(new Sprite(Content.Load<Texture2D>("Textures/LevelSelect/LevelSelectForeground")));

            RectangleButton playButton = new RectangleButton(InputManager, new Rectangle(0, 0, (int)(Game.LogicalSize.X * 0.5f), (int)Game.LogicalSize.Y / 10), Fonts["Standard"], "PLAY")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            playButton.OnClick += (o, e) => StartLevel(currentLevelSelected);

            RectangleButton backButton = new RectangleButton(InputManager, new Rectangle(0, 0, (int)(Game.LogicalSize.X * 0.5f), (int)Game.LogicalSize.Y / 10), Fonts["Standard"], "BACK")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            backButton.OnClick += (o, e) => Game.ChangeState(new MainMenu(Game));
            menu = new VerticalNavigationMenu(InputManager, new List<IButton>
            {
                playButton,
                backButton,
            });
            menu.Position = new Vector2((Game.LogicalSize.X / 2) - (menu.Size.X / 2), (Game.LogicalSize.Y * 0.835f) - (menu.Size.Y / 2));

            currentLevelName = new Text(Fonts["Standard"], LevelsInfo.LevelInfo[currentLevelSelected].Name);
            int margin = 30;
            currentLevelName.Position = new Vector2(menu.Rectangle.Center.X - (currentLevelName.Size.X / 2), menu.Rectangle.Top - currentLevelName.Size.Y - margin);
            currentLevelName.AddSpecialEffect(new RainbowEffect());

            DrawableRectangle backgroundMenu = new DrawableRectangle(new Rectangle(0, 0, (int)(menu.Size.X * 1.1f), (int)((menu.Size.Y + currentLevelName.Size.Y + margin) * 1.2f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            backgroundMenu.Position = new Vector2(menu.Position.X - (0.05f * menu.Size.X), currentLevelName.Position.Y - (0.1f * (menu.Size.Y + currentLevelName.Size.Y)));

            rightLevelButton = new RectangleButton(InputManager, new Rectangle(0, 0, 80, 80), Fonts["Standard"], ">");
            rightLevelButton.OnClick += IncreaseSelectedLevel;
            rightLevelButton.Position = new Vector2(menu.Rectangle.Right - rightLevelButton.Size.X, currentLevelName.Rectangle.Center.Y - (rightLevelButton.Size.Y / 2));

            leftLevelButton = new RectangleButton(InputManager, new Rectangle(0, 0, 80, 80), Fonts["Standard"], "<");
            leftLevelButton.OnClick += DecreaseSelectedLevel;
            leftLevelButton.Position = new Vector2(menu.Rectangle.Left, currentLevelName.Rectangle.Center.Y - (rightLevelButton.Size.Y / 2));

            AddUiComponent(backgroundMenu);
            AddUiComponent(leftLevelButton);
            AddUiComponent(rightLevelButton);
            AddUiComponent(menu);
            AddUiComponent(currentLevelName);
        }

        private void StartLevel(int currentLevelSelected)
        {
            LevelsInfo.LevelInfo[currentLevelSelected].StartLevel();
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

            // Center text
            currentLevelName.Position = new Vector2(menu.Rectangle.Center.X - (currentLevelName.Size.X / 2), currentLevelName.Position.Y);
            currentLevelImg.Texture = LevelsInfo.LevelInfo[currentLevelSelected].Texture;
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
    }
}