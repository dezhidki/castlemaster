using CastleMaster.Entities;
using CastleMaster.Graphics;
using Microsoft.Xna.Framework;

namespace CastleMaster.World.Tiles
{
    public class TileRenderHint : Tile
    {
        private Entity entToRender;
        private bool renderTile;

        public TileRenderHint(Level level, Entity entToRender, bool isSolid, bool renderTile)
            : base(level)
        {
            this.entToRender = entToRender;
            this.renderTile = renderTile;
            IsSolid = isSolid;
        }

        public override void Render(RenderHelper renderer, Level level, Vector2 screenPos, int tileX, int tileZ, byte dataVal)
        {
            if (renderTile)
                renderer.Render(screenPos, dataVal % 8, dataVal / 8, Resources.SPRITESHEET_TILES, Viewport.ZOOM);
            level.AddEntityToRenderList(entToRender);
        }
    }
}
