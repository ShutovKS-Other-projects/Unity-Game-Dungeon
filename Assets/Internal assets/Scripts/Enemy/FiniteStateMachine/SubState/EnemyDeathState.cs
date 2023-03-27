using UnityEngine;

namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyDeathState : EnemyState
    {
        public EnemyDeathState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            StateController.NavMeshAgent.isStopped = true;
            StateController.NavMeshAgent.velocity = Vector3.zero;
            StateController.Rb.velocity = Vector3.zero;
            StateController.Rb.isKinematic = true;
            StateController.Rb.constraints = RigidbodyConstraints.FreezeAll;
            Debug.Log("Enter Death State");
        }
    }
}