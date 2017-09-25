using System.Runtime.CompilerServices;

namespace Experimentation.Domain.Exceptions
{
    public class NonUniqueValueDetectedException : System.Exception
    {
        public NonUniqueValueDetectedException(
            string className, 
            string value, 
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string method = "") 
            : base(BuildErrorMessage(value, className, method, lineNumber))
        {
        }

        private static string BuildErrorMessage(string value, string className, string method, int lineNumber)
        {
            return $"Class: {className} | Method: {method} | Line: {lineNumber} : " +
                   $"The provided value: {value} is not unique and only unique values are permissable in this context.";
        }
    }
}