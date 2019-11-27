using Engine.Input;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class Player : AnimatedObject
    {
        public enum PlayersStates
        {
            Idle,
            RunRight,
            RunLeft
        }
        private PlayersStates playerState = PlayersStates.Idle;
        public PlayersStates PlayerState
        {
            get => playerState;
            private set
            {
                if (playerState != value)
                {
                    playerState = value;
                    switch (value)
                    {
                        case PlayersStates.Idle:
                            PlayAnimation("Idle");
                            break;
                        case PlayersStates.RunRight:
                            SpriteEffects = SpriteEffects.None;
                            PlayAnimation("Run");
                            break;
                        case PlayersStates.RunLeft:
                            SpriteEffects = SpriteEffects.FlipHorizontally;
                            PlayAnimation("Run");
                            break;
                    }
                }
            }
        }
        private readonly InputManager inputManager;
        private Vector2 velocity = Vector2.Zero;

        public Player(Texture2D characterSpritesheetTexture, Dictionary<string, Rectangle> characterMap, InputManager input, Vector2 scale) : base(characterSpritesheetTexture, characterMap, scale)
        {
            inputManager = input;
            AddAnimation("Run", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, frameDuration: 0.1f));
            AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }, frameDuration: 0.1f));
            PlayAnimation("Idle");
        }

        public override void Update(GameTime gameTime)
        {
            ManagePlayerIntent();
            UpdatePlayerState();
            //Update animationSprite
            base.Update(gameTime);
            MovePlayer();
        }

        private void ManagePlayerIntent()
        {
            velocity = new Vector2(4f, 0);
            //if (inputManager.ActionIsPressed("MoveRight"))
            //{
            //    velocity = new Vector2(5f, 0);
            //}
            //else if (inputManager.ActionIsPressed("MoveLeft"))
            //{
            //    velocity = new Vector2(-5f, 0);
            //}
        }

        /// <summary>
        /// Should be called after managing player intent
        /// </summary>
        private void UpdatePlayerState()
        {
            if (velocity.X > 0)
            {
                PlayerState = PlayersStates.RunRight;
            }
            else if (velocity.X < 0)
            {
                PlayerState = PlayersStates.RunLeft;
            }
            else
            {
                PlayerState = PlayersStates.Idle;
            }
        }

        /// <summary>
        /// Moves player and resets velocity, should be called after managing animations
        /// </summary>
        private void MovePlayer()
        {
            Position += velocity;
            velocity = Vector2.Zero;
        }
    }
}
