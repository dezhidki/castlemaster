using CastleMaster.Entities;
using CastleMaster.Entities.TileEntities;
using CastleMaster.Graphics;
using CastleMaster.Guis;
using CastleMaster.Physics;
using CastleMaster.Players;
using CastleMaster.Units;
using CastleMaster.Units.Mobs;
using CastleMaster.World.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Viewport = CastleMaster.Graphics.Viewport;

namespace CastleMaster.World
{

    public class EntityComprarer : Comparer<Entity>
    {
        /// <summary>
        /// Compares entities between them.
        /// </summary>
        /// <param name="e1">Entity 1.</param>
        /// <param name="e2">Entity 2.</param>
        /// <returns>-1 = e1 render before e2 (e1 -> e2). +1 = e1 render after e2 (e2 -> e1).</returns>
        public override int Compare(Entity e1, Entity e2)
        {
            BoundingRectangle br1 = e1.BoundingRectangle;
            BoundingRectangle br2 = e2.BoundingRectangle;

            if (br1.ZFar >= br2.ZNear)
            {
                //if ((e1.Z + e1.X - e1.RenderOffset.Y) - e1.RenderOffset.Y < (e2.Z + e2.X - e2.RenderOffset.Y) - e2.RenderOffset.Y) return -1;
                return +1;
            }
            if (br1.ZNear <= br2.ZFar)
                return -1;
            if (br1.XRight <= br2.XLeft)
                return -1;
            if (br1.XLeft >= br2.XRight)
                return +1;

            return 0;
        }
    }

    public class Level
    {
        public const int TILE_VOID = 0;
        public const int UPDATES_IN_TICK = 500;
        protected List<Tile> registeredTiles;
        protected int[] tiles;
        protected byte[] data;

        private List<Entity> entities;
        private List<TileEntity> tileEntities;
        private List<Entity>[] entitiesInTiles;
        private List<Entity> entitesToRender;
        private List<Unit>[] units;
        private int width, height;
        protected Texture2D tileMap;
        private EntityComprarer comparer = new EntityComprarer();
        private TimerHelper waterAnimation = new TimerHelper(10, 2).Start();
        private Player[] players;

        public Level(Texture2D tileMap)
        {
            this.tileMap = tileMap;
            width = tileMap.Width;
            height = tileMap.Height;

            players = new Player[2];
            tiles = new int[width * height];
            data = new byte[width * height];
            registeredTiles = new List<Tile>();

            units = new List<Unit>[2];
            units[0] = new List<Unit>();
            units[1] = new List<Unit>();

            entities = new List<Entity>();
            tileEntities = new List<TileEntity>();
            entitiesInTiles = new List<Entity>[width * height];
            for (int i = 0; i < width * height; i++)
                entitiesInTiles[i] = new List<Entity>();
            entitesToRender = new List<Entity>();
        }

        public List<Unit>[] Units { get { return units; } }

        public Player[] Players { get { return players; } }

        public int[] Tiles { get { return tiles; } }

        public int Width { get { return width; } }

        public int Height { get { return height; } }

        public List<Tile> RegisteredTiles { get { return registeredTiles; } }

        public int WaterTimer { get { return waterAnimation.CurrentFrame; } }

        protected virtual void InitTiles() { }

        public virtual void InitLevel()
        {
            new Tile(this);
            InitTiles();
        }

        public void RenderBackground(Camera camera, RenderHelper renderer)
        {
            float w = (Game.WIDTH / Viewport.FLOOR_TILE_WIDTH) / Viewport.ZOOM;
            float h = ((Game.HEIGHT - GuiPlayer.GUI_BAR_HEIGHT) / Viewport.Y_STEP) / Viewport.ZOOM;

            int x0 = (int)((camera.XLeft / Viewport.FLOOR_TILE_WIDTH) / Viewport.ZOOM) - 3;
            int y0 = (int)((camera.YTop / Viewport.Y_STEP) / Viewport.ZOOM) - 5;
            int x1 = (int)(x0 + w) + 5;
            int y1 = (int)(y0 + h) + 7;

            renderer.SetOffset(camera);
            int tp;
            for (int y = y0; y < y1; y++)
            {
                for (int x = x0; x < x1; x++)
                {
                    int xTile = x + (y >> 1) + (y & 1);
                    int zTile = (y >> 1) - x;

                    if (xTile >= 0 && zTile >= 0 && xTile < width && zTile < height)
                    {
                        tp = xTile + zTile * width;
                        Tile t = registeredTiles[tiles[tp]];
                        if (t.ID != TILE_VOID)
                            t.Render(renderer, this, new Vector2((x * Viewport.FLOOR_TILE_WIDTH + (y & 1) * Viewport.X_STEP) * Viewport.ZOOM, (y * Viewport.Y_STEP + Viewport.Y_STEP) * Viewport.ZOOM), xTile, zTile, data[tp]);
                        entitiesInTiles[tp].ForEach(AddEntityToRenderList);
                    }
                }
            }

            renderer.SetOffset();
        }

