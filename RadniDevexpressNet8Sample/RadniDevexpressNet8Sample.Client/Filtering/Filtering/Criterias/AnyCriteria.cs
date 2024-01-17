using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Criteria that constrains the property to a specified list of values.
    /// </summary>
    [Serializable]
    public class AnyCriteria : FilterCriteria, IPropertyCriteria
    {
        const String ToFormatString = "[{0}.Any({1})]";

        /// <summary>
        /// Initializes instance of <see cref="AnyCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the constrainted property.</param>
        /// <param name="values">The array of values.</param>
        public AnyCriteria(String propertyName, FilterCriteria anyCriteria)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");
            if (null == anyCriteria)
                throw new ArgumentNullException("anyCriteria");

            PropertyName = propertyName;
            ItemCriteria = anyCriteria;
        }

        /// <summary>
        /// Gets the item operand of the logical criteria.
        /// </summary>
        public FilterCriteria ItemCriteria { get; private set; }

        /// <summary>
        /// Gets the name of the constrainted property.
        /// </summary>
        public String PropertyName { get; set; }

        /// <summary>
        /// Accepts the specified <paramref name="visitor"/>.
        /// </summary>
        /// <param name="visitor">Instance of <see cref="IFilterCriteriaVisitor"/>.</param>
        public override void Accept(IFilterCriteriaVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override String ToString()
        {
            return string.Format(ToFormatString, PropertyName, ItemCriteria);
        }
    }
}