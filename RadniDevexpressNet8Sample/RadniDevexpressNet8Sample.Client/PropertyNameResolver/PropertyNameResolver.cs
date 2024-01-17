
using Common.DataAccess.Filtering;

namespace CommonBlazor.DynamicData
{
    public class PropertyNameResolver
    {
        public Func<string, string> Resolver { get; set; }

        public IEnumerable<string> ResolvePropertyNames(IEnumerable<string> propertyNames)
        {
            if (propertyNames is null)
                return null;

            List<string> result = new List<string>();

            foreach (var propertyName in propertyNames)
            {
                result.Add(ResolvePropertyName(propertyName));
            }

            return result;
        }

        public string ResolvePropertyName(string propertyName)
        {
            return Resolver?.Invoke(propertyName);
        }

        public FilterCriteria ResolvePropertyNames(FilterCriteria criteria)
        {
            if (criteria is null)
                return null;

            HandleFilterCriteria(criteria);

            return criteria;
        }

        private void HandleFilterCriteria(FilterCriteria criteria)
        {
            if (criteria is LogicalCriteria)
            {
                HandleLogicalCriteria((LogicalCriteria)criteria);
                return;
            }
            if (criteria is NotCriteria)
            {
                HandleNotCriteria((NotCriteria)criteria);
                return;
            }
            if (criteria is IPropertyCriteria)
            {
                HandleIPropertyCriteria((IPropertyCriteria)criteria);
                return;
            }

            throw new ArgumentException("Invalid filter criteria: An operand used is not supported!");
        }

        private void HandleIPropertyCriteria(IPropertyCriteria criteria)
        {
            criteria.PropertyName = ResolvePropertyName(criteria.PropertyName);
        }

        private void HandleNotCriteria(NotCriteria criteria)
        {
            HandleFilterCriteria(criteria.Criteria);
        }

        private void HandleLogicalCriteria(LogicalCriteria criteria)
        {
            HandleFilterCriteria(criteria.LeftOperand);
            HandleFilterCriteria(criteria.RightOperand);
        }
    }
}
