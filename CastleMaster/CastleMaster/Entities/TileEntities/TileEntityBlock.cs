using CastleMaster.Graphics;
using CastleMaster.World;

namespace CastleMaster.Entities.TileEntities
{
    public class TileEntityBlock : TileEntity
    {
        private int spriteX, spriteY;

        public TileEntityBlock(Level level, int spriteX, int spriteY)
            : base(level)
        {
            this.spriteX = spriteX;
            this.spriteY = spriteY;

            renderOffset.Y = 16;
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.Render(ScreenPos, spriteX, spriteY, Resources.SPRITESHEET_TILES, Viewport.ZOOM);
        }
    }
}
