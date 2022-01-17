namespace FastDapper.Attributes
{
    /// <summary>
    /// Defines a primary key of some entity
    /// If the entity has composite keys. It's possible
    /// to add this attibute to multiple properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        /// Defines that the primary key is auto incremented
        /// </summary>
        public bool IsAutoIncrement { get; set; }

        /// <summary>
        /// Marks that the primary key is composite, so it <see cref="PrimaryKeyAttribute"/> is 
        /// added to others properties
        /// </summary>
        public bool IsComposite { get; set; }

        /// <summary>
        /// Initialize the attribute
        /// </summary>
        /// <param name="isAutoIncrement">Set if the primay key is auto incremented</param>
        /// <param name="isComposite">Set if the primary key is composite (has others primary keys)</param>
        public PrimaryKeyAttribute(bool isAutoIncrement = true, bool isComposite = false)
        {
            IsAutoIncrement = isAutoIncrement;
            IsComposite = isComposite;
        }
    }
}
