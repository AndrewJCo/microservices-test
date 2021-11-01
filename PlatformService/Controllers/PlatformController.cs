using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepository platformRepository;
        private readonly IMapper mapper;
        private readonly ICommandDataClient commandDataClient;
        private readonly IMessageBusClient messageBusClient;

        public PlatformController(IPlatformRepository platformRepository,
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            this.commandDataClient = commandDataClient;
            this.messageBusClient = messageBusClient;
            this.mapper = mapper;
            this.platformRepository = platformRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("=== Getting platforms");

            var platformItems = platformRepository.GetAllPlatforms();
            return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine("=== Getting platforms");

            var platformItem = platformRepository.GetPlatformById(id);

            if (platformItem != null) return Ok(mapper.Map<PlatformReadDto>(platformItem));

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform)
        {
            var platformModel = mapper.Map<Platform>(platform);

            platformRepository.CreatePlatform(platformModel);
            platformRepository.SaveChanges();

            var platformRead = mapper.Map<PlatformReadDto>(platformModel);

            // Send Sync Message
            try
            {
                 await commandDataClient.SendPlatformToCommand(platformRead);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"=== Could not send sync {ex.Message}");
            }

            // Send Async Message
            try
            {
                var platformPublishDto = mapper.Map<PlatformPublishDto>(platformRead);
                platformPublishDto.Event = "Platform_Published";

                messageBusClient.PublishNewPlatform(platformPublishDto);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"=== Could not send async publish platform {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformRead.Id }, platformRead);
        }

    }
}