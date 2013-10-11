using CastleMaster.Units.Mobs;

namespace CastleMaster.Ai.Orders
{
    public class Order
    {
        protected Mob mob;

        public virtual Order Initialize(Mob mob)
        {
            this.mob = mob;
            return this;
        }

        public virtual void Update() { }

        public virtual bool Finished { get { return true; } }
    }

    public class OrderIdle : Order { }
}
