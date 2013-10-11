using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CastleMaster.Graphics
{
    public static class Resources
    {
        public static int SPRITESHEET_TILES { get; private set; }
        public static int SPRITESHEET_ICONS { get; private set; }
        public static int SPRITESHEET_WOODCUTTER_RED { get; private set; }
        public static int SPRITESHEET_WARRIOR_RED { get; private set; }
        public static int SPRITESHEET_RANGER_RED { get; private set; }
        public static int SPRITESHEET_WOODCUTTER_BLU { get; private set; }
        public static int SPRITESHEET_WARRIOR_BLU { get; private set; }
        public static int SPRITESHEET_RANGER_BLU { get; private set; }
        public static int SPRITE_KING { get; private set; }
        public static int SPRITE_STORE { get; private set; }
        public static int SPRITE_ARMORY { get; private set; }
        public static int SPRITESHEET_HEALTH { get; private set; }
        public static SpriteFont FONT { get; private set; }
        public static Texture2D SPRITE_GUI_UNITMENU { get; private set; }
        public static Texture2D SPRITE_GUI_BACKGROUND { get; private set; }
        public static Texture2D SPRITE_GUI_LOGO { get; private set; }
        public static Texture2D SPRITE_GUI_BACKGROUND_INGAME { get; private set; }
        public static Texture2D SPRITE_GUI_INSCTRUCTIONS { get; private set; }
        public static Texture2D SPRITE_HIGHTLIGHT { get; private set; }

        public static Texture2D SPRITE_PAGE_WELCOME { get; private set; }
        public static Texture2D SPRITE_PAGE_CONTROLS { get; private set; }
        public static Texture2D SPRITE_PAGE_GUI { get; private set; }
        public static Texture2D SPRITE_PAGE_BUILDINGS { get; private set; }
        public static Texture2D SPRITE_PAGE_UNITS { get; private set; }

        public static void LoadResources(ContentManager cm, RenderHelper renderer)
        {
            SPRITESHEET_TILES = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("tiles/tilesheet"), 32, 32));
            SPRITESHEET_ICONS = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("misc/icons"), 16, 16));
            SPRITESHEET_WOODCUTTER_RED = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("mobs/red/woodcutter_red"), 32, 32));
            SPRITESHEET_WARRIOR_RED = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("mobs/red/warrior_red"), 32, 32));
            SPRITESHEET_RANGER_RED = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("mobs/red/archer_red"), 32, 32));
            SPRITESHEET_WOODCUTTER_BLU = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("mobs/blu/woodcutter_blu"), 32, 32));
            SPRITESHEET_WARRIOR_BLU = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("mobs/blu/warrior_blu"), 32, 32));
            SPRITESHEET_RANGER_BLU = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("mobs/blu/archer_blu"), 32, 32));
            SPRITE_KING = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("mobs/king"), 160, 128));
            SPRITE_ARMORY = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("mobs/armory"), 160, 98));
            SPRITE_STORE = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("mobs/store"), 240, 147));
            FONT = cm.Load<SpriteFont>("gui/font");
            SPRITESHEET_HEALTH = renderer.RegisterSpriteSheet(new SpriteSheet(cm.Load<Texture2D>("gui/guiHealth"), 100, 4));
            SPRITE_GUI_UNITMENU = cm.Load<Texture2D>("gui/guiUnitMenu");
            SPRITE_GUI_BACKGROUND = cm.Load<Texture2D>("gui/startMenu");
            SPRITE_GUI_LOGO = cm.Load<Texture2D>("gui/logo");
            SPRITE_GUI_BACKGROUND_INGAME = cm.Load<Texture2D>("gui/inGameBackGround");
            SPRITE_GUI_INSCTRUCTIONS = cm.Load<Texture2D>("gui/instructions");
            SPRITE_HIGHTLIGHT = cm.Load<Texture2D>("gui/highLightBox");

            SPRITE_PAGE_WELCOME = cm.Load<Texture2D>("gui/howto/page_welcome");
            SPRITE_PAGE_CONTROLS = cm.Load<Texture2D>("gui/howto/page_controls");
            SPRITE_PAGE_GUI = cm.Load<Texture2D>("gui/howto/page_gui");
            SPRITE_PAGE_BUILDINGS = cm.Load<Texture2D>("gui/howto/page_buildings");
            SPRITE_PAGE_UNITS = cm.Load<Texture2D>("gui/howto/page_units");
        }
    }
}
