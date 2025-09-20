namespace FEZRepacker.Interface
{
    /// <summary>
    /// Used to store aliases.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal class AliasesAttribute(params string[] aliases) : Attribute
    {
        public string[] Aliases { get; } = aliases;
    }
}