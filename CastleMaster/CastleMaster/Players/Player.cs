using CastleMaster.Graphics;
using CastleMaster.Units;
using CastleMaster.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CastleMaster.Players
{
    public abstract class Player
    {
        public const int START_MONEY = 20;
        public const int START_LUMBER = 10;

        public const int SPAWN_WOODCUTTER = 0;
        public const int SPAWN_WARRIOR = 1;
        public const int SPAWN_RANGER = 2;

        protected Team team;
        protected Level level;
        protected Camera camera;
        protected UnitKing king;
        protected UnitStore store;
        protected UnitArmory armory;
        protected Point[] spawnPoints;

        public Player(Team team, Level level, Camera camera)
        {
            this.team = team;
            this.level = level;
            this.camera = camera;
            LumberAmount = START_LUMBER;
            CoinsAmount = 20;

            spawnPoints = new Point[3];

            king = new UnitKing(level, this);
            store = new UnitStore(level, this);
            armory = new UnitArmory(level, this);
        }

        public Point HomePoint { get; set; }

        public Point[] SpawnPoints { get { return spawnPoints; } }

        public UnitKing King { get { return king; } }

        public UnitStore Store { get { return store; } }

        public UnitArmory Armory { get { return armory; } }

        public List<Unit> AvailableUnits { get { return level.Units[Team.ID]; } }

        public int CoinsAmount { get; set; }

        public int LumberAmount { get; set; }

        public Level Level { get { return level; } }

        public Camera Camera { get { return camera; } }

        public Team Team { get { return team; } }

        public virtual void OnLevelLoaded() { }

        public List<T> GetUnits<T>() where T : Unit
        {
            List<T> result = new List<T>();

            foreach (Unit u in AvailableUnits)
            {
                T unit = u as T;
                if (unit != null)
                    result.Add(unit);
            }

            return result;
        }

        public virtual void Update() { }

        public virtual void Render(RenderHelper renderer) { }

        public abstract Unit SelectUnit(Team team);
    }
}
