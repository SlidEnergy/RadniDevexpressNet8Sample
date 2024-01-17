namespace Common.DataAccess.Filtering
{
    /// <summary>
    /// Interface for <see cref="FilterCriteria"/> visitors.
    /// </summary>
    public interface IFilterCriteriaVisitor
    {
        /// <summary>
        /// Visits the specified <paramref name="criteria"/>.
        /// </summary>
        /// <param name="criteria"><see cref="FilterCriteria"/> to visit.</param>
        void Visit(FilterCriteria criteria);
    }
}
