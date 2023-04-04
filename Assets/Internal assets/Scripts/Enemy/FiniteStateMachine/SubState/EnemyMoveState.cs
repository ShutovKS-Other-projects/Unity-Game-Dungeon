using Manager;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyMoveState : SuperState.EnemyGroundedState
    {
        public EnemyMoveState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Move");

            StateController.NavMeshAgent.isStopped = false;
            StateController.NavMeshAgent.SetDestination(ManagerPlayer.Instance.PlayerPosition);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (PlayerDistance < EnemyStatistic.AttackDistance)
            {
                StateMachine.ChangeState(StateController.IdleState);
                StateController.NavMeshAgent.isStopped = true;
            }
            else
            {
                StateController.NavMeshAgent.SetDestination(ManagerPlayer.Instance.PlayerPosition);
            }
        }
    }
}