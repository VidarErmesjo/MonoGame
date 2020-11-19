namespace MonoGame.Extended.Entities.Systems
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using SpriteManager;

    public class RenderSystem : EntityDrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private Entity _player;

        public RenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(SpriteSheet)))
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp
                //transformMatrix: transformMatrix
            );

            foreach(var entity in ActiveEntities)
            {
                
            }

            _spriteBatch.End();
            //System.Console.WriteLine("ActiveEntities: " + ActiveEntities.Count);
        }
    }
}