using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    private float attackVelocityTimer;
    private float lastTimeAttacked;


    private bool comboAttackQuested;
    private int attackDir;
    private int comboIndex = 1;
    private int comboLimit = 3;
    private const int FirstComboIndex = 1; //攻击从1开始到3结束,播放攻击动画1-3(Parameter basicAttackIndex加1)，超过3则重置为1


    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
        {
            Debug.LogWarning("我已经根据攻击速度数组调整了连击限制");
            comboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQuested = false;
        ResetComboIndexIfNeeded();

        attackDir = player.moveInput.x != 0 ? (int)player.moveInput.x : player.facingDir;

        //if (player.moveInput.x != 0)
        //    attackDir = ((int)player.moveInput.x);
        //else
        //    attackDir = player.facingDir;

        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity();
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack();

        if (triggerCalled) //当动画事件被击发，triggerCalled = true
        {
            HandleStateExit();
        }
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastTimeAttacked = Time.time;
        //当攻击结束时，启动一个协程，在一段时间后重置连击计数器
    }

    private void HandleStateExit()
    {
        if (comboAttackQuested)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay();
        }
        else
            stateMachine.ChangeState(player.idleState);
    }

    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
            comboAttackQuested = true;
    }

    private void ResetComboIndexIfNeeded()
    {
        //如果攻击时间过长，重置连击计数器
        //if (Time.time > lastTimeAttacked + player.comboResetTime)
        //    comboIndex = FirstComboIndex;

        if (comboIndex > comboLimit || Time.time > lastTimeAttacked + player.comboResetTime)
            comboIndex = FirstComboIndex;
    }

    private void ApplyAttackVelocity()
    {
        //获取当前连击的攻击速度
        //连击速度数组是从0开始的，所以需要减1 attackVelocity = player.attackVelocity[comboIndex - 1]; -> Element 0 = Element[1-1]
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }
    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }
}
