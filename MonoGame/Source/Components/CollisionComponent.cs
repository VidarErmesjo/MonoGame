using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Entities.Systems;
using System.Collections.Generic;

namespace MonoGame.Components
{
    public class Collision
    {
        // Needs to update Bounds.Position from sprites Position. How?
        // Do for multiple shapes => Head, Body etc.
        public int entityId;
        public IShapeF Head;
        public IShapeF Torso;
        public IList<IShapeF> Limbs;
    }
}