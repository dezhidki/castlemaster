using CastleMaster.Graphics;
using Microsoft.Xna.Framework;

namespace CastleMaster.World.Tiles
{
    public class TileWater : Tile
    {
        public TileWater(Level level)
            : base(level)
        {
            IsSolid = true;
        }

        public override void Render(RenderHelper renderer, Level level, Vector2 screenPos, int tileX, int tileZ, byte dataVal)
        {
            renderer.Render(screenPos, level.WaterTimer, 1, Resources.SPRITESHEET_TILES, Viewport.ZOOM);
        }
    }
}
