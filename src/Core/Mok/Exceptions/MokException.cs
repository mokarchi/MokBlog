using System;

namespace Mok.Exceptions
{
    /// <summary>
    /// The exception for the system.
    /// </summary>
    /// <remarks>
    /// Caller should log before throw an exception.  I choose not to pass in ILogger here for max flexibility.
    /// The message giving to this class will be displayed on Error.cshtml.
    /// </remarks>
    public class MokException : Exception
    {
        public EExceptionType ExceptionType { get; }

        public MokException()
        {
        }

        /// <summary>
        /// Thrown with a <see cref="EExceptionType"/> and inner exception.
        /// </summary>
        /// <param name="exceptionType"></param>
        /// <param name="inner"></param>
        public MokException(EExceptionType exceptionType, Exception inner)
            : base("", inner)
        {
            ExceptionType = exceptionType;
        }

        /// <summary>
        /// Thrown with a message.
        /// </summary>
        /// <param name="message"></param>
        public MokException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Thrown with a <see cref="EExceptionType"/>.
        /// </summary>
        /// <param name="exceptionType"></param>
        public MokException(EExceptionType exceptionType, string message = "")
            : base(message)
        {
            ExceptionType = exceptionType;
        }
    }
}