        public void AddEntityToRenderList(Entity ent)
        {
            if (!ent.IsRendering) entitesToRender.Add(ent);
            ent.IsRendering = true;
        }

        public void RenderEntities(Camera camera, RenderHelper renderer)
        {
            if (entitesToRender.Count > 0)
            {
                entitesToRender.Sort(comparer);

                renderer.SetOffset(camera);

                foreach (Entity ent in entitesToRender)
                {
                    ent.Render(renderer);
                    ent.IsRendering = false;
                }

                renderer.SetOffset();

                entitesToRender.Clear();
            }
        }

        public void Update()
        {
            waterAnimation.UpdateStep();

            for (int i = 0; i < UPDATES_IN_TICK; i++)
            {
                int xTile = Game.Random.Next(width);
                int zTile = Game.Random.Next(height);

                registeredTiles[tiles[xTile + zTile * width]].Update(this, xTile, zTile);

                if (tileEntities.Count > 0)
                {
                    TileEntity te = tileEntities[Game.Random.Next(tileEntities.Count)];
                    te.Update();
                    if (te.Removed)
                    {
                        RemoveEntity(te);
                        TakeEntity(te, te.XTile, te.ZTile);
                    }
                }
            }

            for (int i = 0; i < entities.Count; i++)
            {
                Entity ent = entities[i];

                int xTile_old = (int)(ent.X / Viewport.TILESIZE);
                int zTile_old = (int)(ent.Z / Viewport.TILESIZE);

                ent.Update();

                if (ent.Removed)
                {
                    RemoveEntity(ent);
                    TakeEntity(ent, xTile_old, zTile_old);
                    i--;
                    continue;
                }

                int xTile = (int)(ent.X / Viewport.TILESIZE);
                int zTile = (int)(ent.Z / Viewport.TILESIZE);

                if (xTile != xTile_old || zTile != zTile_old)
                {
                    TakeEntity(ent, xTile_old, zTile_old);
                    InsertEntity(ent, xTile, zTile);
                }
            }
        }

        public void RemoveUnit(Unit u)
        {
            units[u.Owner.Team.ID].Remove(u);
        }

        public void SetPlayer(Player player, int id)
        {
            players[id] = player;
        }

        public void SetTile(int tileX, int tileZ, int tileID)
        {
            tiles[tileX + tileZ * width] = tileID;
        }

        public void SetData(int tileX, int tileZ, byte dataVal)
        {
            data[tileX + tileZ * width] = dataVal;
        }

        public Tile GetTile(int tileX, int tileZ)
        {
            return IsValidPos(tileX, tileZ) ? registeredTiles[tiles[tileX + tileZ * width]] : registeredTiles[TILE_VOID];
        }

        public byte GetData(int tileX, int tileZ)
        {
            return IsValidPos(tileX, tileZ) ? data[tileX + tileZ * width] : (byte)0;
        }

        public void AddEntity(Entity ent, float xPos, float zPos)
        {
            int xTile = (int)(xPos / Viewport.TILESIZE);
            int zTile = (int)(zPos / Viewport.TILESIZE);

            if (!IsValidPos(xTile, zTile)) return;

            ent.X = xPos;
            ent.Z = zPos;

            ent.Init();

            TileEntity te = ent as TileEntity;
            if (te != null)
                tileEntities.Add(te);
            else
                entities.Add(ent);

            Unit u = ent as Unit;
            if (u != null)
            {
                units[u.Owner.Team.ID].Add(u);
            }

            InsertEntity(ent, xTile, zTile);
        }

