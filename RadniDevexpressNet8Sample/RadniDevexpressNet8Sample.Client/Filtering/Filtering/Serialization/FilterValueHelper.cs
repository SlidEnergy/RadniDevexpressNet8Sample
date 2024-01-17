using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.DataAccess.Filtering
{
    public class FilterValueHelper
    {
        public static string ToString(object value)
        {
            if (value == null)
                return "?";

            if (value is string)
                return $"'{value}'";
            else if (value is int)
                return value.ToString();
            else if (value is float)
                return value.ToString();
            else if (value is bool)
                return value.ToString();
            else if (value is DateTime)
                return $"#{value}#";
            else
                return value.ToString();
        }

        public static object ParseValue(string stringValue)
        {
            if (stringValue.StartsWith("'"))
                return stringValue.Trim('\'');
            if (stringValue == "True")
                return true;
            if (stringValue == "False")
                return false;
            if (stringValue.StartsWith("#"))
                return DateTimeOffset.Parse(stringValue.Trim('#')).DateTime;
            else if (IsNumeric(stringValue))
                return ParseNumberValue(stringValue);
            else if (stringValue == "?")
                return null;
            else
                throw new FilterCriteriaParseException($"Can't parse a value token. Unknown type of value. {stringValue}");
        }

        private static bool IsNumeric(string stringValue) => double.TryParse(stringValue, out double _);

        private static object ParseNumberValue(string stringValue)
        {
            if (stringValue.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                return float.Parse(stringValue);
            else
                return int.Parse(stringValue);
        }
    }
}
