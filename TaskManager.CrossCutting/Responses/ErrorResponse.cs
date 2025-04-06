using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.CrossCutting.Responses
{
    public class ErrorResponse
    {
        public string Message { get; set; } = "";
        public string? Detail { get; set; }
        public int StatusCode { get; set; } 
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; 
    }
}
