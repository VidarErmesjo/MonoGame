namespace MonoGame.Extended.Entities
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Transform : IEquatable<Transform>
    {
        public Type Type { get; }
        public int Id { get; }
        public Vector2 Position { get; set; }

        public Transform(Type type, int id)
        {
            Type = type;
            Id = id;
            Position = new Vector2(0.0f, 0.0f);
        }

        public bool Equals(Transform other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Transform) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Transform left, Transform right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Transform left, Transform right)
        {
            return !Equals(left, right);
        }
    }
}
