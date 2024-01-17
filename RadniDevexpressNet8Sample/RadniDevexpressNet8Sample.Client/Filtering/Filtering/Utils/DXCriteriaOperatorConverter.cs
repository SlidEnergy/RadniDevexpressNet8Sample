using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataAccess.Filtering;
using DevExpress.Data.Filtering;

namespace Common.Windows.Utils
{
    /// <summary>
    /// Helper class for converting Devex's <see cref="CriteriaOperator"/>  to <see cref="FilterCriteria"/>.
    /// </summary>
    public class DXCriteriaOperatorConverter : IDXCriteriaOperatorConverter
    {
        private string GetPropertyName(string propertyName)
        {
            return propertyName;
        }

        /// <summary>
        /// Converts the given <see cref="CriteriaOperator"/> to <see cref="FilterCriteria"/>.
        /// </summary>
        /// <param name="criteria"><see cref="CriteriaOperator"/> to convert.</param>        
        /// <returns>Instance of <see cref="FilterCriteria"/>.</returns>
        public FilterCriteria Convert(CriteriaOperator criteria)
        {
            return InternalConvert(criteria);
        }

        FilterCriteria InternalConvert(CriteriaOperator criteria)
        {
            if (criteria is GroupOperator)
            {
                return ConvertGroupOperator((GroupOperator)criteria);
            }
            if (criteria is UnaryOperator)
            {
                var unary = (UnaryOperator)criteria;
                if (unary.OperatorType == UnaryOperatorType.IsNull)
                    return ConvertNullOperator(unary);
                if (unary.OperatorType == UnaryOperatorType.Not)
                    return ConvertNotOperator(unary);
            }
            else if (criteria is BinaryOperator)
            {
                return ConvertBinaryOperator((BinaryOperator)criteria);
            }
            else if (criteria is InOperator)
            {
                return ConvertInOperator((InOperator)criteria);
            }
            else if (criteria is BetweenOperator)
            {
                return ConvertBetweenOperator((BetweenOperator)criteria);
            }
            else if (criteria is FunctionOperator)
            {
                return ConvertFunctionOperator((FunctionOperator)criteria);
            }
            else if (ReferenceEquals(criteria, null))
            {
                return null;
            }

            throw new ArgumentException("Unsupported filter criteria '{0}'.".FormatInvariantCulture(criteria.GetType().Name));
        }

        FilterCriteria ConvertBetweenOperator(BetweenOperator betweenOperator)
        {
            var field = ((OperandProperty)betweenOperator.TestExpression).PropertyName;
            return CriteriaFactory.Between(
                GetPropertyName(field),
                ConvertFieldOrValue(betweenOperator.BeginExpression),
                ConvertFieldOrValue(betweenOperator.EndExpression)
                );
        }

        FilterCriteria ConvertInOperator(InOperator inOperator)
        {
            var field = ((OperandProperty)inOperator.LeftOperand).PropertyName;
            return CriteriaFactory.In(GetPropertyName(field), inOperator.Operands.Select(o => ((OperandValue)o).Value).ToArray());
        }

        FilterCriteria ConvertBinaryOperator(BinaryOperator binaryOperator)
        {
            var field = ((OperandProperty)binaryOperator.LeftOperand).PropertyName;
            field = GetPropertyName(field);
            var value = ConvertFieldOrValue(binaryOperator.RightOperand);

            switch (binaryOperator.OperatorType)
            {
                case BinaryOperatorType.Equal:
                    return CriteriaFactory.Equal(field, value);
                case BinaryOperatorType.Greater:
                    return CriteriaFactory.Greater(field, value);
                case BinaryOperatorType.GreaterOrEqual:
                    return CriteriaFactory.GreaterOrEqual(field, value);
                case BinaryOperatorType.Less:
                    return CriteriaFactory.Less(field, value);
                case BinaryOperatorType.LessOrEqual:
                    return CriteriaFactory.LessOrEqual(field, value);
                case BinaryOperatorType.Like:
                    return CriteriaFactory.Like(field, value);
                case BinaryOperatorType.NotEqual:
                    return CriteriaFactory.NotEqual(field, value);
                default:
                    throw new NotSupportedException(
                        "Binary operator '{0}' not supported."
                            .FormatInvariantCulture(Enum.GetName(typeof(BinaryOperatorType), binaryOperator.OperatorType)));
            }
        }

        FilterCriteria ConvertNotOperator(UnaryOperator unary)
        {
            return CriteriaFactory.Not(Convert(unary.Operand));
        }

        FilterCriteria ConvertNullOperator(UnaryOperator unary)
        {
            return CriteriaFactory.Null(((OperandProperty)unary.Operand).PropertyName);
        }

