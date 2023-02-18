using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerDamageState : PlayerAbilityState
    {   
        public PlayerDamageState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }
    
        // ReSharper disable Unity.PerformanceAnalysis
        public override void Enter()
        {
            base.Enter();
        
            playerStatistic.Health -= 5;
        }
    
        // ReSharper disable Unity.PerformanceAnalysis
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsAnimationFinished)
                return;
        
            if (playerStatistic.Health > 0)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
            else
            {
                StateMachine.ChangeState(StateController.DeathState);
            }
        }
    
        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();
        
            isAbilityDone = true;
        }
    }
}
