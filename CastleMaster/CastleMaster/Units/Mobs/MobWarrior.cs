using CastleMaster.Ai.Orders;
using CastleMaster.Graphics;
using CastleMaster.Players;
using CastleMaster.World;
using IsometricEngineTest.Ai.Orders;
using System.Collections.Generic;
using CastleMaster.Sound;
using Microsoft.Xna.Framework.Audio;

namespace CastleMaster.Units.Mobs
{
    public class MobWarrior : Mob
    {
        private enum OrderType { ATTACK, WALK, AVOID, NONE }

        private float ATTACK_RANGE_OFFSET = 25.0F;
        private float attackRange, attackRangeSqr;

        private Unit target;
        private OrderType currentOrderType = OrderType.NONE;
        private bool isAttacking = false;
        private int maxDamage = 6;
        private TimerHelper hitAnimation;
        private int spriteX;
        private float pushPower = 5.0F;
        private int lastSameOrderAmount = 0;
        private bool engageAttack = false;

        public MobWarrior(Level level, Player owner)
            : base(level, owner)
        {
            HasHealth = true;
            maxHealth = 50;

            spriteSize.X = 17;
            spriteSize.Y = 20;
            screenRectOffset.Update(8, 12, 8 + spriteSize.X, 12 + spriteSize.Y);
            renderOffset.Y = 20;

            highlightOffset.X = 11;

            rectOffset.Update(-4.0F, -4.0F, 5.0F, 5.0F);
            isSolid = true;
            moveSpeed = 1.25F;
            hitAnimation = new TimerHelper(8, 5, false, 3);
            hitAnimation.RoundEnded += delegate
            {
                if (target != null)
                {
                    int damage = Game.Random.Next(1, maxDamage + 1);
                    Hit(target, damage, Direction, pushPower);
                    if (damage == maxDamage)
                        Audio.PlaySound3D(emitter, screenPos, "sword2");
                    else
                        Audio.PlaySound3D(emitter, screenPos, "sword1");
                }
            };
        }

        public override int TypeID
        {
            get { return 2; }
        }

        public Unit Target { get { return target; } }

        public override void OnRemoved()
        {
            base.OnRemoved();
            Audio.PlaySound3D(emitter, screenPos, "death3");
        }

        private void StartAttack()
        {
            isAttacking = true;
            hitAnimation.Start();
        }

        private void StopAttack()
        {
            isAttacking = false;
            target = null;
            currentOrderType = OrderType.NONE;
            StopCurrentOrder();
        }

        private void PauseAttack()
        {
            isAttacking = false;
            hitAnimation.Stop();
            hitAnimation.Reset();
        }

        public void CreateAttackOrder(Unit u, int orderAmount, bool wasCalledBefore)
        {
            currentOrderType = OrderType.ATTACK;
            attackRange = u.Width / 2 + ATTACK_RANGE_OFFSET;
            attackRangeSqr = attackRange * attackRange;
            SetOrder(new OrderMove(u.X, u.Z, attackRange, orderAmount * 2.0F, true, !wasCalledBefore));
            target = u;
        }

        public override void Damage(Unit attacker, int damage, float dir, float pushPower)
        {
            base.Damage(attacker, damage, dir, pushPower);
            if (!isAttacking && currentOrderType == OrderType.NONE)
                CreateAttackOrder(attacker, 1, false);
        }

        protected override void OnOrderFinished()
        {
            base.OnOrderFinished();
            if (currentOrderType == OrderType.ATTACK)
                StartAttack();
            else if (currentOrderType == OrderType.WALK)
                currentOrderType = OrderType.NONE;
            else if (currentOrderType == OrderType.AVOID)
            {
                currentOrderType = OrderType.ATTACK;
                engageAttack = true;
            }
        }

        public override Order GetNextOrder()
        {
            if (engageAttack)
            {
                engageAttack = false;
                return new OrderMove(target.X, target.Z, attackRange, excludeEndSolidness: true);
            }
            return base.GetNextOrder();
        }

        public override void OnFunctionClick(float x, float z, int sameOrders, bool wasCalledBefore)
        {
            Unit u = Owner.SelectUnit(Game.GetEnemyTeam(Owner.Team));
            if (u != null)
                CreateAttackOrder(u, sameOrders, wasCalledBefore);
            else
            {
                if (target != null) StopAttack();
                currentOrderType = OrderType.WALK;
                SetOrder(new OrderMove(x, z, createNewPathFinder: !wasCalledBefore, stopOnMoveFail: true));
            }
            lastSameOrderAmount = sameOrders;
        }

        public override void Update()
        {
            base.Update();

            if (isAttacking)
            {
                if (DistanceToSqr(target.X, target.Z) > attackRangeSqr)
                {
                    isAttacking = false;
                    CreateAttackOrder(target, 1, false);
                }
                else
                {
                    UpdateDir(target.X, target.Z);
                    hitAnimation.UpdateStep();
                    if (target.Removed)
                        StopAttack();
                }
                UnitBuilding ub = target as UnitBuilding;
                if (target == null || target.Removed || ub != null && ub.IsDestroyed)
                    StopAttack();
            }
            else if (!isAttacking && currentOrderType == OrderType.NONE)
            {
                List<Unit> units = level.GetNearbyEnemyUnits(Game.GetEnemyTeam(Owner.Team), (int)(x / Viewport.TILESIZE), (int)(z / Viewport.TILESIZE), 3);
                if (units.Count > 0)
                    CreateAttackOrder(units[0], 1, false);
            }
            else if (!isAttacking && currentOrderType == OrderType.ATTACK)
            {
                float dist = lastSameOrderAmount * 12.0F;
                if (currentOrder != null && currentOrder is OrderMove && ((OrderMove)currentOrder).FailingToMove && DistanceToSqr(target.X, target.Z) < attackRangeSqr + dist * dist)
                {
                    int tp = level.GetClosestDiagonalOpenPos(this);
                    if (tp > 0)
                    {
                        currentOrderType = OrderType.AVOID;
                        SetOrder(new OrderMove(tp % level.Width * Viewport.TILESIZE, tp / level.Width * Viewport.TILESIZE, stopOnMoveFail: true));
                    }
                }
                UnitBuilding ub = target as UnitBuilding;
                if (target == null || target.Removed || ub != null && ub.IsDestroyed)
                    StopAttack();
            }

            spriteX = isAttacking ? hitAnimation.CurrentFrame : walkingAnimation.CurrentFrame;
        }

        public override void Render(RenderHelper renderer)
        {
            base.Render(renderer);
            renderer.Render(ScreenPos, spriteX, dirID, Owner.Team.SpriteSheetWarrior, colorizer, Viewport.ZOOM);
        }
    }
}
