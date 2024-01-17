using Common.DataAccess.Filtering;

namespace Common.DataAccess.Filtering
{
    public interface IFilterCriteriaParser
    {
        FilterCriteria Parse(string filter);
    }
}