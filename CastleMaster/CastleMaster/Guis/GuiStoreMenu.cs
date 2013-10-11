using CastleMaster.Graphics;
using CastleMaster.Input;
using CastleMaster.Sound;
using CastleMaster.Units;
using Microsoft.Xna.Framework;
using System.Text;
using System.Windows.Forms;

namespace CastleMaster.Guis
{
    public class GuiStoreMenu : Gui
    {
        private const string NAME = "Lumber Exchange";
        private const string HELP_TEXT = "<ENTER> Sell all wood";
        private const string PRICE_TEXT1 = "#1 yields &";

        private UnitStore store;

        private StringBuilder priceText;
        private Vector2 renderPos, helpRenderPos, pricePos, namePos;

        public GuiStoreMenu(GuiManager manager, UnitStore store)
            : base(manager)
        {
            this.store = store;
            renderPos = new Vector2(10, 10);
            namePos = new Vector2(renderPos.X + 60, renderPos.Y + 10);
            helpRenderPos = new Vector2(renderPos.X + 5, renderPos.Y + 120);
            pricePos = new Vector2(renderPos.X + 5, renderPos.Y + 60);
            priceText = new StringBuilder(PRICE_TEXT1).Append(UnitStore.COINS_FOR_LUMBER);
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
            if (InputHandler.HasKeyBeenPressed(Keys.Enter))
            {
                store.SellWood();
                Audio.PlaySound("shop");
            }
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.SpriteBatch.Draw(Resources.SPRITE_GUI_UNITMENU, renderPos, Color.White);
            renderer.SpriteBatch.DrawString(Resources.FONT, NAME, namePos, Color.White);
            renderer.SpriteBatch.DrawString(Resources.FONT, HELP_TEXT, helpRenderPos, Color.White);
            renderer.SpriteBatch.DrawString(Resources.FONT, priceText, pricePos, Color.White);
        }
    }
}
