using CastleMaster.Graphics;
using CastleMaster.Players;
using CastleMaster.Units;
using CastleMaster.Units.Mobs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CastleMaster.Ai.AIStates
{
    public class AgressiveState : AIState
    {
        private List<MobWarrior> warriors;
        private List<MobRanger> rangers;
        private bool isAttacking = false;

        public AgressiveState(PlayerAI player)
            : base(player)
        {
        }

        public override int Importance
        {
            get
            {
                List<MobWarrior> mobUnits = AI.GetUnits<MobWarrior>();

                if (AI.CoinsAmount >= 200)
                    return IMPORTANCE_HIGH;
                else if (mobUnits.Count > 5 && AI.CoinsAmount >= 100)
                    return IMPORTANCE_MEDIUM;

                return IMPORTANCE_LOW;
            }
        }

        public override Unit SelectUnit(Team team)
        {
            int attackTarget = Game.Random.Next(2);
            if (attackTarget == 0)
            {
                List<Unit> toAttack = new List<Unit>();
                if (AI.Enemy.King.Health <= 300) return AI.Enemy.King;

                toAttack.Add(AI.Enemy.King);
                if (!AI.Enemy.Store.IsDestroyed) toAttack.Add(AI.Enemy.Store);

                return toAttack[Game.Random.Next(toAttack.Count)];
            }
            else if (attackTarget == 1)
            {
                return AI.Enemy.AvailableUnits[Game.Random.Next(AI.Enemy.AvailableUnits.Count)];
            }

            return null;
        }

        public override void OnStateChange()
        {
            isAttacking = false;
        }

        public override void OnStateChosen()
        {
            warriors = AI.GetUnits<MobWarrior>();
            rangers = AI.GetUnits<MobRanger>();
        }

        public override void OnStateReChosen()
        {
            warriors = AI.GetUnits<MobWarrior>();
            rangers = AI.GetUnits<MobRanger>();
        }

        public override void ApplyState()
        {
            if (!AI.Armory.IsDestroyed && warriors.Count + rangers.Count <= 8 && AI.CoinsAmount >= UnitArmory.PRICE_WARRIOR)
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
                    xTile = spawnPoint.X + (int)(Math.Sin(Game.Random.NextDouble() * MathHelper.TwoPi) * 4.0F);
                    zTile = spawnPoint.Y + (int)(Math.Cos(Game.Random.NextDouble() * MathHelper.TwoPi) * 4.0F);
                    if (xTile < 0 || xTile >= AI.Level.Width || zTile < 0 || zTile >= AI.Level.Height) continue;
                    if (!AI.Level.RegisteredTiles[AI.Level.Tiles[xTile + zTile * AI.Level.Width]].IsSolidTo(m)) solid = false;
                }

                AI.Level.AddEntity(m, xTile * Viewport.TILESIZE + 8.0F, zTile * Viewport.TILESIZE + 8.0F);
                if (typeToBuy == typeof(MobWarrior))
                {
                    warriors.Add((MobWarrior)m);
                    ((MobWarrior)m).OnFunctionClick(AI.SpawnPoints[Player.SPAWN_WARRIOR].X * Viewport.TILESIZE, AI.SpawnPoints[Player.SPAWN_WARRIOR].Y * Viewport.TILESIZE, 1, false);
                }
                else
                {
                    rangers.Add((MobRanger)m);
                    ((MobRanger)m).OnFunctionClick(AI.SpawnPoints[Player.SPAWN_RANGER].X * Viewport.TILESIZE, AI.SpawnPoints[Player.SPAWN_RANGER].Y * Viewport.TILESIZE, 1, false);
                }

                return;
            }

            if (!isAttacking)
            {
                if (warriors.Count > 0)
                    warriors[0].OnGroupOrder(warriors, AI.SpawnPoints[Player.SPAWN_WARRIOR].X * Viewport.TILESIZE, AI.SpawnPoints[Player.SPAWN_WARRIOR].Y * Viewport.TILESIZE);
                if (rangers.Count > 0)
                    rangers[0].OnGroupOrder(rangers, AI.SpawnPoints[Player.SPAWN_RANGER].X * Viewport.TILESIZE, AI.SpawnPoints[Player.SPAWN_RANGER].Y * Viewport.TILESIZE);
                isAttacking = true;
            }
            else
            {

                for (int i = 0; i < warriors.Count; i++)
                {
                    MobWarrior warrior = warriors[i];
                    if (warrior.Removed)
                        warriors.Remove(warrior);

                    if (!warrior.HasActiveOrders && warrior.Target == null)
                        warrior.OnFunctionClick(AI.SpawnPoints[Player.SPAWN_WARRIOR].X * Viewport.TILESIZE, AI.SpawnPoints[Player.SPAWN_WARRIOR].Y * Viewport.TILESIZE, 1, false);
                }

                for (int i = 0; i < rangers.Count; i++)
                {
                    MobRanger ranger = rangers[i];
                    if (ranger.Removed)
                        rangers.Remove(ranger);

                    if (!ranger.HasActiveOrders && ranger.Target == null)
                        ranger.OnFunctionClick(AI.SpawnPoints[Player.SPAWN_RANGER].X * Viewport.TILESIZE, AI.SpawnPoints[Player.SPAWN_RANGER].Y * Viewport.TILESIZE, 1, false);
                }
            }
        }
    }
}
