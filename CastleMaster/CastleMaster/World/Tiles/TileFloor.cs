using CastleMaster.Graphics;
using Microsoft.Xna.Framework;

namespace CastleMaster.World.Tiles
{
    public class TileFloor : Tile
    {
        public TileFloor(Level level, bool isSolid = false)
            : base(level)
        {
            this.IsSolid = isSolid;
        }

        public override void Render(RenderHelper renderer, Level level, Vector2 screenPos, int tileX, int tileZ, byte dataVal)
        {
            renderer.Render(screenPos, dataVal % 8, dataVal / 8, Resources.SPRITESHEET_TILES, Viewport.ZOOM);
        }
    }
}
