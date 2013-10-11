using CastleMaster.Graphics;
using CastleMaster.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using System.Windows.Forms;

namespace CastleMaster.Guis
{
    public class GuiInstructions : Gui
    {
        private const String HELP_MOVE = "<LEFT, RIGHT> Switch Pages";
        private const String HELP_CLOSE = "<X> Main menu";
        private const String NAME = "Instructions";
        private const String PAGE = "Page";
        private const char AST = '/';

        private GuiMainScreen mainMenu;
        private Rectangle backGroundRect;
        private Vector2 helpPos, pageNumPos, helpPos2;
        private StringBuilder pageNumText;
        private int currentPage = 0;
        private Texture2D[] pages;

        public GuiInstructions(GuiMainScreen mainMenu, GuiManager manager)
            : base(manager)
        {
            this.mainMenu = mainMenu;
            backGroundRect = new Rectangle(0, 0, Game.WIDTH, Game.HEIGHT);
            helpPos = new Vector2(10, Game.HEIGHT - 50);

            pages = new Texture2D[] { Resources.SPRITE_PAGE_WELCOME, Resources.SPRITE_PAGE_CONTROLS, Resources.SPRITE_PAGE_GUI, Resources.SPRITE_PAGE_BUILDINGS, Resources.SPRITE_PAGE_UNITS };
            pageNumText = new StringBuilder(PAGE).Append(" 1/").Append(pages.Length);
            pageNumPos = new Vector2(Game.WIDTH - 150, Game.HEIGHT - 50);

            helpPos2 = new Vector2(helpPos.X, helpPos.Y + 16);
        }

        public override int Importance
        {
            get
            {
                return Gui.IMPORTANCE_HIGH;
            }
        }

        private void ShowMainMenu()
        {
            Remove();
            manager.AddGui(mainMenu, true, true);
        }

        public override void Update()
        {
            if (InputHandler.HasKeyBeenPressed(Keys.X))
                ShowMainMenu();

            if (InputHandler.HasKeyBeenPressed(Keys.Right))
            {
                currentPage++;
                if (currentPage >= pages.Length)
                    currentPage = 0;

                pageNumText.Clear().Append(PAGE).Append(' ').Append(currentPage + 1).Append(AST).Append(pages.Length);
            }
            else if (InputHandler.HasKeyBeenPressed(Keys.Left))
            {
                currentPage--;
                if (currentPage < 0)
                    currentPage = pages.Length - 1;
                pageNumText.Clear().Append(PAGE).Append(' ').Append(currentPage + 1).Append(AST).Append(pages.Length);
            }

        }

        public override void Render(RenderHelper renderer)
        {
            renderer.SpriteBatch.Draw(pages[currentPage], backGroundRect, Color.White);

            renderer.SpriteBatch.DrawString(Resources.FONT, HELP_CLOSE, helpPos, Color.White);
            renderer.SpriteBatch.DrawString(Resources.FONT, pageNumText, pageNumPos, Color.White);
            renderer.SpriteBatch.DrawString(Resources.FONT, HELP_MOVE, helpPos2, Color.White);
        }
    }

    internal class Page
    {
        public virtual void Render(RenderHelper renderer) { }
    }

    internal class Page1 : Page
    {
        private const string TEST = "Hello, world!";

        public override void Render(RenderHelper renderer)
        {
            renderer.SpriteBatch.DrawString(Resources.FONT, TEST, new Vector2(10, 100), Color.White);
        }
    }
}
