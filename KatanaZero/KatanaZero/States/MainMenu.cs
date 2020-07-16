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
using Engine.States;
using Engine;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Engine.Controls.Buttons;
using Engine.Controls;
using KatanaZero.SpecialEffects;

namespace KatanaZero.States
{
    public class MainMenu : State
    {
        public MainMenu(Game1 gameReference) : base(gameReference)
        {
            if(!game.IsThisSongPlaying(songs["MainMenu"]))
                game.PlaySong(songs["MainMenu"]);
            AddUiComponent(new Sprite(commonTextures["MainMenu"]));
            AddUiComponent(new Rain());
            var playButton = new RectangleButton(inputManager, new Rectangle(0, 0, (int)(game.LogicalSize.X * 0.5f), (int)game.LogicalSize.Y / 10), fonts["Standard"], "PLAY")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            playButton.OnClick += (o, e) => game.ChangeState(/*new ClubNeon(game, true)*/new LevelSelect(game));
            var rankingButton = new RectangleButton(inputManager, new Rectangle(0, 0, (int)(game.LogicalSize.X * 0.5f), (int)game.LogicalSize.Y / 10), fonts["Standard"], "HIGHSCORES")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            rankingButton.OnClick += (o, e) => game.ChangeState(new Highscores(game));
            var menu = new VerticalNavigationMenu(inputManager, new List<IButton>
                {
                    playButton,
                    rankingButton,
                });
            menu.Position = new Vector2(game.LogicalSize.X / 2 - menu.Size.X / 2, game.LogicalSize.Y * (4/5f) - menu.Size.Y / 2);
            var backgroundMenu = new DrawableRectangle(new Rectangle(0, 0, (int)(menu.Size.X * 1.1f), (int)(menu.Size.Y * 1.4f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            backgroundMenu.Position = new Vector2(menu.Position.X - 0.05f * menu.Size.X, menu.Position.Y - 0.2f * menu.Size.Y);
            AddUiComponent(backgroundMenu);
            AddUiComponent(menu);
        }
    }
}