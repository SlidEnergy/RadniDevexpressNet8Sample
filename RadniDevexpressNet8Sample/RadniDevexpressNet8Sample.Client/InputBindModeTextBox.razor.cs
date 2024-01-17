using Microsoft.AspNetCore.Components;
using DevExpress.Blazor;
using CommonBlazor.Infrastructure;

namespace CommonBlazor.UI.Components
{
    public partial class InputBindModeTextBox : ComponentBase
    {
        private Timer? _timer;

        private string? _text;

        [Parameter]
        public string? Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        [Parameter]
        public string? NullText { get; set; }

        [Parameter]
        public EventCallback<string> TextChanged { get; set; }

        [Parameter]
        public bool Enabled { get; set; } = true;

        private DxTextBox TextBox { get => _textBox ?? throw ThrowHelper.ComponentReferenceIsNull(); set => _textBox = value; }

        private DxTextBox? _textBox;

        private void OnTextChanged(string value)
        {
            _text = value;

            if (_timer != null)
                _timer.Dispose();

            _timer = new Timer(OnTimerElapsed, null, 500, 0);
        }

        private void OnTimerElapsed(object? state)
        {
            TextChanged.InvokeAsync(Text);

            if (_timer != null)
                _timer.Dispose();
        }

        public ValueTask FocusAsync()
        {
            var result = TextBox.FocusAsync();

            InvokeAsync(StateHasChanged);

            return result;
        }
    }
}
