using System.Collections;
using System.Collections.Generic;
using dnlib.DotNet;
using UnityEngine;

public static class RoleConst 
{
    /// <summary>
    /// 角色类型
    /// </summary>
    public enum RoleType
    {
        Player, //玩家
        Monster, //怪物
        Npc, //npc
    }

    public enum RoleComponentType
    {
        Move, //移动组件
        Attr, //属性组件
        Attack, //战斗组件
    }

    public static Dictionary<RoleComponentType, System.Type> RoleComponent =
        new Dictionary<RoleComponentType, System.Type>()
        {
            [RoleComponentType.Move] = typeof(RoleMoveComponent),
            [RoleComponentType.Attr] = typeof(RoleAttrComponent),
            [RoleComponentType.Attack] = typeof(RoleAttackComponent),
        };

    //角色指令类型
    public enum RoleCommandType
    {
        Move,
        Attack,
    }
}