        FilterCriteria ConvertGroupOperator(GroupOperator groupOperator)
        {
            FilterCriteria result;
            if (groupOperator.OperatorType == GroupOperatorType.And)
            {
                result = CriteriaFactory.And(Convert(groupOperator.Operands[0]), Convert(groupOperator.Operands[1]));
                groupOperator.Operands.Skip(2).ToList().ForEach(
                    o =>
                    {
                        result = CriteriaFactory.And(result, Convert(o));
                    });
            }
            else
            {
                result = CriteriaFactory.Or(Convert(groupOperator.Operands[0]), Convert(groupOperator.Operands[1]));
                groupOperator.Operands.Skip(2).ToList().ForEach(
                    o =>
                    {
                        result = CriteriaFactory.Or(result, Convert(o));
                    });
            }
            return result;
        }

        FilterCriteria ConvertFunctionOperator(FunctionOperator funcOperator)
        {
            if (funcOperator.OperatorType == FunctionOperatorType.Custom)
            {
                throw new NotSupportedException(
                        "Function operator '{0}' not supported."
                            .FormatInvariantCulture(Enum.GetName(typeof(FunctionOperatorType), funcOperator.OperatorType)));
            }

            var field = ((OperandProperty)funcOperator.Operands[0]).PropertyName;
            field = GetPropertyName(field);

            switch (funcOperator.OperatorType)
            {
                case FunctionOperatorType.StartsWith:
                    return CriteriaFactory.StartsWith(field, ((OperandValue)funcOperator.Operands[1]).Value);

                case FunctionOperatorType.EndsWith:
                    return CriteriaFactory.EndsWith(field, ((OperandValue)funcOperator.Operands[1]).Value);

                case FunctionOperatorType.Contains:
                    return CriteriaFactory.Contains(field, ((OperandValue)funcOperator.Operands[1]).Value);

                case FunctionOperatorType.IsNullOrEmpty:
                    return CriteriaFactory.IsNullOrEmpty(field);

                case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
                    return CriteriaFactory.IntervalBeyondThisYear(field);

                case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
                    return CriteriaFactory.IntervalLaterThisYear(field);

                case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
                    return CriteriaFactory.IntervalLaterThisMonth(field);

                case FunctionOperatorType.IsOutlookIntervalNextWeek:
                    return CriteriaFactory.IntervalNextWeek(field);

                case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
                    return CriteriaFactory.IntervalLaterThisWeek(field);

                case FunctionOperatorType.IsOutlookIntervalTomorrow:
                    return CriteriaFactory.IntervalTomorrow(field);

                case FunctionOperatorType.IsOutlookIntervalToday:
                    return CriteriaFactory.IntervalToday(field);

                case FunctionOperatorType.IsOutlookIntervalYesterday:
                    return CriteriaFactory.IntervalYesterday(field);

                case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
                    return CriteriaFactory.IntervalEarlierThisWeek(field);

                case FunctionOperatorType.IsOutlookIntervalLastWeek:
                    return CriteriaFactory.IntervalLastWeek(field);

                case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
                    return CriteriaFactory.IntervalEarlierThisMonth(field);

                case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
                    return CriteriaFactory.IntervalEarlierThisYear(field);

                case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
                    return CriteriaFactory.IntervalPriorThisYear(field);

                case FunctionOperatorType.IsSameDay:
                    return ConvertIsSameDay(funcOperator, field);

                default:
                    throw new NotSupportedException(
                        "Function operator '{0}' not supported."
                            .FormatInvariantCulture(Enum.GetName(typeof(FunctionOperatorType), funcOperator.OperatorType)));
            }
        }

        FilterCriteria ConvertIsSameDay(FunctionOperator funcOperator, string field)
        {
            FilterCriteria filter = null;

            for (var i = 1; i < funcOperator.Operands.Count; i++)
            {
                var date = ((OperandValue)funcOperator.Operands[i]).Value as DateTime?;
                if (date == null)
                    throw new Exception(
                        "Invalid function operator: a conversion method was expecting a not nullable value of DateTime type!");

                var day = date.Value.Date;

                var criteria = CriteriaFactory.And(CriteriaFactory.GreaterOrEqual(field, day),
                    CriteriaFactory.LessOrEqual(field, day.AddDays(1).AddMilliseconds(-1)));

                if (filter == null)
                    filter = criteria;
                else
                    filter = CriteriaFactory.Or(filter, criteria);
            }

            return filter;
        }

        Object ConvertFieldOrValue(CriteriaOperator criteriaOperator)
        {
            if (criteriaOperator is OperandProperty)
                // the second argument is obviously ignored, conversion is used only when the right hand side is
                // a literal value.
                return GetPropertyName(((OperandProperty)criteriaOperator).PropertyName);
            if (criteriaOperator is OperandValue)
                return ((OperandValue)criteriaOperator).Value;
            throw new ArgumentException("Invalid filter criteria: a conversion method was expecting a field or a value!");
        }
    }
}
