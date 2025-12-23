using UnityEngine;

public abstract class EntityState //抽象类，必须将EntityState作为父类继承
{
    protected Player player; //引用Player类
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;
    protected PlayerInputSet input; //快捷方式，直接访问player的input属性

    protected float stateTimer;
    protected bool triggerCalled;

    public EntityState(Player player, StateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

        anim = player.Anim;
        rb = player.rb;
        input = player.input;
    }

    public virtual void Enter()
    {
        anim.SetBool(animBoolName, true);
        triggerCalled = false;

    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
            stateMachine.ChangeState(player.dashState);
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);

    }

    public void CallAnimationTrigger()
    {
                triggerCalled = true;
    }


    private bool CanDash()
    {
        if (player.wallDetected)
            return false;

        if (stateMachine.currentState == player.dashState)
            return false;

        return true;
    }
}

