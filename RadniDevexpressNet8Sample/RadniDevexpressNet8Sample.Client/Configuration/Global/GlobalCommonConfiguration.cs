using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonBlazor.UI.Configuration
{
    public class GlobalCommonConfiguration
    {
        internal const bool DefaultGridVirtualScrollEnabled = false;
        private bool _gridVirtualScrollEnabled;

        public bool GridVirtualScrollEnabled { get; set; } = DefaultGridVirtualScrollEnabled;
    }
}
