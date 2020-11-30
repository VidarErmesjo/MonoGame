using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public class ExpirySystem : EntityProcessingSystem
    {
        private ComponentMapper<ExpiryComponent> _expiryMapper;

        public ExpirySystem() : base(Aspect.All(typeof(ExpiryComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _expiryMapper = mapperService.GetMapper<ExpiryComponent>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            ExpiryComponent expiryComponent = _expiryMapper.Get(entityId);
            
            if(expiryComponent.isPersistent)
                return;

            expiryComponent.TimeRemaining -= gameTime.GetElapsedSeconds();
            if(expiryComponent.TimeRemaining < 0)
                DestroyEntity(entityId);
        }
    }
}