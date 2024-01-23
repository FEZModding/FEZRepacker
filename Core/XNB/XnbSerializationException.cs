namespace FEZRepacker.Core.XNB
{
    [Serializable]
    public class XnbSerializationException : Exception
    {
        internal string? _message;
        public override string Message
        {
            get => _message ?? base.Message;
        }

        public XnbSerializationException(string message) : base(message)
        {
            _message = message;
        }

        public XnbSerializationException(string message, Exception innerException) : base(message, innerException)
        {
            _message = message;
        }
    }
}
