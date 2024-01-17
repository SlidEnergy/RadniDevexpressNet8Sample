using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure.Exceptions
{
    public class ErrorResponseModel
    {
        public int InternalCode { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string MessageDetail { get; set; }
    }
}
