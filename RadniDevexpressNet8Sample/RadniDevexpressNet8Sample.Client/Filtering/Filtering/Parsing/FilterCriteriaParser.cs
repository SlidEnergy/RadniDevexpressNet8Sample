using Common.DataAccess.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.DataAccess.Filtering
{
    public class FilterCriteriaParser : IFilterCriteriaParser
    {
        private Regex _notRegex = new Regex(@"^not\s\(.*\)");
        private Regex _currentUserRegex = new Regex(@"^CurrentUser.*");

        public FilterCriteria Parse(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return null;

            var position = 0;
            return ParseInternal(filter, ref position);
        }

        private FilterCriteria ParseInternal(string filter, ref int position)
        {
            for (; position < filter.Length; position++)
            {
                if (filter[position] == '(')
                {
                    position++;
                    return ParseGroupCriteria(filter, ref position);
                }

                if (filter[position] == '[')
                {
                    position++;
                    return ParsePropertyCriteria(filter, ref position);
                }

                if (_notRegex.IsMatch(filter.Substring(position)))
                {
                    return ParseNotCriteria(filter, ref position);
                }

                if (_currentUserRegex.IsMatch(filter.Substring(position)))
                {
                    return ParseCurrentUserCriteria(filter, ref position);
                }
            }

            throw new FilterCriteriaParseException($"Can't parse filter string, because supported token hasn't been found. Filter: {filter}, Position: {position}");
        }

        private FilterCriteria ParseGroupCriteria(string filter, ref int position)
        {
            var leftOperand = ParseInternal(filter, ref position);
            position++; // plus open parenthesis
            var operands = filter.Substring(position).Split(new[] { ' ' }, 2);
            position += operands[0].Length + 1; // plus one space
            var rightOperand = ParseInternal(filter, ref position);
            position++; // plus close parenthesis

            switch (operands[0])
            {
                case "AND":
                    return CriteriaFactory.And(leftOperand, rightOperand);
                case "OR":
                    return CriteriaFactory.Or(leftOperand, rightOperand);

                default:
                    throw new FilterCriteriaParseException("Operator not supported: " + operands[0]);
            }

            throw new FilterCriteriaParseException("Filter string don't have close parenthesis");
        }

        private FilterCriteria ParseNotCriteria(string filter, ref int position)
        {
            position += 5; // "not" plus space and open parenthesis
            var includedCriteria = ParseInternal(filter, ref position);
            position++; // plus close parenthesis
            return CriteriaFactory.Not(includedCriteria);
        }

        private FilterCriteria ParseCurrentUserCriteria(string filter, ref int position)
        {
            position += 12; // "CurrentUser" plus space

            return CriteriaFactory.CurrentUserCriteria();
        }

        private FilterCriteria ParsePropertyCriteria(string filter, ref int position)
        {
            var startPosition = position;

            for (; position < filter.Length; position++)
            {
                if (filter[position] == ']')
                {
                    position++;
                    return ParsePropertyCriteria(filter.Substring(startPosition, position - startPosition - 1));
                }
            }

            throw new FilterCriteriaParseException("Filter string don't have close square bracket");
        }

        private FilterCriteria ParsePropertyCriteria(string propertyToken)
        {
            propertyToken = propertyToken.Trim(' ');

            if (string.IsNullOrEmpty(propertyToken))
                throw new FilterCriteriaParseException("Can't parse a property token, the token is empty");

            var anyRegex = new Regex(@"([a-zA-Z0-9_]+)\.Any\((.*\))");

            if (anyRegex.IsMatch(propertyToken))
            {
                throw new FilterCriteriaParseException("The function criteria \'Any\' not supported: " + propertyToken);
            }

            var tokens = SplitPropertyToken(propertyToken);

            CheckAndThrowMinimumCountTokens(tokens, 2);

            switch (tokens[1])
            {
                case "StartsWith":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.StartsWith(tokens[0], ParseValue(tokens[2]));

                case "EndsWith":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.EndsWith(tokens[0], ParseValue(tokens[2]));

                case "Contains":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.Contains(tokens[0], ParseValue(tokens[2]));

                case "IsNullOrEmpty":
                    return CriteriaFactory.IsNullOrEmpty(tokens[0]);

                case "between":
                    CheckAndThrowCountTokens(tokens, 5);
                    return CriteriaFactory.Between(tokens[0], ParseValue(tokens[2]), ParseValue(tokens[4]));

                case "==":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.Equal(tokens[0], ParseValue(tokens[2]));

                case "<>":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.NotEqual(tokens[0], ParseValue(tokens[2]));

                case ">":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.Greater(tokens[0], ParseValue(tokens[2]));

                case "<":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.Less(tokens[0], ParseValue(tokens[2]));

                case "<=":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.LessOrEqual(tokens[0], ParseValue(tokens[2]));

                case ">=":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.GreaterOrEqual(tokens[0], ParseValue(tokens[2]));

                case "Like":
                    CheckAndThrowCountTokens(tokens, 3);
                    return CriteriaFactory.Like(tokens[0], ParseValue(tokens[2]));

                case "in":
                    CheckAndThrowMinimumCountTokens(tokens, 3);
                    return CriteriaFactory.In(tokens[0], ParseInValuesWithParenthesis(tokens[2]));

                case "is":
                    CheckAndThrowMinimumCountTokens(tokens, 3);

                    if (tokens[2] == "NULL")
                        return CriteriaFactory.Null(tokens[0]);
                    else
                        return CriteriaFactory.NotNull(tokens[0]);

                case "IntervalBeyondThisYear":
                    return CriteriaFactory.IntervalBeyondThisYear(tokens[0]);
                case "IntervalLaterThisYear":
                    return CriteriaFactory.IntervalLaterThisYear(tokens[0]);
                case "IntervalLaterThisMonth":
                    return CriteriaFactory.IntervalLaterThisMonth(tokens[0]);
                case "IntervalNextWeek":
                    return CriteriaFactory.IntervalNextWeek(tokens[0]);
                case "IntervalLaterThisWeek":
                    return CriteriaFactory.IntervalLaterThisWeek(tokens[0]);
                case "IntervalTomorrow":
                    return CriteriaFactory.IntervalTomorrow(tokens[0]);
                case "IntervalToday":
                    return CriteriaFactory.IntervalToday(tokens[0]);
                case "IntervalYesterday":
                    return CriteriaFactory.IntervalYesterday(tokens[0]);
                case "IntervalEarlierThisWeek":
                    return CriteriaFactory.IntervalEarlierThisWeek(tokens[0]);
                case "IntervalLastWeek":
                    return CriteriaFactory.IntervalLastWeek(tokens[0]);
                case "IntervalEarlierThisMonth":
                    return CriteriaFactory.IntervalEarlierThisMonth(tokens[0]);
                case "IntervalEarlierThisYear":
                    return CriteriaFactory.IntervalEarlierThisYear(tokens[0]);
                case "IntervalPriorThisYear":
                    return CriteriaFactory.IntervalPriorThisYear(tokens[0]);

                default:
                    throw new FilterCriteriaParseException("Function criteria not supported: " + propertyToken);
            }
        }

        private string[] SplitPropertyToken(string token)
        {
            token.Trim();

            var tokens = new List<string>();
            var isValueToken = false;
            var isParenthesis = false;
            var lastStartPosition = 0;

            for (int i = 0; i < token.Length; i++)
            {
                // Parse values for In criteria
                if (token[i] == '(' || token[i] == ')')
                {
                    isParenthesis = !isParenthesis;
                    continue;
                }

                if (isParenthesis)
                    continue;

                // Parse value for string or date time value. These types can contains space.
                if (token[i] == '\'' || token[i] == '#' )
                {
                    isValueToken = !isValueToken;
                    continue;
                }

                if (token[i] == ' ' && !isValueToken)
                {
                    tokens.Add(token.Substring(lastStartPosition, i - lastStartPosition));
                    lastStartPosition = i + 1;
                }
            }

            tokens.Add(token.Substring(lastStartPosition));

            return tokens.ToArray();
        }

        private void CheckAndThrowMinimumCountTokens(string[] tokens, int count)
        {
            if (tokens.Length < count)
                throw new FilterCriteriaParseException($"Can't parse a property token. The token must have minimum {count} parts separated by space. [{string.Join(" ", tokens)}]");
        }

        private void CheckAndThrowCountTokens(string[] tokens, int count)
        {
            if (tokens.Length != count)
                throw new FilterCriteriaParseException($"Can't parse a property token. The token must have {count} parts separated by space. [{string.Join(" ", tokens)}]");
        }

        private object[] ParseInValuesWithParenthesis(string stringInValues)
        {
            return stringInValues
                .TrimStart('(')
                .TrimEnd(')')
                .Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => ParseValue(x))
                .ToArray();
        }

        private object ParseValue(string stringValue) => FilterValueHelper.ParseValue(stringValue);
    }
}
