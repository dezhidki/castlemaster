using CastleMaster.Entities;
using CastleMaster.Graphics;
using CastleMaster.Physics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CastleMaster.World.Tiles
{
    public class Tile : BoundingRectangleOwner
    {
        private int id;

        public Tile(Level level)
        {
            IsSolid = false;
            level.RegisteredTiles.Add(this);
            id = level.RegisteredTiles.Count - 1;
        }

        public int ID { get { return id; } }

        public bool IsSolid { get; protected set; }

        public virtual bool IsSolidTo(Entity ent)
        {
            return IsSolid && !(ent is EntityArrow);
        }

        public virtual BoundingRectangle GetBoundingRect(int xTile, int zTile)
        {
            float x = xTile * Viewport.TILESIZE;
            float z = zTile * Viewport.TILESIZE;

            return new BoundingRectangle(x, z, x + Viewport.TILESIZE, z + Viewport.TILESIZE, this);
        }

        public virtual void AddBoundingRect(ref List<BoundingRectangle> list, int xTile, int zTile)
        {
            list.Add(GetBoundingRect(xTile, zTile));
        }

        public virtual void Update(Level level, int tileX, int tileZ) { }

        public virtual void Render(RenderHelper renderer, Level level, Vector2 screenPos, int tileX, int tileZ, byte dataVal) { }

        public virtual void OnTouched(BoundingRectangleOwner touching) { }

        public virtual void OnTouchedBy(BoundingRectangleOwner toucher) { }

        public virtual void OnCollidedWith(BoundingRectangleOwner colliding) { }

        public virtual void OnCollidedBy(BoundingRectangleOwner collidable) { }
    }
}
