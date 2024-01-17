using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Criteria that constrains the property to between two values.
    /// </summary>
    [Serializable]
    public class BetweenCriteria : FilterCriteria, IPropertyCriteria
    {
        const String ToFormatString = "[{0} between {1} and {2}]";

        /// <summary>
        /// Initializes instance of <see cref="BetweenCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the constrainted property.</param>
        /// <param name="begin">The begin value.</param>
        /// <param name="end">The end value.</param>
        public BetweenCriteria(String propertyName, Object begin, Object end)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");

            // for now, NULL is allowed altough it will not make any sense

            //if (null == begin)
            //    throw new ArgumentNullException("begin");
            //if (null == end)
            //    throw new ArgumentNullException("end");

            PropertyName = propertyName;
            Begin = begin;
            End = end;
        }

        /// <summary>
        /// Gets the name of the constrainted property.
        /// </summary>
        public String PropertyName { get; set; }

        /// <summary>
        /// Gets the begin value.
        /// </summary>
        public Object Begin { get; private set; }

        /// <summary>
        /// Gets the end value.
        /// </summary>
        public Object End { get; private set; }

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
            return ToFormatString.FormatCurrentCulture(PropertyName, FilterValueHelper.ToString(Begin), FilterValueHelper.ToString(End));
        }
    }
}
