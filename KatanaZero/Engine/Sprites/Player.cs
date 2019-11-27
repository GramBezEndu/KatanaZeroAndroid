using Engine.Input;
using Engine.PlayerIntents;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Engine
{
    public class Player : AnimatedObject
    {
        public enum PlayersStates
        {
            Idle,
            RunRight,
            RunLeft,
            Attack
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
                        case PlayersStates.Attack:
                            //Adjust player position on attack start
                            //Position = new Vector2(Position.X - 0.3f * Size.X, Position.Y);
                            PlayAnimation("Attack", () =>
                            {
                                //Adjust player position after attacking
                                //Position = new Vector2(Position.X + 0.3f * Size.X, Position.Y);
                                PlayerState = PlayersStates.Idle;
                            });
                            break;
                    }
                }
            }
        }
        private readonly InputManager inputManager;
        private Vector2 velocity = Vector2.Zero;
        private List<IPlayerIntent> playerIntents = new List<IPlayerIntent>();

        public Player(Texture2D characterSpritesheetTexture, Dictionary<string, Rectangle> characterMap, InputManager input, Vector2 scale) : base(characterSpritesheetTexture, characterMap, scale)
        {
            inputManager = input;
            AddAnimation("Attack", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6 }, frameDuration: 0.05f));
            AddAnimation("Run", new SpriteSheetAnimationData(new int[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, frameDuration: 0.1f));
            AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 }, frameDuration: 0.1f));
            PlayAnimation("Idle");
            //Debug.WriteLine("Player size: " + Size);
        }

        public override void Update(GameTime gameTime)
        {
            ManagePlayerIntent(gameTime);
            UpdatePlayerState();
            //Update animationSprite
            base.Update(gameTime);
            MovePlayer();
        }

        private void ManagePlayerIntent(GameTime gameTime)
        {
            if(playerIntents.Count > 0)
            {
                playerIntents[0].Update(gameTime);
                if (playerIntents[0].IntentFinished())
                    playerIntents.Remove(playerIntents[0]);
            }
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
        }

        public void MoveRight()
        {
            velocity = new Vector2(5f, 0);
        }

        public void MoveLeft()
        {
            velocity = new Vector2(-5f, 0);
        }

        public void Kill(Enemy e)
        {
            PlayerState = PlayersStates.Attack;
        }

        /// <summary>
        /// Moves player and resets velocity, should be called after managing animations
        /// </summary>
        private void MovePlayer()
        {
            Position += velocity;
            velocity = Vector2.Zero;
        }

        public void AddIntent(IPlayerIntent intent)
        {
            playerIntents.Add(intent);
        }
    }
}
