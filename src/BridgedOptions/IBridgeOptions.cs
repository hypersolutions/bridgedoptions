namespace BridgedOptions
{
    public interface IBridgeOptions<in TSource, out TTarget> where TSource : class where TTarget : class
    {
        TTarget BridgeFrom(TSource source);
    }
}