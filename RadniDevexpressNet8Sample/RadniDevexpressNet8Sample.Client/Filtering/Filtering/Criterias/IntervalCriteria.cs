using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Base class for objects used as interval filters in data queries.
    /// </summary>
    [Serializable]
    public class IntervalCritera : FilterCriteria, IPropertyCriteria
    {
        /// <summary>
        /// Initializes instance of <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="intervalCriteraType">Interval type.</param>
        /// <param name="propertyName">The name of the property to compare.</param>        
        public IntervalCritera(
            IntervalCriteraType intervalCriteraType,
            String propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            IntervalCriteraType = intervalCriteraType;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the function type.
        /// </summary>
        public IntervalCriteraType IntervalCriteraType { get; private set; }

        /// <summary>
        /// Get the name of the property to compare.
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
        public override string ToString()
        {
            return "[{0} {1}]".FormatCurrentCulture(PropertyName, GetCriteraTypeDisplay());
        }

        String GetCriteraTypeDisplay()
        {
            String result;
            switch (IntervalCriteraType)
            {
                case IntervalCriteraType.IntervalBeyondThisYear:
                    result = "IntervalBeyondThisYear";
                    break;
                case IntervalCriteraType.IntervalLaterThisYear:
                    result = "IntervalLaterThisYear";
                    break;
                case IntervalCriteraType.IntervalLaterThisMonth:
                    result = "IntervalLaterThisMonth";
                    break;
                case IntervalCriteraType.IntervalNextWeek:
                    result = "IntervalNextWeek";
                    break;
                case IntervalCriteraType.IntervalLaterThisWeek:
                    result = "IntervalLaterThisWeek";
                    break;
                case IntervalCriteraType.IntervalTomorrow:
                    result = "IntervalTomorrow";
                    break;
                case IntervalCriteraType.IntervalToday:
                    result = "IntervalToday";
                    break;
                case IntervalCriteraType.IntervalYesterday:
                    result = "IntervalYesterday";
                    break;
                case IntervalCriteraType.IntervalEarlierThisWeek:
                    result = "IntervalEarlierThisWeek";
                    break;
                case IntervalCriteraType.IntervalLastWeek:
                    result = "IntervalLastWeek";
                    break;
                case IntervalCriteraType.IntervalEarlierThisMonth:
                    result = "IntervalEarlierThisMonth";
                    break;
                case IntervalCriteraType.IntervalEarlierThisYear:
                    result = "IntervalEarlierThisYear";
                    break;
                case IntervalCriteraType.IntervalPriorThisYear:
                    result = "IntervalPriorThisYear";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }

    /// <summary>
    /// Enumerates possible types of <see cref="IntervalCritera"/>.
    /// </summary>
    public enum IntervalCriteraType
    {
        // Summary:
        //     The Boolean Is Beyond This Year operator for date/time values. Requires one
        //     argument.
        //     The operator is defined as follows: date @gt;= First Day of Next Year
        IntervalBeyondThisYear,
        //
        // Summary:
        //     The Boolean Is Later This Year operator for date/time values. Requires one
        //     argument.
        //     The operator is defined as follows: First Day of Next Month @lt;= date @lt;
        //     First Day of Next Year
        IntervalLaterThisYear,
        //
        // Summary:
        //     The Boolean Is Later This Month operator for date/time values. Requires one
        //     argument.
        //     The operator is defined as follows: Last Day of Next Week @lt; date @lt;
        //     First Day of Next Month
        IntervalLaterThisMonth,
        //
        // Summary:
        //     The Boolean Is Next Week operator for date/time values. Requires one argument.
        //     The operator is defined as follows: First Day of Next Week @lt;= date @lt;=
        //     Last Day of Next Week
        IntervalNextWeek,
        //
        // Summary:
        //     The Boolean Is Later This Week operator for date/time values. Requires one
        //     argument.
        //     The operator is defined as follows: Day After Tomorrow @lt;= date @lt; First
        //     Day of Next Week
        IntervalLaterThisWeek,
        //
        // Summary:
        //     The Boolean Is Tomorrow operator for date/time values. Requires one argument.
        IntervalTomorrow,
        //
        // Summary:
        //     The Boolean Is Today operator for date/time values. Requires one argument.
        IntervalToday,
        //
        // Summary:
        //     The Boolean Is Yesterday operator for date/time values. Requires one argument.
        IntervalYesterday,
        //
        // Summary:
        //     The Boolean Is Earlier This Week operator for date/time values. Requires
        //     one argument.
        //     The operator is defined as follows: First Day of This Week @lt;= date @lt;
        //     Yesterday
        IntervalEarlierThisWeek,
        //
        // Summary:
        //     The Boolean Is Last Week operator for date/time values. Requires one argument.
        //     The operator is defined as follows: First Day of Last Week @lt;= date @lt;
        //     First Day of This Week
        IntervalLastWeek,
        //
        // Summary:
        //     The Boolean Is Earlier This Month operator for date/time values. Requires
        //     one argument.
        //     The operator is defined as follows: First Day of This Month @lt;= date @lt;
        //     First Day of Last Week
        IntervalEarlierThisMonth,
        //
        // Summary:
        //     The Boolean Is Earlier This Year operator for date/time values. Requires
        //     one argument.
        //     The operator is defined as follows: First Day of This Year @lt;= date @lt;
        //     First Day of This Month
        IntervalEarlierThisYear,
        //
        // Summary:
        //     The Boolean Is Prior This Year operator for date/time values. Requires one
        //     argument.
        //     The operator is defined as follows: date @lt; First Day of This Year
        IntervalPriorThisYear
    }
}
