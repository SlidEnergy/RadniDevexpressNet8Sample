using CommonBlazor.DynamicData;
using CommonBlazor.Infrastructure;

namespace CommonBlazor.UI.List
{
    public class EntityContext : EntityContextBase
    {
        private Type? _queryRequest;

        public Type QueryRequest 
        { 
            get => _queryRequest ?? throw ThrowHelper.PropertyIsNull();
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (!value.IsSubclassOf(typeof(EntityQueryRequest)))
                    throw new ArgumentException($"{nameof(QueryRequest)} must be inherited from EntityQueryRequest", nameof(QueryRequest));

                _queryRequest = value;
            }
        }

        public Type? _getCountRequest;

        public Type GetCountRequest
        {
            get => _getCountRequest ?? throw ThrowHelper.PropertyIsNull();
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (!value.IsSubclassOf(typeof(EntityQueryRequest)))
                    throw new ArgumentException($"{nameof(GetCountRequest)} must be inherited from EntityQueryRequest", nameof(GetCountRequest));

                _getCountRequest = value;
            }
        }

        public EntityContext(Type queryRequest, Type getCountRequest) : base()
        {
            QueryRequest = queryRequest;
            GetCountRequest = getCountRequest;
        }
    }
}
