namespace KatanaZERO.States
{
    using System.Collections.Generic;
    using Engine;
    using Engine.Physics;
    using Engine.Sprites;
    using Engine.Sprites.Enemies;
    using Engine.States;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Extended.Animations.SpriteSheets;

    public class TrafficManager : IComponent
    {
        public List<StreetCar> Cars { get; private set; }

        public List<BikeEnemy> Enemies { get; private set; }

        public List<AnimatedObject> EnemyWarnings { get; private set; }

        private bool[] honkUsed;

        private int soundCounter = 0;

        public List<AnimatedObject> TrafficWarnings { get; private set; }

        public List<ICollidable> Items { get; private set; }

        public List<AnimatedObject> ItemNotifications { get; private set; }

        private readonly Camera camera;

        private readonly GameState gameState;

        private readonly Game1 game;

        private readonly Player player;

        public TrafficManager(Game1 gameRef, GameState gs, Player p, Camera cam, ContentManager content)
        {
            camera = cam;
            game = gameRef;
            gameState = gs;
            player = p;
            Cars = CreateCars(content);
            Enemies = CreateBikeEnemies(content);
            EnemyWarnings = CreateEnemyWarnings(content);
            TrafficWarnings = CreateTrafficWarnings(gameRef, cam, content);
            Items = CreateItems(content);
            ItemNotifications = CreateHelpfulItemsNotifications(gameRef, cam, content);
        }

        private List<AnimatedObject> CreateEnemyWarnings(ContentManager content)
        {
            List<AnimatedObject> warnings = new List<AnimatedObject>();
            for (int i = 0; i < Enemies.Count; i++)
            {
                BikeEnemy enemy = Enemies[i];
                AnimatedObject t1 = new AnimatedObject(content.Load<Texture2D>("Textures/BikeWarning/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Textures/BikeWarning/Map"), new Vector2(2.5f, 2.5f));
                t1.AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, frameDuration: 0.1f));
                t1.PlayAnimation("Idle");
                warnings.Add(t1);
            }

