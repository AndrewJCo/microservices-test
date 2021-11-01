using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.DTOs
{
    public class PlatformPublishDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Event { get; set; }
        // public string Publisher { get; set; }
        // public int Cost { get; set; }
    }
}