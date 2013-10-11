using CastleMaster.Entities.TileEntities;
using CastleMaster.World.Tiles;
using Microsoft.Xna.Framework.Graphics;
using CastleMaster.Entities;
using CastleMaster.Units;
using CastleMaster.Units.Mobs;
using Viewport = CastleMaster.Graphics.Viewport;

namespace CastleMaster.World
{
    public class LevelTest : Level
    {
        private int TILE_FLOOR, TILE_WATER;

        public LevelTest(Texture2D tileMap)
            : base(tileMap)
        {
        }

        protected override void InitTiles()
        {
            TILE_FLOOR = new TileFloor(this).ID;
            TILE_WATER = new TileWater(this).ID;
        }

        public override void InitLevel()
        {
            LevelBuilder lb = new LevelBuilder(this, tileMap);

            lb.AddTile(0xFFFF0000, TILE_FLOOR);
            lb.AddTile(0xFF0000FF, TILE_FLOOR);
            lb.AddTile(0xFF21007F, TILE_FLOOR);
            lb.AddTile(0xFF404040, TILE_FLOOR);
            lb.AddTile(0xFF0094FF, TILE_WATER);
            lb.AddTile(0xFF808080, TILE_FLOOR, 1);
            lb.AddTile(0xFF303030, TILE_FLOOR, 2);
            lb.AddTile(0xFF202020, TILE_FLOOR, 4);
            lb.AddTile(0xFF202021, TILE_FLOOR, 5);
            lb.AddTile(0xFF101010, TILE_FLOOR, 3);
            lb.AddTile(0xFF0094FF, TILE_WATER, 8);
            lb.AddTile(0xFF00B200, TILE_FLOOR, 6);
            lb.AddTile(0xFF00CC00, TILE_FLOOR, 6);
            lb.AddTile(0xFFFF0000, TILE_FLOOR);
            lb.AddTile(0xFFFF0001, TILE_FLOOR);
            lb.AddEntity(0xFFFF6A00, typeof(TileEntityBlock), 8.0F, 8.0F, this, 0, 0);
            lb.AddEntity(0xFFE55B00, typeof(TileEntityBlock), 8.0F, 8.0F, this, 4, 1);
            lb.AddEntity(0xFF00B200, typeof(TileEntityBlock), 8.0F, 8.0F, this, 2, 2);
            lb.AddEntity(0xFF00CC00, typeof(TileEntityBlock), 8.0F, 8.0F, this, 1, 2);
            lb.AddEntity(0xFF00E500, typeof(TileEntityBlock), 8.0F, 8.0F, this, 0, 2);
            lb.AddEntity(0xFF00FF00, typeof(TileEntityBlock), 8.0F, 8.0F, this, 6, 0);
            lb.AddEntity(0xFF0000FF, typeof(MobWarrior), 8.0F, 8.0F, this, Players[0]);
            lb.AddEntity(0xFFFF0000, typeof(MobWoodcutter), 8.0F, 8.0F, this, Players[0]);
            lb.AddEntity(0xFF21007F, typeof(MobWarrior), 8.0F, 8.0F, this, Players[1]);

            lb.AddCustom(0xFF7F3300, delegate(Level level, int xTile, int zTile)
            {
                level.AddEntity(new TileEntityTree(level, 1, Game.Random.Next(3, 9)), xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
            });

            lb.AddTile(0xFF7F3300, TILE_FLOOR);

            lb.BuildLevel();
        }
    }
}
