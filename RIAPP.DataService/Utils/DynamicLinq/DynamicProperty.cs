namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// DynamicProperty
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DynamicProperty"/> class.
    /// </remarks>
    /// <param name="name">The name from the property.</param>
    /// <param name="type">The type from the property.</param>
    public class DynamicProperty(string name, Type type)
    {

        /// <summary>
        /// Gets the name from the property.
        /// </summary>
        /// <value>
        /// The name from the property.
        /// </value>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the type from the property.
        /// </summary>
        /// <value>
        /// The type from the property.
        /// </value>
        public Type Type { get; } = type;
    }
}