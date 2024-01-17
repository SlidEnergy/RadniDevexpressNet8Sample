using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Common.DataAccess.Filtering.Utils
{
    /// <summary>
    /// Converts Devextreme filter expressions to FilterCritera
    /// </summary>
    public static class FilterCriteriaDevextremeConverterNew
    {
        public static FilterCriteria Convert(string filterExpression, string prefixColumnName = "")
        {
            try
            {
                return ConvertInternal((JArray)JsonConvert.DeserializeObject(filterExpression), prefixColumnName);
            }
            catch (Exception e)
            {
                throw new Exception("Cannot convert: " + filterExpression, e);
            }
        }

        private static FilterCriteria ConvertInternal(JArray filterExpression, string prefixColumnName)
        {
            var node = ConvertJArrayToExpressionNode(filterExpression, prefixColumnName);
            node.ToFilterCriteria();
            return (FilterCriteria)node.Value;
        }

        private static FilterCriteria ConvertBinary(string getter, string comparisonOperator, string value)
        {
            //getter = getter.FirstCharToUpper();
            switch (comparisonOperator)
            {
                case "=":
                    if (value == null)
                        return CriteriaFactory.Null(getter);
                    else
                        return CriteriaFactory.Equal(getter, value);
                case "<>":
                    if(value == null)
                        return CriteriaFactory.NotNull(getter);
                    else
                        return CriteriaFactory.NotEqual(getter, value);
                case ">":
                    return CriteriaFactory.Greater(getter, value);
                case ">=":
                    return CriteriaFactory.GreaterOrEqual(getter, value);
                case "<":
                    return CriteriaFactory.Less(getter, value);
                case "<=":
                    return CriteriaFactory.LessOrEqual(getter, value);
                case "startswith":
                    return CriteriaFactory.StartsWith(getter, value);
                case "endswith":
                    return CriteriaFactory.EndsWith(getter, value);
                case "contains":
                    return CriteriaFactory.Contains(getter, value);
                case "notcontains":
                    return CriteriaFactory.Not(CriteriaFactory.Contains(getter, value));
                default:
                    throw new Exception("Cannot convert!");
            }
        }

        private static FilterCriteria ConvertLogical(FilterCriteria left, string logicalOperator, FilterCriteria right)
        {
            switch (logicalOperator)
            {
                case "and":
                    return CriteriaFactory.And(left, right);
                case "or":
                    return CriteriaFactory.Or(left, right);
                default:
                    throw new Exception("Cannot convert!");
            }
        }

        //private static ExpressionNode ConvertJArrayToExpressionNode(JArray filterExpression)
        //{
        //    var node = new ExpressionNode();
        //    node.NodeType = NodeType.List;
        //    var list = new List<ExpressionNode>();
        //    for(int i = 0; i < filterExpression.Count; i++)
        //    {
        //        if (filterExpression[i].Type == JTokenType.Array)
        //            list.Add(ConvertJArrayToExpressionNode(filterExpression[i].Value<JArray>()));
        //        else
        //            list.Add(new ExpressionNode() { Value = filterExpression[i].Value<String>(), NodeType = NodeType.String });
        //    }
        //    node.Value = list;
        //    return node;
        //}
        private static ExpressionNode ConvertJArrayToExpressionNode(JArray filterExpression, string prefixColumnName)
        {
            var node = new ExpressionNode();
            node.NodeType = NodeType.List;
            var list = new List<ExpressionNode>();
            for (int i = 0; i < filterExpression.Count; i++)
            {
                if (filterExpression[i].Type == JTokenType.Array)
                    list.Add(ConvertJArrayToExpressionNode(filterExpression[i].Value<JArray>(), prefixColumnName));
                else
                {
                    var val = filterExpression[i].Value<String>();
                    if (i == 0)
                    {
                        //val = StringUtil.InsertDotBeforeCapital(val, prefixColumnName);
                        //val = Regex.Replace(val, @"(?<!_)([A-Z])", ".$1");

                        //TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                        //if (!String.IsNullOrEmpty(prefixColumnName))
                        //{
                        //    val = prefixColumnName + ".";
                        //}
                        //val += textInfo.ToTitleCase(val);
                    }
                    list.Add(new ExpressionNode() { Value = val, NodeType = NodeType.String });
                }
            }
            node.Value = list;

            return node;
        }

        private static void ConvertSimple(ExpressionNode node)
        {
            var array = node.Value as IList<ExpressionNode>;

            if (array[0].NodeType == NodeType.String && (string)array[0].Value == "!")
            {
                array[1].ToFilterCriteria();
                node.Value = CriteriaFactory.Not((FilterCriteria)array[1].Value);
                node.NodeType = NodeType.FilterCritera;
                return;
            }
            if (array[0].NodeType == NodeType.String)
            {
                if (array.Count == 3)
                    node.Value = ConvertBinary((string)array[0].Value, (string)array[1].Value, (string)array[2].Value);
                else
                    node.Value = ConvertBinary((string)array[0].Value, "=", (string)array[1].Value);
                node.NodeType = NodeType.FilterCritera;
            }
            if (array[0].NodeType == NodeType.List || array[0].NodeType == NodeType.FilterCritera)
            {
                if (array.Count == 3)
                {
                    array[0].ToFilterCriteria();
                    array[2].ToFilterCriteria();
                    node.Value = ConvertLogical((FilterCriteria)array[0].Value, (string)array[1].Value, (FilterCriteria)array[2].Value);
                }
                else
                {
                    array[0].ToFilterCriteria();
                    array[1].ToFilterCriteria();
                    node.Value = ConvertLogical((FilterCriteria)array[0].Value, "and", (FilterCriteria)array[1].Value);
                }
                node.NodeType = NodeType.FilterCritera;
            }
        }

        private class ExpressionNode
        {
            public object Value { get; set; }
            public NodeType NodeType { get; set; }

            public void ToFilterCriteria()
            {
                if (NodeType == NodeType.String || NodeType == NodeType.FilterCritera)
                    return;

                var array = Value as IList<ExpressionNode>;
                if (array == null)
                    throw new Exception("Cannot convert!");

                if (array.Count <= 3)
                {
                    ConvertSimple(this);
                    return;
                }

                for (int i = 0; i < array.Count; i++)
                {
                    if ((array[i].NodeType == NodeType.String && (string)array[i].Value == "and") ||
                        (i > 0 && array[i].NodeType != NodeType.String && array[i - 1].NodeType != NodeType.String))
                    {
                        ExpressionNode left, right;
                        string logicalOperator;
                        if (i > 0 && array[i].NodeType != NodeType.String && array[i - 1].NodeType != NodeType.String)
                        {
                            left = array[i - 1];
                            right = array[i];
                            logicalOperator = "and";
                        }
                        else
                        {
                            left = array[i - 1];
                            right = array[i + 1];
                            logicalOperator = (string)array[i].Value;
                        }

                        left.ToFilterCriteria();
                        right.ToFilterCriteria();

                        var criteria = ConvertLogical((FilterCriteria)left.Value, logicalOperator, (FilterCriteria)right.Value);
                        if (i > 0 && array[i].NodeType != NodeType.String && array[i - 1].NodeType != NodeType.String)
                        {
                            array.RemoveAt(i - 1);
                            array.RemoveAt(i - 1);
                        }
                        else
                        {
                            array.RemoveAt(i - 1);
                            array.RemoveAt(i - 1);
                            array.RemoveAt(i - 1);
                        }

                        array.Insert(i - 1, new ExpressionNode() { Value = criteria, NodeType = NodeType.FilterCritera });
                    }
                }

                while (array.Count > 1)
                {
                    ExpressionNode left, right;
                    string logicalOperator;
                    if (array[1].NodeType != NodeType.String)
                    {
                        left = array[0];
                        right = array[1];
                        logicalOperator = "and";
                    }
                    else
                    {
                        left = array[0];
                        right = array[2];
                        logicalOperator = (string)array[1].Value;
                    }

                    left.ToFilterCriteria();
                    right.ToFilterCriteria();

                    var criteria = ConvertLogical((FilterCriteria)left.Value, logicalOperator, (FilterCriteria)right.Value);
                    if (array[1].NodeType != NodeType.String)
                    {
                        array.RemoveAt(0);
                        array.RemoveAt(0);
                    }
                    else
                    {
                        array.RemoveAt(0);
                        array.RemoveAt(0);
                        array.RemoveAt(0);
                    }

                    array.Insert(0, new ExpressionNode() { Value = criteria, NodeType = NodeType.FilterCritera });
                }

                if (array.Count > 1 || array[0].NodeType != NodeType.FilterCritera)
                    throw new Exception("Cannot convert!");

                Value = array[0].Value;
                NodeType = NodeType.FilterCritera;
            }
        }

        private enum NodeType
        {
            String,
            List,
            FilterCritera
        }
    }
}
