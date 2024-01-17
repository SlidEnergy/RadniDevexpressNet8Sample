using System;

namespace Common.DataAccess.Filtering
{
    public class CurrentUserCriteria : FilterCriteria
    {
        /// <summary>
        /// Initializes instance of <see cref="CurrentUserCriteria"/>.
        /// </summary>
        public CurrentUserCriteria()
        {
        }

        #region Overrides of FilterCriteria

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
            return "CurrentUser";
        }

        #endregion
    }
}
