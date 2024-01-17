using Common.DataAccess.Filtering;
using CommonBlazor.DynamicData.Abstractions;

namespace CommonBlazor.UI.Filtering.QuickFilters
{
    public class QuickFilterInfo
    {
        private readonly QuickFilterDto _model;

        public int Id { get => _model.Id; set => _model.Id = value; }

        public string Key { get => _model.Key; set => _model.Key = value; }

        public string DisplayName { get => _model.DisplayName; set => _model.DisplayName = value; }

        public string FullPropertyName { get => _model.Property; set => _model.Property = value; }

        public string TableName { get => _model.TableName; set => _model.TableName = value; }

        public FunctionCriteriaType Operator { get => _model.Operator; set => _model.Operator = value; }

        public FunctionCriteria Criteria { get; set; }

        public QuickFilterInfo(QuickFilterDto model)
        {
            _model = model;
            Criteria = new FunctionCriteria(model.Operator, model.Property, "");
        }
    }
}
