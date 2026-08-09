public class Player : Role
{
    protected override void OnInit()
    {
        base.OnInit();
        RoleType = RoleConst.RoleType.Player;
        AddRoleComponent<RoleMoveComponent>(); //添加移动组件
        AddRoleComponent<RoleAttackComponent>(); //添加攻击组件
        AddRoleComponent<RoleAttrComponent>(); //添加属性组件
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }
}