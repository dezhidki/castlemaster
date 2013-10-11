using CastleMaster.Graphics;
using CastleMaster.Guis;
using CastleMaster.Input;
using CastleMaster.Players;
using CastleMaster.Sound;
using CastleMaster.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpNeatLib.Maths;
using System;
using System.Windows.Forms;
using Keys = System.Windows.Forms.Keys;
using Viewport = CastleMaster.Graphics.Viewport;

namespace CastleMaster
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public const int WIDTH = 800;
        public const int HEIGHT = 600;
        public const string TITLE = "Castle Master";
        public const string VERSION = "Beta 1.0";

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputHandler input;
        private RenderHelper renderer;
        private Camera camera;
        private LevelForest level;
        private static Player[] players;
        private static GuiManager guiManager;
        private GuiDebug debugScreen;
        private GuiPlayer ingame;
        private GuiMainScreen mainScreen;
        private bool isGameRunning = false;
        private static Game instance;
        private Audio sound;

        #region Counter
        private readonly TimeSpan ONE_SECOND = TimeSpan.FromSeconds(1.0);
        private TimeSpan currentTime = TimeSpan.Zero;
        private int frames = 0, updates = 0, fps = 0, ups = 0;
        #endregion

        public static FastRandom Random { get; private set; }

        public static Team TEAM1 { get; private set; }
        public static Team TEAM2 { get; private set; }

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            input = new InputHandler(Window);
            Random = new FastRandom();
            players = new Player[2];
            guiManager = new GuiManager();
            IsGamePaused = true;
            instance = this;
        }

        public static GuiManager GuiManager { get { return guiManager; } }

        public int FPS { get { return fps; } }

        public int UPS { get { return ups; } }

        public bool IsGameRunning { get { return isGameRunning; } }

        public bool IsGamePaused { get; set; }

        public static Game Instance { get { return instance; } }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            // NOTE: If enabled on some computers, the FPS will drop too much to play
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
            Window.Title = TITLE;

            InitializeInput();

            base.Initialize();
        }

        private void InitializeInput()
        {
            input.RegisterKeyboardKey(Keys.Escape);
            input.RegisterKeyboardKey(Keys.W);
            input.RegisterKeyboardKey(Keys.A);
            input.RegisterKeyboardKey(Keys.S);
            input.RegisterKeyboardKey(Keys.D);
            input.RegisterKeyboardKey(Keys.F4);
            input.RegisterKeyboardKey(Keys.F1);
            input.RegisterKeyboardKey(Keys.F2);
            input.RegisterKeyboardKey(Keys.ShiftKey);
            input.RegisterKeyboardKey(Keys.ControlKey);
            input.RegisterKeyboardKey(Keys.Up);
            input.RegisterKeyboardKey(Keys.Down);
            input.RegisterKeyboardKey(Keys.Left);
            input.RegisterKeyboardKey(Keys.Right);
            input.RegisterKeyboardKey(Keys.Enter);
            input.RegisterKeyboardKey(Keys.X);

            input.RegisterMouseKey(MouseButtons.Middle);
            input.RegisterMouseKey(MouseButtons.Right);
            input.RegisterMouseKey(MouseButtons.Left);
        }

        public void OpenMainMenu()
        {
            ResetGame();
            guiManager.AddGui(mainScreen, true, true);
        }

        public void ResetGame()
        {
            IsGamePaused = true;
            isGameRunning = false;
            level = null;
            camera = null;
            players = new Player[2];
            if (ingame != null)
                ingame.Remove();
            GC.Collect();
        }

        public void StartGame()
        {
            if (isGameRunning)
                ResetGame();

            IsGamePaused = false;
            isGameRunning = true;

            mainScreen.Remove();

            level = new LevelForest(Content.Load<Texture2D>("levels/levelForest"));
            camera = new Camera(level);
            sound.Camera = camera;
            AddPlayer(new PlayerReal(TEAM1, level, camera));
            AddPlayer(new PlayerAI(TEAM2, level, camera, PlayerAI.DIFFICULTY_EASY, players[0]));
            level.InitLevel();
            foreach (Player p in players)
                if (p != null) p.OnLevelLoaded();
            camera.CenterOn(100 * Viewport.TILESIZE, 15 * Viewport.TILESIZE);

            ingame = new GuiPlayer(guiManager, players[0]);
            guiManager.AddGui(ingame, true);
        }

        private void AddPlayer(Player p)
        {
            players[p.Team.ID] = p;
            level.SetPlayer(p, p.Team.ID);
        }

        public static Team GetEnemyTeam(Team team)
        {
            return team == TEAM1 ? TEAM2 : TEAM1;
        }

        public static Player GetEnemyPlayer(Player player)
        {
            return player.Team == TEAM1 ? players[1] : players[0];
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderer = new RenderHelper(spriteBatch);

            Resources.LoadResources(Content, renderer);
            TEAM1 = new Team("BLUE", 0, Resources.SPRITESHEET_WOODCUTTER_BLU, Resources.SPRITESHEET_WARRIOR_BLU, Resources.SPRITESHEET_RANGER_BLU);
            TEAM2 = new Team("RED", 1, Resources.SPRITESHEET_WOODCUTTER_RED, Resources.SPRITESHEET_WARRIOR_RED, Resources.SPRITESHEET_RANGER_RED);
            sound = new Audio("Content\\sound\\castlemaster.xgs", "Content\\sound\\waves.xwb", "Content\\sound\\sounds.xsb");
        }

        protected override void UnloadContent()
        {
            RenderHelper.EmptyTexture.Dispose();
            sound.Dispose();
        }

        protected override void BeginRun()
        {
            base.BeginRun();

            debugScreen = new GuiDebug(guiManager, this);
            guiManager.AddGui(debugScreen, false);

            mainScreen = new GuiMainScreen(guiManager, this);
            OpenMainMenu();
        }

        protected override void Update(GameTime gameTime)
        {
            input.Update();

            if (InputHandler.HasKeyBeenPressed(Keys.F1))
            {
                debugScreen.IsActive = !debugScreen.IsActive;
            }
            if (InputHandler.HasKeyBeenPressed(Keys.F4))
                IsFixedTimeStep = !IsFixedTimeStep;
            if (InputHandler.HasKeyBeenPressed(Keys.Escape) && (guiManager.ImportantGui == null || !(guiManager.ImportantGui is GuiWinLooseMessage)))
            {
                if (isGameRunning)
                {
                    if (!mainScreen.IsActive)
                        guiManager.AddGui(mainScreen, true, true);
                    else
                        mainScreen.Remove();

                    IsGamePaused = mainScreen.IsActive;
                }
                else Exit();
            }

            if (isGameRunning && !IsGamePaused)
            {
                foreach (Player p in players)
                    if (p != null)
                        p.Update();
                //players[0].Update();

                camera.Update();

                level.Update();
            }

            guiManager.Update();

            updates++;
            currentTime += gameTime.ElapsedGameTime;
            if (currentTime >= ONE_SECOND)
            {
                currentTime = TimeSpan.Zero;
                fps = frames;
                ups = updates;
                frames = updates = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            renderer.BeginRender();

            if (isGameRunning)
            {
                level.RenderBackground(camera, renderer);
                camera.RenderSelelctor(renderer);
                level.RenderEntities(camera, renderer);
                foreach (Player p in players)
                    if (p != null) p.Render(renderer);
            }

            guiManager.Render(renderer);
            if (isGameRunning)
                camera.RenderCursor(renderer);
            renderer.EndRender();

            frames++;
            base.Draw(gameTime);
        }
    }
}
