using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public static class Constants
    {
        public const string Issuer = "https://localhost:44357/";//자기 자신의 ip
        public const string Audiance = "https://localhost:44357/";//자기 자신의 ip
        public const string Secret = "not_too_short_secret";
    }
}
