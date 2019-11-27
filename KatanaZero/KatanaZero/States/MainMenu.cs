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

namespace KatanaZero.States
{
    public class MainMenu : State
    {
        public MainMenu(Game1 gameReference) : base(gameReference)
        {
            game.PlaySong(songs["MainMenu"]);
            AddUiComponent(new Sprite(commonTextures["MainMenu"]));
            var playButton = new TextButton(inputManager, font, "PLAY")
            {
                OnClick = (o, e) => game.ChangeState(new Hotel(game))
            };
            var rankingButton = new TextButton(inputManager, font, "RANKINGS");
            var exitButton = new TextButton(inputManager, font, "EXIT");
            var menu = new VerticalNavigationMenu(inputManager, new List<IButton>
                {
                    playButton,
                    rankingButton,
                    exitButton
                });
            menu.Position = new Vector2(game.LogicalSize.X / 2 - menu.Size.X / 2, game.LogicalSize.Y * (4/5f) - menu.Size.Y / 2);
            AddUiComponent(menu);
        }
    }
}