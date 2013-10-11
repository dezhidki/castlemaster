using CastleMaster.Entities.TileEntities;
using CastleMaster.Players;
using CastleMaster.Units.Mobs;
using CastleMaster.World.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viewport = CastleMaster.Graphics.Viewport;

namespace CastleMaster.World
{
    public class LevelForest : Level
    {
        private int TILE_FLOOR, TILE_WATER, TILE_FLOOR_SOLID, TILE_KING1_HINT, TILE_KING1_HINT2, TILE_STORE1_HINT, TILE_KING2_HINT, TILE_KING2_HINT2, TILE_STORE2_HINT, TILE_ARMORY1_HINT, TILE_ARMORY2_HINT;
        
        public LevelForest(Texture2D tileMap)
            : base(tileMap)
        {
        }

        protected override void InitTiles()
        {
            TILE_FLOOR = new TileFloor(this).ID;
            TILE_WATER = new TileWater(this).ID;
            TILE_FLOOR_SOLID = new TileFloor(this, true).ID;
            TILE_KING1_HINT = new TileRenderHint(this, Players[0].King, true, false).ID;
            TILE_KING1_HINT2 = new TileRenderHint(this, Players[0].King, false, false).ID;
            TILE_STORE1_HINT = new TileRenderHint(this, Players[0].Store, true, true).ID;
            TILE_KING2_HINT = new TileRenderHint(this, Players[1].King, true, false).ID;
            TILE_STORE2_HINT = new TileRenderHint(this, Players[1].Store, true, true).ID;
            TILE_KING2_HINT2 = new TileRenderHint(this, Players[1].King, false, false).ID;
            TILE_ARMORY1_HINT = new TileRenderHint(this, Players[0].Armory, true, true).ID;
            TILE_ARMORY2_HINT = new TileRenderHint(this, Players[1].Armory, true, true).ID;
        }

