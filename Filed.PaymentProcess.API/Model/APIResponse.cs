﻿using System.Collections.Generic;

namespace Filed.PaymentProcess.API.Model
{
    public class APIResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}