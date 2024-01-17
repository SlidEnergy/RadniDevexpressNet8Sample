using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Criteria that represents logical AND expression. 
    /// </summary>
    [Serializable]
    public class AndCriteria : LogicalCriteria
    {
        const String ToFormatString = "({0} AND {1})";

        /// <summary>
        /// Initializes instance of <see cref="AndCriteria"/>.
        /// </summary>
        /// <param name="leftOperand">Left side operand.</param>
        /// <param name="rightOperand">Right side operand.</param>
        public AndCriteria(FilterCriteria leftOperand, FilterCriteria rightOperand)
            : base(leftOperand, rightOperand)
        { }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override String ToString()
        {
            return ToFormatString.FormatCurrentCulture(LeftOperand, RightOperand);
        }
    }
}
