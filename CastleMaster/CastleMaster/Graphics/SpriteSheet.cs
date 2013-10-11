using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CastleMaster.Graphics
{
    public class SpriteSheet
    {
        private Texture2D sheetTexture;
        private int spriteWidth, spriteHeight, rows, columns;

        public SpriteSheet(Texture2D sheetTexture, int spriteWidth, int spriteHeight)
        {
            this.sheetTexture = sheetTexture;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;

            columns = sheetTexture.Width / spriteWidth;
            rows = sheetTexture.Height / spriteHeight;
        }

        public Rectangle this[int x, int y]
        {
            get
            {
                return new Rectangle(x * spriteWidth, y * spriteHeight, spriteWidth, spriteHeight);
            }
        }

        public int Rows { get { return rows; } }

        public int Columns { get { return columns; } }

        public Texture2D SheetTexture { get { return sheetTexture; } }
    }
}
