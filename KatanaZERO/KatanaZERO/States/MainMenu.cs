namespace KatanaZERO.States
{
    using System.Collections.Generic;
    using Engine;
    using Engine.Controls;
    using Engine.Controls.Buttons;
    using Engine.Sprites;
    using Engine.States;
    using KatanaZERO.SpecialEffects;
    using Microsoft.Xna.Framework;

    public class MainMenu : State
    {
        public MainMenu(Game1 gameReference)
            : base(gameReference)
        {
            if (!Game.IsThisSongPlaying(Songs["MainMenu"]))
            {
                Game.PlaySong(Songs["MainMenu"]);
            }

            AddUiComponent(new Sprite(Textures["MainMenu"]));
            AddUiComponent(new Rain());
            RectangleButton playButton = new RectangleButton(InputManager, new Rectangle(0, 0, (int)(Game.LogicalSize.X * 0.5f), (int)Game.LogicalSize.Y / 10), Fonts["Standard"], "PLAY")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            playButton.OnClick += (o, e) => Game.ChangeState(new LevelSelect(Game));
            RectangleButton rankingButton = new RectangleButton(InputManager, new Rectangle(0, 0, (int)(Game.LogicalSize.X * 0.5f), (int)Game.LogicalSize.Y / 10), Fonts["Standard"], "HIGHSCORES")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            rankingButton.OnClick += (o, e) => Game.ChangeState(new Highscores(Game));
            VerticalNavigationMenu menu = new VerticalNavigationMenu(InputManager, new List<IButton>
                {
                    playButton,
                    rankingButton,
                });
            menu.Position = new Vector2((Game.LogicalSize.X / 2) - (menu.Size.X / 2), (Game.LogicalSize.Y * 0.8f) - (menu.Size.Y / 2));
            DrawableRectangle backgroundMenu = new DrawableRectangle(new Rectangle(0, 0, (int)(menu.Size.X * 1.1f), (int)(menu.Size.Y * 1.4f)))
            {
                Color = Color.Black * 0.7f,
                Filled = true,
            };
            backgroundMenu.Position = new Vector2(menu.Position.X - (0.05f * menu.Size.X), menu.Position.Y - (0.2f * menu.Size.Y));
            AddUiComponent(backgroundMenu);
            AddUiComponent(menu);
        }
    }
}