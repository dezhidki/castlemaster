using CastleMaster.Entities.TileEntities;
using CastleMaster.Graphics;
using CastleMaster.Players;
using CastleMaster.Units;
using CastleMaster.Units.Mobs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CastleMaster.Ai.AIStates
{
    public class CollectiveState : AIState
    {
        private List<MobWoodcutter> woodCutters;
        private List<MobWarrior> warriors;
        private List<MobRanger> rangers;
        private bool walkingToWood = false, gettingWood = false;

        public CollectiveState(PlayerAI player)
            : base(player)
        {
        }

        public override int Importance
        {
            get
            {
                if (AI.Store.IsDestroyed) return IMPORTANCE_LOW;

                if (AI.CoinsAmount <= Player.START_MONEY || AI.LumberAmount <= Player.START_LUMBER)
                    return IMPORTANCE_HIGH;
                if (AI.CoinsAmount <= Player.START_MONEY * 3)
                    return IMPORTANCE_MEDIUM;

                return IMPORTANCE_LOW;
            }
        }

        public override void OnStateChosen()
        {
            woodCutters = AI.GetUnits<MobWoodcutter>();
            warriors = AI.GetUnits<MobWarrior>();
            rangers = AI.GetUnits<MobRanger>();
        }

        public override void OnStateChange()
        {
            walkingToWood = false;
            gettingWood = false;
            if (woodCutters.Count > 0)
            {
                MobWoodcutter mw = woodCutters[0];
                AI.ForestPoint = new Point((int)(mw.X / Viewport.TILESIZE), (int)(mw.Z / Viewport.TILESIZE));
                woodCutters[0].OnGroupOrder<MobWoodcutter>(woodCutters, AI.SpawnPoints[Player.SPAWN_WOODCUTTER].X * Viewport.TILESIZE, AI.SpawnPoints[Player.SPAWN_WOODCUTTER].Y * Viewport.TILESIZE);
            }
        }

        public override void ApplyState()
        {
            if (!AI.Armory.IsDestroyed && AI.CoinsAmount >= UnitArmory.PRICE_WOODCUTTER)
            {
                if (woodCutters.Count <= 5)
                {
                    MobWoodcutter m = AI.Armory.BuyUnit<MobWoodcutter>();
                    bool solid = true;
                    int xTile = 0, zTile = 0;
                    while (solid)
                    {
                        xTile = AI.SpawnPoints[Player.SPAWN_WOODCUTTER].X + (int)(Math.Sin(Game.Random.NextDouble() * MathHelper.TwoPi) * Game.Random.NextDouble() * 4.0F);
                        zTile = AI.SpawnPoints[Player.SPAWN_WOODCUTTER].Y + (int)(Math.Cos(Game.Random.NextDouble() * MathHelper.TwoPi) * Game.Random.NextDouble() * 4.0F);
                        if (xTile < 0 || xTile >= AI.Level.Width || zTile < 0 || zTile >= AI.Level.Height) continue;
                        if (!AI.Level.SolidTo(m, xTile, zTile)) solid = false;
                    }

                    AI.Level.AddEntity(m, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
                    woodCutters.Add(m);
                    m.OnFunctionClick(AI.ForestPoint.X * Viewport.TILESIZE, AI.ForestPoint.Y * Viewport.TILESIZE, 1, false);
                    return;
                }
                else if (warriors.Count + rangers.Count <= 5 && AI.CoinsAmount >= UnitArmory.PRICE_WARRIOR)
                {
                    Type typeToBuy = null;
                    int buyID = Game.Random.Next(AI.AttackerTypes.Count);
                    typeToBuy = AI.AttackerTypes[buyID];

                    Mob m = AI.Armory.BuyUnit<Mob>(typeToBuy);
                    Point spawnPoint = AI.SpawnPoints[buyID + 1];

                    bool solid = true;
                    int xTile = 0, zTile = 0;
                    while (solid)
                    {
                        xTile = spawnPoint.X + (int)(Math.Sin(Game.Random.NextDouble() * MathHelper.TwoPi) * Game.Random.NextDouble() * 4.0F);
                        zTile = spawnPoint.Y + (int)(Math.Cos(Game.Random.NextDouble() * MathHelper.TwoPi) * Game.Random.NextDouble() * 4.0F);
                        if (xTile < 0 || xTile >= AI.Level.Width || zTile < 0 || zTile >= AI.Level.Height) continue;
                        if (!AI.Level.SolidTo(m, xTile, zTile)) solid = false;
                    }

                    AI.Level.AddEntity(m, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
                    if (typeToBuy == typeof(MobWarrior))
                        warriors.Add((MobWarrior)m);
                    else
                        rangers.Add((MobRanger)m);
                }
            }

            if (!walkingToWood)
            {
                woodCutters[0].OnGroupOrder(woodCutters, AI.ForestPoint.X * Viewport.TILESIZE, AI.ForestPoint.Y * Viewport.TILESIZE);
                walkingToWood = true;
            }
            else
            {
                foreach (MobWoodcutter woodCutter in woodCutters)
                {
                    if (!woodCutter.HasActiveOrders)
                    {
                        woodCutter.OrderChop(AI.Level.GetNearestEntity<TileEntityTree>(woodCutter, 5), 1, false);
                        if (woodCutter.Removed) woodCutters.Remove(woodCutter);
                    }
                }

                if (!AI.Store.IsDestroyed)
                {
                    while (AI.LumberAmount > Player.START_LUMBER + 1)
                    {
                        AI.LumberAmount--;
                        AI.CoinsAmount += UnitStore.COINS_FOR_LUMBER;
                    }
                }
            }
        }
    }
}
