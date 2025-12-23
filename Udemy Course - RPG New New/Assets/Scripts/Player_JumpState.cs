using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //使物体向上跳跃，增加垂直速度
        player.SetVelocity(rb.linearVelocity.x,player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        //如果yvelocity下降，玩家在下落，切换到下落状态
        //考虑到跳跃攻击状态不应该切换到下落状态，所以加一个判断（会在一帧内完成判断，进入jumpAttackState,进入FallState,updateFallState）
        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }

}
