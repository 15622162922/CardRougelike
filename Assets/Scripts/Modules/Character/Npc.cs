public class Npc : Role
{
    protected override void OnInit()
    {
        base.OnInit();
        RoleType = RoleConst.RoleType.Npc;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }
}