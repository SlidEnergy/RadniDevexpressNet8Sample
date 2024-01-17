using Common.DataAccess.Filtering;
using System.Collections.Generic;

namespace AndromedaWin.CommonWindows.Filtering
{
    public static class FilterCriteriaExtensions
    {
        public static IEnumerable<string> GetUsedPropertyNames(this FilterCriteria filterCriteria)
        {
            var extractor = new PropertyCriteriesExtractor();

            var propertyCriteries = extractor.GetPropertyCriteries(filterCriteria);

            return propertyCriteries.Select(x => x.PropertyName);
        }

        public static IEnumerable<IPropertyCriteria> GetPropertyCriteries(this FilterCriteria filterCriteria)
        {
            var extractor = new PropertyCriteriesExtractor();

            return extractor.GetPropertyCriteries(filterCriteria);
        }
    }
}
