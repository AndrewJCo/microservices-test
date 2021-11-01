using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandService.Models;

namespace CommandService.DTOs
{
    public class PlatformReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // public ICollection<Command> Commands { get; set; } = new List<Command>();
    }
}