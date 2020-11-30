using Microsoft.Xna.Framework;

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

            foreach(var entity in ActiveEntities)
            {
                RaindropComponent raindropComponent = _raindropComponent.Get(entity);

                raindropComponent.Velocity = new Vector2(0, VelocityOfRaindrop);
                raindropComponent.Position += raindropComponent.Velocity;// * elapsedSeconds;

                if(raindropComponent.Position.Y >= raindropComponent.ImpactPoint.Y && _expiryComponent.Get(entity).isPersistent)
                    _expiryComponent.Get(entity).isPersistent = false;

                if(!_expiryComponent.Has(entity))
                {
                    for(int i = 0; i < 5; i++)
                    {
                        Vector2 velocity = new Vector2(
                            _random.NextSingle(-100, 100),
                            -raindropComponent.Velocity.Y * _random.NextSingle(10f, 20f));
                        int id = CreateRaindrop(
                            raindropComponent.Position.SetY(raindropComponent.Position.Y),
                            velocity, (i + 1) * 0.5f);
                        _expiryComponent.Put(id, new ExpiryComponent(1f));
                    }

                    DestroyEntity(entity);
                }
            }

            _spawnDelay -= gameTime.GetElapsedSeconds();

            if(_spawnDelay <= 0)
            {
                for(int q = 0; q < 5; q++)
                {
                    Vector2 position = new Vector2(
                        _random.NextSingle(0, Core.VirtualResolution.Width),
                        _random.NextSingle(0, Core.VirtualResolution.Height));
                    int id = CreateRaindrop(position);
                    _expiryComponent.Put(
                        id,
                        new ExpiryComponent(1f, true));
                }
                
                _spawnDelay = _random.NextSingle(MinSpawnDelay, MaxSpawnDelay);
            }
        }

        private int CreateRaindrop(Vector2 position, Vector2 velocity = default(Vector2), float size = 1f)
        {
            Entity entity = CreateEntity();
            entity.Attach(
                new RaindropComponent
                {
                    Position = new Vector2(position.X + _camera.Position.X, 0 + _camera.Position.Y),
                    ImpactPoint = position + _camera.Position,
                    Velocity = velocity,
                    Size = size 
                });

            return entity.Id;
        }
    }
}