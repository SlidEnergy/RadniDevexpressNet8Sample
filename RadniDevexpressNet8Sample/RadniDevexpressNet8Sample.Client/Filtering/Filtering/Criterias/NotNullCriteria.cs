using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Criteria that constrains a property to be non-null.
    /// </summary>
    [Serializable]
    public class NotNullCriteria : FilterCriteria, IPropertyCriteria
    {
        const String ToFormatString = "[{0} is not NULL]";

        /// <summary>
        /// Initializes instance of <see cref="NotNullCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the constrainted property.</param>
        public NotNullCriteria(String propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");

            PropertyName = propertyName;
        }

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
            return ToFormatString.FormatCurrentCulture(PropertyName);
        }
    }
}
