using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Base class for objects used as filters in data queries.
    /// </summary>
    [Serializable]
    public abstract class FilterCriteria
    {
        /// <summary>
        /// Name of derived of FilterCriteria class.
        /// </summary>
        public string Type
        {
            get
            {
                return this.GetType().Name;
            }
        }


        /// <summary>
        /// Accepts the specified <paramref name="visitor"/>.
        /// </summary>
        /// <param name="visitor">Instance of <see cref="IFilterCriteriaVisitor"/>.</param>
        public abstract void Accept(IFilterCriteriaVisitor visitor);

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public abstract override String ToString();

        /// <summary>
        /// Logical AND operator overload.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Instance of <see cref="FilterCriteria"/>.</returns>
        public static FilterCriteria operator &(FilterCriteria left, FilterCriteria right)
        {
            return new AndCriteria(left, right);
        }

        /// <summary>
        /// Logical OR operator overload.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Instance of <see cref="FilterCriteria"/>.</returns>
        public static FilterCriteria operator |(FilterCriteria left, FilterCriteria right)
        {
            return new OrCriteria(left, right);
        }

        /// <summary>
        /// Logical NOT operator overload.
        /// </summary>
        /// <param name="criteria"><see cref="FilterCriteria"/> to negate.</param>        
        /// <returns>Instance of <see cref="FilterCriteria"/>.</returns>
        public static FilterCriteria operator !(FilterCriteria criteria)
        {
            return new NotCriteria(criteria);
        }

        /// <summary>
        /// Equality operator overload.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Instance of <see cref="FilterCriteria"/>.</returns>
        public static Boolean operator ==(FilterCriteria left, FilterCriteria right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (null == left) return false;
            if (null == right) return false;
            return left.ToString() == right.ToString();
        }

        /// <summary>
        /// Equality operator overload.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Instance of <see cref="FilterCriteria"/>.</returns>
        public static Boolean operator !=(FilterCriteria left, FilterCriteria right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Return if
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(FilterCriteria other)
        {
            return this == other;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override Boolean Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(FilterCriteria)) return false;
            return Equals((FilterCriteria)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override Int32 GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static FilterCriteria Parse(string filter, FilterCriteriaParseOptions options = null)
        {
            IFilterCriteriaParser parser = options?.Parser ?? new FilterCriteriaParser();

            return parser.Parse(filter);
        }
    }
}
