using CastleMaster.Input;
using CastleMaster.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CastleMaster.Graphics
{
    public class Camera
    {
        private int xOffs = 0, yOffs = 0;
        private Vector2 mouseWorldPos, selectorPos, selectorPosZoomed;
        private Point mouseTilePos;
        private Level level;
        private AudioListener listener;

        public Camera(Level level)
        {
            this.level = level;
            mouseWorldPos = Vector2.Zero;
            selectorPos = Vector2.Zero;
            selectorPosZoomed = Vector2.Zero;
            mouseTilePos = Point.Zero;
            listener = new AudioListener();
            listener.Position = new Vector3(xOffs + Game.WIDTH / 2, yOffs + Game.HEIGHT / 2, 0.0F);
        }

        #region Properties
        public int XLeft
        {
            get { return xOffs; }
            set { xOffs = value; }
        }

        public int YTop
        {
            get { return yOffs; }
            set { yOffs = value; }
        }

        public int XRight
        {
            get { return xOffs + Game.WIDTH; }
            set { xOffs = value - Game.WIDTH; }
        }

        public int YBottom
        {
            get { return yOffs + Game.HEIGHT; }
            set { yOffs = value - Game.HEIGHT; }
        }

        public Vector2 MouseWorldPos { get { return mouseWorldPos; } }

        public Point MouseTilePos { get { return mouseTilePos; } }

        public AudioListener AudioListener { get { return listener; } }
        #endregion

        public void Update()
        {
            mouseWorldPos = Viewport.ScreenToWorld(InputHandler.MouseX + xOffs, InputHandler.MouseY + yOffs);
            int xTile = (int)(mouseWorldPos.X / Viewport.TILESIZE);
            int zTile = (int)(mouseWorldPos.Y / Viewport.TILESIZE);
            if (xTile < 0) xTile = 0;
            else if (xTile >= level.Width) xTile = level.Width - 1;
            if (zTile < 0) zTile = 0;
            else if (zTile >= level.Height) zTile = level.Height - 1;

            if (xTile != mouseTilePos.X || zTile != mouseTilePos.Y)
            {
                mouseTilePos.X = xTile;
                mouseTilePos.Y = zTile;
                selectorPos.X = (xTile - zTile) * Viewport.X_STEP;
                selectorPos.Y = (xTile + zTile) * Viewport.Y_STEP + Viewport.Y_STEP;
                selectorPosZoomed = selectorPos * Viewport.ZOOM;
            }
        }

        public void Zoom(float factor)
        {
            Viewport.ZOOM += factor;

            xOffs = (int)((mouseWorldPos.X - mouseWorldPos.Y + Viewport.TILESIZE) * Viewport.X_SCALE_ZOOMED) - InputHandler.MouseX;
            yOffs = (int)((mouseWorldPos.X + mouseWorldPos.Y + Viewport.TILESIZE) * Viewport.Y_SCALE_ZOOMED) - InputHandler.MouseY;
            selectorPosZoomed = selectorPos * Viewport.ZOOM;
        }

        public void CenterOn(float x, float z)
        {
            xOffs = (int)((x - z + Viewport.TILESIZE) * Viewport.X_SCALE_ZOOMED) - Game.WIDTH / 2;
            yOffs = (int)((x + z + Viewport.TILESIZE) * Viewport.Y_SCALE_ZOOMED) - Game.HEIGHT / 2;
        }

        public void RenderSelelctor(RenderHelper renderer)
        {
            renderer.SetOffset(this);
            renderer.Render(selectorPosZoomed, 3, 1, Resources.SPRITESHEET_TILES, Viewport.ZOOM);
            renderer.SetOffset();
        }

        public void RenderCursor(RenderHelper renderer)
        {
            renderer.Render(InputHandler.MouseX, InputHandler.MouseY, 0, 0, Resources.SPRITESHEET_ICONS, Color.White, 3.0F);
        }

        public void UpdateAudioListener()
        {
            listener.Position = new Vector3((xOffs + Game.WIDTH / 2), 0.0F, yOffs + Game.HEIGHT / 2);
            listener.Position /= Viewport.ZOOM;
        }
    }
}
