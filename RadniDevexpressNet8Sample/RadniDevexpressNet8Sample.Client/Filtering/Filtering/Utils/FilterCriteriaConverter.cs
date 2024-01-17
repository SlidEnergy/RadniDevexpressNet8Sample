using System;
using System.Linq;
using Common.DataAccess.Filtering;
using DevExpress.Data.Filtering;

namespace Common.Windows.Utils
{
    /// <summary>
    /// Helper class for converting <see cref="FilterCriteria"/>  to Devex's <see cref="CriteriaOperator"/>.
    /// </summary>
    public class FilterCriteriaConverter : IFilterCriteriaConverter
    {
        /// <summary>
        /// Converts the given <see cref="CriteriaOperator"/> to <see cref="FilterCriteria"/>.
        /// </summary>
        /// <param name="criteria"><see cref="CriteriaOperator"/> to convert.</param>
        /// <returns>Instance of <see cref="FilterCriteria"/>.</returns>
        public CriteriaOperator Convert(FilterCriteria criteria)
        {
            return InternalConvert(criteria);
        }

        CriteriaOperator InternalConvert(FilterCriteria criteria)
        {
            if (criteria is LogicalCriteria)
            {
                return ConvertLogicalCriteria((LogicalCriteria)criteria);
            }
            if (criteria is NotCriteria)
            {
                return ConvertNotCriteria((NotCriteria)criteria);
            }
            if (criteria is NullCriteria)
            {
                return ConvertNullCriteria((NullCriteria)criteria);
            }
            if (criteria is NotNullCriteria)
            {
                return ConvertNotNullCriteria((NotNullCriteria)criteria);
            }
            if (criteria is ComparisonCriteria)
            {
                return ConvertComparisonCriteria((ComparisonCriteria)criteria);
            }
            if (criteria is FunctionCriteria)
            {
                return ConvertFunctionCriteria((FunctionCriteria)criteria);
            }
            if (criteria is IntervalCritera)
            {
                return ConvertIntervalCritera((IntervalCritera)criteria);
            }
            if (criteria is InCriteria)
            {
                return ConvertInCriteria((InCriteria)criteria);
            }
            if (criteria is BetweenCriteria)
            {
                return ConvertBetweenCriteria((BetweenCriteria)criteria);
            }
            if (ReferenceEquals(criteria, null))
            {
                return null;
            }

            throw new ArgumentException("Invalid filter criteria: An operand used is not supported!");
        }

        CriteriaOperator ConvertBetweenCriteria(BetweenCriteria criteria)
        {
            return new BetweenOperator(criteria.PropertyName, criteria.Begin, criteria.End);
        }

        CriteriaOperator ConvertInCriteria(InCriteria criteria)
        {
            return new InOperator(new OperandProperty(criteria.PropertyName), criteria.Values.Select(v => new OperandValue(v)).ToList());
        }

        CriteriaOperator ConvertComparisonCriteria(ComparisonCriteria criteria)
        {
            switch (criteria.Operator)
            {
                case ComparisonOperator.Equal:
                    return new BinaryOperator(criteria.PropertyName, criteria.Value, BinaryOperatorType.Equal);
                case ComparisonOperator.Greater:
                    return new BinaryOperator(criteria.PropertyName, criteria.Value, BinaryOperatorType.Greater);
                case ComparisonOperator.GreaterOrEqual:
                    return new BinaryOperator(criteria.PropertyName, criteria.Value, BinaryOperatorType.GreaterOrEqual);
                case ComparisonOperator.Less:
                    return new BinaryOperator(criteria.PropertyName, criteria.Value, BinaryOperatorType.Less);
                case ComparisonOperator.LessOrEqual:
                    return new BinaryOperator(criteria.PropertyName, criteria.Value, BinaryOperatorType.LessOrEqual);
                case ComparisonOperator.Like:
                    return new BinaryOperator(criteria.PropertyName, criteria.Value, BinaryOperatorType.Like);
                case ComparisonOperator.NotEqual:
                    return new BinaryOperator(criteria.PropertyName, criteria.Value, BinaryOperatorType.NotEqual);
                default:
                    throw new NotSupportedException("Filter  conversion system encountered an unsupported comparison operator!");
            }
        }

