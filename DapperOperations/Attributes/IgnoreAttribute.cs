namespace DapperOperations.Attributes
{
    /// <summary>
    /// Denotes an entitie's property that should me ignored in mapping
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class IgnoreAttribute : Attribute
    {
    }
}
