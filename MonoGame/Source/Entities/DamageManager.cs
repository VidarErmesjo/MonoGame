using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Entities
{
    public class DamageManager
    {
        private readonly Dictionary<string, float> _parts;

        public Dictionary<string, float> Parts
        {
            get => _parts;
        }

        public DamageManager(List<string> parts)
        {
            _parts = new Dictionary<string, float>();
            foreach(var part in parts)
                _parts.Add(part, 0f);
        }

        public void AddDamage(string part, float damage)
        {
            _parts[part] += damage;
        }

        public void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            foreach(var part in _parts.Where(x => x.Value > 0f))
                _parts[part.Key] *= deltaTime;
        }
    }
}