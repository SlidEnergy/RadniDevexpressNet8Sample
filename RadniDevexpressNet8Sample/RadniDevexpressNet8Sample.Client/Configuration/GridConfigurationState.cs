using CommonBlazor.DynamicData;
using CommonBlazor.Infrastructure;

namespace CommonBlazor.UI.Configuration
{
    public class GridConfigurationState
    {
        private bool _popupVisible;

        public bool PopupVisible
        {
            get => _popupVisible;
            set
            {
                _popupVisible = value;
                PopupVisibleChanged?.Invoke();
            }
        }

        public event Action? PopupVisibleChanged;

        private DynamicEntityContext? _entityContext;
        public DynamicEntityContext EntityContext { get => _entityContext ?? throw ThrowHelper.ParameterIsNull(); set => _entityContext = value; }
    }
}