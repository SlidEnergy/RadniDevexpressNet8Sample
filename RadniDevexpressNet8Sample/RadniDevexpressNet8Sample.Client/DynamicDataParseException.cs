using System.Runtime.Serialization;

namespace CommonBlazor.DynamicData
{
    [Serializable]
    internal class DynamicDataParseException : Exception
    {
        public DynamicDataParseException()
        {
        }

        public DynamicDataParseException(string message) : base(message)
        {
        }

        public DynamicDataParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DynamicDataParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
