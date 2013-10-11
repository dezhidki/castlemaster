using CastleMaster.Ai.Orders;
using CastleMaster.Graphics;
using CastleMaster.Players;
using CastleMaster.World;
using IsometricEngineTest.Ai;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastleMaster.Units.Mobs
{
    public abstract class Mob : Unit
    {
        protected Order currentOrder;
        private float dir = 0.0F;
        protected AStar pathFinder;
        protected int dirID;
        protected TimerHelper walkingAnimation;
        protected float pushResistance;
        protected bool canBePushed;
        private bool isPushed;
        private float pushDir, pushPower;
        protected AudioEmitter emitter;

        public Mob(Level level, Player owner)
            : base(level, owner)
        {
            emitter = new AudioEmitter();
            currentOrder = new Order();
            pathFinder = new AStar(level, this);
            walkingAnimation = new TimerHelper(10, 2).Start();
            immunityTime = 20;
            canBePushed = true;
            pushResistance = 0.0F;
        }

        public bool HasActiveOrders { get { return currentOrder != null && !(currentOrder is OrderIdle) && !currentOrder.Finished; } }

        public Level Level { get { return level; } }

        protected float Direction { get { return dir; } }

        public AStar PathFinder { get { return pathFinder; } }

        //public override bool IsSolidTo(Entity ent)
        //{
        //    Mob mob = ent as Mob;
        //    if (mob != null)
        //        return mob.Owner != Owner;
        //    return base.IsSolidTo(ent);
        //}

        public override void Update()
        {
            base.Update();
            if (!(currentOrder is OrderIdle))
            {
                currentOrder.Update();
                if (currentOrder.Finished)
                {
                    StopCurrentOrder();
                }
            }

            if (isPushed)
            {
                MoveTo(dir, pushPower);
                isPushed = false;
            }
        }

        public void OnGroupOrder<T>(List<T> mobs, float x, float z) where T : Mob
        {
            foreach (var mobGroup in mobs.GroupBy(m => m.TypeID))
            {
                List<T> ms = mobGroup.ToList();
                ms[0].OnSameTypeGroupOrder(ms, x, z);
            }
        }

        protected virtual void OnSameTypeGroupOrder<T>(List<T> mobs, float x, float z) where T : Mob
        {
            OnFunctionClick(x, z, mobs.Count, false);
            foreach (Mob m in mobs)
            {
                if (m == this) continue;
                m.OnFunctionClick(x, z, mobs.Count, true);
                m.pathFinder = pathFinder;
            }
        }

        protected void StopCurrentOrder()
        {
            OnOrderFinished();
            SetOrder(GetNextOrder());
        }

        protected virtual void OnOrderFinished()
        {
            walkingAnimation.Reset();
        }

        public void SetOrder(Order order)
        {
            currentOrder = order;
            order.Initialize(this);
        }

        public override void Damage(Unit attacker, int damage, float dir, float pushPower)
        {
            base.Damage(attacker, damage, dir, pushPower);
            if (canBePushed)
                Push(dir, pushPower);
        }

        private void Push(float dir, float pushPower)
        {
            isPushed = true;
            pushDir = dir;
            this.pushPower = pushPower - pushResistance;
        }

        private void MoveTo(float dir, float amount)
        {
            float moveX = (float)Math.Cos(dir) * amount;
            float moveZ = (float)Math.Sin(dir) * amount;

            Move(moveX, moveZ);
        }

        public bool MoveForward()
        {
            float moveX = (float)Math.Cos(dir) * moveSpeed;
            float moveZ = (float)Math.Sin(dir) * moveSpeed;

            if (Move(moveX, moveZ))
            {
                walkingAnimation.UpdateStep();
                return true;
            }

            return false;
        }

        public virtual Order GetNextOrder()
        {
            return new Order();
        }

        public bool TurnTowards(float x, float z)
        {
            float angleTowards = (float)Math.Atan2(z - Z, x - X);

            dir = MathHelper.WrapAngle(dir);

            float angleDifferece = angleTowards - dir;
            angleDifferece = MathHelper.WrapAngle(angleDifferece);

            float maxTurn = 0.2F;
            float near = 1.0F;
            bool turnDone = angleDifferece * angleDifferece < near * near;
            if (angleDifferece < -maxTurn) angleDifferece = -maxTurn;
            if (angleDifferece > maxTurn) angleDifferece = maxTurn;
            dir += angleDifferece;
            dirID = GetDirectionID();

            return turnDone;
        }

        protected void UpdateDir(float x, float z)
        {
            dir = (float)Math.Atan2(z - Z, x - X);
            dirID = GetDirectionID();
        }

        public int GetDirectionID()
        {
            return (int)(Math.Floor(dir * 8 / MathHelper.TwoPi + 0.5F)) & 7;
        }

        public abstract int TypeID { get; }
    }
}