        private void InsertEntity(Entity ent, int xTile, int zTile)
        {
            entitiesInTiles[xTile + zTile * width].Add(ent);
        }

        private void TakeEntity(Entity ent, int xTile, int zTile)
        {
            entitiesInTiles[xTile + zTile * width].Remove(ent);
        }

        private void RemoveEntity(Entity ent)
        {
            ent.OnRemoved();

            entities.Remove(ent);

            TakeEntity(ent, (int)(ent.X / Viewport.TILESIZE), (int)(ent.Z / Viewport.TILESIZE));
        }

        public TileEntity GetTileEntity(Entity ent, int xTile, int zTile)
        {
            TileEntity te = null;
            List<Entity> ents = entitiesInTiles[xTile + zTile * width];
            if (ents.Count > 0)
                te = ents[0] as TileEntity;
            if (te == null) return null;

            for (int z = zTile - 1; z <= zTile + 1; z++)
            {
                if (z < 0 || z >= height) continue;
                for (int x = xTile - 1; x <= xTile + 1; x++)
                {
                    if (x < 0 || x >= width) continue;

                    ents = entitiesInTiles[x + z * width];

                    if (ents.Count > 0)
                    {
                        Entity e = ents[0];
                        if (e != te && !e.Blocks(te)) return te;
                    }
                    else return te;
                }
            }

            return null;
        }

        public List<Unit> GetUnitsWithinScreenSpace(float x0, float y0, float x1, float y1, Team team)
        {
            List<Unit> result = new List<Unit>();

            foreach (Unit u in units[team.ID])
            {
                if (u.IsSelectable && u.IntersectsWithScreenSpace(x0, y0, x1, y1))
                    result.Add(u);
            }

            return result;
        }

        public List<T> GetUnitsWithinScreenSpace<T>(BoundingRectangle rect, Team team) where T : Unit
        {
            List<T> result = new List<T>();

            foreach (Unit u in units[team.ID])
            {
                T unit = u as T;
                if (unit != null && unit.IsSelectable && unit.IntersectsWithScreenSpace(rect.XLeft, rect.ZFar, rect.XRight, rect.ZNear))
                    result.Add(unit);
            }

            return result;
        }

        public List<BoundingRectangle> GetCollidables(Entity ent, BoundingRectangle br = null)
        {
            List<BoundingRectangle> result = new List<BoundingRectangle>();

            BoundingRectangle entBR = br == null ? ent.BoundingRectangle : br;

            int x0 = (int)(entBR.XLeft / Viewport.TILESIZE) - 1;
            int z0 = (int)(entBR.ZFar / Viewport.TILESIZE) - 1;
            int x1 = (int)(entBR.XRight / Viewport.TILESIZE) + 1;
            int z1 = (int)(entBR.ZNear / Viewport.TILESIZE) + 1;

            for (int z = z0; z <= z1; z++)
            {
                if (z < 0 || z >= height) continue;
                for (int x = x0; x <= x1; x++)
                {
                    if (x < 0 || x >= width) continue;

                    List<Entity> entits = entitiesInTiles[x + z * width];

                    foreach (Entity e in entits)
                    {
                        if (e != ent && entBR.Intersects(e.BoundingRectangle))
                            e.AddBoundingRect(ref result, ent);
                    }

                    Tile t = registeredTiles[tiles[x + z * width]];
                    if (t.IsSolidTo(ent) && t.GetBoundingRect(x, z).Intersects(entBR))
                        t.AddBoundingRect(ref result, x, z);
                }
            }

            return result;
        }


        public List<Unit> GetNearbyEnemyUnits(Team enemyTeam, int xTile, int zTile, int radius)
        {
            List<Unit> result = new List<Unit>();

            for (int z = zTile - radius; z <= zTile + radius; z++)
            {
                if (z < 0 || z >= height) continue;
                for (int x = xTile - radius; x <= xTile + radius; x++)
                {
                    if (x < 0 || x >= width) continue;

                    List<Entity> ents = entitiesInTiles[x + z * width];

                    foreach (Entity ent in ents)
                    {
                        Mob m = ent as Mob;
                        if (m != null && m.Owner.Team.ID == enemyTeam.ID)
                            result.Add(m);
                    }
                }
            }

            return result;
        }

