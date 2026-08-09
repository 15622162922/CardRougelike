public class BaseModule<T> : BaseModule where T : BaseModule<T>, new()
{
    private static BaseModule _module;

    public static BaseModule Module
    {
        get
        {
            if (_module == null) _module = new T();
            return _module;
        }
    }
}

public class BaseModule
{
    public void Register()
    {
        OnRegisterHandler();
        OnInit();
    }

    public void UnRegister()
    {
        OnUnRegisterHandler();
        OnRelease();
    }

    protected virtual void OnInit()
    {
    }

    protected virtual void OnRelease()
    {
    }

    protected virtual void OnRegisterHandler()
    {
    }

    protected virtual void OnUnRegisterHandler()
    {
    }
}