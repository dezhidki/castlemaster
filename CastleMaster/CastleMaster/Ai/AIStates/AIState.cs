using CastleMaster.Players;
using CastleMaster.Units;

namespace CastleMaster.Ai.AIStates
{
    public abstract class AIState
    {
        public const int IMPORTANCE_NONE = 0;
        public const int IMPORTANCE_LOW = 1;
        public const int IMPORTANCE_MEDIUM = 2;
        public const int IMPORTANCE_HIGH = 3;

        private PlayerAI player;

        public AIState(PlayerAI player)
        {
            this.player = player;
        }

        protected PlayerAI AI { get { return player; } }

        public abstract int Importance { get; }

        public virtual void ApplyState() { }

        public virtual void OnStateChosen() { }

        public virtual void OnStateChange() { }

        public virtual void OnStateReChosen() { }

        public virtual Unit SelectUnit(Team team) { return null; }
    }

    public class IdleState : AIState
    {

        public IdleState(PlayerAI player)
            : base(player)
        {
        }

        public override int Importance
        {
            get { return IMPORTANCE_NONE; }
        }
    }
}
