using CommonBlazor.DynamicData.Models;

namespace CommonBlazor.UI.List.FixedList
{
    public class EntityGridController<T> : EntityGridControllerBase<T> where T : class
    {
        public EntityGridController(Type queryRequest, Type getCountRequest, bool createScope = true) : base(new EntityContext(queryRequest, getCountRequest), createScope)
        {

        }

        public EntityGridController(EntityContext entityContext, bool createScope = true) : base(entityContext, createScope)
        {
        }

        public override async Task InitializeAsync(bool initDefaultFilter, IEnumerable<GenericColumnSettings>? columns = null, CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return;
            
            InitializeEntityAsync(columns);

            await base.InitializeAsync(initDefaultFilter, columns, cancellationToken);
        }

        private void InitializeEntityAsync(IEnumerable<GenericColumnSettings>? columns = null)
        {
            if (columns == null || columns.Count() == 0)
                throw new ArgumentException("Columns must be set for fixed entity grid controller", nameof(columns));

            EntityContext.Properties = columns.ToList();
            EntityContext.IsInitialized = true;
        }

        protected override GridCustomDataSourceBase CreateDataSource()
        {
            return new LoadOnDemandEntityDataSource<T>((EntityContext)EntityContext);
        }
    }
}
