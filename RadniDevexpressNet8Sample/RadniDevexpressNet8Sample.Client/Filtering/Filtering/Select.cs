using System;
using System.Linq.Expressions;

namespace Common.DataAccess
{
    /// <summary>
    /// Defines the select field.
    /// </summary>
    public class Select
    {

        /// <summary>
        /// Initializes instance of <see cref="Select"/>.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="ascending">Sort direction. Default is ascending.</param>
        public Select(String fieldName, string destinationFieldName)
        {
            FieldName = fieldName;
            DestinationFieldName = destinationFieldName;
        }

        public Select(Expression fieldExpression, string destinationFieldName)
        {
            _fieldExpression = fieldExpression;
            DestinationFieldName = destinationFieldName;
        }


        /// <summary>
        /// Gets the field's name.
        /// </summary>
        public String FieldName { get; private set; }

        /// <summary>
        /// Gets the field's name.
        /// </summary>
        public String DestinationFieldName { get; private set; }

        private readonly Expression _fieldExpression;
        public Expression FieldExpression
        {
            get { return _fieldExpression; }
        }


        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override String ToString()
        {
            return "{0}".FormatInvariantCulture(FieldName);
        }
    }
}