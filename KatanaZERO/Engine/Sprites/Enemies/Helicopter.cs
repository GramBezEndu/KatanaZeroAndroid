using Engine.Physics;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Sprites.Enemies
{
    public class Helicopter : AnimatedObject, ICollidable
    {
        public bool RotationBack
        {
            get => rotationBack;
            set
            {
                if (rotationBack != value)
                {
                    rotationBack = value;
                    ChangeAnimation(_moveableBodyState);
                }
            }
        }
        private bool rotationBack = false;
        private readonly GameState gameState;
        private readonly ContentManager content;
        protected readonly Player player;
        HelicopterScript heliScript;

        public Helicopter(GameState gs, ContentManager c, Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, Player p) : base(spritesheet, map, scale)
        {
            player = p;
            gameState = gs;
            content = c;
            AddAnimation("Right", new SpriteSheetAnimationData(new int[] { 2, 3 }, frameDuration: 0.1f));
            AddAnimation("Back", new SpriteSheetAnimationData(new int[] { 4, 5 }, frameDuration: 0.1f));
            PlayAnimation("Right");
            heliScript = new HelicopterScript(this);
        }

        internal void Fire(int verticalLane)
        {
            //TODO: Finish
            for (int i = 0; i < 5; i++)
            {
                Mortar mortar = new Mortar(gameState, content, content.Load<Texture2D>("Enemies/Mortar/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Enemies/Mortar/Map"), Vector2.One, verticalLane, i);
                mortar.Position = new Vector2(Rectangle.Right - 10 + i, Rectangle.Center.Y - 15 + i * 1);
                gameState.AddMoveableBody(mortar);
                State.Sounds["Fire"].Play();
            }
        }

        private MoveableBodyStates _moveableBodyState;

        public MoveableBodyStates MoveableBodyState
        {
            get => _moveableBodyState;
            set
            {
                if (_moveableBodyState != value)
                {
                    ChangeAnimation(value);
                }
            }
        }

        private void ChangeAnimation(MoveableBodyStates value)
        {
            _moveableBodyState = value;
            switch (value)
            {
                case MoveableBodyStates.Idle:
                case MoveableBodyStates.InAir:
                case MoveableBodyStates.InAirRight:
                    SpriteEffects = SpriteEffects.None;
                    if (RotationBack)
                        PlayAnimation("Back");
                    else
                        PlayAnimation("Right");
                    break;
            }
        }

        public Vector2 CollisionSize { get { return new Vector2(30, 38); } }

        public EventHandler OnMapCollision { get; set; }

        public Vector2 Velocity { get; set; }

        public Rectangle CollisionRectangle { get { return new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y); } }

        public void PrepareMove(GameTime gameTime)
        {
            heliScript.Update(gameTime);
        }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
            
        }
    }
}
