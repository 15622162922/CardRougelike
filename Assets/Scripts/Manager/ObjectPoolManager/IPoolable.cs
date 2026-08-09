public interface IPoolable
{
    /// <summary>
    ///     从池中取出后触发
    /// </summary>
    void OnGet();

    /// <summary>
    ///     放回池前触发
    /// </summary>
    void OnRecycle();
}