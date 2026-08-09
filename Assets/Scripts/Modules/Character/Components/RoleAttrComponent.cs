/// <summary>
/// 属性组件（存储属性数据）
/// </summary>
public class RoleAttrComponent : BaseRoleComponent
{
    protected override void OnInit()
    {
        base.OnInit();
    }

    protected override void OnRegisterCommandHandler()
    {
        base.OnRegisterCommandHandler();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }

    //是否包含该属性
    public bool ContainAttr(int attrId)
    {
        return false;
    }

    //尝试获取该属性值
    public bool TryGetAttr(int attrId, out float attrValue)
    {
        attrValue = 0;
        return false;
    }

    #region 战斗相关

    public bool IsCombatRole()
    {
        //todo 检测是否有生命属性
        return false;
    }

    #endregion
}