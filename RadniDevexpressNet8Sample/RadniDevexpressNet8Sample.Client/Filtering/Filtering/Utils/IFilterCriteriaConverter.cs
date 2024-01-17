using Common.DataAccess.Filtering;
using DevExpress.Data.Filtering;

namespace Common.Windows.Utils
{
    public interface IFilterCriteriaConverter
    {
        CriteriaOperator Convert(FilterCriteria criteria);
    }
}