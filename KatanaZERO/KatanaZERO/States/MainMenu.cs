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
            if (!game.IsThisSongPlaying(songs["MainMenu"]))
            {
                game.PlaySong(songs["MainMenu"]);
            }

            AddUiComponent(new Sprite(commonTextures["MainMenu"]));
            AddUiComponent(new Rain());
            RectangleButton playButton = new RectangleButton(inputManager, new Rectangle(0, 0, (int)(game.LogicalSize.X * 0.5f), (int)game.LogicalSize.Y / 10), fonts["Standard"], "PLAY")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            playButton.OnClick += (o, e) => game.ChangeState(new LevelSelect(game));
            RectangleButton rankingButton = new RectangleButton(inputManager, new Rectangle(0, 0, (int)(game.LogicalSize.X * 0.5f), (int)game.LogicalSize.Y / 10), fonts["Standard"], "HIGHSCORES")
            {
                Color = Color.Gray * 0.3f,
                Filled = true,
            };
            rankingButton.OnClick += (o, e) => game.ChangeState(new Highscores(game));
            VerticalNavigationMenu menu = new VerticalNavigationMenu(inputManager, new List<IButton>
                {
                    playButton,
                    rankingButton,
                });
            menu.Position = new Vector2((game.LogicalSize.X / 2) - (menu.Size.X / 2), (game.LogicalSize.Y * 0.8f) - (menu.Size.Y / 2));
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