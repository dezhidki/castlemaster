using CastleMaster.Graphics;
using CastleMaster.Input;
using CastleMaster.Players;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace CastleMaster.Guis
{
    public class GuiWinLooseMessage : Gui
    {
        private const string WIN = "Congratulations!\n\nYou win!";
        private const string LOOSE = "Oh noes!\n\nYou loose!";
        private const string HELP = "<ESC> Main menu";

        private Game game;
        private Team winner;
        private Vector2 messagePos, helpPos, textPos;
        private string text;

        public GuiWinLooseMessage(GuiManager manager, Game game, Team winner)
            : base(manager)
        {
            this.game = game;
            this.winner = winner;

            text = winner.ID == Game.TEAM1.ID ? WIN : LOOSE;

            messagePos = new Vector2(Game.WIDTH / 2 - 165, Game.HEIGHT / 2 - 75);
            helpPos = new Vector2(messagePos.X + 5, messagePos.Y + 130);
            textPos = new Vector2(messagePos.X + 5, messagePos.Y + 10);
        }

        public override void Update()
        {
            if (InputHandler.HasKeyBeenPressed(Keys.Escape))
            {
                Remove();
                game.OpenMainMenu();
            }
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.SpriteBatch.Draw(Resources.SPRITE_GUI_UNITMENU, messagePos, Color.White);
            renderer.SpriteBatch.DrawString(Resources.FONT, HELP, helpPos, Color.White);
            renderer.SpriteBatch.DrawString(Resources.FONT, text, textPos, Color.White);
        }
    }
}
