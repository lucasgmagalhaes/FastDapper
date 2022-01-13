namespace DapperOperations
{
    /// <summary>
    /// Definition of each naming convetion that can be used for tables and column's name
    /// </summary>
    public enum NamingConvetion
    {
        /// <summary>
        /// Defines that tables and columns are in snake case format.
        /// 
        /// <br></br>
        /// 
        /// Ex: <b>user_info</b>
        /// </summary>
        SnakeCase,
        /// <summary>
        /// Defines that tables and columns are in camel case format.
        /// 
        /// <br></br>
        /// 
        /// Ex: <b>userInfo</b>
        /// </summary>
        CamelCase,
        /// <summary>
        /// Defines that tables and columns are in camel case format.
        /// 
        /// <br></br>
        /// 
        /// Ex: <b>UserInfo</b>
        /// </summary>
        PascalCase,
        /// <summary>
        /// Defines that tables and columns are in camel case format.
        /// 
        /// <br></br>
        /// 
        /// Ex: <b>user-info</b>
        /// </summary>
        KebabCase
    }
}
