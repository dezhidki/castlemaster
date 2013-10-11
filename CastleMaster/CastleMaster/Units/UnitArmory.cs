using CastleMaster.Graphics;
using CastleMaster.Guis;
using CastleMaster.Players;
using CastleMaster.Units.Mobs;
using CastleMaster.World;
using System;
using System.Collections.Generic;

namespace CastleMaster.Units
{
    public class UnitArmory : UnitBuilding
    {
        public const int PRICE_WOODCUTTER = 5;
        public const int PRICE_WARRIOR = 20;
        public const int PRICE_RANGER = 10;
        private Dictionary<Type, int> prices;
        private GuiArmoryMenu menu;
        private int spriteX = 0;

        public UnitArmory(Level level, Player owner)
            : base(level, owner)
        {
            width = 5 * Viewport.TILESIZE;
            depth = 5 * Viewport.TILESIZE;
            HasHealth = true;
            maxHealth = 400;

            renderOffset.X = 80;
            renderOffset.Y = 58;

            spriteSize.X = 160;
            spriteSize.Y = 98;
            screenRectOffset.Update(32, 0, spriteSize.X - 32, spriteSize.Y - 10);

            highlightOffset.X = spriteSize.X / 2 - 4;

            rectOffset.Update(-2 * Viewport.TILESIZE, -2 * Viewport.TILESIZE, 2 * Viewport.TILESIZE, 2 * Viewport.TILESIZE);

            prices = new Dictionary<Type, int>();
            prices.Add(typeof(MobWoodcutter), PRICE_WOODCUTTER);
            prices.Add(typeof(MobWarrior), PRICE_WARRIOR);
            prices.Add(typeof(MobRanger), PRICE_RANGER);

            menu = new GuiArmoryMenu(Game.GuiManager, this);
        }

        public Dictionary<Type, int> Prices { get { return prices; } }

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

        public T BuyUnit<T>(Type toBuy = null) where T : Mob
        {
            Type unitType = toBuy == null ? typeof(T) : toBuy;
            int cost = prices[unitType];
            if (Owner.CoinsAmount - cost < 0) return null;
            T m = (T)Activator.CreateInstance(unitType, level, Owner);
            Owner.CoinsAmount -= cost;
            return m;
        }

        public override void Remove()
        {
            base.Remove();
            level.RemoveUnit(this);
            spriteX = 1;
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.Render(ScreenPos, spriteX, 0, Resources.SPRITE_ARMORY, colorizer, Viewport.ZOOM);
            base.Render(renderer);
        }
    }
}
