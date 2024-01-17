using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Base class for logical filter criterias.
    /// </summary>
    [Serializable]
    public abstract class LogicalCriteria : FilterCriteria
    {
        /// <summary>
        /// Initializes instance of <see cref="LogicalCriteria"/>
        /// </summary>
        /// <param name="leftOperand">Left side operand.</param>
        /// <param name="rightOperand">Right side operand.</param>
        protected LogicalCriteria(FilterCriteria leftOperand, FilterCriteria rightOperand)
        {
            if (null == leftOperand)
                throw new ArgumentNullException("leftOperand");
            if (null == rightOperand)
                throw new ArgumentNullException("rightOperand");

            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        /// <summary>
        /// Gets the left side operand of the logical criteria.
        /// </summary>
        public FilterCriteria LeftOperand { get; private set; }

        /// <summary>
        /// Gets the right side operand of the logical criteria.
        /// </summary>
        public FilterCriteria RightOperand { get; private set; }

        /// <summary>
        /// Accepts the specified <paramref name="visitor"/>.
        /// </summary>
        /// <param name="visitor">Instance of <see cref="IFilterCriteriaVisitor"/>.</param>
        public override void Accept(IFilterCriteriaVisitor visitor)
        {
            visitor.Visit(this);
            LeftOperand.Accept(visitor);
            RightOperand.Accept(visitor);
        }
    }
}
