using System.ComponentModel;
using Common.DataAccess.Filtering;
using CommonBlazor.DynamicData.Abstractions;
using DevExpress.Data.Filtering;

namespace CommonBlazor.DynamicData.Filtering
{
    /// <summary>
    /// Class that encapsulate info about a single filter.
    /// </summary>
    [Serializable]
    public class FilterInfo : IFilterInfo, INotifyPropertyChanged
    {
        public int Id { get => Model.Id; set => Model.Id = value; }

        [NonSerialized]
        FilterCriteria _criteria;

        public FilterInfo(CustomFilterDto model)
        {
            Model = model;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the filter was modified.
        /// </summary>
        public bool IsDirty { get => Model.IsDirty; private set => Model.IsDirty = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the filter is new.
        /// For use only in <see cref="FilterInfoEditor"/> control. Don't use it otherwise. 
        /// </summary>
        public bool IsNew { get => Model.IsNew; private set => Model.IsNew = value; }

        /// <summary>
        /// Gets or sets filter info name.
        /// </summary>
        public string Name
        {
            get => Model.Name;
            set
            {
                if (Model.Name == value)
                    return;

                Model.Name = value;

                OnPropertyChanged(nameof(Name));
                IsDirty = true;
            }
        }

        public string DisplayName
        {
            get => Model.DisplayName;
            set
            {
                if (Model.DisplayName == value)
                    return;

                Model.DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
                IsDirty = true;
            }
        }

        public bool Prefilter
        {
            get => Model.Prefilter;
            set
            {
                if (Model.Prefilter == value)
                    return;

                Model.Prefilter = value;
                OnPropertyChanged(nameof(Prefilter));
                IsDirty = true;
            }
        }

        public bool DefaultFilter
        {
            get => Model.DefaultFilter;
            set
            {
                if (Model.DefaultFilter == value)
                    return;

                Model.DefaultFilter = value;
                OnPropertyChanged(nameof(DefaultFilter));
                IsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets filter info expression.
        /// </summary>
        public FilterCriteria? Criteria
        {
            get
            {
                return _criteria;
            }
            set
            {
                if (_criteria == value)
                    return;
                _criteria = value;
                OnPropertyChanged(nameof(Criteria));
                IsDirty = true;
            }
        }

        private Dictionary<string, FilterCriteria?> _criteriaCollection;

        public Dictionary<string, FilterCriteria?> CriteriaCollection
        {
            get
            {
                return _criteriaCollection;
            }
            set
            {
                if (_criteriaCollection == value)
                    return;

                _criteriaCollection = value;
                OnPropertyChanged(nameof(CriteriaCollection));
                IsDirty = true;
            }
        }

        public Dictionary<string, string?> DevextremeCriteriaCollection { get; set; }

        public CriteriaOperator CriteriaOperator { get; set; }

        public string? DevextremeCriteria { get; set; }

        public bool PredefinedFilter { get; set; }

        public CustomFilterDto Model { get; set; }


        #region INotifyPropertyChanged members

        /// <summary>
        /// Raised when a property gets changed.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged.IsNotNull())
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Override that simply returns the filter name.
        /// </summary>
        /// <returns>The filter name.</returns>
        public override String ToString()
        {
            return String.IsNullOrEmpty(DisplayName) ? "*" : DisplayName;
        }
    }
}