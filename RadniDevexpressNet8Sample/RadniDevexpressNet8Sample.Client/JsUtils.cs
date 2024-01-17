using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CommonBlazor.UI
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class JsUtils : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public JsUtils(IJSRuntime jsRuntime)
        {
            moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/RadniDevexpressNet8Sample.Client/utils.js").AsTask());
        }

        public async ValueTask SetTextAsync(string text, string selector)
        {
            var module = await moduleTask.Value;
            await module.InvokeAsync<string>("utils.setText", selector, text);
        }

        public async ValueTask FocusEditor(string selector)
        {
            var module = await moduleTask.Value;
            await module.InvokeAsync<string>("utils.focusEditor", selector);
        }

        public async ValueTask DragSplitter(ElementReference splitter, ElementReference leftPane, ElementReference rightPane, string direction)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.splitter.dragSplitter", splitter, leftPane, rightPane, direction);
        }

        public async ValueTask AllowResize(ElementReference drawer, ElementReference resizer)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.allowResize", drawer, resizer);
        }
        public async ValueTask WriteTextToClipboard(string text)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.clipboard.writeText", text);
        }

        public async ValueTask ShowToast(ElementReference toast, int duration)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.showToast", toast, duration);
        }

        public async ValueTask OpenBlankUrl(string url)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.openBlankUrl", url);
        }

        public async ValueTask<bool> ChildOfById(ElementReference element, string id)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("utils.childOfById", element, id);
        }

        public async ValueTask SubscribeToTransitionEnd<T>(DotNetObjectReference<T> dotNetObjectReference, ElementReference element) where T : class
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.subscribeToTransitionEnd", dotNetObjectReference, element);
        }

        public async ValueTask ScrollToView(string element)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.scrollToView", element);
        }

        public async ValueTask ScrollToTop(string element)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.scrollToTop", element);
        }

        public async ValueTask WatchScroll<T>(string element, string s, DotNetObjectReference<T> dotNetObjectReference, string methodName) where T : class
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.watchScroll", element, s, dotNetObjectReference, methodName);
        }

        public async ValueTask SetCustomDropDownWidth(string id)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.setCustomDropDownWidth", id);
        }

        public async ValueTask ClearCustomDropDownWidth(string id)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.clearCustomDropDownWidth", id);
        }

        public async ValueTask AddClass(string selector, string className)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.addClass", selector, className);
        }

        public async ValueTask RemoveClass(string selector, string className)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("utils.removeClass", selector, className);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}