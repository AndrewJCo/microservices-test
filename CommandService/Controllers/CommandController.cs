using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platform/{platformId}/[controller]")]
    [ApiController]

    public class CommandController : ControllerBase
    {
        private readonly ICommandRepository repository;
        private readonly IMapper mapper;

        public CommandController(ICommandRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine("=== Getting commands for platform");

            if (!repository.PlatformExists(platformId)) return NotFound();

            var items = repository.GetCommandsForPlatform(platformId);

            return Ok(mapper.Map<IEnumerable<CommandReadDto>>(items));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine("=== Getting GetCommandForPlatform");

            if (!repository.PlatformExists(platformId)) return NotFound();

            var item = repository.GetCommand(platformId, commandId);
            if (item == null) return NotFound();

            return Ok(mapper.Map<CommandReadDto>(item));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine("=== Hit CreateCommandForPlatform");

            if (!repository.PlatformExists(platformId)) return NotFound();

            var command = mapper.Map<Command>(commandCreateDto);

            repository.CreateCommand(platformId, command);
            repository.SaveChanges();

            var commandReadDto = mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}