            return warnings;
        }

        private List<StreetCar> CreateCars(ContentManager content)
        {
            List<StreetCar> cars = new List<StreetCar>();
            cars.Add(SpawnCar(content, 4200f, 1));
            cars.Add(SpawnCar(content, 5600f, 3));
            cars.Add(SpawnCar(content, 7500f, 1));
            cars.Add(SpawnCar(content, 8500f, 3));
            cars.Add(SpawnCar(content, 13400f, 2));
            cars.Add(SpawnCar(content, 13400f, 3));
            cars.Add(SpawnCar(content, 15300f, 1));
            cars.Add(SpawnCar(content, 16200f, 2));
            cars.Add(SpawnCar(content, 17100f, 3));
            cars.Add(SpawnCar(content, 17500f, 1));
            cars.Add(SpawnCar(content, 19100f, 3));
            cars.Add(SpawnCar(content, 19700f, 2));
            cars.Add(SpawnCar(content, 20120f, 2));
            cars.Add(SpawnCar(content, 20300f, 3));
            cars.Add(SpawnCar(content, 21200f, 1));
            cars.Add(SpawnCar(content, 24120f, 1));
            cars.Add(SpawnCar(content, 24700f, 2));
            cars.Add(SpawnCar(content, 29600f, 1));
            cars.Add(SpawnCar(content, 30200f, 2));
            cars.Add(SpawnCar(content, 30800f, 3));
            cars.Add(SpawnCar(content, 31300f, 2));
            honkUsed = new bool[cars.Count];
            return cars;
        }

        private List<BikeEnemy> CreateBikeEnemies(ContentManager content)
        {
            List<BikeEnemy> enemies = new List<BikeEnemy>();
            enemies.Add(CreateBikeEnemy(content, 2000f/*27000f*/, 1));
            enemies.Add(CreateBikeEnemy(content, 29500f, 3));
            return enemies;
        }

        private List<AnimatedObject> CreateTrafficWarnings(Game1 game, Camera camera, ContentManager content)
        {
            List<AnimatedObject> notifications = new List<AnimatedObject>();
            for (int i = 0; i < Cars.Count; i++)
            {
                StreetCar car = Cars[i];
                AnimatedObject t1 = new AnimatedObject(content.Load<Texture2D>("Textures/BikeWarning/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Textures/BikeWarning/Map"), new Vector2(2.5f, 2.5f));
                t1.AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, frameDuration: 0.1f));
                t1.PlayAnimation("Idle");
                notifications.Add(t1);
            }

            return notifications;
        }

        private List<AnimatedObject> CreateHelpfulItemsNotifications(Game1 gameRef, Camera cam, ContentManager content)
        {
            List<AnimatedObject> notifications = new List<AnimatedObject>();
            for (int i = 0; i < Items.Count; i++)
            {
                ICollidable item = Items[i];
                AnimatedObject t1 = new AnimatedObject(content.Load<Texture2D>("Textures/BikeItem/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Textures/BikeItem/Map"), new Vector2(2.5f, 2.5f));
                t1.AddAnimation("Idle", new SpriteSheetAnimationData(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, frameDuration: 0.1f));
                t1.PlayAnimation("Idle");
                notifications.Add(t1);
            }

            return notifications;
        }

        public List<ICollidable> CreateItems(ContentManager content)
        {
            List<ICollidable> items = new List<ICollidable>
            {
                CreateNitro(content, 8000f, 2),
                CreateNitro(content, 22000f, 3),
            };
            //items.Add(CreateBottlePickUp(content, 27000f, 2));
            return items;
        }

        private ICollidable CreateBottlePickUp(ContentManager content, float posX, int lane)
        {
            float posY = 180f;
            switch (lane)
            {
                case 1:
                    posY = 180f;
                    break;
                case 2:
                    posY = 220f;
                    break;
                case 3:
                    posY = 260f;
                    break;
            }

            BottlePickUp botttle = new BottlePickUp(gameState, content.Load<Texture2D>("Textures/Bottle"), new Vector2(0.7f, 0.7f))
            {
                Position = new Vector2(posX, posY),
            };
            return botttle;
        }

        private StreetCar SpawnCar(ContentManager content, float posX, int lane)
        {
            float posY = 162f;
            switch (lane)
            {
                case 1:
                    posY = 162f;
                    break;
                case 2:
                    posY = 200f;
                    break;
                case 3:
                    posY = 240f;
                    break;
            }

            StreetCar car = new StreetCar(content.Load<Texture2D>("Textures/StreetCar"))
            {
                Position = new Vector2(posX, posY),
            };
            return car;
        }

        public BikeEnemy CreateBikeEnemy(ContentManager content, float posX, int lane)
        {
            float posY = 162f;
            switch (lane)
            {
                case 1:
                    posY = 162f;
                    break;
                case 2:
                    posY = 200f;
                    break;
                case 3:
                    posY = 240f;
                    break;
            }

            BikeEnemy bikeEnemy = new BikeEnemy(gameState, content, content.Load<Texture2D>("Enemies/BikeMachinegun/Spritesheet"), content.Load<Dictionary<string, Rectangle>>("Enemies/BikeMachinegun/Map"), Vector2.One, player)
            {
                Position = new Vector2(posX, posY),
                Hidden = true,
            };
            return bikeEnemy;
        }

        private Nitro CreateNitro(ContentManager content, float posX, int lane)
        {
            float posY = 175f;
            switch (lane)
            {
                case 1:
                    posY = 175f;
                    break;
                case 2:
                    posY = 215f;
                    break;
                case 3:
                    posY = 255f;
                    break;
            }

            Nitro nitro = new Nitro(content.Load<Texture2D>("Textures/Nitro"), new Vector2(0.5f, 0.5f))
            {
                Position = new Vector2(posX, posY),
            };
            return nitro;
        }

        public void Update(GameTime gameTime)
        {
            UpdateTrafficCarsSound();
            UpdateTrafficWarnings();
            UpdateEnemyWarnings();
            UpdateItemNotifications();
        }

        private void UpdateItemNotifications()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                ICollidable nitro = Items[i];
                AnimatedObject notification = ItemNotifications[i];
                if (nitro.Position.X - camera.Position.X < 1400f && nitro.Position.X - camera.Position.X > 450f)
                {
                    notification.Hidden = false;
                    notification.Position = new Vector2(game.LogicalSize.X - notification.Size.X, camera.WorldToScreen(new Vector2(0f, nitro.CollisionRectangle.Center.Y - 15f)).Y);
                    notification.SpriteEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    notification.Hidden = true;
                }
            }
        }

        private void UpdateTrafficWarnings()
        {
            for (int i = 0; i < TrafficWarnings.Count; i++)
            {
                StreetCar car = Cars[i];
                AnimatedObject notification = TrafficWarnings[i];
                if (car.Position.X - camera.Position.X < 1400f && car.Position.X - camera.Position.X > 450f)
                {
                    notification.Hidden = false;
                    notification.Position = new Vector2(game.LogicalSize.X - notification.Size.X, camera.WorldToScreen(new Vector2(0f, car.CollisionRectangle.Center.Y - 15f)).Y);
                    notification.SpriteEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    notification.Hidden = true;
                }
            }
        }

        private void UpdateEnemyWarnings()
        {
            for (int i = 0; i < EnemyWarnings.Count; i++)
            {
                BikeEnemy enemy = Enemies[i];
                AnimatedObject notification = EnemyWarnings[i];

                // TODO: Finish
                if (enemy.Hidden == false && enemy.CurrentPhase == 0 && enemy.Position.X < camera.Position.X)
                {
                    notification.Hidden = false;
                    notification.Position = new Vector2(0, camera.WorldToScreen(new Vector2(0f, enemy.CollisionRectangle.Center.Y - 15f)).Y);
                    notification.SpriteEffects = SpriteEffects.None;
                }
                else
                {
                    notification.Hidden = true;
                }
            }
        }

        private void UpdateTrafficCarsSound()
        {
            for (int i = 0; i < Cars.Count; i++)
            {
                StreetCar car = (StreetCar)Cars[i];
                if (car.Position.X - camera.Position.X < 450f && !honkUsed[i])
                {
                    honkUsed[i] = true;
                    if (soundCounter == 0)
                    {
                        Engine.States.State.Sounds["SpeedingVehicle"].Play();
                        soundCounter = 1;
                    }
                    else
                    {
                        Engine.States.State.Sounds["SpeedingVehicle2"].Play();
                        soundCounter = 0;
                    }
                }
            }
        }
    }
}