using Common.DataAccess.Filtering;

namespace Common.Filtering.Utils
{
    public interface IFilterCriteriaToDevextremeConverter
    {
        string Convert(FilterCriteria criteria);
    }
}