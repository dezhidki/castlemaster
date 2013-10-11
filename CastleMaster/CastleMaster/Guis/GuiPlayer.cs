using CastleMaster.Graphics;
using CastleMaster.Players;
using CastleMaster.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace CastleMaster.Guis
{
    public class GuiPlayer : Gui
    {
        private class HealthMeter
        {
            private Unit unit;

            private Rectangle unitHealthGreenRectangle, unitHealthRedRectangle;
            private Vector2 unitHealthPos, unitRedHealthPos, unitTextPos;
            private int oldUnitHealthBarWidth = HEALTH_BAR_WIDTH;
            private string text;
            private Color textColor;

            public HealthMeter(Unit unit, Vector2 renderPos, string text, Color textColor)
            {
                this.text = text;
                this.unit = unit;
                this.textColor = textColor;
                unitHealthGreenRectangle = new Rectangle(0, 0, HEALTH_BAR_WIDTH, 4);
                unitHealthRedRectangle = new Rectangle(HEALTH_BAR_WIDTH, 4, 0, 4);
                unitHealthPos = renderPos;
                unitRedHealthPos = new Vector2(unitHealthPos.X + HEALTH_BAR_WIDTH, unitHealthPos.Y);
                unitTextPos = new Vector2(unitHealthPos.X - text.Length * 13, unitHealthPos.Y - 5);
            }

            public void Update()
            {
                unitHealthGreenRectangle.Width = (int)((unit.Health / (float)unit.MaxHealth) * HEALTH_BAR_WIDTH);
                if (unitHealthGreenRectangle.Width != oldUnitHealthBarWidth)
                {
                    oldUnitHealthBarWidth = unitHealthGreenRectangle.Width;
                    unitRedHealthPos.X = unitHealthPos.X + unitHealthGreenRectangle.Width * SCALE;
                    unitHealthRedRectangle.X = unitHealthGreenRectangle.Width;
                    unitHealthRedRectangle.Width = HEALTH_BAR_WIDTH - unitHealthGreenRectangle.Width;
                }
            }

            public void Render(RenderHelper renderer)
            {
                renderer.SpriteBatch.DrawString(Resources.FONT, text, unitTextPos, textColor);
                if (unitHealthGreenRectangle.Width > 0)
                    renderer.SpriteBatch.Draw(renderer.RegisteredSpriteSheets[Resources.SPRITESHEET_HEALTH].SheetTexture, unitHealthPos, unitHealthGreenRectangle, Color.White, 0.0F, Vector2.Zero, SCALE, SpriteEffects.None, 0);
                if (unitHealthRedRectangle.Width > 0)
                    renderer.SpriteBatch.Draw(renderer.RegisteredSpriteSheets[Resources.SPRITESHEET_HEALTH].SheetTexture, unitRedHealthPos, unitHealthRedRectangle, Color.White, 0.0F, Vector2.Zero, SCALE, SpriteEffects.None, 0);
            }
        }

        private const string NEWLINE = "\n";
        private const string ICON_COINS = "&";
        private const string ICON_WOOD = "#";
        private const string KING_TEXT = "KING HEALTH: ";
        private const string KING_ENEMY_TEXT = "ENEMY KING HEALTH: ";
        private const string STORE_TEXT = "STORE HEALTH: ";
        private const string ARMORY_TEXT = "ARMORY HEALTH: ";
        public const int GUI_BAR_HEIGHT = 100;
        private const int HEALTH_BAR_WIDTH = 100;
        private const int SCALE = 2;

        private Player player;
        private StringBuilder itemStats;
        private Color guiCol;
        private Rectangle screenRect;
        private Vector2 itemStatsPos;

        private HealthMeter kingHealthMeter, storeHealthMeter, armoryHealthMeter, enemyKingHealth;

        public GuiPlayer(GuiManager manager, Player player)
            : base(manager)
        {
            this.player = player;
            itemStats = new StringBuilder();
            guiCol = new Color(165, 89, 38);
            screenRect = new Rectangle(0, Game.HEIGHT - GUI_BAR_HEIGHT, Game.WIDTH, GUI_BAR_HEIGHT);
            itemStatsPos = new Vector2(0, Game.HEIGHT - GUI_BAR_HEIGHT + 5);

            kingHealthMeter = new HealthMeter(player.King, new Vector2(Game.WIDTH - HEALTH_BAR_WIDTH * SCALE - 10, Game.HEIGHT - GUI_BAR_HEIGHT + 20), KING_TEXT, Color.White);
            enemyKingHealth = new HealthMeter(Game.GetEnemyPlayer(player).King, new Vector2(Game.WIDTH - HEALTH_BAR_WIDTH * SCALE - 10, Game.HEIGHT - GUI_BAR_HEIGHT + 40), KING_ENEMY_TEXT, new Color(160, 0, 0));
            storeHealthMeter = new HealthMeter(player.Store, new Vector2(Game.WIDTH - HEALTH_BAR_WIDTH * SCALE - 10, Game.HEIGHT - GUI_BAR_HEIGHT + 60), STORE_TEXT, Color.White);
            armoryHealthMeter = new HealthMeter(player.Armory, new Vector2(Game.WIDTH - HEALTH_BAR_WIDTH * SCALE - 10, Game.HEIGHT - GUI_BAR_HEIGHT + 80), ARMORY_TEXT, Color.White);
        }

        public override int Importance
        {
            get
            {
                return IMPORTANCE_HIGH;
            }
        }


        public override void Update()
        {
            itemStats.Clear();
            itemStats.Append(ICON_COINS).Append(player.CoinsAmount).Append(NEWLINE);
            itemStats.Append(ICON_WOOD).Append(player.LumberAmount);
            kingHealthMeter.Update();
            storeHealthMeter.Update();
            armoryHealthMeter.Update();
            enemyKingHealth.Update();
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.SpriteBatch.Draw(RenderHelper.EmptyTexture, screenRect, guiCol);
            renderer.SpriteBatch.DrawString(Resources.FONT, itemStats, itemStatsPos, Color.White, 0.0F, Vector2.Zero, 1.5F, SpriteEffects.None, 0);

            kingHealthMeter.Render(renderer);
            storeHealthMeter.Render(renderer);
            armoryHealthMeter.Render(renderer);
            enemyKingHealth.Render(renderer);
        }
    }
}
