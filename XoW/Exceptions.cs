using System;

namespace XoW
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
    }
}
