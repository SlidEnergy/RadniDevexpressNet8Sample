using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Criteria class for complex comparisons involving custom functions.
    /// </summary>
    [Serializable]
    public class FunctionCriteria : FilterCriteria, IPropertyCriteria
    {
        /// <summary>
        /// Initializes instance of <see cref="FunctionCriteria"/>.
        /// </summary>
        /// <param name="functionType">Function type.</param>
        /// <param name="propertyName">The name of the property to compare.</param>        
        /// <param name="value">The value which will be provided to the function against which the property will be compared.</param>
        public FunctionCriteria(
            FunctionCriteriaType functionType,
            String propertyName,
            Object value)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            FunctionType = functionType;
            PropertyName = propertyName;
            Value = value;
        }

        /// <summary>
        /// Gets the function type.
        /// </summary>
        public FunctionCriteriaType FunctionType { get; private set; }

        /// <summary>
        /// Get the name of the property to compare.
        /// </summary>
        public String PropertyName { get; set; }

        /// <summary>
        /// Gets the value(s) which will be provided to the function against which the property will be compared.
        /// </summary>
        public Object Value { get; set; }

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
            if (FunctionType == FunctionCriteriaType.IsNullOrEmpty)
                return "[{0} {1}]".FormatCurrentCulture(PropertyName, GetOperatorDisplay());
            return "[{0} {1} {2}]".FormatCurrentCulture(PropertyName, GetOperatorDisplay(), FilterValueHelper.ToString(Value));
        }

        String GetOperatorDisplay()
        {
            String result;
            switch (FunctionType)
            {
                case FunctionCriteriaType.StartsWith:
                    result = "StartsWith";
                    break;
                case FunctionCriteriaType.EndsWith:
                    result = "EndsWith";
                    break;
                case FunctionCriteriaType.Contains:
                    result = "Contains";
                    break;
                case FunctionCriteriaType.IsNullOrEmpty:
                    result = "IsNullOrEmpty";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }
}
