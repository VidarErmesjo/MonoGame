using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame
{
    public class TestComponent : IEquatable<TestComponent>
    {
        public Type Type { get; }
        public int Id { get; }

        public float charge { get; set; }
        public bool isCharging { get; private set; }
        public Vector2 origin { get ; set; }
        public Vector2 destination { get; set; }

        public TestComponent(Type type, int id)
        {
            Type = type;
            Id = id;
            charge = 0.0f;
            isCharging = false;
            destination = new Vector2(0.0f, 0.0f);
        }

        public void toggleIsCharging()
        {
            isCharging = !isCharging;
        }

        public bool Equals(TestComponent other)
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
            return Equals((TestComponent) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(TestComponent left, TestComponent right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestComponent left, TestComponent right)
        {
            return !Equals(left, right);
        }
    }
}
