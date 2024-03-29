﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Configuration
{
    public class AuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime CurrentTime { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}
