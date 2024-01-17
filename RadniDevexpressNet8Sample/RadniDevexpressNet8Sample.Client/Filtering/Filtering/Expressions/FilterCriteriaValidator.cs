using System;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Validates <see cref="FilterCriteria"/> instances against types whose properties
    /// are constrainted by the criteria. This validation makes sense only for criterias 
    /// that constraint a single type (i.e. are used only for filtering instances of one object type).
    /// </summary>
    public static class FilterCriteriaValidator
    {
        /// <summary>
        /// Determines whether the filter criteria can be applied on the specified <paramref name="objectType"/>.
        /// The evaluation takes into consideration properties constrainted by the criteria. If a constrainted
        /// property is not exposed by the specified <paramref name="objectType"/> then the criteria is not
        /// valid for it.
        /// </summary>
        /// <param name="criteria">Filter criteria.</param>
        /// <param name="objectType">Filtered object type.</param>
        /// <returns><c>True</c> if the <paramref name="criteria"/> is valid for the <paramref name="objectType"/>
        /// (i.e. can be applied on it); otherwise <c>false</c>.</returns>
        public static Boolean IsValidOn(this FilterCriteria criteria, Type objectType)
        {
            var validator = new FilterCriteriaValidatorVisitor(objectType);
            criteria.Accept(validator);
            return validator.IsValid;
        }
    }

    /// <summary>
    /// Validates <see cref="FilterCriteria"/> instances against types whose properties
    /// are constrainted by the criteria. This validation makes sense only for criterias 
    /// that constraint a single type (i.e. are used only for filtering instances of one object type).
    /// </summary>
    public class FilterCriteriaValidatorVisitor : IFilterCriteriaVisitor
    {
        /// <summary>
        /// Initializes instance of <see cref="FilterCriteriaValidatorVisitor"/>.
        /// </summary>
        /// <param name="objectType">Filtered object type.</param>
        public FilterCriteriaValidatorVisitor(Type objectType)
        {
            ObjectType = objectType;
            if (objectType == null)
                throw new ArgumentNullException("objectType");
            IsValid = true;
        }

        /// <summary>
        /// Visits the specified <paramref name="criteria"/>.
        /// </summary>
        /// <param name="criteria"><see cref="FilterCriteria"/> to visit.</param>
        public void Visit(FilterCriteria criteria)
        {
            if (!IsValid) // if already not valid
                return;
            var propCriteria = criteria as IPropertyCriteria;
            if (null == propCriteria) // we dont know how to validate
                return;
            IsValid = null != ObjectType.GetProperty(propCriteria.PropertyName);
        }

        /// <summary>
        /// Gets a value indicating whether the filter criteria can be applied on <see cref="ObjectType"/>.
        /// The evaluation takes into consideration properties constrainted by the criteria. If a constrainted
        /// property is not exposed by <see cref="ObjectType"/> then the criteria is not valid for it.
        /// </summary>
        public Boolean IsValid { get; private set; }

        /// <summary>
        /// Type of the filtered object.
        /// </summary>
        public Type ObjectType { get; private set; }

        /// <summary>
        /// Resets the evaluation. You can call <see cref="Visit"/> again after this call.
        /// </summary>
        public void Reset()
        {
            IsValid = true;
        }
    }
}
