using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepository repository;
        private readonly IMapper mapper;

        public GrpcPlatformService(IPlatformRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context )
        {
            var response = new PlatformResponse();
            var platforms = repository.GetAllPlatforms();

            foreach(var platform in platforms) {
                response.Platform.Add(mapper.Map<GrpcPlatformModel>(platform));
            }

            return Task.FromResult(response);
        }
    }
}