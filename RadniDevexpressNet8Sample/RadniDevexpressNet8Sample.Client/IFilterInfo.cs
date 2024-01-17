using Common.DataAccess.Filtering;
using DevExpress.Data.Filtering;

namespace CommonBlazor.DynamicData.Abstractions
{
    public interface IFilterInfo
    {
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the filter was modified.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the filter is new.
        /// For use only in <see cref="FilterInfoEditor"/> control. Don't use it otherwise. 
        /// </summary>
        public bool IsNew { get; }

        /// <summary>
        /// Gets or sets filter info name.
        /// </summary>
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool Prefilter { get; set; }

        public bool DefaultFilter { get; set; }

        /// <summary>
        /// Gets or sets filter info expression.
        /// </summary>
        public FilterCriteria? Criteria { get; set; }

        public Dictionary<string, FilterCriteria?> CriteriaCollection { get; set; }

        public CriteriaOperator CriteriaOperator { get; set; }

        public string? DevextremeCriteria { get; set; }

        public bool PredefinedFilter { get; set; }

        public CustomFilterDto Model { get; set; }
    }
}