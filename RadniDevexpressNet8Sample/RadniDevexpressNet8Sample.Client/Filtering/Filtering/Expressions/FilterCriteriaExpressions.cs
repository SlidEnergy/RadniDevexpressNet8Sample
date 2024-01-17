using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Builds <see cref="Expression"/> trees based on <see cref="FilterCriteria"/>
    /// and determines whether the filter criteria is satisfied for the given object.
    /// </summary>
    public static class FilterCriteriaExpressions
    {
        /// <summary>
        /// Determines whether the filter criteria is satisfied for the given <paramref name="obj"/>.
        /// </summary>
        /// <param name="criteria">Filter criteria.</param>
        /// <param name="type"> Filtered object.</param>
        /// <param name="obj">Filtered object.</param>
        /// <returns><c>True</c> if the <paramref name="criteria"/> is satisfied for the <paramref name="obj"/>; otherwise <c>false</c>.</returns>
        public static Boolean IsSatisfiedOn(this FilterCriteria criteria, Type type, object obj)
        {
            return criteria.IsValidOn(type) && new FilterCriteriaExpressionBuilder(type, obj).IsSatisfied(criteria);
        }

        /// <summary>
        /// Determines whether the filter criteria is satisfied for the given <paramref name="obj"/>.
        /// </summary>
        /// <typeparam name="T">Filtered object type.</typeparam>
        /// <param name="criteria">Filter criteria.</param>
        /// <param name="obj">Filtered object.</param>
        /// <returns><c>True</c> if the <paramref name="criteria"/> is satisfied for the <paramref name="obj"/>; otherwise <c>false</c>.</returns>
        public static Boolean IsSatisfiedOn<T>(this FilterCriteria criteria, T obj)
        {
            return criteria.IsSatisfiedOn(typeof(T), obj);
        }
    }

    public class FilterCriteriaExpressionBuilder
    {
        private readonly Type _type;
        private readonly object _obj;
        private readonly ParameterExpression _parameter;

        public FilterCriteriaExpressionBuilder(Type type, object obj)
        {
            _type = type;
            _obj = obj;
            _parameter = Expression.Parameter(_type, "e");
        }

        public FilterCriteriaExpressionBuilder(Type type, ParameterExpression parameter)
        {
            _type = type;
            _obj = null;
            _parameter = parameter;
        }


        public bool IsSatisfied(FilterCriteria criteria)
        {
            try
            {
                var result = Expression
                    .Lambda(typeof(Func<,>).MakeGenericType(_type, typeof(bool)), InternalBuild(criteria), _parameter)
                    .Compile()
                    .DynamicInvoke(_obj);
                return (bool)result;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public Expression BuildExpression(FilterCriteria criteria)
        {
            return InternalBuild(criteria);
        }

        Expression InternalBuild(FilterCriteria criteria)
        {
            Expression body;

            if (criteria is AndCriteria)
                body = BuildAndCriteria((LogicalCriteria)criteria);
            else if (criteria is OrCriteria)
                body = BuildOrCriteria((LogicalCriteria)criteria);
            else if (criteria is NotCriteria)
                body = BuildNotCriteria((NotCriteria)criteria);
            else if (criteria is NullCriteria)
                body = BuildNullCriteria((NullCriteria)criteria);
            else if (criteria is NotNullCriteria)
                body = BuildNotNullCriteria((NotNullCriteria)criteria);
            else if (criteria is ComparisonCriteria)
                body = BuildComparisonCriteria((ComparisonCriteria)criteria);
            else if (criteria is BetweenCriteria)
                body = BuildBetweenCriteria((BetweenCriteria)criteria);
            else if (criteria is FunctionCriteria)
                body = BuildFunctionCriteria((FunctionCriteria)criteria);
            else if (criteria is InCriteria)
                body = BuildInCriteria((InCriteria)criteria);
            else if (criteria is AnyCriteria)
                body = BuildAnyCriteria((AnyCriteria)criteria);
            else if (criteria is IntervalCritera)
                body = BuildIntervalCriteria((IntervalCritera)criteria);
            else
                throw new ArgumentOutOfRangeException();

            return body;

        }

        #region Internal builder

        Expression BuildAndCriteria(LogicalCriteria criteria)
        {
            return Expression.AndAlso(InternalBuild(criteria.LeftOperand), InternalBuild(criteria.RightOperand));
        }

        Expression BuildOrCriteria(LogicalCriteria criteria)
        {
            return Expression.Or(InternalBuild(criteria.LeftOperand), InternalBuild(criteria.RightOperand));
        }

        Expression BuildNotCriteria(NotCriteria criteria)
        {
            return Expression.Not(InternalBuild(criteria.Criteria));
        }

        Expression BuildNullCriteria(IPropertyCriteria criteria)
        {
            var memberExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            return BuildEqualCriteria(CriteriaFactory.Equal(criteria.PropertyName, null));
                return memberExpression.Type.IsNullable()
                ? BuildEqualCriteria(CriteriaFactory.Equal(criteria.PropertyName, null))
                : Expression.Constant(false, memberExpression.Type);
        }

        Expression BuildNotNullCriteria(IPropertyCriteria criteria)
        {
            var memberExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            return memberExpression.Type.IsNullable()
                ? BuildNotEqualCriteria(CriteriaFactory.NotEqual(criteria.PropertyName, null))
                : Expression.Constant(true, memberExpression.Type);
        }

        Expression BuildComparisonCriteria(ComparisonCriteria criteria)
        {
            switch (criteria.Operator)
            {
                case ComparisonOperator.Equal:
                    return BuildEqualCriteria(criteria);
                case ComparisonOperator.NotEqual:
                    return BuildNotEqualCriteria(criteria);
                case ComparisonOperator.Greater:
                    return BuildGreaterCriteria(criteria);
                case ComparisonOperator.GreaterOrEqual:
                    return BuildGreaterOrEqualCriteria(criteria);
                case ComparisonOperator.Less:
                    return BuildLessCriteria(criteria);
                case ComparisonOperator.LessOrEqual:
                    return BuildLessOrEqualCriteria(criteria);
                case ComparisonOperator.Like:
                    return BuildLikeCriteria(criteria);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Expression BuildEqualCriteria(ComparisonCriteria criteria)
        {
            var memberExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            var constantExpression = GetExpressionConstant(criteria.Value, memberExpression.Type);

            if (memberExpression.Type.IsEnum)
            {
                var enumType = memberExpression.Type;
                var compareType = Enum.GetUnderlyingType(enumType);
                var enumField = Expression.Convert(memberExpression, compareType);
                var enumValue = Expression.Convert(constantExpression, compareType);
                return Expression.Equal(enumField, enumValue);
            }
            else
            {
                return Expression.Equal(memberExpression, constantExpression);
            }
        }

        public static MemberExpression GetMemberExpression(Expression expr, string propertyName)
        {
            var properties = propertyName.Split('.');

            MemberExpression expression = null;

            foreach (var property in properties)
            {
                if (expression == null)
                    expression = Expression.Property(expr, property);
                else
                    expression = Expression.Property(expression, property);
            }

            return expression;
        }

        public static Expression GetExpressionConstant(object value, Type memberType)
        {
            object criteriaValue = value;
            var memberExpressionType = memberType;
            //if (memberType.IsNullable())
            //{
            //    var underLyingType = Nullable.GetUnderlyingType(memberType);
            //    if (underLyingType != null && underLyingType.IsEnum)
            //    {
            //        memberExpressionType = typeof(int);
            //    }
            //}
            if (memberType.IsEnum)
            {
                memberExpressionType = typeof(int);
            }
            if (memberType.IsDecimal() && criteriaValue.IsNotNull())
            {
                criteriaValue = Convert.ToDecimal(criteriaValue);
            }
            return Expression.Constant(criteriaValue, memberExpressionType);
        }

        Expression BuildNotEqualCriteria(ComparisonCriteria criteria)
        {
            var memberExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            var constantExpression = GetExpressionConstant(criteria.Value, memberExpression.Type);

            if (memberExpression.Type.IsEnum)
            {
                var enumType = memberExpression.Type;
                var compareType = Enum.GetUnderlyingType(enumType);
                var enumField = Expression.Convert(memberExpression, compareType);
                var enumValue = Expression.Convert(constantExpression, compareType);
                return Expression.NotEqual(enumField, enumValue);
            }
            else
            {
                return Expression.NotEqual(memberExpression, constantExpression);
            }
        }

        Expression BuildGreaterCriteria(ComparisonCriteria criteria)
        {
            var left = GetMemberExpression(_parameter, criteria.PropertyName);
            var right = GetExpressionConstant(criteria.Value, left.Type);

            return left.Type == typeof(string)
                ? Expression.GreaterThan(Expression.Call(null, left.Type.GetMethod("Compare", new[] { left.Type, right.Type }), left, right), Expression.Constant(0))
                : Expression.GreaterThan(left, right);
        }

        Expression BuildGreaterOrEqualCriteria(ComparisonCriteria criteria)
        {
            var left = GetMemberExpression(_parameter, criteria.PropertyName);
            var right = GetExpressionConstant(criteria.Value, left.Type);

            return left.Type == typeof(string)
                ? Expression.GreaterThanOrEqual(Expression.Call(null, left.Type.GetMethod("Compare", new[] { left.Type, right.Type }), left, right), Expression.Constant(0))
                : Expression.GreaterThanOrEqual(left, right);
        }

        Expression BuildLessCriteria(ComparisonCriteria criteria)
        {
            var left = GetMemberExpression(_parameter, criteria.PropertyName);
            var right = GetExpressionConstant(criteria.Value, left.Type);

            return left.Type == typeof(string)
                ? Expression.LessThan(Expression.Call(null, left.Type.GetMethod("Compare", new[] { left.Type, right.Type }), left, right), Expression.Constant(0))
                : Expression.LessThan(left, right);
        }

        Expression BuildLessOrEqualCriteria(ComparisonCriteria criteria)
        {
            var left = GetMemberExpression(_parameter, criteria.PropertyName);
            var right = GetExpressionConstant(criteria.Value, left.Type);

            return left.Type == typeof(string)
                ? Expression.LessThanOrEqual(Expression.Call(null, left.Type.GetMethod("Compare", new[] { left.Type, right.Type }), left, right), Expression.Constant(0))
                : Expression.LessThanOrEqual(left, right);
        }

        Expression BuildLikeCriteria(ComparisonCriteria criteria)
        {
            var left = GetMemberExpression(_parameter, criteria.PropertyName);

            if (left.Type != typeof(string))
                return Expression.Constant(false, left.Type);

            var likePattern = criteria.Value as string;
            if (!string.IsNullOrEmpty(likePattern))
            {
                var startsWithPercent = likePattern.StartsWith("%");
                var endsWithPercent = likePattern.EndsWith("%");
                if (endsWithPercent)
                {
                    if (!startsWithPercent)
                        return BuildStartsWith(CriteriaFactory.StartsWith(criteria.PropertyName, likePattern.Substring(0, likePattern.Length - 1)));
                    if (likePattern.Length >= 2)
                        return BuildContains(CriteriaFactory.Contains(criteria.PropertyName, likePattern.Substring(1, likePattern.Length - 2)));
                    return BuildStartsWith(CriteriaFactory.StartsWith(criteria.PropertyName, string.Empty));
                }
                if (startsWithPercent)
                    return BuildEndsWith(CriteriaFactory.EndsWith(criteria.PropertyName, likePattern.Substring(1)));
                return BuildEqualCriteria(CriteriaFactory.Equal(criteria.PropertyName, likePattern));
            }

            return Expression.Constant(false, left.Type);
        }

        Expression BuildBetweenCriteria(BetweenCriteria criteria)
        {
            return Expression.AndAlso(
                BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, criteria.Begin)),
                BuildLessOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, criteria.End)));
        }

        Expression BuildFunctionCriteria(FunctionCriteria criteria)
        {
            switch (criteria.FunctionType)
            {
                case FunctionCriteriaType.StartsWith:
                    return BuildStartsWith(criteria);
                case FunctionCriteriaType.EndsWith:
                    return BuildEndsWith(criteria);
                case FunctionCriteriaType.Contains:
                    return BuildContains(criteria);
                case FunctionCriteriaType.IsNullOrEmpty:
                    return BuildIsNullOrEmpty(criteria);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Expression BuildStartsWith(FunctionCriteria criteria)
        {
            var memberExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            var valueString = Convert.ToString(criteria.Value);
            if (memberExpression.Type == typeof(string))
                return Expression.Call(
                    memberExpression,
                    typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                    Expression.Constant(criteria.Value));
            else if (memberExpression.Type == typeof(int) || memberExpression.Type == typeof(int?) 
                || memberExpression.Type == typeof(long) || memberExpression.Type == typeof(long?))
            {
                var trimmedString = convertIntToString(memberExpression);
                return Expression.Call(trimmedString, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                    Expression.Constant(valueString));
            }
            else if (memberExpression.Type == typeof(double) || memberExpression.Type == typeof(float) /*|| memberExpression.Type == typeof(decimal)*/
                || memberExpression.Type == typeof(double?) || memberExpression.Type == typeof(float?)) //|| memberExpression.Type == typeof(decimal?))
            {
                var trimmed = GetDoubleTrimmedExpression(memberExpression);
                return Expression.Call(trimmed, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                    Expression.Constant(valueString));
            }
            else if (memberExpression.Type == typeof(decimal) || memberExpression.Type == typeof(decimal?))
            {
                var trimmed = GetDecimalTrimmedExpression(memberExpression);
                return Expression.Call(trimmed, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                    Expression.Constant(valueString));
            }
            else if (memberExpression.Type == typeof(DateTime) || memberExpression.Type == typeof(DateTime?))
            {
                var strDatePartsExpression = getDatePartsSTRFromMemberExpressions(memberExpression);
                var sqlFormatDateString = convertToSQLFormatDateStr((string)criteria.Value);

                return Expression.Call(strDatePartsExpression, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                    Expression.Constant(sqlFormatDateString));
            }
            return Expression.Constant(false, memberExpression.Type);
        }

        Expression BuildEndsWith(FunctionCriteria criteria)
        {
            var memberExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            var valueString = Convert.ToString(criteria.Value);
            if (memberExpression.Type == typeof(string))
            {
                return Expression.Call(
                    memberExpression,
                    typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                    Expression.Constant(criteria.Value));
            }
            else if (memberExpression.Type == typeof(int) || memberExpression.Type == typeof(int?)
               || memberExpression.Type == typeof(long) || memberExpression.Type == typeof(long?))
            {
                var trimmedString = convertIntToString(memberExpression);
                return Expression.Call(trimmedString, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                    Expression.Constant(valueString));
            }
            else if (memberExpression.Type == typeof(double) || memberExpression.Type == typeof(float) /*|| memberExpression.Type == typeof(decimal)*/
                || memberExpression.Type == typeof(double?) || memberExpression.Type == typeof(float?)) //|| memberExpression.Type == typeof(decimal?))
            {
                var trimmed = GetDoubleTrimmedExpression(memberExpression);
                return Expression.Call(trimmed, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                    Expression.Constant(valueString));
            }
            else if (memberExpression.Type == typeof(decimal) || memberExpression.Type == typeof(decimal?))
            {
                var trimmed = GetDecimalTrimmedExpression(memberExpression);
                return Expression.Call(trimmed, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                    Expression.Constant(valueString));
            }
            else if (memberExpression.Type == typeof(DateTime) || memberExpression.Type == typeof(DateTime?))
            {
                var strDatePartsExpression = getDatePartsSTRFromMemberExpressions(memberExpression);
                var sqlFormatDateString = convertToSQLFormatDateStr((string)criteria.Value);

                return Expression.Call(strDatePartsExpression, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                    Expression.Constant(sqlFormatDateString));
            }
            return Expression.Constant(false, memberExpression.Type);
        }

        Expression BuildContains(FunctionCriteria criteria)
        {
            var memberExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            var valueString = Convert.ToString(criteria.Value);
            if (memberExpression.Type == typeof(string))
            {
                return Expression.Call(
                    memberExpression,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                    Expression.Constant(criteria.Value));
            }
            else if (memberExpression.Type == typeof(int) || memberExpression.Type == typeof(int?)
               || memberExpression.Type == typeof(long) || memberExpression.Type == typeof(long?))
            {
                var trimmedString = convertIntToString(memberExpression);
                return Expression.Call(trimmedString, typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                    Expression.Constant(valueString));
            }
            else if (memberExpression.Type == typeof(double) || memberExpression.Type == typeof(float) /*|| memberExpression.Type == typeof(decimal)*/
                || memberExpression.Type == typeof(double?) || memberExpression.Type == typeof(float?)) //|| memberExpression.Type == typeof(decimal?))
            {
                var trimmed = GetDoubleTrimmedExpression(memberExpression);
                return Expression.Call(trimmed, typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                    Expression.Constant(valueString));
            }
            else if (memberExpression.Type == typeof(decimal) || memberExpression.Type == typeof(decimal?))
            {
                var trimmed = GetDecimalTrimmedExpression(memberExpression);
                return Expression.Call(trimmed, typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                    Expression.Constant(valueString));
            }
            else if (memberExpression.Type == typeof(DateTime) || memberExpression.Type == typeof(DateTime?))
            {
                var strDatePartsExpression = getDatePartsSTRFromMemberExpressions(memberExpression);
                var sqlFormatDateString = convertToSQLFormatDateStr((string)criteria.Value);

                return Expression.Call(strDatePartsExpression, typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                    Expression.Constant(sqlFormatDateString));
            }

            return Expression.Constant(false, memberExpression.Type);
        }

        Expression BuildIsNullOrEmpty(IPropertyCriteria criteria)
        {
            var memberExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            if (memberExpression.Type == typeof(string))
                return Expression.Or(BuildNullCriteria(CriteriaFactory.Null(criteria.PropertyName)), BuildEqualCriteria(CriteriaFactory.Equal(criteria.PropertyName, string.Empty)));
            else if (memberExpression.Type == typeof(int) || memberExpression.Type == typeof(double) || memberExpression.Type == typeof(decimal)
                || memberExpression.Type == typeof(int?) || memberExpression.Type == typeof(double?) || memberExpression.Type == typeof(decimal?))
            {
                return BuildNullCriteria(criteria);
            }
            return Expression.Constant(false, memberExpression.Type);
        }

        private static MethodCallExpression convertIntToString(Expression memberExpression)
        {
            //convert your Expression to a nullable double (or nullable decimal),
            //so that you can use SqlFunctions.StringConvert
            var castExpression = Expression.Convert(memberExpression, typeof(double?));
            //get the SqlFunctions.StringConvert method for nullable double
            //var stringConvertMethod = typeof(SqlFunctions).GetMethod("StringConvert", new[] { typeof(double?) });
            //call StringConvert on your converted expression
            //var convertAndCast = Expression.Call(null, stringConvertMethod, castExpression);
            //bez TrimStart ne radi
            //var trimmed = Expression.Call(convertAndCast, typeof(string).GetMethod("Trim", new Type[] { }));
            //return trimmed;
            return null;
        }

        private Expression GetDoubleTrimmedExpression(MemberExpression memberExpression)
        {
            var castExpression = Expression.Convert(memberExpression, typeof(double?));
            //var stringConvertMethodInfo = typeof(SqlFunctions).GetMethod("StringConvert", new[] { typeof(double?) });
            //var convertExpression = Expression.Call(stringConvertMethodInfo, castExpression);
            //var trimmed = Expression.Call(convertExpression, typeof(string).GetMethod("Trim", new Type[] { }));
            //return trimmed;
            return null;
        }

        private Expression GetDecimalTrimmedExpression(MemberExpression memberExpression)
        {
            var castExpression = Expression.Convert(memberExpression, typeof(decimal?));
            //var stringConvertMethodInfo = typeof(SqlFunctions).GetMethod("StringConvert", new[] { typeof(decimal?) });
            //var convertExpression = Expression.Call(stringConvertMethodInfo, castExpression);
            //var trimmed = Expression.Call(convertExpression, typeof(string).GetMethod("Trim", new Type[] { }));
            //return trimmed;
            return null;
        }

        Expression BuildInCriteria(InCriteria criteria)
        {
            FilterCriteria filter = null;
            bool isFirstValue = true;
            foreach (var value in criteria.Values)
            {
                if (isFirstValue)
                {
                    filter = CriteriaFactory.Equal(criteria.PropertyName, value);
                    isFirstValue = false;
                }
                else
                {
                    filter = CriteriaFactory.Or(filter, CriteriaFactory.Equal(criteria.PropertyName, value));
                }
            }

            return InternalBuild(filter);


            //var type = criteria.Values.GetType().GetElementType();
            //var method = typeof(Queryable)
            //    .GetMethods()
            //    .Where(m => m.Name == "Any")
            //    .Select(m => m.MakeGenericMethod(type))
            //    .Skip(1)
            //    .First();

            //var memberExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            //var constantExpression = Expression.Constant(criteria.Values[0], memberExpression.Type);

            ////var constantExp = Expression.Constant(memberExpression.Member.ReflectedType.GetProperty(memberExpression.Member.Name).GetValue(_obj, null), memberExpression.Type);

            //var body = Expression.Equal(memberExpression, constantExpression);
            //var predicate = Expression.Lambda(body, _parameter);
            //return Expression.Call(method, predicate);
        }

        Expression BuildIntervalCriteria(IntervalCritera criteria)
        {
            switch (criteria.IntervalCriteraType)
            {
                case IntervalCriteraType.IntervalBeyondThisYear:
                    //     The operator is defined as follows: date @gt;= First Day of Next Year
                    return BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, new DateTime(DateTime.Today.Year + 1, 1, 1)));
                case IntervalCriteraType.IntervalLaterThisYear:
                    //     The operator is defined as follows: First Day of Next Month @lt;= date @lt;
                    //     First Day of Next Year
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfNextMonth())),
                        BuildLessCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, new DateTime(DateTime.Today.Year + 1, 1, 1))));
                case IntervalCriteraType.IntervalLaterThisMonth:
                    //     The operator is defined as follows: Last Day of Next Week @lt; date @lt;
                    //     First Day of Next Month
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getLastDayOfNextWeek().AddDays(1).AddMilliseconds(-1))),
                        BuildLessCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfNextMonth())));
                case IntervalCriteraType.IntervalNextWeek:
                    //     The operator is defined as follows: First Day of Next Week @lt;= date @lt;=
                    //     Last Day of Next Week
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfNextWeek())),
                        BuildLessCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getLastDayOfNextWeek().AddDays(1).AddMilliseconds(-1))));
                case IntervalCriteraType.IntervalLaterThisWeek:
                    //     The operator is defined as follows: Day After Tomorrow @lt;= date @lt; First
                    //     Day of Next Week
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, DateTime.Today.AddDays(2))),
                        BuildLessCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfNextWeek())));
                case IntervalCriteraType.IntervalTomorrow:
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, DateTime.Today.AddDays(1))),
                        BuildLessOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, DateTime.Today.AddDays(2).AddMilliseconds(-1))));
                case IntervalCriteraType.IntervalToday:
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, DateTime.Today)),
                        BuildLessOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, DateTime.Today.AddDays(1).AddMilliseconds(-1))));
                case IntervalCriteraType.IntervalYesterday:
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, DateTime.Today.AddDays(-1))),
                        BuildLessOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, DateTime.Today.AddMilliseconds(-1))));
                case IntervalCriteraType.IntervalEarlierThisWeek:
                    //     The operator is defined as follows: First Day of This Week @lt;= date @lt;
                    //     Yesterday
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfThisWeek())),
                        BuildLessCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, DateTime.Today.AddDays(-1))));
                case IntervalCriteraType.IntervalLastWeek:
                    //     The operator is defined as follows: First Day of Last Week @lt;= date @lt;
                    //     First Day of This Week
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfLastThisWeek())),
                        BuildLessCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfThisWeek())));
                case IntervalCriteraType.IntervalEarlierThisMonth:
                    //     The operator is defined as follows: First Day of This Month @lt;= date @lt;
                    //     First Day of Last Week
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfThisMonth())),
                        BuildLessCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfLastThisWeek())));
                case IntervalCriteraType.IntervalEarlierThisYear:
                    //     The operator is defined as follows: First Day of This Year @lt;= date @lt;
                    //     First Day of This Month
                    return Expression.AndAlso(
                        BuildGreaterOrEqualCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, new DateTime(DateTime.Today.Year, 1, 1))),
                        BuildLessCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, getFirstDayOfThisMonth())));
                case IntervalCriteraType.IntervalPriorThisYear:
                    //     The operator is defined as follows: date @lt; First Day of This Year
                    return BuildLessCriteria(CriteriaFactory.GreaterOrEqual(criteria.PropertyName, new DateTime(DateTime.Today.Year, 1, 1)));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private DateTime getFirstDayOfNextMonth()
        {
            var start = DateTime.Today;
            do
            {
                start = start.AddDays(1);
            } while (start.Day != 1);
            return start;
        }

        private DateTime getFirstDayOfThisMonth()
        {
            var start = DateTime.Today;
            do
            {
                if (start.Day == 1)
                {
                    break;
                }
                start = start.AddDays(-1);
            } while (true);
            return start;
        }


        private DateTime getFirstDayOfNextWeek()
        {
            var start = DateTime.Today;
            bool weekPassed = false;
            do
            {
                start = start.AddDays(1);
            } while (start.DayOfWeek != DayOfWeek.Monday);
            return start;
        }


        private DateTime getLastDayOfNextWeek()
        {
            var start = DateTime.Today;
            var startDay = start.DayOfWeek;
            bool weekPassed = false;
            do
            {
                start = start.AddDays(1);
                if (start.DayOfWeek == startDay)
                {
                    weekPassed = true;
                }
            } while (!weekPassed || start.DayOfWeek != DayOfWeek.Sunday);
            return start;
        }

        private DateTime getFirstDayOfThisWeek()
        {
            var start = DateTime.Today;
            do
            {
                if (start.DayOfWeek == DayOfWeek.Monday)
                {
                    break;
                }

                start = start.AddDays(-1);
            } while (true);
            return start;
        }

        private DateTime getFirstDayOfLastThisWeek()
        {
            var start = DateTime.Today;
            bool isLastWeek = false;
            do
            {
                if (start.DayOfWeek == DayOfWeek.Monday)
                {
                    if (isLastWeek)
                    {
                        break;
                    }
                    isLastWeek = true;
                }

                start = start.AddDays(-1);
            } while (true);
            return start;
        }



        Expression BuildAnyCriteria(AnyCriteria criteria)
        {
            var collectionExpression = GetMemberExpression(_parameter, criteria.PropertyName);
            var collectionType = collectionExpression.Member.ReflectedType.GetProperty(collectionExpression.Member.Name).PropertyType;

            var itemType = collectionType.GetGenericArguments().Single();
            var itemParameter = Expression.Parameter(itemType, "c");
            FilterCriteriaExpressionBuilder criteriaExpressionBuilder = new FilterCriteriaExpressionBuilder(itemType, itemParameter);
            Expression itemExpressions = criteriaExpressionBuilder.BuildExpression(criteria.ItemCriteria);

            var lambdaExpression = Expression.Lambda(itemExpressions, itemParameter);

            var overload = typeof(Enumerable).GetMethods().Where(info => info.Name == "Any").Single(mi => mi.GetParameters().Count() == 2);
            var genericOverload = overload.MakeGenericMethod(itemType);
            var call = Expression.Call(genericOverload, collectionExpression, lambdaExpression);

            return call;
        }

        private static MethodCallExpression getDatePartsSTRFromMemberExpressions(MemberExpression memberExpression)
        {
            var dateSeparatorChar = Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator;
            var shortTimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

            //var datePartMethod = typeof(SqlFunctions).GetMethod("DatePart", new[] { typeof(string), typeof(DateTime?) });
            //var castExpression = Expression.Convert(memberExpression, typeof(DateTime?));
            //var dayPart = Expression.Call(null, datePartMethod, Expression.Constant("day"), castExpression);
            //var monthPart = Expression.Call(null, datePartMethod, Expression.Constant("month"), castExpression);
            //var yearPart = Expression.Call(null, datePartMethod, Expression.Constant("year"), castExpression);
            //var dateSeparator = Expression.Constant(dateSeparatorChar);

            ////join all parts to a d.M.YYYY format
            ////culture specific Date Separators and DAY, MONTH, YEAR order
            //List<Expression> orderedDatePartExpressions = new List<Expression>();

            //if (shortTimeFormat.IndexOf("d", StringComparison.OrdinalIgnoreCase) < shortTimeFormat.IndexOf("M", StringComparison.OrdinalIgnoreCase) && shortTimeFormat.IndexOf("d", StringComparison.OrdinalIgnoreCase) < shortTimeFormat.IndexOf("Y", StringComparison.OrdinalIgnoreCase))
            //{
            //    orderedDatePartExpressions.Add(convertIntToString(dayPart));
            //    if (shortTimeFormat.IndexOf("M", StringComparison.OrdinalIgnoreCase) < shortTimeFormat.IndexOf("Y", StringComparison.OrdinalIgnoreCase))
            //    {
            //        //d.M.YYYY
            //        orderedDatePartExpressions.Add(convertIntToString(monthPart));
            //        orderedDatePartExpressions.Add(convertIntToString(yearPart));
            //    }
            //    else
            //    {
            //        //d.YYYY.M
            //        orderedDatePartExpressions.Add(convertIntToString(yearPart));
            //        orderedDatePartExpressions.Add(convertIntToString(monthPart));
            //    }
            //}
            //else if (shortTimeFormat.IndexOf("M", StringComparison.OrdinalIgnoreCase) < shortTimeFormat.IndexOf("d", StringComparison.OrdinalIgnoreCase) && shortTimeFormat.IndexOf("M", StringComparison.OrdinalIgnoreCase) < shortTimeFormat.IndexOf("Y", StringComparison.OrdinalIgnoreCase))
            //{
            //    orderedDatePartExpressions.Add(convertIntToString(monthPart));
            //    if (shortTimeFormat.IndexOf("d", StringComparison.OrdinalIgnoreCase) < shortTimeFormat.IndexOf("Y", StringComparison.OrdinalIgnoreCase))
            //    {
            //        //M.d.YYYY
            //        orderedDatePartExpressions.Add(convertIntToString(dayPart));
            //        orderedDatePartExpressions.Add(convertIntToString(yearPart));
            //    }
            //    else
            //    {
            //        //M.YYYY.d
            //        orderedDatePartExpressions.Add(convertIntToString(yearPart));
            //        orderedDatePartExpressions.Add(convertIntToString(dayPart));
            //    }
            //}
            //else if (shortTimeFormat.IndexOf("Y", StringComparison.OrdinalIgnoreCase) < shortTimeFormat.IndexOf("d", StringComparison.OrdinalIgnoreCase) && shortTimeFormat.IndexOf("Y", StringComparison.OrdinalIgnoreCase) < shortTimeFormat.IndexOf("M", StringComparison.OrdinalIgnoreCase))
            //{
            //    orderedDatePartExpressions.Add(convertIntToString(yearPart));
            //    if (shortTimeFormat.IndexOf("d", StringComparison.OrdinalIgnoreCase) < shortTimeFormat.IndexOf("M", StringComparison.OrdinalIgnoreCase))
            //    {
            //        //YYYY.d.M
            //        orderedDatePartExpressions.Add(convertIntToString(dayPart));
            //        orderedDatePartExpressions.Add(convertIntToString(monthPart));
            //    }
            //    else
            //    {
            //        //YYYY.M.d
            //        orderedDatePartExpressions.Add(convertIntToString(monthPart));
            //        orderedDatePartExpressions.Add(convertIntToString(dayPart));
            //    }
            //}

            //List<Expression> allParts = new List<Expression>()
            //{
            //    orderedDatePartExpressions[0],
            //    dateSeparator,
            //    orderedDatePartExpressions[1],
            //    dateSeparator,
            //    orderedDatePartExpressions[2]
            //};

            //// Create an expression tree that represents creating and  
            //// initializing a one-dimensional array of type string.
            //NewArrayExpression newArrayExpression = Expression.NewArrayInit(typeof(string), allParts);
            //var strDate = Expression.Call(null, typeof(string).GetMethod("Concat", new[] { typeof(string[]) }), newArrayExpression);
            //return strDate;
            return null;
        }

        private string convertToSQLFormatDateStr(string dateStr)
        {
            //expected output is d.M.YYYY
            //need to eliminate possible leading 0

            if (string.IsNullOrEmpty(dateStr))
            {
                return dateStr;
            }

            var dateSeparatorChar = Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator;

            if (dateStr.IndexOf("0") == 0)
            {
                dateStr = dateStr.Substring(1);
            }

            var indexOf0 = dateStr.IndexOf("0");
            var startIndex = 1;
            while (indexOf0 != -1 && dateStr.IndexOf(dateSeparatorChar, startIndex) == indexOf0 - 1)
            {
                dateStr = dateStr.Remove(indexOf0, 1);
                startIndex = indexOf0;
                indexOf0 = dateStr.IndexOf("0");
            }

            return dateStr;
        }
        #endregion
    }
}
