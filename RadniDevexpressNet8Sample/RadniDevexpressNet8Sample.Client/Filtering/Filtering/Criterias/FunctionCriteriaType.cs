using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Enumerates possible types of <see cref="FunctionCriteria"/>.
    /// </summary>
    public enum FunctionCriteriaType
    {
        /// <summary>
        /// Returns true if the begining of one string matches the another string.
        /// </summary>
        StartsWith,
        /// <summary>
        /// Returns true if the end of one string matches the another string.
        /// </summary>
        EndsWith,
        /// <summary>
        /// Returns true if one string occures within another string.
        /// </summary>
        Contains,
        /// <summary>
        /// Returns true if the specified operand is null or an empty string,
        /// otherwise false is returned.
        /// </summary>
        IsNullOrEmpty
    }
}
