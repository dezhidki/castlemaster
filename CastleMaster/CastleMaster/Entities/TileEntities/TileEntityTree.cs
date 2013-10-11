using CastleMaster.Graphics;
using CastleMaster.World;

namespace CastleMaster.Entities.TileEntities
{
    public class TileEntityTree : TileEntity
    {
        private int spriteID;

        public TileEntityTree(Level level, int spriteID, int logAmount)
            : base(level)
        {
            this.spriteID = spriteID;

            renderOffset.X = 6;
            renderOffset.Y = 18;

            AvaliableLogs = logAmount;
        }

        public int AvaliableLogs { get; set; }

        public override void Render(RenderHelper renderer)
        {
            renderer.Render(ScreenPos, spriteID, 7, Resources.SPRITESHEET_TILES, Viewport.ZOOM);
        }
    }
}
