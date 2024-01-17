using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Criteria that negates another criteria.
    /// </summary>
    [Serializable]
    public class NotCriteria : FilterCriteria
    {
        const String ToFormatString = "not ({0})";

        /// <summary>
        /// Initializes instance of <see cref="NotCriteria"/>.
        /// </summary>
        /// <param name="criteria">The negated <see cref="FilterCriteria"/> instance.</param>
        public NotCriteria(FilterCriteria criteria)
        {
            if (null == criteria)
                throw new ArgumentNullException("criteria");
            Criteria = criteria;
        }

        /// <summary>
        /// Gets the negated <see cref="FilterCriteria"/> instance.
        /// </summary>
        public FilterCriteria Criteria { get; private set; }

        /// <summary>
        /// Accepts the specified <paramref name="visitor"/>.
        /// </summary>
        /// <param name="visitor">Instance of <see cref="IFilterCriteriaVisitor"/>.</param>
        public override void Accept(IFilterCriteriaVisitor visitor)
        {
            visitor.Visit(this);
            Criteria.Accept(visitor);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override String ToString()
        {
            return ToFormatString.FormatCurrentCulture(Criteria);
        }
    }
}
