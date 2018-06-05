using Microsoft.Xna.Framework;

namespace Gridly
{
    public abstract class Collidable
    {
        public Vector2 Position { get; set; }
        protected Vector2 velocity;
        protected float friction = 0;
        public abstract Rectangle GetBounds();

        public void UpdatePhysics()
        {
            Position += velocity;
            velocity -= velocity * friction;
        }

        public void AddForce(Vector2 force, float power = 1f)
            => velocity += force * power;

        public void AddForceTo(Vector2 pos, float power = 1f)
            => AddForce(pos - Position, power);
    }
}
