using CastleMaster.Graphics;
using CastleMaster.Input;
using CastleMaster.Sound;
using CastleMaster.Units;
using CastleMaster.Units.Mobs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Windows.Forms;
using Viewport = CastleMaster.Graphics.Viewport;

namespace CastleMaster.Guis
{
    public class GuiArmoryMenu : Gui
    {
        private const string NAME = "ARMORY";
        private const string HELP = "<UP,DOWN> Select       <ENTER> Buy";
        private const string ARROW = "@";
        private const string COIN = "&";

        private UnitArmory armory;

        private Type[] availableUnits = { typeof(MobWoodcutter), typeof(MobWarrior), typeof(MobRanger) };
        private string[] unitNames = { "Woodcutter", "Warrior", "Ranger" };
        private string[] unitPrices = { COIN + UnitArmory.PRICE_WOODCUTTER, COIN + UnitArmory.PRICE_WARRIOR, COIN + UnitArmory.PRICE_RANGER };

        private int currentOption;
        private Vector2 renderPos, helpPos, unitNamePos, unitNameOffset, unitPriceOffset, arrowOffset;

        public GuiArmoryMenu(GuiManager manager, UnitArmory armory)
            : base(manager)
        {
            this.armory = armory;
            currentOption = 0;

            renderPos = new Vector2(10, 10);
            helpPos = new Vector2(renderPos.X + 5, renderPos.Y + 130);
            unitNamePos = new Vector2(renderPos.X + 40, renderPos.Y + 50);
            unitNameOffset = new Vector2(0, 20);
            unitPriceOffset = new Vector2(240, 0);
            arrowOffset = new Vector2(-25, 0);
        }

        public override int Importance
        {
            get
            {
                return IMPORTANCE_MEDIUM;
            }
        }

        public override void Update()
        {
            if (InputHandler.HasKeyBeenPressed(Keys.Up))
            {
                currentOption--;
                if (currentOption < 0) currentOption = 2;
            }
            else if (InputHandler.HasKeyBeenPressed(Keys.Down))
            {
                currentOption++;
                if (currentOption > 2) currentOption = 0;
            }
            else if (InputHandler.HasKeyBeenPressed(Keys.Enter))
            {
                Mob m = armory.BuyUnit<Mob>(availableUnits[currentOption]);
                if (m != null)
                {
                    Point spawnPoint = armory.Owner.SpawnPoints[currentOption];

                    bool solid = true;
                    int xTile = 0, zTile = 0;
                    while (solid)
                    {
                        xTile = spawnPoint.X + (int)(Math.Sin(Game.Random.NextDouble() * MathHelper.TwoPi) * Game.Random.NextDouble() * 4.0F);
                        zTile = spawnPoint.Y + (int)(Math.Cos(Game.Random.NextDouble() * MathHelper.TwoPi) * Game.Random.NextDouble() * 4.0F);
                        if (xTile < 0 || xTile >= armory.Owner.Level.Width || zTile < 0 || zTile >= armory.Owner.Level.Height) continue;
                        if (!armory.Owner.Level.SolidTo(m, xTile, zTile)) solid = false;
                    }

                    armory.Owner.Level.AddEntity(m, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
                    Audio.PlaySound("shop");
                }
            }
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.SpriteBatch.Draw(Resources.SPRITE_GUI_UNITMENU, renderPos, Color.White);
            renderer.SpriteBatch.DrawString(Resources.FONT, HELP, helpPos, Color.White, 0.0F, Vector2.Zero, 0.8F, SpriteEffects.None, 0.0F);

            for (int i = 0; i < unitNames.Length; i++)
            {
                if (i == currentOption)
                    renderer.SpriteBatch.DrawString(Resources.FONT, ARROW, unitNamePos + unitNameOffset * i + arrowOffset, Color.White);
                renderer.SpriteBatch.DrawString(Resources.FONT, unitNames[i], unitNamePos + unitNameOffset * i, Color.White);
                renderer.SpriteBatch.DrawString(Resources.FONT, unitPrices[i], unitNamePos + unitNameOffset * i + unitPriceOffset, Color.White);
            }
        }
    }
}
