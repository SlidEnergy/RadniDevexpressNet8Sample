using Common.DataAccess.Filtering;

namespace AndromedaWin.CommonWindows.Filtering
{
    public class PropertyCriteriesExtractor
    {
        public IEnumerable<IPropertyCriteria> GetPropertyCriteries(FilterCriteria filterCriteria)
        {
            return GetPropertyCriteriesInternal(filterCriteria);
        }

        private IEnumerable<IPropertyCriteria> GetPropertyCriteriesInternal(FilterCriteria criteria)
        {
            if (criteria is LogicalCriteria)
                return GetPropertyNames((LogicalCriteria)criteria);
            else if (criteria is IPropertyCriteria)
                return GetPropertyNames((IPropertyCriteria)criteria);
            else if (criteria is NotCriteria)
                return GetPropertyNames((NotCriteria)criteria);
            else
                throw new NotSupportedException("Unsupported criteria type ({0})".FormatInvariantCulture(criteria.GetType().Name));
        }

        IEnumerable<IPropertyCriteria> GetPropertyNames(NotCriteria notCriteria)
        {
            return GetPropertyCriteriesInternal(notCriteria.Criteria);
        }

        IEnumerable<IPropertyCriteria> GetPropertyNames(IPropertyCriteria criteria)
        {
            return new IPropertyCriteria[] { criteria };
        }

        IEnumerable<IPropertyCriteria> GetPropertyNames(LogicalCriteria criteria)
        {
            var leftPropertyNames = GetPropertyCriteriesInternal(criteria.LeftOperand);
            var rightPropertyNames = GetPropertyCriteriesInternal(criteria.RightOperand);

            return leftPropertyNames.Union(rightPropertyNames);
        }
    }
}
