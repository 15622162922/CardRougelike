using System.Collections;
using UnityEngine;

public class RoleMoveComponent : BaseRoleComponent
{
    private float _speed;
    protected override void OnInit()
    {
        base.OnInit();
    }

    protected override void OnRegisterCommandHandler()
    {
        base.OnRegisterCommandHandler();
        role.RegisterCommandType<RoleMoveCommand>(RoleConst.RoleCommandType.Move, Move);
    }

    public void Move(RoleMoveCommand command)
    {
        //todo 移动逻辑
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}