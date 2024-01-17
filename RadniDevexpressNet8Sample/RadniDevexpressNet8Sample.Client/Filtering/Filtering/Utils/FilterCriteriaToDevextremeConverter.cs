using Common.DataAccess.Filtering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Filtering.Utils
{
    public class FilterCriteriaToDevextremeConverter : IFilterCriteriaToDevextremeConverter
    {
        public string Convert(FilterCriteria criteria)
        {
            var jarray = InternalConvert(criteria);

            return JsonConvert.SerializeObject(jarray);
        }

        JArray InternalConvert(FilterCriteria criteria)
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
            //if (criteria is IntervalCritera)
            //{
            //    return ConvertIntervalCritera((IntervalCritera)criteria);
            //}
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

        JArray ConvertBetweenCriteria(BetweenCriteria criteria)
        {
            return ConvertLogicalCriteria(CriteriaFactory.And(CriteriaFactory.Greater(criteria.PropertyName, criteria.Begin), CriteriaFactory.Less(criteria.PropertyName, criteria.End)));
        }

        JArray ConvertInCriteria(InCriteria criteria)
        {
            FilterCriteria left = CriteriaFactory.Equal(criteria.PropertyName, criteria.Values[0]);

            if (criteria.Values.Length == 1)
                return ConvertComparisonCriteria((ComparisonCriteria)left);

            for (int i = 0; i < criteria.Values.Length; i++)
            {
                var right = CriteriaFactory.Equal(criteria.PropertyName, criteria.Values[i]);
                left = CriteriaFactory.Or(left, right);
            }

            return ConvertLogicalCriteria((LogicalCriteria)left);
        }

        JArray ConvertComparisonCriteria(ComparisonCriteria criteria)
        {
            string op = "";

            switch (criteria.Operator)
            {
                case ComparisonOperator.Equal:
                    op = "=";
                    break;
                case ComparisonOperator.Greater:
                    op = ">";
                    break;
                case ComparisonOperator.GreaterOrEqual:
                    op = ">=";
                    break;
                case ComparisonOperator.Less:
                    op = "<";
                    break;
                case ComparisonOperator.LessOrEqual:
                    op = "<=";
                    break;
                case ComparisonOperator.Like:
                    throw new NotSupportedException("Filter  conversion system encountered an unsupported comparison operator!");
                case ComparisonOperator.NotEqual:
                    op = "<>";
                    break;
                default:
                    throw new NotSupportedException("Filter  conversion system encountered an unsupported comparison operator!");
            }

            return new JArray(criteria.PropertyName, op, criteria.Value);
        }

        JArray ConvertFunctionCriteria(FunctionCriteria criteria)
        {
            var op = "";

            switch (criteria.FunctionType)
            {
                case FunctionCriteriaType.StartsWith:
                    op = "startswith";
                    break;
                case FunctionCriteriaType.EndsWith:
                    op = "endswith";
                    break;
                case FunctionCriteriaType.Contains:
                    op = "contains";
                    break;
                case FunctionCriteriaType.IsNullOrEmpty:
                    return ConvertLogicalCriteria(CriteriaFactory.Or(CriteriaFactory.Equal(criteria.PropertyName, null), CriteriaFactory.Equal(criteria.PropertyName, "")));
                default:
                    throw new NotSupportedException("Filter conversion system encountered an unsupported function operator!");
            }

            return new JArray(criteria.PropertyName, op, criteria.Value);
        }

        //string ConvertIntervalCritera(IntervalCritera criteria)
        //{
        //    switch (criteria.IntervalCriteraType)
        //    {
        //        case IntervalCriteraType.IntervalToday:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalToday, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalBeyondThisYear:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalBeyondThisYear, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalLaterThisYear:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLaterThisYear, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalLaterThisMonth:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLaterThisMonth, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalNextWeek:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalNextWeek, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalLaterThisWeek:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLaterThisWeek, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalTomorrow:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalTomorrow, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalYesterday:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalYesterday, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalEarlierThisWeek:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalEarlierThisWeek, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalLastWeek:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLastWeek, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalEarlierThisMonth:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalEarlierThisMonth, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalEarlierThisYear:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalEarlierThisYear, new OperandProperty(criteria.PropertyName));
        //        case IntervalCriteraType.IntervalPriorThisYear:
        //            return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalPriorThisYear, new OperandProperty(criteria.PropertyName));
        //        default:
        //            throw new NotSupportedException("Filter conversion system encountered an unsupported function operator!");
        //    }
        //}

        JArray ConvertNotCriteria(NotCriteria criteria)
        {
            return new JArray("!", InternalConvert(criteria.Criteria));
        }

        JArray ConvertNullCriteria(NullCriteria criteria)
        {
            return ConvertComparisonCriteria(CriteriaFactory.Equal(criteria.PropertyName, null));
        }

        JArray ConvertNotNullCriteria(NotNullCriteria criteria)
        {
            return ConvertNotCriteria(CriteriaFactory.Not(CriteriaFactory.Equal(criteria.PropertyName, null)));
        }

        JArray ConvertLogicalCriteria(LogicalCriteria criteria)
        {
            return new JArray(InternalConvert(criteria.LeftOperand), criteria is AndCriteria ? "and" : "or", InternalConvert(criteria.RightOperand));
        }
    }
}
