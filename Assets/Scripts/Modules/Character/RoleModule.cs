using System.Collections.Generic;
using System.Linq;

public class RoleModule : BaseModule<RoleModule>
{
    private Dictionary<RoleConst.RoleType, Dictionary<string, Role>> m_allRoles;

    protected override void OnInit()
    {
        base.OnInit();
        m_allRoles = new Dictionary<RoleConst.RoleType, Dictionary<string, Role>>();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        foreach (var roleTypes in m_allRoles)
        foreach (var role in roleTypes.Value)
            role.Value.Release();

        m_allRoles.Clear();
    }

    protected override void OnRegisterHandler()
    {
        base.OnRegisterHandler();
        MsgManager.Instance.RegisterMsgHandler<PlayerEnterMsg>(CreatePlayer);
        MsgManager.Instance.RegisterMsgHandler<MonsterEnterMsg>(CreateMonster);
    }

    protected override void OnUnRegisterHandler()
    {
        base.OnUnRegisterHandler();
        MsgManager.Instance.UnRegisterMsgHandler<PlayerEnterMsg>(CreatePlayer);
        MsgManager.Instance.UnRegisterMsgHandler<MonsterEnterMsg>(CreateMonster);
    }

    #region 获取Role

    // 根据unitID来获取单个Role
    public Role GetRoleByUnitID(RoleConst.RoleType roleType, string unitID)
    {
        if (m_allRoles.TryGetValue(roleType, out var roleDir))
            if (roleDir.TryGetValue(unitID, out var role))
                return role;

        return null;
    }

    // 根据RoleType来获取Role列表
    public List<Role> GetRoleListByType(RoleConst.RoleType roleType)
    {
        if (m_allRoles.TryGetValue(roleType, out var roleDir)) return roleDir.Values.ToList();

        return null;
    }

    #endregion

    #region 操作Role

    public void CreatePlayer(PlayerEnterMsg msg)
    {
        var player = ObjectPoolManager.Instance.Get<Player>();
        player.SetUnitID(msg.UnitID);
        player.SetUnitName(msg.PlayerName);
        player.Init();
    }

    public void CreateMonster(MonsterEnterMsg msg)
    {
        var monster = ObjectPoolManager.Instance.Get<Monster>();
        monster.SetUnitID(msg.UnitID);
        monster.Init();
    }

    #endregion
}