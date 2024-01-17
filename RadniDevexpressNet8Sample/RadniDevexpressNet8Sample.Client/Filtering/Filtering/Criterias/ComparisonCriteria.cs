using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Criteria class for "simple" comparisons.
    /// </summary>
    [Serializable]
    public class ComparisonCriteria : FilterCriteria, IPropertyCriteria
    {        
        const String ToFormatString = "[{0} {1} {2}]";

        /// <summary>
        /// Initializes instance of <see cref="AndCriteria"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="value">The value against which the property will be compared.</param>
        /// <param name="binaryOperator">Comparison operator.</param>
        public ComparisonCriteria(String propertyName, Object value, ComparisonOperator binaryOperator)
        {
            if(String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");            

            PropertyName = propertyName;
            Value = value;
            Operator = binaryOperator;
        }

        /// <summary>
        /// Get the name of the property to compare.
        /// </summary>
        public String PropertyName { get; set; }

        /// <summary>
        /// Gets the value against which the property will be compared.
        /// </summary>
        public Object Value { get; private set; }

        /// <summary>
        /// Gets the comparison operator.
        /// </summary>
        public ComparisonOperator Operator { get; private set; }

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
        public override String ToString()
        {
            return ToFormatString.FormatCurrentCulture(PropertyName, GetOperatorDisplay(), FilterValueHelper.ToString(Value));
        }

        String GetOperatorDisplay()
        {
            String result;
            switch (Operator)
            {
                case ComparisonOperator.Equal:
                    result = "==";
                    break;
                case ComparisonOperator.NotEqual:
                    result = "<>";
                    break;
                case ComparisonOperator.Greater:
                    result = ">";
                    break;
                case ComparisonOperator.Less:
                    result = "<";
                    break;
                case ComparisonOperator.LessOrEqual:
                    result = "<=";
                    break;
                case ComparisonOperator.GreaterOrEqual:
                    result = ">=";
                    break;
                case ComparisonOperator.Like:
                    result = "Like";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }
    /// <summary>
    /// Enumerates possible operators used with <see cref="ComparisonCriteria"/>.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// Marks == operator.
        /// </summary>
        Equal,
        /// <summary>
        /// Marks &lt;&gt; operator.
        /// </summary>
        NotEqual,
        /// <summary>
        /// Marks &gt; operator.
        /// </summary>
        Greater,
        /// <summary>
        /// Marks &lt; operator.
        /// </summary>
        Less,
        /// <summary>
        /// Marks &lt;= operator.
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// Marks &gt;= operator.
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        /// Marks % operator.
        /// </summary>
        Like
    }
}