        private bool IsValidPos(int tileX, int tileZ)
        {
            return (tileX >= 0 && tileZ >= 0 && tileX < width && tileZ < height);
        }

        public bool[] BuildSolidnessTable(Mob mob, bool excludeEndSolidness = false)
        {
            bool[] result = new bool[tiles.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                Tile t = registeredTiles[tiles[i]];
                if (t is TileFloor && excludeEndSolidness)
                    result[i] = false;
                else
                    result[i] = tiles[i] == TILE_VOID || t.IsSolid;

                List<Entity> entInTiles = entitiesInTiles[i];

                if (entInTiles.Count > 0)
                {
                    TileEntity te = entInTiles[0] as TileEntity;
                    result[i] = te != null && te.Blocks(mob);
                }
            }

            return result;
        }

        public int GetClosestDiagonalOpenPos(Entity ent)
        {
            int xTile = (int)(ent.X / Viewport.TILESIZE);
            int zTile = (int)(ent.Z / Viewport.TILESIZE);

            bool blocks = false;
            int tp;

            for (int z = zTile - 1; z <= zTile + 1; z++)
            {
                if (z < 0 || z >= height) continue;
                if (z == zTile) continue;

                tp = xTile + z * width;
                Tile t = registeredTiles[tiles[tp]];
                if (t.ID == TILE_VOID || t.IsSolidTo(ent)) blocks |= true;

                List<Entity> ents = entitiesInTiles[tp];

                foreach (Entity e in ents)
                {
                    if (e != ent)
                        blocks |= e.Blocks(ent);
                }
                if (!blocks) return tp;
            }

            blocks = false;
            for (int x = xTile - 1; x <= xTile + 1; x++)
            {
                if (x < 0 || x >= width) continue;
                if (x == xTile) continue;

                tp = x + zTile * width;
                Tile t = registeredTiles[tiles[tp]];
                if (t.ID == TILE_VOID || t.IsSolidTo(ent)) blocks |= true;

                List<Entity> ents = entitiesInTiles[tp];

                foreach (Entity e in ents)
                {
                    if (e != ent)
                        blocks |= e.Blocks(ent);
                }
                if (!blocks) return tp;
            }

            return -1;
        }

        public T GetNearestEntity<T>(Entity caller, int radius) where T : Entity
        {
            T nearest = null;
            float nearestDist = 0.0F, currentDist = 0.0F;

            int xTile = (int)(caller.X / Viewport.TILESIZE);
            int zTile = (int)(caller.Z / Viewport.TILESIZE);

            List<T> inRadius = new List<T>();
            for (int z = zTile - radius; z <= zTile + radius; z++)
            {
                if (z < 0 || z >= height) continue;
                for (int x = xTile - radius; x <= xTile + radius; x++)
                {
                    if (x < 0 || x >= width) continue;
                    if (x == xTile && z == zTile) continue;

                    entitiesInTiles[x + z * width].ForEach(delegate(Entity ent)
                    {
                        T e = ent as T;
                        if (e != null) 
                            inRadius.Add(e);
                    });
                }
            }

            foreach (T ent in inRadius)
            {
                currentDist = ent.DistanceToSqr(caller.X, caller.Z);
                if (nearest == null || currentDist < nearestDist)
                {
                    nearest = ent;
                    nearestDist = currentDist;
                }
            }

            return nearest;
        }

        public bool SolidTo(Entity ent, int xTile, int zTile)
        {
            if (registeredTiles[tiles[xTile + zTile * width]].IsSolidTo(ent))
                return true;

            foreach (Entity e in entitiesInTiles[xTile + zTile * width])
                if (e.Blocks(ent)) return true;

            return false;
        }

        public List<int> GetEntitySolidnessList(Entity caller, int xTile, int zTile, int radius)
        {
            List<int> result = new List<int>();

            for (int z = zTile - radius; z <= zTile + radius; z++)
            {
                if (z < 0 || z >= height) continue;
                for (int x = xTile - radius; x <= xTile + radius; x++)
                {
                    if (x < 0 || x >= width) continue;
                    if (x == xTile && z == zTile) continue;

                    foreach (Entity ent in entitiesInTiles[x + z * width])
                    {
                        if (ent.Blocks(caller))
                        {
                            result.Add(x + z * width);
                            continue;
                        }
                    }
                }
            }

            return result;
        }
    }
}
