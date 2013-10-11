using CastleMaster.Graphics;
using CastleMaster.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using System.Windows.Forms;

namespace CastleMaster.Guis
{
    public class GuiMainScreen : Gui
    {
        private const string ARROW = "@";
        //private const string DEMO_TEXT = " Demo";
        private const string HELP1 = "<UP,DOWN> SCROLL";
        private const string HELP2 = "<ENTER> SELECT";
        private const string AUTHOR = "Game by Denis Zhidkikh";

        private Game game;
        private string[] optionsStart = { "Start", "How to play", "Exit" };
        private string[] optionsInGame = { "Continue", "Main menu", "Exit" };
        private Action[] actionsStart, actionsInGame;
        private int currentOption = 0;
        private Rectangle backgroundRect;
        private Vector2 logoPos, infoPos, helpPos, optionPos, optionOffset, arrowOffset;
        private StringBuilder infoText, helpText;
        private GuiInstructions guiInstructions;

        public GuiMainScreen(GuiManager manager, Game game)
            : base(manager)
        {
            this.game = game;

            guiInstructions = new GuiInstructions(this, manager);

            actionsStart = new Action[] { game.StartGame, ShowInstructions, game.Exit };
            actionsInGame = new Action[] { ReturnToGame, ReturnToMain, game.Exit };
            backgroundRect = new Rectangle(0, 0, Game.WIDTH, Game.HEIGHT);
            logoPos = new Vector2(Game.WIDTH / 2 - 225, 10);

            infoText = new StringBuilder(Game.VERSION);
            infoText.Append("\n").Append(AUTHOR);
            infoPos = new Vector2(Game.WIDTH - 320, Game.HEIGHT - 50);

            helpText = new StringBuilder(HELP1).Append("\n").Append(HELP2);
            helpPos = new Vector2(10, Game.HEIGHT - 50);

            optionPos = new Vector2(40, Game.HEIGHT / 2 - 40);
            optionOffset = new Vector2(0, 25);
            arrowOffset = new Vector2(-35, 0);
        }

        public override int Importance
        {
            get
            {
                return IMPORTANCE_HIGH;
            }
        }

        public void ShowInstructions()
        {
            Remove();
            manager.AddGui(guiInstructions, true, true);
        }

        public void ReturnToMain()
        {
            game.ResetGame();
            manager.AddGui(this, true, true);
        }

        public void ReturnToGame()
        {
            Remove();
            game.IsGamePaused = false;
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
                    if (game.IsGameRunning)
                        actionsInGame[currentOption].Invoke();
                    else
                        actionsStart[currentOption].Invoke();
                }
        }

        public override void Render(RenderHelper renderer)
        {
            if (!game.IsGameRunning)
            {
                    renderer.SpriteBatch.Draw(Resources.SPRITE_GUI_BACKGROUND, backgroundRect, Color.White);
                    renderer.SpriteBatch.Draw(Resources.SPRITE_GUI_LOGO, logoPos, Color.White);
                    renderer.SpriteBatch.DrawString(Resources.FONT, infoText, infoPos, Color.White);
                    renderer.SpriteBatch.DrawString(Resources.FONT, helpText, helpPos, Color.White);

                    for (int i = 0; i < optionsStart.Length; i++)
                    {
                        if (i == currentOption)
                            renderer.SpriteBatch.DrawString(Resources.FONT, ARROW, optionPos + optionOffset * i + arrowOffset, Color.White, 0.0F, Vector2.Zero, 1.5F, SpriteEffects.None, 0.0F);
                        renderer.SpriteBatch.DrawString(Resources.FONT, optionsStart[i], optionPos + optionOffset * i, Color.White, 0.0F, Vector2.Zero, 1.5F, SpriteEffects.None, 0.0F);
                    }
            }
            else
            {
                    renderer.SpriteBatch.Draw(Resources.SPRITE_GUI_BACKGROUND_INGAME, backgroundRect, Color.White);
                    renderer.SpriteBatch.DrawString(Resources.FONT, helpText, helpPos, Color.White);
                    for (int i = 0; i < optionsInGame.Length; i++)
                    {
                        if (i == currentOption)
                            renderer.SpriteBatch.DrawString(Resources.FONT, ARROW, optionPos + optionOffset * i + arrowOffset, Color.White, 0.0F, Vector2.Zero, 1.5F, SpriteEffects.None, 0.0F);
                        renderer.SpriteBatch.DrawString(Resources.FONT, optionsInGame[i], optionPos + optionOffset * i, Color.White, 0.0F, Vector2.Zero, 1.5F, SpriteEffects.None, 0.0F);
                    }
                
            }

            
        }
    }

}
