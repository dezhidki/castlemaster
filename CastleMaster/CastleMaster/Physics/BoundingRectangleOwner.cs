
namespace CastleMaster.Physics
{
    public interface BoundingRectangleOwner
    {
        void OnTouched(BoundingRectangleOwner touching);

        void OnTouchedBy(BoundingRectangleOwner toucher);

        void OnCollidedWith(BoundingRectangleOwner colliding);

        void OnCollidedBy(BoundingRectangleOwner collidable);
    }
}
