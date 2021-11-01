using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            var httpHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            Console.WriteLine($"=== Calling GRPC Service {configuration["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(configuration["GrpcPlatform"], new GrpcChannelOptions { HttpHandler = httpHandler });
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                Console.WriteLine($"=== Calling GRPC Service GetAllPlatforms"); 
                var reply = client.GetAllPlatforms(request);
                return mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"=== Error calling GRPC Service {ex.Message}");
            }

            return null;
        }
    }
}