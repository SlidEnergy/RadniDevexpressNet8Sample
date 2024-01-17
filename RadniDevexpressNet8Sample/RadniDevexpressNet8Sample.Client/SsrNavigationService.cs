using CommonBlazor.DynamicData;
using CommonBlazor.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace CommonBlazor.UI.Shared
{
    public class SsrNavigationService
    {
        private readonly NavigationManager _navigationManager;

        public SsrNavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }
        public async Task OpenBlankUrlAsync(string url)
        {
            var jsUtils = ServiceResolver.Resolve<JsUtils>();
            await jsUtils.OpenBlankUrl(url);
        }

        public void NavigateTo(string url)
        {
            _navigationManager.NavigateTo(url);
        }
    }
}
