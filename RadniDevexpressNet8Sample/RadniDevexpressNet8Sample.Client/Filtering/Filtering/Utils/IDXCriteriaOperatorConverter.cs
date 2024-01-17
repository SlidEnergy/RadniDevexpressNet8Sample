using Common.DataAccess.Filtering;
using DevExpress.Data.Filtering;
using System;

namespace Common.Windows.Utils
{
    public interface IDXCriteriaOperatorConverter
    {
        FilterCriteria Convert(CriteriaOperator criteria);
    }
}