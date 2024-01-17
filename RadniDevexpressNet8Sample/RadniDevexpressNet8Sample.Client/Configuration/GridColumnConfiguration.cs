using CommonBlazor.DynamicData.Models;

namespace CommonBlazor.UI.Configuration
{
    public class GridColumnConfiguration
    {
        private readonly GenericColumnSettings _model;

        public int Order { get => _model.Order; set => _model.Order = value; }

        public string FullPropertyName
        {
            get => _model.FullPropertyName;
            set => _model.FullPropertyName = value;
        }

        public string PropertyName
        {
            get => _model.PropertyName;
            set => _model.PropertyName = value;
        }

        public string DisplayName
        {
            get
            {
                return _model.DisplayName;
            }
            set
            {
                _model.DisplayName = value;
            }
        }

        public string DisplayFormat
        {
            get
            {
                return _model.DisplayFormat;
            }
            set
            {
                _model.DisplayFormat = value;
            }
        }

        public int SortOrder { get => _model.SortOrder; set => _model.SortOrder = value; }

        public bool SortDesceding { get => _model.SortDesceding; set => _model.SortDesceding = value; }

        public decimal? ColumnWiddth { get => _model.ColumnWidth; set => _model.ColumnWidth = value; }

        public GenericColumnSettings Model => _model;

        public GridColumnConfiguration(GenericColumnSettings model)
        {
            _model = model;
        }
    }
}
