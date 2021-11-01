using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IMapper mapper;
        private readonly IServiceScopeFactory scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            this.scopeFactory = scopeFactory;
            this.mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEventType(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    break;
            }
        }

        private EventType DetermineEventType(string notificationMessage)
        {
            Console.WriteLine("=== Determing Event Type");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    AddPlatform(notificationMessage);
                    Console.WriteLine("=== Event Type is Platform Published");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("=== Event Type is undetermined");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                try
                {
                    var platform = mapper.Map<Platform>(platformPublishedDto);

                    if (!repository.ExternalPlatformExists(platform.ExternalId))
                    {
                        Console.WriteLine("=== Creating platform");
                        repository.CreatePlatform(platform);
                        repository.SaveChanges();
                    } else {
                        Console.WriteLine("=== Platform already exists");
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"=== Could not add platform to db { ex.Message}");
                }
            }
        }
    }

    public enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}