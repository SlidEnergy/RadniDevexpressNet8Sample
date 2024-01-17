using CommonBlazor.DynamicData;
using CommonBlazor.UI.List;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonBlazor.UI.Filtering.HeaderFilter
{
    public class HeaderFilterDynamicEntityGridController : DynamicEntityGridController
    {
        private readonly string _fieldName;

        public HeaderFilterDynamicEntityGridController(string entity, string fieldName, bool createScope = true) : base(entity, createScope)
        {
            _fieldName = fieldName;
        }

        protected override GridCustomDataSourceBase CreateDataSource()
        {
            return new LoadOnDemandHeaderFilterDynamicEntityDataSource<ExpandoObject>(_fieldName, (DynamicEntityContext)EntityContext, this);
        }
    }
}
