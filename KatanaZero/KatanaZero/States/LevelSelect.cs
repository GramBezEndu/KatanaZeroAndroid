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
using Engine;
using Engine.Controls;
using Engine.Controls.Buttons;
using Engine.Sprites;
using Engine.States;
using KatanaZero.SpecialEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KatanaZero.States
{
    public class LevelSelect : State
    {
        List<LevelInfo> levelInfos = new List<LevelInfo>();
        private int currentLevelSelected = 0;
        private Text currentLevelName;
        Sprite currentLevelImg;
        RectangleButton rightLevelButton;
        RectangleButton leftLevelButton;
        VerticalNavigationMenu menu;

        public LevelSelect(Game1 gameReference) : base(gameReference)
        {
            levelInfos.Add(new LevelInfo("CLUB NEON", content.Load<Texture2D>("Textures/LevelSelect/ClubNeon")));
            levelInfos.Add(new LevelInfo("TEST", content.Load<Texture2D>("Textures/PoliceCar")));

            AddUiComponent(new Sprite(content.Load<Texture2D>("Textures/LevelSelect/LevelSelectBackground")));
            currentLevelImg = new Sprite(levelInfos[currentLevelSelected].Texture)
            {
                Position = new Vector2(455, 45),
            };
            AddUiComponent(currentLevelImg);

            AddUiComponent(new Sprite(content.Load<Texture2D>("Textures/LevelSelect/LevelSelectForeground")));

            var playButton = new RectangleButton(inputManager, new Rectangle(0, 0, (int)(game.LogicalSize.X * 0.5f), (int)game.LogicalSize.Y / 10), fonts["Standard"], "PLAY")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            playButton.OnClick += (o, e) => game.ChangeState(new ClubNeon(game, true));

            var backButton = new RectangleButton(inputManager, new Rectangle(0, 0, (int)(game.LogicalSize.X * 0.5f), (int)game.LogicalSize.Y / 10), fonts["Standard"], "BACK")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            backButton.OnClick += (o, e) => game.ChangeState(new MainMenu(game));
            menu = new VerticalNavigationMenu(inputManager, new List<IButton>
            {
                playButton,
                backButton,
            });
            menu.Position = new Vector2(game.LogicalSize.X / 2 - menu.Size.X / 2, game.LogicalSize.Y * (0.835f) - menu.Size.Y / 2);

            currentLevelName = new Text(fonts["Standard"], levelInfos[currentLevelSelected].Name);
            int margin = 30;
            currentLevelName.Position = new Vector2(menu.Rectangle.Center.X - currentLevelName.Size.X / 2, menu.Rectangle.Top - currentLevelName.Size.Y - margin);

            var backgroundMenu = new DrawableRectangle(new Rectangle(0, 0, (int)((menu.Size.X) * 1.1f), (int)((menu.Size.Y + currentLevelName.Size.Y + margin) * 1.2f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            backgroundMenu.Position = new Vector2(menu.Position.X - 0.05f * (menu.Size.X), currentLevelName.Position.Y - 0.1f * (menu.Size.Y + currentLevelName.Size.Y));

            rightLevelButton = new RectangleButton(inputManager, new Rectangle(0, 0, 80, 80), fonts["Standard"], ">");
            rightLevelButton.OnClick += IncreaseSelectedLevel;
            rightLevelButton.Position = new Vector2(menu.Rectangle.Right - rightLevelButton.Size.X, currentLevelName.Rectangle.Center.Y - rightLevelButton.Size.Y / 2);

            leftLevelButton = new RectangleButton(inputManager, new Rectangle(0, 0, 80, 80), fonts["Standard"], "<");
            leftLevelButton.OnClick += DecreaseSelectedLevel;
            leftLevelButton.Position = new Vector2(menu.Rectangle.Left, currentLevelName.Rectangle.Center.Y - rightLevelButton.Size.Y / 2);

            AddUiComponent(backgroundMenu);
            AddUiComponent(leftLevelButton);
            AddUiComponent(rightLevelButton);
            AddUiComponent(menu);
            AddUiComponent(currentLevelName);
        }

        private void DecreaseSelectedLevel(object sender, EventArgs e)
        {
            currentLevelSelected--;
            if (currentLevelSelected < 0)
                currentLevelSelected = levelInfos.Count - 1;
            OnSelectedChange();
        }

        private void IncreaseSelectedLevel(object sender, EventArgs e)
        {
            currentLevelSelected++;
            if (currentLevelSelected > levelInfos.Count - 1)
                currentLevelSelected = 0;
            OnSelectedChange();
        }

        private void OnSelectedChange()
        {
            currentLevelName.Message = levelInfos[currentLevelSelected].Name;
            //Center text
            currentLevelName.Position = new Vector2(menu.Rectangle.Center.X - currentLevelName.Size.X / 2, currentLevelName.Position.Y);
            currentLevelImg.Texture = levelInfos[currentLevelSelected].Texture;
        }
    }
}