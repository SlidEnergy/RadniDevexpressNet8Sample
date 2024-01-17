using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure.Exceptions
{
    internal class HttpErrorExceptionModel
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
        public HttpErrorExceptionDetailModel Errors { get; set; }
    }

    internal class HttpErrorExceptionDetailModel
    {
        public string[] Id { get; set; }
    }
}