        public override void InitLevel()
        {
            base.InitLevel();

            LevelBuilder lb = new LevelBuilder(this, tileMap);

            lb.AddTile(0xFF707070, TILE_KING1_HINT);
            lb.AddTile(0xFF606060, TILE_KING1_HINT2);
            lb.AddTile(0xFF505050, TILE_STORE1_HINT);
            lb.AddTile(0xFF50505B, TILE_ARMORY1_HINT);

            lb.AddTile(0xFF707071, TILE_KING2_HINT);
            lb.AddTile(0xFF606061, TILE_KING2_HINT2);
            lb.AddTile(0xFF505051, TILE_STORE2_HINT);
            lb.AddTile(0xFF60606B, TILE_ARMORY2_HINT);

            lb.AddTile(0xFF808080, TILE_FLOOR_SOLID);
            lb.AddTile(0xFF404040, TILE_FLOOR);
            lb.AddTile(0xFF0094FF, TILE_WATER);
            lb.AddTile(0xFF303030, TILE_FLOOR, 2);

            lb.AddTile(0xFF00FF00, TILE_FLOOR);
            lb.AddTile(0xFF00FF01, TILE_FLOOR);
            lb.AddTile(0xFF00FF10, TILE_FLOOR);
            lb.AddTile(0xFF00FF20, TILE_FLOOR);
            lb.AddTile(0xFF00FF30, TILE_FLOOR);
            lb.AddTile(0xFF00FF40, TILE_FLOOR);
            lb.AddTile(0xFF00FF11, TILE_FLOOR);
            lb.AddTile(0xFF00FF21, TILE_FLOOR);
            lb.AddTile(0xFF00FF31, TILE_FLOOR);
            lb.AddTile(0xFF00FF50, TILE_FLOOR);

            lb.AddTile(0xFF00FFFF, TILE_FLOOR);
            lb.AddTile(0xFF0000FF, TILE_FLOOR);
            lb.AddTile(0xFFFFD800, TILE_FLOOR);
            lb.AddTile(0xFFFF0000, TILE_FLOOR);
            lb.AddTile(0xFF21007F, TILE_FLOOR);
            lb.AddEntity(0xFFE55B00, typeof(TileEntityBlock), 8.0F, 8.0F, this, 4, 1);
            lb.AddEntity(0xFF00B200, typeof(TileEntityBlock), 8.0F, 8.0F, this, 2, 2);
            lb.AddEntity(0xFF00CC00, typeof(TileEntityBlock), 8.0F, 8.0F, this, 1, 2);
            lb.AddEntity(0xFF00E500, typeof(TileEntityBlock), 8.0F, 8.0F, this, 0, 2);
            lb.AddEntity(0xFF0000FF, typeof(MobWarrior), 8.0F, 8.0F, this, Players[0]);
            lb.AddEntity(0xFFFF0000, typeof(MobWoodcutter), 8.0F, 8.0F, this, Players[0]);
            lb.AddEntity(0xFFFFD800, typeof(MobRanger), 8.0F, 8.0F, this, Players[0]);
            lb.AddEntity(0xFF21007F, typeof(MobWarrior), 8.0F, 8.0F, this, Players[1]);
            lb.AddEntity(0xFF00FFFF, typeof(MobWoodcutter), 8.0F, 8.0F, this, Players[1]);

            lb.AddCustom(0xFF00FF00, delegate(Level level, int xTile, int zTile)
            {
                Players[1].HomePoint = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFF00FF20, delegate(Level level, int xTile, int zTile)
            {
                Players[1].SpawnPoints[Player.SPAWN_WOODCUTTER] = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFF00FF30, delegate(Level level, int xTile, int zTile)
            {
                Players[1].SpawnPoints[Player.SPAWN_WARRIOR] = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFF00FF40, delegate(Level level, int xTile, int zTile)
            {
                Players[1].SpawnPoints[Player.SPAWN_RANGER] = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFF00FF10, delegate(Level level, int xTile, int zTile)
            {
                ((PlayerAI)Players[1]).ForestPoint = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFF00FF50, delegate(Level level, int xTile, int zTile)
            {
                ((PlayerAI)Players[1]).DefencePoint = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFF00FF01, delegate(Level level, int xTile, int zTile)
            {
                Players[0].HomePoint = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFF00FF11, delegate(Level level, int xTile, int zTile)
            {
                Players[0].SpawnPoints[Player.SPAWN_WOODCUTTER] = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFF00FF21, delegate(Level level, int xTile, int zTile)
            {
                Players[0].SpawnPoints[Player.SPAWN_WARRIOR] = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFF00FF31, delegate(Level level, int xTile, int zTile)
            {
                Players[0].SpawnPoints[Player.SPAWN_RANGER] = new Point(xTile, zTile);
            });

            lb.AddCustom(0xFFFF6A03, delegate(Level level, int xTile, int zTile)
            {
                level.AddEntity(Players[1].King, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
            });
            lb.AddCustom(0xFFFF6A02, delegate(Level level, int xTile, int zTile)
            {
                level.AddEntity(Players[1].Store, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
            });
            lb.AddCustom(0xFFFF6C11, delegate(Level level, int xTile, int zTile)
            {
                level.AddEntity(Players[1].Armory, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
            });

            lb.AddCustom(0xFFFF6A00, delegate(Level level, int xTile, int zTile)
            {
                level.AddEntity(Players[0].King, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
            });
            lb.AddCustom(0xFFFF6A01, delegate(Level level, int xTile, int zTile)
            {
                level.AddEntity(Players[0].Store, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
            });
            lb.AddCustom(0xFFFF6B11, delegate(Level level, int xTile, int zTile)
            {
                level.AddEntity(Players[0].Armory, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
            });

            lb.AddCustom(0xFF7F3300, delegate(Level level, int xTile, int zTile)
            {
                level.AddEntity(new TileEntityTree(level, Game.Random.Next(0, 5), Game.Random.Next(3, 9)), xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
            });

            lb.AddTile(0xFF7F3300, TILE_FLOOR);

            lb.BuildLevel();
        }
    }
}
