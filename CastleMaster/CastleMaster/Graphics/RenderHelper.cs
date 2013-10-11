using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CastleMaster.Graphics
{
    public class RenderHelper
    {
        private List<SpriteSheet> registeredSpriteSheets;
        private SpriteBatch renderer;
        private bool isRendering = false;
        private Point renderOffset = Point.Zero;
        private static Texture2D empty;

        public RenderHelper(SpriteBatch batch)
        {
            registeredSpriteSheets = new List<SpriteSheet>();
            renderer = batch;
            empty = new Texture2D(batch.GraphicsDevice, 1, 1);
            uint[] c = { 0xFFFFFFFF };
            empty.SetData(c);
        }

        public static Texture2D EmptyTexture { get { return empty; } }

        public SpriteBatch SpriteBatch { get { return renderer; } }

        public List<SpriteSheet> RegisteredSpriteSheets { get { return registeredSpriteSheets; } }

        public int RegisterSpriteSheet(SpriteSheet spriteSheet)
        {
            registeredSpriteSheets.Add(spriteSheet);
            return registeredSpriteSheets.Count - 1;
        }

        public void SetOffset(int x = 0, int y = 0)
        {
            renderOffset.X = x;
            renderOffset.Y = y;
        }

        public void SetOffset(Camera camera)
        {
            renderOffset.X = camera.XLeft;
            renderOffset.Y = camera.YTop;
        }

        public void BeginRender()
        {
            if (isRendering) return;
            isRendering = true;
            renderer.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }

        public void EndRender()
        {
            if (!isRendering) return;
            isRendering = false;
            renderer.End();
        }

        public void Render(float xPos, float yPos, int spriteX, int spriteY, int spriteSheet, Color color, float scale = 1.0F)
        {
            xPos = (int)xPos - renderOffset.X;
            yPos = (int)yPos - renderOffset.Y;

            SpriteSheet sp = registeredSpriteSheets[spriteSheet];
            renderer.Draw(sp.SheetTexture, new Vector2(xPos, yPos), sp[spriteX, spriteY], color, 0.0F, Vector2.Zero, scale, SpriteEffects.None, 1.0F);
        }

        public void Render(Vector2 vec, int spriteX, int spriteY, int spriteSheet, Color color, float scale = 1.0F)
        {
            vec.X = (int)vec.X - renderOffset.X;
            vec.Y = (int)vec.Y - renderOffset.Y;

            SpriteSheet sp = registeredSpriteSheets[spriteSheet];
            renderer.Draw(sp.SheetTexture, vec, sp[spriteX, spriteY], color, 0.0F, Vector2.Zero, scale, SpriteEffects.None, 1.0F);
        }

        public void Render(Vector2 vec, int spriteX, int spriteY, int spriteSheet, float scale = 1.0F)
        {
            vec.X = (int)vec.X - renderOffset.X;
            vec.Y = (int)vec.Y - renderOffset.Y;

            SpriteSheet sp = registeredSpriteSheets[spriteSheet];
            renderer.Draw(sp.SheetTexture, vec, sp[spriteX, spriteY], Color.White, 0.0F, Vector2.Zero, scale, SpriteEffects.None, 1.0F);
        }

        public void RenderModalRect(Vector2 screenPos, Point texturePos, int width, int height, Texture2D tex, float scale = 1.0F)
        {
            screenPos.X = (int)screenPos.X - renderOffset.X;
            screenPos.Y = (int)screenPos.Y - renderOffset.Y;

            renderer.Draw(tex, new Rectangle((int)(texturePos.X * scale), (int)(texturePos.Y * scale), width, height), Color.White);
        }

        public void RenderModalRect(Vector2 screenPos, Point texturePos, int width, int height, int spriteX, int spriteY, int spriteSheet, float scale = 1.0F)
        {
            screenPos.X = (int)screenPos.X - renderOffset.X;
            screenPos.Y = (int)screenPos.Y - renderOffset.Y;

            SpriteSheet sp = registeredSpriteSheets[spriteSheet];
            renderer.Draw(sp.SheetTexture, new Rectangle((int)(texturePos.X * scale), (int)(texturePos.Y * scale), width, height), sp[spriteX, spriteY], Color.White);
        }
    }
}
