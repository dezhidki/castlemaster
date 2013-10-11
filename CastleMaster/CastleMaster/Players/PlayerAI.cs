using CastleMaster.Ai.AIStates;
using CastleMaster.Graphics;
using CastleMaster.Units;
using CastleMaster.Units.Mobs;
using CastleMaster.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CastleMaster.Players
{
    public class PlayerAI : Player
    {
        public const int DIFFICULTY_EASY = 60 * 10;
        public const int DIFFICULTY_NORMAL = 60 * 8;
        public const int DIFFICULTY_HARD = 60 * 3;

        private List<AIState> availableStates;
        private TimerHelper brainTimer;
        private AIState currentBrain;
        private Player enemy;
        private List<Type> unitTypes;

        public PlayerAI(Team team, Level level, Camera camera, int difficuly, Player enemyPlayer)
            : base(team, level, camera)
        {
            enemy = enemyPlayer;
            availableStates = new List<AIState>();
            availableStates.Add(new IdleState(this));
            availableStates.Add(new AgressiveState(this));
            availableStates.Add(new DefensiveState(this));
            availableStates.Add(new CollectiveState(this));

            unitTypes = new List<Type>();
            unitTypes.Add(typeof(MobWarrior));
            unitTypes.Add(typeof(MobRanger));

            currentBrain = availableStates[0];
            brainTimer = new TimerHelper(difficuly, 1).Start();
            brainTimer.RoundEnded += ReThinkBestState;
        }

        public Point DefencePoint { get; set; }

        public List<Type> AttackerTypes { get { return unitTypes; } }

        public Player Enemy { get { return enemy; } }

        public Point ForestPoint { get; set; }

        public override void OnLevelLoaded()
        {
        }

        private void ReThinkBestState()
        {


            AIState newState = null;
            int importance = -1;
            foreach (AIState aiState in availableStates)
            {
                int stateImportance = aiState.Importance;
                if (importance == -1 || stateImportance > importance)
                {
                    importance = stateImportance;
                    newState = aiState;
                }
            }
            if (newState != currentBrain)
            {
                if (currentBrain != null) currentBrain.OnStateChange();
                currentBrain = newState;
                currentBrain.OnStateChosen();
#if DEBUG
                Console.WriteLine("New state:" + currentBrain);
#endif
            }
            else
                currentBrain.OnStateReChosen();
        }

        public override void Update()
        {
            brainTimer.UpdateStep();
            if (currentBrain != null) currentBrain.ApplyState();
        }

        public override Unit SelectUnit(Team team)
        {
            if (currentBrain != null)
                return currentBrain.SelectUnit(team);
            return null;
        }
    }
}
