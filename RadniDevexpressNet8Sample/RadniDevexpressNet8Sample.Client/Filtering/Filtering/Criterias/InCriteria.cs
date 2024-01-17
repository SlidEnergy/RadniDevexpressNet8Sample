using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Criteria that constrains the property to a specified list of values.
    /// </summary>
    [Serializable]
    public class InCriteria : FilterCriteria, IPropertyCriteria
    {
        const String ToFormatString = "[{0} in ({1})]";

        /// <summary>
        /// Initializes instance of <see cref="InCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the constrainted property.</param>
        /// <param name="values">The array of values.</param>
        public InCriteria(String propertyName, Object[] values)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");
            if (null == values)
                throw new ArgumentNullException("values");

            PropertyName = propertyName;
            Values = values;
        }

        /// <summary>
        /// Gets the name of the constrainted property.
        /// </summary>
        public String PropertyName { get; set; }

        /// <summary>
        /// Gets the array of values.
        /// </summary>
        public Object[] Values { get; private set; }

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
            var values = String.Empty;
            Values.ForEach(o => values += FilterValueHelper.ToString(o) + ", ");
            values = values.Trim().TrimEnd(',');
            return ToFormatString.FormatCurrentCulture(PropertyName, values);
        }
    }
}