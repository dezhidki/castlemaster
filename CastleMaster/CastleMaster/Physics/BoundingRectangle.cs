
using Microsoft.Xna.Framework;
namespace CastleMaster.Physics
{
    public class BoundingRectangle
    {
        private float x0, z0, x1, z1;
        private BoundingRectangleOwner owner;

        public BoundingRectangle(float x0, float z0, float x1, float z1, BoundingRectangleOwner owner)
        {
            this.owner = owner;
            this.x0 = x0;
            this.z0 = z0;
            this.x1 = x1;
            this.z1 = z1;
        }

        public BoundingRectangle(Point p0, Point p1, BoundingRectangleOwner owner)
        {
            this.owner = owner;
            this.x0 = p1.X < p0.X ? p1.X : p0.X;
            this.z0 = p1.Y < p0.Y ? p1.Y : p0.Y;
            this.x1 = p1.X > p0.X ? p1.X : p0.X;
            this.z1 = p1.Y > p0.Y ? p1.Y : p0.Y;
        }

        public float XLeft { get { return x0; } }

        public float XRight { get { return x1; } }

        public float ZFar { get { return z0; } }

        public float ZNear { get { return z1; } }

        public BoundingRectangleOwner Owner { get { return owner; } }

        public BoundingRectangle Update(float x0, float z0, float x1, float z1)
        {
            this.x0 = x0;
            this.z0 = z0;
            this.x1 = x1;
            this.z1 = z1;

            return this;
        }

        public bool Intersects(float x0, float z0, float x1, float z1)
        {
            if (x0 >= this.x1 || x1 <= this.x0 || z0 >= this.z1 || z1 <= this.z0) return false;
            return true;
        }

        public bool Intersects(BoundingRectangle br)
        {
            return Intersects(br.x0, br.z0, br.x1, br.z1);
        }

        public BoundingRectangle AddSelf(float x0, float z0, float x1, float z1)
        {
            this.x0 += x0;
            this.z0 += z0;
            this.x1 += x1;
            this.z1 += z1;

            return this;
        }

        public BoundingRectangle AddSelf(BoundingRectangle br)
        {
            this.x0 += br.x0;
            this.z0 += br.z0;
            this.x1 += br.x1;
            this.z1 += br.z1;

            return this;
        }

        public BoundingRectangle Translate(Vector2 vec)
        {
            this.x0 += vec.X;
            this.z0 += vec.Y;
            this.x1 += vec.X;
            this.z1 += vec.Y;

            return this;
        }

        public static BoundingRectangle operator +(BoundingRectangle br, Vector2 vec)
        {
            return new BoundingRectangle(br.x0 + vec.X, br.z0 + vec.Y, br.x1 + vec.X, br.z1 + vec.Y, br.owner);
        }

        public BoundingRectangle Scale(float s)
        {
            this.x0 *= s;
            this.z0 *= s;
            this.x1 *= s;
            this.z1 *= s;

            return this;
        }
    }
}
