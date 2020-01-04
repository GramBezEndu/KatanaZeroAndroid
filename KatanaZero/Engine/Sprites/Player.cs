using Engine.Input;
using Engine.Physics;
using Engine.PlayerIntents;
using Engine.Sprites;
using Engine.States;
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
    public class Player : AnimatedObject, ICollidable
    {

        //private void SetPlayerState(PlayersStates value, Enemy enemy = null)
        //{
        //    if (playerState != value)
        //    {
        //        playerState = value;
        //        switch (value)
        //        {
        //            case PlayersStates.Idle:
        //                PlayAnimation("Idle");
        //                break;
        //            case PlayersStates.RunRight:
        //                SpriteEffects = SpriteEffects.None;
        //                PlayAnimation("Run");
        //                break;
        //            case PlayersStates.RunLeft:
        //                SpriteEffects = SpriteEffects.FlipHorizontally;
        //                PlayAnimation("Run");
        //                break;
        //            case PlayersStates.Attack:
        //                State.sounds["WeaponSlash"].Play();
        //                KatanaSlash.Hidden = false;
        //                KatanaSlash.Position = this.Position;
        //                KatanaSlash.PlayAnimation("Slash");
        //                //Adjust player position on attack start
        //                //Position = new Vector2(Position.X - 0.3f * Size.X, Position.Y);
        //                PlayAnimation("Attack", () =>
        //                {
        //                    //Adjust player position after attacking
        //                    //Position = new Vector2(Position.X + 0.3f * Size.X, Position.Y);
        //                    SetPlayerState(PlayersStates.Idle);
        //                    enemy.Die();
        //                    KatanaSlash.Hidden = true;
        //                });
        //                break;
        //        }
        //    }
        //}
        private readonly InputManager inputManager;
        private List<IPlayerIntent> playerIntents = new List<IPlayerIntent>();
        public AnimatedObject KatanaSlash;
        private MoveableBodyStates _moveableBodyState;

        public MoveableBodyStates MoveableBodyState
        {
            get => _moveableBodyState;
            set
            {
                if(_moveableBodyState != value)
                {
                    _moveableBodyState = value;
                    switch (value)
                    {
                        case MoveableBodyStates.Idle:
                            PlayAnimation("Idle");
                            break;
                        case MoveableBodyStates.WalkRight:
                            SpriteEffects = SpriteEffects.None;
                            PlayAnimation("Run");
                            break;
                        case MoveableBodyStates.WalkLeft:
                            SpriteEffects = SpriteEffects.FlipHorizontally;
                            PlayAnimation("Run");
                            break;
                        case MoveableBodyStates.Attack:
                            State.sounds["WeaponSlash"].Play();
                            KatanaSlash.Hidden = false;
                            KatanaSlash.Position = this.Position;
                            KatanaSlash.PlayAnimation("Slash");
                            PlayAnimation("Attack", () =>
                            {
                                //Adjust player position after attacking
                                //Position = new Vector2(Position.X + 0.3f * Size.X, Position.Y);
                                //SetPlayerState(PlayersStates.Idle);
                                //enemy.Die();
                                KatanaSlash.Hidden = true;
                            });
                            break;
                    }
                }
            }
        }
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public Player(Texture2D characterSpritesheetTexture, Dictionary<string, Rectangle> characterMap, InputManager input, Vector2 scale) : base(characterSpritesheetTexture, characterMap, scale)
        {
            inputManager = input;
            AddAnimation("Attack", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6 }, frameDuration: 0.05f));
            AddAnimation("Run", new SpriteSheetAnimationData(new int[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, frameDuration: 0.1f));
            AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 }, frameDuration: 0.1f));
            PlayAnimation("Idle");
        }

        public override void Update(GameTime gameTime)
        {
            //Update animationSprite
            base.Update(gameTime);
            KatanaSlash.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            KatanaSlash.Draw(gameTime, spriteBatch);
        }

        private void ManagePlayerIntent(GameTime gameTime)
        {
            if (playerIntents.Count > 0)
            {
                playerIntents[0].Update(gameTime);
                if (playerIntents[0].IntentFinished())
                    playerIntents.Remove(playerIntents[0]);
            }
            //else
            //{
            //    SetPlayerState(PlayersStates.Idle);
            //}
        }

        public void MoveRight()
        {
            Velocity = new Vector2(5f, 0);
        }

        public void MoveLeft()
        {
            Velocity = new Vector2(-5f, 0);
        }

        public void Kill(Enemy e)
        {
            throw new NotImplementedException();
            //SetPlayerState(PlayersStates.Attack, e);
        }

        public void AddIntent(IPlayerIntent intent)
        {
            playerIntents.Add(intent);
        }

        public void PrepareMove(GameTime gameTime)
        {
            ManagePlayerIntent(gameTime);
        }
    }
}
