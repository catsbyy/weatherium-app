using System;
using System.Collections.Generic;
using System.Text;

namespace WeatheriumApp.Helper
{
    public class ApiResponse
    {
        public bool Successful => ErrorMessage == null; 
        public string ErrorMessage { get; set; }
        public string Response { get; set; }
    }
}
