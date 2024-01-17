using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Factory of <see cref="FilterCriteria"/> objects.
    /// </summary>
    public static class CriteriaFactory
    {
        /// <summary>
        /// Produces <see cref="AndCriteria"/>.
        /// </summary>
        /// <param name="left">Left side operand.</param>
        /// <param name="right">Right side operand.</param>
        /// <returns>Instance of <see cref="AndCriteria"/>.</returns>
        public static AndCriteria And(FilterCriteria left, FilterCriteria right)
        {
            return new AndCriteria(left, right);
        }

        /// <summary>
        /// Produces <see cref="OrCriteria"/>.
        /// </summary>
        /// <param name="left">Left side operand.</param>
        /// <param name="right">Right side operand.</param>
        /// <returns>Instance of <see cref="OrCriteria"/>.</returns>
        public static OrCriteria Or(FilterCriteria left, FilterCriteria right)
        {
            return new OrCriteria(left, right);
        }

        /// <summary>
        /// Produces <see cref="NotCriteria"/>.
        /// </summary>
        /// <param name="criteria"><see cref="FilterCriteria"/> instance to negate.</param>
        /// <returns>Instance of <see cref="NotCriteria"/>.</returns>
        public static NotCriteria Not(FilterCriteria criteria)
        {
            return new NotCriteria(criteria);
        }

        /// <summary>
        /// Produces <see cref="NullCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the constrainted property.</param>
        /// <returns>Instance of <see cref="NullCriteria"/>.</returns>
        public static NullCriteria Null(String propertyName)
        {
            return new NullCriteria(propertyName);
        }

        /// <summary>
        /// Produces <see cref="NotNullCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the constrainted property.</param>
        /// <returns>Instance of <see cref="NotNullCriteria"/>.</returns>
        public static NotNullCriteria NotNull(String propertyName)
        {
            return new NotNullCriteria(propertyName);
        }

        /// <summary>
        /// Produces <see cref="ComparisonCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value against which the property will be compared.</param>
        /// <returns>Instance of <see cref="ComparisonCriteria"/>.</returns>
        public static ComparisonCriteria Equal(String propertyName, Object? value)
        {
            return new ComparisonCriteria(propertyName, value, ComparisonOperator.Equal);
        }

        /// <summary>
        /// Produces <see cref="ComparisonCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value against which the property will be compared.</param>
        /// <returns>Instance of <see cref="ComparisonCriteria"/>.</returns>
        public static ComparisonCriteria NotEqual(String propertyName, Object value)
        {
            return new ComparisonCriteria(propertyName, value, ComparisonOperator.NotEqual);
        }

        /// <summary>
        /// Produces <see cref="ComparisonCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value against which the property will be compared.</param>
        /// <returns>Instance of <see cref="ComparisonCriteria"/>.</returns>
        public static ComparisonCriteria Greater(String propertyName, Object value)
        {
            return new ComparisonCriteria(propertyName, value, ComparisonOperator.Greater);
        }

        /// <summary>
        /// Produces <see cref="ComparisonCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value against which the property will be compared.</param>
        /// <returns>Instance of <see cref="ComparisonCriteria"/>.</returns>
        public static ComparisonCriteria Less(String propertyName, Object value)
        {
            return new ComparisonCriteria(propertyName, value, ComparisonOperator.Less);
        }

        /// <summary>
        /// Produces <see cref="ComparisonCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value against which the property will be compared.</param>
        /// <returns>Instance of <see cref="ComparisonCriteria"/>.</returns>
        public static ComparisonCriteria LessOrEqual(String propertyName, Object value)
        {
            return new ComparisonCriteria(propertyName, value, ComparisonOperator.LessOrEqual);
        }

        /// <summary>
        /// Produces <see cref="ComparisonCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value against which the property will be compared.</param>
        /// <returns>Instance of <see cref="ComparisonCriteria"/>.</returns>
        public static ComparisonCriteria GreaterOrEqual(String propertyName, Object value)
        {
            return new ComparisonCriteria(propertyName, value, ComparisonOperator.GreaterOrEqual);
        }

        /// <summary>
        /// Produces <see cref="ComparisonCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value against which the property will be compared.</param>
        /// <returns>Instance of <see cref="ComparisonCriteria"/>.</returns>
        public static ComparisonCriteria Like(String propertyName, Object value)
        {
            return new ComparisonCriteria(propertyName, value, ComparisonOperator.Like);
        }

        /// <summary>
        /// Produces <see cref="InCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to constraint.</param>
        /// <param name="values">The array of values.</param>
        /// <returns>Instance of <see cref="InCriteria"/>.</returns>
        public static InCriteria In(String propertyName, Object[] values)
        {
            return new InCriteria(propertyName, values);
        }

        /// <summary>
        /// Produces <see cref="BetweenCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to constraint.</param>
        /// <param name="begin">The begin value.</param>
        /// <param name="end">The end value.</param>
        /// <returns>Instance of <see cref="BetweenCriteria"/>.</returns>
        public static BetweenCriteria Between(String propertyName, Object begin, Object end)
        {
            return new BetweenCriteria(propertyName, begin, end);
        }

        /// <summary>
        /// Produces <see cref="FunctionCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value which will be provided to the function against which the property will be compared.</param>
        /// <returns>Instance of <see cref="FunctionCriteria"/>.</returns>
        public static FunctionCriteria StartsWith(String propertyName, Object value)
        {
            return new FunctionCriteria(FunctionCriteriaType.StartsWith, propertyName, value);
        }

        /// <summary>
        /// Produces <see cref="FunctionCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value which will be provided to the function against which the property will be compared.</param>
        /// <returns>Instance of <see cref="FunctionCriteria"/>.</returns>
        public static FunctionCriteria EndsWith(String propertyName, Object value)
        {
            return new FunctionCriteria(FunctionCriteriaType.EndsWith, propertyName, value);
        }

        /// <summary>
        /// Produces <see cref="FunctionCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value which will be provided to the function against which the property will be compared.</param>
        /// <returns>Instance of <see cref="FunctionCriteria"/>.</returns>
        public static FunctionCriteria Contains(String propertyName, Object value)
        {
            return new FunctionCriteria(FunctionCriteriaType.Contains, propertyName, value);
        }

        /// <summary>
        /// Produces <see cref="FunctionCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="FunctionCriteria"/>.</returns>
        public static FunctionCriteria IsNullOrEmpty(String propertyName)
        {
            return new FunctionCriteria(FunctionCriteriaType.IsNullOrEmpty, propertyName, null);
        }

        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalToday(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalToday, propertyName);
        }

        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalBeyondThisYear(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalBeyondThisYear, propertyName);
        }

        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalLaterThisYear(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalLaterThisYear, propertyName);
        }

        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalLaterThisMonth(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalLaterThisMonth, propertyName);
        }

        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalNextWeek(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalNextWeek, propertyName);
        }

        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalLaterThisWeek(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalLaterThisWeek, propertyName);
        }

        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalTomorrow(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalTomorrow, propertyName);
        }

        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalYesterday(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalYesterday, propertyName);
        }


        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalEarlierThisWeek(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalEarlierThisWeek, propertyName);
        }


        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalLastWeek(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalLastWeek, propertyName);
        }


        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalEarlierThisMonth(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalEarlierThisMonth, propertyName);
        }


        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalEarlierThisYear(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalEarlierThisYear, propertyName);
        }


        /// <summary>
        /// Produces <see cref="IntervalCritera"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="IntervalCritera"/>.</returns>
        public static IntervalCritera IntervalPriorThisYear(String propertyName)
        {
            return new IntervalCritera(IntervalCriteraType.IntervalPriorThisYear, propertyName);
        }


        /// <summary>
        /// Produces <see cref="CurrentUserCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <returns>Instance of <see cref="CurrentUserCriteria"/>.</returns>
        public static CurrentUserCriteria CurrentUserCriteria()
        {
            return new CurrentUserCriteria();
        }
    }
}
