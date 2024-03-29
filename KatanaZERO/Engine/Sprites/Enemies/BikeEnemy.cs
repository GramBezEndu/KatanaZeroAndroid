﻿namespace Engine.Sprites.Enemies
{
    using System;
    using System.Collections.Generic;
    using Engine.Physics;
    using Engine.States;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Extended.Animations.SpriteSheets;
    using PlatformerEngine.Timers;

    public class BikeEnemy : AnimatedObject, ICollidable
    {
        private readonly GameState gameState;

        private readonly ContentManager content;

        private readonly Player player;

        private readonly GameTimer[] phases;

        private MovableBodyState movableBodyState;

        public BikeEnemy(GameState gs, ContentManager c, Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, Player p)
            : base(spritesheet, map, scale)
        {
            player = p;
            gameState = gs;
            content = c;
            AddAnimation("Right", new SpriteSheetAnimationData(new int[] { 3, 4, 5, 6 }, frameDuration: 0.1f));
            PlayAnimation("Right");
            phases = new GameTimer[3];
            phases[0] = new GameTimer(2.5f);
            phases[0].OnTimedEvent += (o, e) => Adavance();

            phases[1] = new GameTimer(7f);
            phases[1].OnTimedEvent += (o, e) => Adavance();

            phases[2] = new GameTimer(6f);
            phases[2].OnTimedEvent += (o, e) => Adavance();
        }

        public event EventHandler OnMapCollision;

        public int CurrentPhase { get; private set; } = 0;

        public MovableBodyState MovableBodyState
        {
            get => movableBodyState;
            set
            {
                if (movableBodyState != value)
                {
                    ChangeAnimation(value);
                }
            }
        }

        public Vector2 CollisionSize => new Vector2(30, 25);

        public Vector2 Velocity { get; set; }

        public Rectangle CollisionRectangle => new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y);

        public void PrepareMove(GameTime gameTime)
        {
            // TODO: Finish movement, add shooting
            if (Hidden && CurrentPhase == 0)
            {
                if (Math.Abs(gameState.Camera.Position.X - Position.X) > 265f && Position.X < gameState.Camera.Position.X)
                {
                    Hidden = false;
                }
            }

            if (!Hidden)
            {
                if (CurrentPhase >= 0 && CurrentPhase < phases.Length)
                {
                    phases[CurrentPhase].Update(gameTime);
                }

                switch (CurrentPhase)
                {
                    case 0:
                        Velocity = new Vector2(11f, Velocity.Y);
                        break;
                    case 1:
                        Velocity = new Vector2(9f, Velocity.Y);
                        break;
                    case 2:
                        Velocity = new Vector2(5f, Velocity.Y);
                        break;
                }
            }
        }

        public void InvokeOnMapCollision(object sender, EventArgs args)
        {
            OnMapCollision?.Invoke(sender, args);
        }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
        }

        private void Adavance()
        {
            CurrentPhase++;
        }

        private void ChangeAnimation(MovableBodyState value)
        {
            movableBodyState = value;
            switch (value)
            {
                case MovableBodyState.InAir:
                case MovableBodyState.InAirRight:
                case MovableBodyState.InAirLeft:
                case MovableBodyState.Idle:
                    SpriteEffects = SpriteEffects.None;
                    PlayAnimation("Right");
                    break;
            }
        }
    }
}
