namespace Engine.Sprites.Enemies
{
    using System;
    using System.Collections.Generic;
    using Engine.Physics;
    using Engine.States;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Extended.Animations.SpriteSheets;

    public class Helicopter : AnimatedObject, ICollidable
    {
        private readonly GameState gameState;

        private readonly ContentManager content;

        private readonly HelicopterScript heliScript;

        private readonly Player player;

        private bool rotationBack = false;

        private MovableBodyState movableBodyState;

        public Helicopter(GameState gs, ContentManager c, Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, Player p)
            : base(spritesheet, map, scale)
        {
            player = p;
            gameState = gs;
            content = c;
            AddAnimation("Right", new SpriteSheetAnimationData(new int[] { 2, 3 }, frameDuration: 0.1f));
            AddAnimation("Back", new SpriteSheetAnimationData(new int[] { 4, 5 }, frameDuration: 0.1f));
            PlayAnimation("Right");
            heliScript = new HelicopterScript(this);
        }

        public event EventHandler OnMapCollision;

        public Vector2 CollisionSize => new Vector2(30, 38);

        public Vector2 Velocity { get; set; }

        public Rectangle CollisionRectangle => new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y);

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

        public bool RotationBack
        {
            get => rotationBack;
            set
            {
                if (rotationBack != value)
                {
                    rotationBack = value;
                    ChangeAnimation(movableBodyState);
                }
            }
        }

        public void PrepareMove(GameTime gameTime)
        {
            heliScript.Update(gameTime);
        }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
        }

        public void InvokeOnMapCollision(object sender, EventArgs args)
        {
            OnMapCollision?.Invoke(sender, args);
        }

        internal void Fire(int verticalLane)
        {
            // TODO: Finish
            for (int i = 0; i < 5; i++)
            {
                Mortar mortar = new Mortar(gameState, content, content.Load<Texture2D>("Enemies/Mortar/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Enemies/Mortar/Map"), Vector2.One, verticalLane, i)
                {
                    Position = new Vector2(Rectangle.Right - 10 + i, Rectangle.Center.Y - 15 + (i * 1)),
                };
                gameState.AddMoveableBody(mortar);
                State.Sounds["Fire"].Play();
            }
        }

        private void ChangeAnimation(MovableBodyState value)
        {
            movableBodyState = value;
            switch (value)
            {
                case MovableBodyState.Idle:
                case MovableBodyState.InAir:
                case MovableBodyState.InAirRight:
                    SpriteEffects = SpriteEffects.None;
                    if (RotationBack)
                    {
                        PlayAnimation("Back");
                    }
                    else
                    {
                        PlayAnimation("Right");
                    }

                    break;
            }
        }
    }
}
