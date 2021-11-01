using System;
using System.Collections.Generic;
using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly ICommandRepository repository;
        private readonly IMapper mapper;

        public PlatformController(ICommandRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("=== Getting platforms");

            var items = repository.GetAllPlatforms();

            return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(items));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("=== Inbound POST");

            return Ok("Inbound test of platform controller");
        }


    }
}