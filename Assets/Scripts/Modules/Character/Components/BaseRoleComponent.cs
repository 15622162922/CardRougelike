public class BaseRoleComponent : IRoleComponent
{
    public Role role { get; set; }
    public void Bind(Role role)
    {
        this.role = role;
        OnRegisterCommandHandler();
        OnInit();
    }

    public void Unbind()
    {
        OnRelease();
        this.role = null;
    }

    protected virtual void OnInit()
    {
        
    }

    protected virtual void OnRegisterCommandHandler()
    {
        
    }

    protected virtual void OnRelease()
    {
        
    }
}