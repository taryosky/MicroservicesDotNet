using Grpc.Core;
using System.Threading.Tasks;
using AutoMapper;
using PlatformService.Data;
using System;
namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _repo;
        public GrpcPlatformService(IPlatformRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            Console.WriteLine("---> Returning all platforms using GRPC");
            var platforms = _repo.GetAllPlatforms();
            var response = new PlatformResponse();
            foreach(var platform in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
            }

            Console.WriteLine("---> Returned all Platforms");
            return Task.FromResult(response);
        }
    }
}