using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorControlClientNet
{
    public static class TorStatusCodes
    {
        public static string Tor_Ok = "250";
        public static string Tor_Unnecessary = "251";
        public static string Tor_Exhausted = "451";
        public static string Tor_Error = "500";
        public static string Tor_Unrecognized = "510";
        public static string Tor_Unimplemented = "511";
        public static string Tor_SyntaxError = "512";
        public static string Tor_UnrecognizedArgument = "513";
        public static string Tor_AuthenticationRequired = "514";
        public static string Tor_BadAuthentication = "515";
        public static string Tor_UnspecifiedError = "550";
        public static string Tor_UnrecognizedEntity = "552";
        public static string Tor_InvalidConfig = "553";
        public static string Tor_InvalidDescriptor = "554";
        public static string Tor_UnmanagdEntity = "555";
        public static string Tor_AsynchronousEvent = "650";
    }
}
