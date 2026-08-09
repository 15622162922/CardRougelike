/// <summary>
///     最基础的逻辑单位
/// </summary>
public class Unit
{
    /// <summary>
    ///     单位唯一id
    /// </summary>
    protected string m_unitId;

    protected string m_unitName;

    /// <summary>
    ///     单位名称
    /// </summary>
    public string UnitName
    {
        get => m_unitName;
        set => m_unitName = value;
    }

    public void Init()
    {
        OnInit();
    }

    public void Release()
    {
        OnRelease();
    }

    protected virtual void OnInit()
    {
    }

    protected virtual void OnRelease()
    {
    }

    public string GetUnitID()
    {
        return m_unitId;
    }

    public void SetUnitID(string unitId)
    {
        m_unitId = unitId;
    }

    public string GetUnitName()
    {
        return m_unitName;
    }

    public void SetUnitName(string unitName)
    {
        m_unitName = unitName;
    }
}