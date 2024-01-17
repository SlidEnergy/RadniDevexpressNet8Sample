using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using CommonBlazor.Infrastructure;
//using CommonBlazor.Infrastructure.Dispatcher;
//using CommonBlazor.UI.Components;
//using CommonBlazor.UI.Shared;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace CommonBlazor.UI
{
    public abstract class ApplicationComponentBase : ComponentBase, IDisposable
    {
        List<Action> _unregisters = new List<Action>();

        protected IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ServiceResolver.Resolve<IMessenger>() ?? throw ThrowHelper.InjectIsNull();

                return _messenger;
            }
        }

        protected IMessenger? _messenger;

        //protected DialogService? _dialogService;

        //protected DialogService DialogService
        //{
        //    get
        //    {
        //        if (_dialogService == null)
        //            _dialogService = ServiceResolver.Resolve<DialogService>() ?? throw ThrowHelper.InjectIsNull();

        //        return _dialogService;
        //    }
        //}

        //protected IToastService? _toastService;

        //protected IToastService ToastService
        //{
        //    get
        //    {
        //        if (_toastService == null)
        //            _toastService = ServiceResolver.Resolve<IToastService>() ?? throw ThrowHelper.InjectIsNull();

        //        return _toastService;
        //    }
        //}

        protected JsUtils JsUtils
        {
            get
            {
                if (_jsUtils == null)
                    _jsUtils = ServiceResolver.Resolve<JsUtils>() ?? throw ThrowHelper.InjectIsNull();

                return _jsUtils;
            }
        }

        protected JsUtils? _jsUtils;

        //private NavigationService? _navigationService;

        //public NavigationService NavigationService
        //{
        //    get
        //    {
        //        if (_navigationService == null)
        //            _navigationService =
        //                ServiceResolver.Resolve<NavigationService>() ?? throw ThrowHelper.InjectIsNull();

        //        return _navigationService;
        //    }
        //}

        //private IDispatcher? _dispatcher;

        //public IDispatcher Dispatcher
        //{
        //    get
        //    {
        //        if (_dispatcher == null)
        //            _dispatcher = ServiceResolver.Resolve<IDispatcher>() ?? throw ThrowHelper.InjectIsNull();
        //        return _dispatcher;
        //    }
        //}

        protected ILogger<ApplicationComponentBase> Logger
        {
            get
            {
                if (_logger == null)
                    _logger = ServiceResolver.Resolve<ILogger<ApplicationComponentBase>>() ?? throw ThrowHelper.InjectIsNull();

                return _logger;
            }
        }

        protected ILogger<ApplicationComponentBase>? _logger;

        private bool _shouldNotRender = false;

        [Parameter]
        public bool ShouldNotRender
        {
            get => _shouldNotRender;
            set
            {
                var oldValue = _shouldNotRender;

                if (oldValue == value)
                    return;

                _shouldNotRender = value;

                if (oldValue == false && value == true)
                    _renderCount = 0;

                if (oldValue == true && value == false && (_renderCount > 0 || ManualRenderStrategy))
                    InvokeStateHasChanged();
            }
        }

        private int _renderCount = 0;

        public virtual bool ManualRenderStrategy => false;

        private bool _manualRenderStrategyShouldRender = false;

        public virtual bool UseParametersChangeChecking => false;

        private bool _parametersInitialized = false;

        protected override bool ShouldRender()
        {
            _renderCount++;

            if (ShouldNotRender)
                return false;

            if (ManualRenderStrategy)
            {
                //if (_manualRenderStrategyShouldRender)
                //{
                //    Logger.LogInformation("MANUAL RENDERING");
                //    Logger.LogInformation(this.GetType().FullName);
                //}

                return _manualRenderStrategyShouldRender;
            }

            //Logger.LogInformation("RENDERING");
            //Logger.LogInformation(this.GetType().FullName);

            return true;
        }

        protected async Task InvokeStateHasChanged()
        {
            //Logger.LogInformation("CALL MANUAL RENDERING");
            //Logger.LogInformation(this.GetType().FullName);

            _manualRenderStrategyShouldRender = true;
            await InvokeAsync(StateHasChanged);
            _manualRenderStrategyShouldRender = false;
        }

        protected void RegisterMessage<T>(Action<T> handler) where T : class
        {
            Messenger.Register(this, handler);

            _unregisters.Add(() => Messenger.UnregisterAll(this));
        }

        protected void SendMessage<T>(T message) where T : class
        {
            Messenger.Send(message);
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (!UseParametersChangeChecking)
                return base.SetParametersAsync(parameters);

            if (!_parametersInitialized)
            {
                _parametersInitialized = true;

                return base.SetParametersAsync(parameters);
            }

            var dict = parameters.ToDictionary().ToDictionary(x => x.Key, x => x.Value);

            var props = GetProperties(this.GetType());

            foreach (var propertyInfo in props)
            {
                var parameterAttribute = propertyInfo.GetCustomAttribute<ParameterAttribute>();
                var cascadingParameterAttribute = propertyInfo.GetCustomAttribute<CascadingParameterAttribute>();
                var isParameter = parameterAttribute != null || cascadingParameterAttribute != null;
                if (!isParameter)
                {
                    continue;
                }

                foreach (var parameter in parameters)
                {
                    if (parameter.Name != propertyInfo.Name)
                        continue;

                    var currentValue = propertyInfo.GetValue(this);

                    if (parameter.Value == null && currentValue == null)
                        continue;

                    if (currentValue == null || parameter.Value == null)
                    {
                        return base.SetParametersAsync(parameters);
                    }

                    if (!currentValue.Equals(parameter.Value))
                        return base.SetParametersAsync(parameters);

                    break;
                }
            }

            return Task.CompletedTask;
        }

        private IEnumerable<PropertyInfo> GetProperties(Type targetType)
        {
            var dictionary = new Dictionary<string, PropertyInfo>(StringComparer.Ordinal);
            var currentType = targetType;

            while (currentType != null)
            {
                var properties = currentType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);

                foreach (var property in properties)
                {
                    if (!dictionary.ContainsKey(property.Name))
                    {
                        dictionary.Add(property.Name, property);
                    }
                }

                currentType = currentType.BaseType;
            }

            return dictionary.Values.ToList();
        }

        public virtual void Dispose()
        {
            if (_messenger != null)
            {
                foreach (var unregister in _unregisters)
                {
                    unregister.Invoke();
                }
            }
        }
    }
}
