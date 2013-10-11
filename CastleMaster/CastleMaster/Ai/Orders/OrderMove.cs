using CastleMaster.Ai.Orders;
using CastleMaster.Graphics;
using CastleMaster.Units.Mobs;

namespace IsometricEngineTest.Ai.Orders
{
    public class OrderMove : Order
    {
        protected float x, z;
        protected int ppi = 0;
        protected bool excludeEndSolidness;
        protected float stopDistance;
        protected int moveFails = 0;
        protected float lastDist = 0.0F;
        protected bool shouldFindPath = false;
        protected bool createNewPathFinder;
        protected float turnDistance;
        protected bool stopOnMoveFail;

        public OrderMove(float x, float z, float stopDistance = 2.0F, float turnDistance = 8.0F, bool excludeEndSolidness = false, bool createNewPathFinder = true, bool stopOnMoveFail = false)
        {
            this.stopDistance = stopDistance;
            this.excludeEndSolidness = excludeEndSolidness;
            this.createNewPathFinder = createNewPathFinder;
            this.x = x;
            this.z = z;
            this.turnDistance = turnDistance;
            this.stopOnMoveFail = stopOnMoveFail;
        }

        public override Order Initialize(Mob mob)
        {
            base.Initialize(mob);

            if (createNewPathFinder)
            {
                if (mob.DistanceTo(x, z) > 64.0F)
                {
                    mob.PathFinder.InitializePathFinder((int)(mob.X / Viewport.TILESIZE), (int)(mob.Z / Viewport.TILESIZE), (int)(x / Viewport.TILESIZE), (int)(z / Viewport.TILESIZE), excludeEndSolidness);
                    shouldFindPath = true;
                }
                else
                {
                    shouldFindPath = false;
                    mob.PathFinder.Reset();
                }
            }
            return this;
        }

        public override void Update()
        {
            if (shouldFindPath)
            {
                if (!mob.PathFinder.CanFindPath)
                    return;
                if (mob.PathFinder.IsPathFinding)
                    mob.PathFinder.FindPath(125);
            }

            if (mob.PathFinder.Path.Count > 0 && ppi < mob.PathFinder.Path.Count)
            {
                Node currentTarget = mob.PathFinder.Path[ppi];
                float xtPoint = currentTarget.X * Viewport.TILESIZE + 8.0F;
                float ztPoint = currentTarget.Z * Viewport.TILESIZE + 8.0F;

                float dist = mob.DistanceTo(xtPoint, ztPoint);
                if (dist > turnDistance)
                {
                    if (mob.TurnTowards(xtPoint, ztPoint))
                    {
                        mob.MoveForward();

                        if (dist == lastDist)
                            moveFails++;
                        else
                            moveFails = 0;
                        lastDist = dist;
                    }
                }
                else
                    ppi++;
            }
            else
            {
                float dist = mob.DistanceTo(x, z);
                if (dist > 1.0F)
                {
                    if (mob.TurnTowards(x, z))
                    {
                        mob.MoveForward();

                        if (dist == lastDist)
                            moveFails++;
                        else
                            moveFails = 0;
                        lastDist = dist;
                    }
                }
            }
        }

        public bool FailingToMove { get { return moveFails > 20; } }

        public override bool Finished
        {
            get
            {
                return (mob.DistanceTo(x, z) < stopDistance) || !mob.PathFinder.CanFindPath || (stopOnMoveFail && moveFails > 20);
            }
        }
    }
}
