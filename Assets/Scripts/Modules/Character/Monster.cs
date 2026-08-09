public class Monster : Role
{
    protected override void OnInit()
    {
        base.OnInit();
        RoleType = RoleConst.RoleType.Monster;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }
}