        CriteriaOperator ConvertFunctionCriteria(FunctionCriteria criteria)
        {
            switch (criteria.FunctionType)
            {
                case FunctionCriteriaType.StartsWith:
                    return new FunctionOperator(FunctionOperatorType.StartsWith, new OperandProperty(criteria.PropertyName), new OperandValue(criteria.Value));
                case FunctionCriteriaType.EndsWith:
                    return new FunctionOperator(FunctionOperatorType.EndsWith, new OperandProperty(criteria.PropertyName), new OperandValue(criteria.Value));
                case FunctionCriteriaType.Contains:
                    return new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(criteria.PropertyName), new OperandValue(criteria.Value));
                case FunctionCriteriaType.IsNullOrEmpty:
                    return new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(criteria.PropertyName));
                default:
                    throw new NotSupportedException("Filter conversion system encountered an unsupported function operator!");
            }
        }

        CriteriaOperator ConvertIntervalCritera(IntervalCritera criteria)
        {
            switch (criteria.IntervalCriteraType)
            {
                case IntervalCriteraType.IntervalToday:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalToday, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalBeyondThisYear:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalBeyondThisYear, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalLaterThisYear:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLaterThisYear, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalLaterThisMonth:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLaterThisMonth, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalNextWeek:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalNextWeek, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalLaterThisWeek:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLaterThisWeek, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalTomorrow:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalTomorrow, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalYesterday:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalYesterday, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalEarlierThisWeek:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalEarlierThisWeek, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalLastWeek:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLastWeek, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalEarlierThisMonth:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalEarlierThisMonth, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalEarlierThisYear:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalEarlierThisYear, new OperandProperty(criteria.PropertyName));
                case IntervalCriteraType.IntervalPriorThisYear:
                    return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalPriorThisYear, new OperandProperty(criteria.PropertyName));
                default:
                    throw new NotSupportedException("Filter conversion system encountered an unsupported function operator!");
            }
        }

        CriteriaOperator ConvertNotCriteria(NotCriteria criteria)
        {
            return new UnaryOperator(UnaryOperatorType.Not, Convert(criteria.Criteria));
        }

        CriteriaOperator ConvertNullCriteria(NullCriteria criteria)
        {
            return new UnaryOperator(UnaryOperatorType.IsNull, criteria.PropertyName);
        }

        CriteriaOperator ConvertNotNullCriteria(NotNullCriteria criteria)
        {
            return new UnaryOperator(UnaryOperatorType.Not, new UnaryOperator(UnaryOperatorType.IsNull, criteria.PropertyName));
        }

        CriteriaOperator ConvertLogicalCriteria(LogicalCriteria criteria)
        {
            CriteriaOperator result;
            if (criteria is AndCriteria)
            {
                result = new GroupOperator(GroupOperatorType.And, Convert(criteria.LeftOperand), Convert(criteria.RightOperand));
            }
            else
            {
                result = new GroupOperator(GroupOperatorType.Or, Convert(criteria.LeftOperand), Convert(criteria.RightOperand));
            }
            return result;
        }

        Object ConvertFieldOrValue(CriteriaOperator criteriaOperator)
        {
            if (criteriaOperator is OperandProperty)
                // the second argument is obviously ignored, conversion is used only when the right hand side is
                // a literal value.
                return ((OperandProperty)criteriaOperator).PropertyName;
            if (criteriaOperator is OperandValue)
                return ((OperandValue)criteriaOperator).Value;
            throw new ArgumentException("Invalid filter criteria: a conversion method was expecting a field or a value!");
        }
    }
}
