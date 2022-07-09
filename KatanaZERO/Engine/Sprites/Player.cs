namespace Engine
{
    using System;
    using System.Collections.Generic;
    using Engine.Input;
    using Engine.Physics;
    using Engine.PlayerIntents;
    using Engine.Sprites;
    using Engine.States;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Extended.Animations.SpriteSheets;
    using PlatformerEngine.Timers;

    public class Player : AnimatedObject, ICollidable
    {
        private MovableBodyState movableBodyState;

        private bool nitroActive;

        private Intent currentIntent;

        private GameTimer nitroTimer;

        public Player(Texture2D characterSpritesheetTexture, Dictionary<string, Rectangle> characterMap, Vector2 scale)
            : base(characterSpritesheetTexture, characterMap, scale)
        {
            AddAnimation("Attack", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6 }, frameDuration: 0.05f));
            AddAnimation("Run", new SpriteSheetAnimationData(new int[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, frameDuration: 0.1f));
            AddAnimation("Dance", new SpriteSheetAnimationData(new int[] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 }, frameDuration: 0.1f, isPingPong: true));
            AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 }, frameDuration: 0.1f));
            AddAnimation("BikeIdle", new SpriteSheetAnimationData(new int[] { 40, 41, 42, 43 }, frameDuration: 0.1f));
            AddAnimation("BikeDown", new SpriteSheetAnimationData(new int[] { 44, 45, 46, 47 }, frameDuration: 0.1f));
            AddAnimation("BikeUp", new SpriteSheetAnimationData(new int[] { 48 }, frameDuration: 0.1f));
            PlayAnimation("Idle");
        }

        public event EventHandler OnMapCollision;

        public static Vector2 NitroBonus => new Vector2(4f, 0f);

        public static Vector2 BikeVelocity => new Vector2(9f, 0f);

        public AnimatedObject KatanaSlash { get; set; }

        public AnimatedObject HiddenNotification { get; set; }

        public MovableBodyState MovableBodyState
        {
            get => movableBodyState;
            set
            {
                if (movableBodyState == MovableBodyState.Dead)
                {
                    return;
                }

                if (movableBodyState != value)
                {
                    movableBodyState = value;
                    switch (value)
                    {
                        case MovableBodyState.InAir:
                        case MovableBodyState.InAirRight:
                        case MovableBodyState.InAirLeft:
                        case MovableBodyState.Idle:
                            HiddenNotification.Hidden = true;
                            Color = Color.White;
                            if (OnBike)
                            {
                                PlayAnimation("BikeIdle");
                            }
                            else
                            {
                                PlayAnimation("Idle");
                            }

                            break;
                        case MovableBodyState.WalkRight:
                            HiddenNotification.Hidden = true;
                            Color = Color.White;
                            SpriteEffects = SpriteEffects.None;
                            if (OnBike)
                            {
                                PlayAnimation("BikeIdle");
                            }
                            else
                            {
                                PlayAnimation("Run");
                            }

                            break;
                        case MovableBodyState.WalkLeft:
                            HiddenNotification.Hidden = true;
                            Color = Color.White;
                            SpriteEffects = SpriteEffects.FlipHorizontally;
                            if (OnBike)
                            {
                                PlayAnimation("BikeIdle");
                            }
                            else
                            {
                                PlayAnimation("Run");
                            }

                            break;
                        case MovableBodyState.Attack:
                            HiddenNotification.Hidden = true;
                            Color = Color.White;
                            GameState.Sounds["WeaponSlash"].Play();
                            KatanaSlash.Hidden = false;
                            KatanaSlash.Position = Position;
                            KatanaSlash.PlayAnimation("Slash");
                            PlayAnimation("Attack", () =>
                            {
                                KatanaSlash.Hidden = true;
                            });
                            break;
                        case MovableBodyState.Dance:
                            HiddenNotification.Hidden = false;
                            Color = Color.Black;
                            PlayAnimation("Dance");
                            HiddenNotification.PlayAnimation("Idle");
                            break;
                        case MovableBodyState.Hidden:
                            Color = Color.Black;
                            PlayAnimation("Idle");
                            HiddenNotification.Hidden = false;
                            HiddenNotification.PlayAnimation("Idle");
                            break;
                        case MovableBodyState.Dead:
                            Hidden = true;
                            break;
                    }
                }
            }
        }

        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public Vector2 CollisionSize
        {
            get
            {
                if (OnBike)
                {
                    return new Vector2(40, 20);
                }
                else
                {
                    return new Vector2(22, 35);
                }
            }
        }

        public Rectangle CollisionRectangle
        {
            get
            {
                if (OnBike)
                {
                    return new Rectangle((int)Position.X, (int)(Position.Y - 10), (int)(CollisionSize.X - 10), (int)CollisionSize.Y);
                }
                else
                {
                    return new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y);
                }
            }
        }

        public bool HasBottle { get; set; }

        public bool OnBike { get; set; }

        public bool NitroActive
        {
            get => nitroActive;
            set
            {
                if (nitroActive != value)
                {
                    nitroActive = value;
                    nitroTimer = new GameTimer(3f);
                    nitroTimer.OnTimedEvent += (o, e) => DeactivateNitro();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Update animationSprite
            base.Update(gameTime);
            KatanaSlash.Update(gameTime);
            if (SpriteEffects == SpriteEffects.None)
            {
                HiddenNotification.Position = new Vector2(Position.X + (CollisionSize.X / 2) - (HiddenNotification.Size.X / 2) - 5, DrawingPosition.Y - HiddenNotification.Size.Y);
            }
            else
            {
                HiddenNotification.Position = new Vector2(Position.X + (CollisionSize.X / 2) - (HiddenNotification.Size.X / 2) - 3, DrawingPosition.Y - HiddenNotification.Size.Y);
            }

            HiddenNotification.Update(gameTime);
            nitroTimer?.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            KatanaSlash.Draw(gameTime, spriteBatch);
            HiddenNotification.Draw(gameTime, spriteBatch);
        }

        public void MoveRight()
        {
            if (movableBodyState != MovableBodyState.Dead)
            {
                if (OnBike)
                {
                    Velocity = new Vector2(Velocity.X + 4f, Velocity.Y);
                }
                else
                {
                    Velocity = new Vector2(Velocity.X + 1.9f, Velocity.Y);
                }
            }
        }

        public void MoveLeft()
        {
            if (movableBodyState != MovableBodyState.Dead)
            {
                if (OnBike)
                {
                    Velocity = new Vector2(Velocity.X + (-4f), Velocity.Y);
                }
                else
                {
                    Velocity = new Vector2(Velocity.X + (-1.9f), Velocity.Y);
                }
            }
        }

        public void MoveUp()
        {
            if (movableBodyState != MovableBodyState.Dead)
            {
                if (OnBike)
                {
                    Velocity = new Vector2(Velocity.X, Velocity.Y - 3f);
                }
            }
        }

        public void MoveDown()
        {
            if (movableBodyState != MovableBodyState.Dead)
            {
                if (OnBike)
                {
                    Velocity = new Vector2(Velocity.X, Velocity.Y + 3f);
                }
            }
        }

        public void Kill(Enemy e)
        {
            throw new NotImplementedException();
        }

        public bool HasIntent()
        {
            if (currentIntent == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void AddIntent(Intent intent)
        {
            currentIntent = intent;
        }

        public void ResetIntent()
        {
            currentIntent = null;
        }

        public void PrepareMove(GameTime gameTime)
        {
            // Set base velocity (remember to reset Y)
            if (OnBike && NitroActive)
            {
                Velocity = BikeVelocity + NitroBonus;
            }
            else if (OnBike)
            {
                Velocity = BikeVelocity;
            }

            ManagePlayerIntent(gameTime);
        }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
        }

        public void InvokeOnMapCollision(object sender, EventArgs args)
        {
            OnMapCollision?.Invoke(sender, args);
        }

        private void DeactivateNitro()
        {
            nitroActive = false;
            nitroTimer = null;
        }

        private void ManagePlayerIntent(GameTime gameTime)
        {
            if (currentIntent != null)
            {
                currentIntent.Update(gameTime);
                if (currentIntent.Finished)
                {
                    // Resetting intent will alow to complete it again in the future
                    currentIntent.ResetIntent();
                    currentIntent = null;
                }
            }
        }
    }
}
