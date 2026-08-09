using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleAttackComponent : BaseRoleComponent
{
    public float Hp = 0;
    public float MaxHp = 0;

    protected override void OnInit()
    {
        Hp = 0;
        MaxHp = 0;
    }

    public float GetHp()
    {
        return Hp;
    }

    public float GetMaxHp()
    {
        return MaxHp;
    }
}
