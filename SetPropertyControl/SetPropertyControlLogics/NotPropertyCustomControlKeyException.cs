using System;

namespace SetPropertyControl.SetPropertyControlLogics
{
    internal class NotPropertyCustomControlKeyException : Exception
    {
        public NotPropertyCustomControlKeyException()
        {
        }

        public NotPropertyCustomControlKeyException(string message) : base(message)
        {
        }

        public NotPropertyCustomControlKeyException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}