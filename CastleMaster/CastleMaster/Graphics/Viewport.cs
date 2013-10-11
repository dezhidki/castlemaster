using Microsoft.Xna.Framework;

namespace CastleMaster.Graphics
{
    public static class Viewport
    {
        public const float TILESIZE = 16.0F;
        public const int FLOOR_TILE_WIDTH = 32;
        public const int FLOOR_TILE_HEIGHT = 16;
        public const int X_STEP = FLOOR_TILE_WIDTH / 2;
        public const int Y_STEP = FLOOR_TILE_HEIGHT / 2;
        public const float X_SCALE = X_STEP / TILESIZE;
        public const float Y_SCALE = Y_STEP / TILESIZE;
        public const float MAX_ZOOM = 3.0F;
        public const float MIN_ZOOM = 1.0F;
        public const float ZOOM_STEP = 0.25F;

        private static float xScaleZoomed = X_SCALE, yScaleZOomed = Y_SCALE;
        private static float zoom = 1.0F;

        public static float X_SCALE_ZOOMED { get { return xScaleZoomed; } }

        public static float Y_SCALE_ZOOMED { get { return yScaleZOomed; } }

        public static float ZOOM
        {
            get { return zoom; }
            set
            {
                if (value < MIN_ZOOM || value > MAX_ZOOM) return;
                zoom = value;
                xScaleZoomed = X_SCALE * zoom;
                yScaleZOomed = Y_SCALE * zoom;
            }
        }

        public static Vector2 WorldToScreen(float x, float z)
        {
            return new Vector2((x- z) * X_SCALE_ZOOMED, (x +  z) * Y_SCALE_ZOOMED);
        }

        public static Vector2 ScreenToWorld(int x, int y)
        {
            float xRegion = (x / ZOOM) / X_SCALE;
            float yRegion = (y / ZOOM) / Y_SCALE;

            return new Vector2((xRegion + yRegion) / 2 - Viewport.TILESIZE, (yRegion - xRegion) / 2);
        }
    }
}
