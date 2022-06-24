using Engine.Physics;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;
using PlatformerEngine.Timers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Sprites
{
    public class Mortar : AnimatedObject, ICollidable
    {
        MortarTarget target;
        GameTimer firstStage;
        bool secondStageActivated;
        bool collidedWithTarget;
        readonly Vector2 movementVector;
        int verticalLane;
        int horizontalLane;
        int travelTimeInFrames = 0;
        Vector2 destination;
        Vector2 cameraMovement = Vector2.Zero;
        Vector2 previousCameraPosition = Vector2.Zero;
        readonly GameState gameState;
        readonly ContentManager content;
        public Mortar(GameState gs, ContentManager c, Texture2D spritesheet, Dictionary<string, Rectangle> map, Vector2 scale, int verticalLane, int horizontalLane) : base(spritesheet, map, scale)
        {
            AddAnimation("Far", new SpriteSheetAnimationData(new int[] { 1 }, frameDuration: 0.1f));
            AddAnimation("Close", new SpriteSheetAnimationData(new int[] { 0 }, frameDuration: 0.1f));
            AddAnimation("Explode", new SpriteSheetAnimationData(new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, frameDuration: 0.1f, isLooping: false));
            PlayAnimation("Far");
            this.verticalLane = verticalLane;
            this.horizontalLane = horizontalLane;
            gameState = gs;
            content = c;
            movementVector = new Vector2(0.3f + verticalLane * 1.45f/*0.725f*/, 3f + horizontalLane * 0.2f);
        }

        public EventHandler OnMapCollision { get; set; }

        private MoveableBodyStates _moveableBodyState;

        public MoveableBodyStates MoveableBodyState
        {
            get => _moveableBodyState;
            set
            {
                if (_moveableBodyState != value)
                {
                    _moveableBodyState = value;
                    switch (value)
                    {
                        case MoveableBodyStates.InAir:
                        case MoveableBodyStates.InAirRight:
                        case MoveableBodyStates.InAirLeft:
                        case MoveableBodyStates.Idle:
                            if (!collidedWithTarget)
                                PlayAnimation("Far");
                            break;
                    }
                }
            }
        }
        public Vector2 Velocity { get; set; }

        public Vector2 CollisionSize => new Vector2(10f, 10f);

        public Rectangle CollisionRectangle { get { return new Rectangle((int)Position.X, (int)Position.Y, (int)CollisionSize.X, (int)CollisionSize.Y); } }

        public void NotifyHorizontalCollision(GameTime gameTime, object collider)
        {
            
        }

        public void PrepareMove(GameTime gameTime)
        {
            if (!Hidden)
            {
                if (firstStage == null)
                {
                    firstStage = new GameTimer(1.2f);
                    firstStage.OnTimedEvent = (o, e) => ActivateSecondStage();
                }
                firstStage?.Update(gameTime);
                if (secondStageActivated)
                {
                    Velocity = Vector2.Zero;
                    if (Position.Y < destination.Y)
                    {
                        Velocity = movementVector;
                    }
                    else if (!collidedWithTarget)
                    {
                        Velocity = Vector2.Zero;
                        collidedWithTarget = true;
                        target.Hidden = true;
                        PlayAnimation("Explode", onCompleted: new Action(HideMortar));
                    }
                }
                else
                {
                    Velocity = new Vector2(15f, -2.2f);
                }
            }
        }

        private void HideMortar()
        {
            Hidden = true;
        }

        private void CreateTargetUI()
        {
            target = new MortarTarget(content.Load<Texture2D>("Enemies/Mortar/MortarTarget/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Enemies/Mortar/MortarTarget/Map"), new Vector2(2f, 2f));
            target.Color = Color.Red;
            target.Position = gameState.Camera.WorldToScreen(destination - travelTimeInFrames * new Vector2(Player.BIKE_VELOCITY.X, Player.BIKE_VELOCITY.Y)) - target.Size / 2;
            gameState.AddUiComponent(target);
        }

        private void ActivateSecondStage()
        {
            if (!secondStageActivated)
            {
                float tempY = 0f;
                switch (horizontalLane)
                {
                    case 0:
                        tempY = 190f;
                        break;
                    case 1:
                        tempY = 210f;
                        break;
                    case 2:
                        tempY = 230f;
                        break;
                    case 3:
                        tempY = 250f;
                        break;
                    case 4:
                        tempY = 270f;
                        break;
                }
                destination = new Vector2(Position.X + movementVector.X, Position.Y + movementVector.Y);
                travelTimeInFrames = 1;
                while (destination.Y < tempY)
                {
                    destination += movementVector;
                    travelTimeInFrames++;
                }
                PlayAnimation("Close");
                CreateTargetUI();
                secondStageActivated = true;
            }
        }
    }
}
