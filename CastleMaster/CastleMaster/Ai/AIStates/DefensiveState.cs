using CastleMaster.Graphics;
using CastleMaster.Players;
using CastleMaster.Units;
using CastleMaster.Units.Mobs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CastleMaster.Ai.AIStates
{
    public class DefensiveState : AIState
    {
        private List<MobWarrior> warriors;
        private List<MobRanger> rangers;
        private int kingHitTimes = 0;

        public DefensiveState(PlayerAI player)
            : base(player)
        {
        }

        public override int Importance
        {
            get
            {
                List<Mob> mobUnits = AI.GetUnits<Mob>();
                int importance = IMPORTANCE_LOW;

                if (kingHitTimes != AI.King.TimesHit)
                    importance = IMPORTANCE_HIGH;
                kingHitTimes = AI.King.TimesHit;

                return importance;
            }
        }

        public override void OnStateChosen()
        {
            warriors = AI.GetUnits<MobWarrior>();
            rangers = AI.GetUnits<MobRanger>();

            while (!AI.Armory.IsDestroyed && warriors.Count + rangers.Count < 30 && AI.CoinsAmount >= UnitArmory.PRICE_WARRIOR)
            {
                Type typeToBuy = null;
                typeToBuy = AI.AttackerTypes[Game.Random.Next(AI.AttackerTypes.Count)];

                Mob m = AI.Armory.BuyUnit<Mob>(typeToBuy);

                bool solid = true;
                int xTile = 0, zTile = 0;
                while (solid)
                {
                    xTile = AI.DefencePoint.X + (int)(Math.Sin(Game.Random.NextDouble() * MathHelper.TwoPi) * Game.Random.NextDouble() * 4.0F);
                    zTile = AI.DefencePoint.Y + (int)(Math.Cos(Game.Random.NextDouble() * MathHelper.TwoPi) * Game.Random.NextDouble() * 4.0F);
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

        public override void OnStateChange()
        {
        }

        public override void ApplyState()
        {
        }
    }
}
