using Microsoft.Xna.Framework;
using MonoGame.Tools;

namespace MonoGame.Extended.Entities.Systems
{
    public class RainfallSystem : EntityUpdateSystem
    {
        private readonly FastRandom _random;
        private ComponentMapper<RaindropComponent> _raindropComponent;
        private ComponentMapper<ExpiryComponent> _expiryComponent;

        private readonly float VelocityOfRaindrop = Globals.Units.MetersPerScreen.Height / 9f;

        private const float MinSpawnDelay = 0.0f;
        private const float MaxSpawnDelay = 0.0f;
        private float _spawnDelay = MaxSpawnDelay;

        public RainfallSystem() : base(Aspect.All(typeof(RaindropComponent)))
        {
            _random = new FastRandom();
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

                raindropComponent.Velocity = new Vector2(MonoGame.WindSpeed, VelocityOfRaindrop);
                raindropComponent.Position += raindropComponent.Velocity;

                if(raindropComponent.Position.Y >= impactPosition && !_expiryComponent.Has(entity))
                {
                    for(int i = 0; i < 5; i++)
                    {
                        Vector2 velocity = new Vector2(_random.NextSingle(-100, 100), -raindropComponent.Velocity.Y * _random.NextSingle(0.1f, 0.2f));
                        int id = CreateRaindrop(raindropComponent.Position.SetY(impactPosition), velocity, (i + 1) * 0.5f);
                        _expiryComponent.Put(id, new ExpiryComponent(1f));
                    }

                    DestroyEntity(entity);
                }
            }

            _spawnDelay -= gameTime.GetElapsedSeconds();

            if(_spawnDelay <= 0)
            {
                for(int q = 0; q < 1; q++)
                {
                    Vector2 position = new Vector2(_random.NextSingle(0, 3840), 0f);//;_random.NextSingle(-240, -480));
                    CreateRaindrop(position);
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
                    Position = position,
                    Velocity = velocity,
                    Size = size 
                });

            return entity.Id;
        }
    }
}