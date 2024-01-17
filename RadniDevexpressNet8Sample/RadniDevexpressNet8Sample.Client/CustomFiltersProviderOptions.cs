using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonBlazor.DynamicData.Filtering
{
    public class CustomFiltersProviderOptions
    {
        public string? BaseAddress { get; set; }
        public string? GetAll { get; set; }
        public string? GetById { get; set; }
        public string? Create { get; set; }
        public string? Save { get; set; }
        public string? Delete { get; set; }
        public string? ClientId { get; set; }
    }
}
