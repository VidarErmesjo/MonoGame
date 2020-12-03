using Microsoft.Xna.Framework;

namespace MonoGame.Components
{
    public class PlayerComponent : ActorComponent
    {
        public string Name { get; private set; }
        public int Health { get; private set; }

        public PlayerComponent(int id, Vector2 position = default(Vector2), Vector2 velocity = default(Vector2))
            : base(id, position, velocity)
        {
        }
    }
}