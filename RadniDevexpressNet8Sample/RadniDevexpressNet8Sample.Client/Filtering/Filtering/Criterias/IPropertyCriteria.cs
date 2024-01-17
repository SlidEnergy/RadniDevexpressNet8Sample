using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Marker interface for <see cref="FilterCriteria"/> applied
    /// on a single property.
    /// </summary>
    public interface IPropertyCriteria
    {
        /// <summary>
        /// Gets the name of the constrainted property.
        /// </summary>
        String PropertyName { get; set; }
    }
}
