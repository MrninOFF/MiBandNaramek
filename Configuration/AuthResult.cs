using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Configuration
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool Sucsess { get; set; }
        public List<string> Errors { get; set; }
    }
}
