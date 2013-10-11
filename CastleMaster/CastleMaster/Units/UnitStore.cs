using CastleMaster.Graphics;
using CastleMaster.Guis;
using CastleMaster.Players;
using CastleMaster.World;

namespace CastleMaster.Units
{
    public class UnitStore : UnitBuilding
    {
        public const int COINS_FOR_LUMBER = 2;
        private GuiStoreMenu menu;
        private int spriteX = 0;

        public UnitStore(Level level, Player owner)
            : base(level, owner)
        {
            width = 8 * Viewport.TILESIZE;
            depth = 8 * Viewport.TILESIZE;
            HasHealth = true;
            maxHealth = 400;

            renderOffset.X = 128;
            renderOffset.Y = 83;

            spriteSize.X = 240;
            spriteSize.Y = 147;
            screenRectOffset.Update(32, 0, spriteSize.X - 32, spriteSize.Y - 10);

            highlightOffset.X = spriteSize.X / 2 - 4;

            rectOffset.Update(-4 * Viewport.TILESIZE, -4 * Viewport.TILESIZE, 4 * Viewport.TILESIZE, 4 * Viewport.TILESIZE);

            menu = new GuiStoreMenu(Game.GuiManager, this);
        }

        public void SellWood()
        {
            while (Owner.LumberAmount >= 1)
            {
                Owner.LumberAmount--;
                Owner.CoinsAmount += COINS_FOR_LUMBER;
            }
        }

        public override void OnFocus()
        {
            if (!isDestroyed)
                Game.GuiManager.AddGui(menu, true);
        }

        public override void OnFocusLost()
        {
            if (!isDestroyed)
                Game.GuiManager.RemoveGui(menu);
        }

        public override void Remove()
        {
            base.Remove();
            menu.Remove();
            level.RemoveUnit(this);
            spriteX = 1;
        }

        public override void Render(RenderHelper renderer)
        {
            base.Render(renderer);
            renderer.Render(ScreenPos, spriteX, 0, Resources.SPRITE_STORE, colorizer, Viewport.ZOOM);
        }
    }
}
