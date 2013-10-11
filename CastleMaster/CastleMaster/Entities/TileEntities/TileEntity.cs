using CastleMaster.Graphics;
using CastleMaster.World;

namespace CastleMaster.Entities.TileEntities
{
    public class TileEntity : Entity
    {
        public TileEntity(Level level)
            : base(level)
        {
            width = Viewport.TILESIZE;
            depth = Viewport.TILESIZE;
            isSolid = true;
        }

        public int XTile { get { return (int)(X / Viewport.TILESIZE); } }

        public int ZTile { get { return (int)(Z / Viewport.TILESIZE); } }
    }
}
