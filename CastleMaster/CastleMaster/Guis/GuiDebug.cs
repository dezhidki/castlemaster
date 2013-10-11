using CastleMaster.Graphics;
using Microsoft.Xna.Framework;
using System.Text;

namespace CastleMaster.Guis
{
    public class GuiDebug : Gui
    {
        private const string FPS_TEXT = "FPS: ";
        private const string UPS_TEXT = " UPS: ";
        private const string NEWLINE = "\n";
        private const string FIXEDTIMESTEP_TEXT = "IsFixedTimeStep: ";
        private const string DEBUGNOTATION = " (DEBUG)";

        private StringBuilder text;
        private Game game;

        public GuiDebug(GuiManager manager, Game game)
            : base(manager)
        {
            this.game = game;
            text = new StringBuilder();
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
            text.Clear();
            text.Append(Game.TITLE).Append(" ").Append(Game.VERSION);
#if DEBUG
            text.Append(DEBUGNOTATION);
#endif
            text.Append(NEWLINE).Append(FPS_TEXT).Append(game.FPS).Append(UPS_TEXT).Append(game.UPS);
            text.Append(NEWLINE).Append(FIXEDTIMESTEP_TEXT).Append(game.IsFixedTimeStep);
        }

        public override void Render(RenderHelper renderer)
        {
            renderer.SpriteBatch.DrawString(Resources.FONT, text, new Vector2(0, 0), Color.LightGray);
        }
    }
}
