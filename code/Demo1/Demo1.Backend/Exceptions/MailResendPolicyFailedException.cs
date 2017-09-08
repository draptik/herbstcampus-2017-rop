using System;
using System.Runtime.Serialization;

namespace Demo1.Backend.Exceptions
{
    public class MailResendPolicyFailedException : Exception
    {
        public MailResendPolicyFailedException()
        {
        }

        public MailResendPolicyFailedException(string message) : base(message)
        {
        }

        public MailResendPolicyFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MailResendPolicyFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}