using Microsoft.Xna.Framework;

namespace MonoGame.Components
{
    public class ActorComponent
    {
        public readonly int Id;
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        public ActorComponent(int id, Vector2 position = default(Vector2), Vector2 velocity = default(Vector2))
        {
            this.Id = id;
            this.Position = position == null ? Vector2.Zero : new Vector2(position.X, position.Y);
            this.Velocity = velocity == null ? Vector2.Zero : new Vector2(velocity.X, velocity.Y);
        }        
    }
}