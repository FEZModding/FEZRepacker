namespace FEZRepacker.Core.Conversion
{
    [Serializable]
    public class FormatConversionException : Exception
    {
        internal string? _message;
        public override string Message
        {
            get => _message ?? base.Message;
        }

        public FormatConversionException(string message) : base(message)
        {
            _message = message;
        }

        public FormatConversionException(string message, Exception innerException) : base(message, innerException)
        {
            _message = message;
        }
    }
}
