using Microsoft.Xna.Framework;
using MonoGame.Tools;

namespace MonoGame.Extended.Entities.Systems
{
    public class RainfallSystem : EntityUpdateSystem
    {
        private readonly FastRandom _random;
        private readonly OrthographicCamera _camera;
        private ComponentMapper<RaindropComponent> _raindropComponent;
        private ComponentMapper<ExpiryComponent> _expiryComponent;

        private readonly float VelocityOfRaindrop = Core.MetersPerScreen.Height / 9f;

        private const float MinSpawnDelay = 0.0f;
        private const float MaxSpawnDelay = 0.0f;
        private float _spawnDelay = MaxSpawnDelay;

        public RainfallSystem() : base(Aspect.All(typeof(RaindropComponent)))
        {
            _random = new FastRandom();
            _camera = Core.Camera;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _raindropComponent = mapperService.GetMapper<RaindropComponent>();
            _expiryComponent = mapperService.GetMapper<ExpiryComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = gameTime.GetElapsedSeconds();
            float impactPosition = _random.NextSingle(0, 2160); 

            foreach(var entity in ActiveEntities)
            {
                RaindropComponent raindropComponent = _raindropComponent.Get(entity);

                raindropComponent.Velocity = new Vector2(0, 500);// * elapsedSeconds;
                raindropComponent.Position += raindropComponent.Velocity * elapsedSeconds;

                if(!_expiryComponent.Has(entity))
                {System.Console.WriteLine("HI");
                    for(int i = 0; i < 5; i++)
                    {
                        Vector2 velocity = new Vector2(
                            _random.NextSingle(-100, 100),
                            -raindropComponent.Velocity.Y * _random.NextSingle(0.1f, 0.2f));
                        int id = CreateRaindrop(
                            raindropComponent.Position.SetY(_random.NextSingle(0, 2160)), velocity, (i + 1) * 0.5f);
                        _expiryComponent.Put(id, new ExpiryComponent(1f));
                    }

                    DestroyEntity(entity);
                }
            }

            _spawnDelay -= gameTime.GetElapsedSeconds();

            if(_spawnDelay <= 0)
            {
                for(int q = 0; q < 10; q++)
                {
                    Vector2 position = new Vector2(_random.NextSingle(0, Core.VirtualResolution.Width * 2f), 0f);
                    int id = CreateRaindrop(position);
                    _expiryComponent.Put(
                        id,
                        new ExpiryComponent(
                            _random.NextSingle(0, Core.VirtualResolution.Height*0.5f) * Core.MetersPerPixel / VelocityOfRaindrop));
                }
                
                _spawnDelay = _random.NextSingle(MinSpawnDelay, MaxSpawnDelay);
            }
        }

        private int CreateRaindrop(Vector2 position, Vector2 velocity = default(Vector2), float size = 4)
        {
            var entity = CreateEntity();
            entity.Attach(
                new RaindropComponent
                { 
                    Position = position + _camera.Position,
                    Velocity = velocity,
                    Size = size 
                });

            return entity.Id;
        }
    }
}