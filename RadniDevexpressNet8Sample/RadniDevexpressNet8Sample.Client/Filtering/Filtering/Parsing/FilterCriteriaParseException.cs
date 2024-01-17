using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataAccess.Filtering
{
    public class FilterCriteriaParseException : Exception
    {
        public FilterCriteriaParseException(string message) : base(message)
        {
        }
    }
}
