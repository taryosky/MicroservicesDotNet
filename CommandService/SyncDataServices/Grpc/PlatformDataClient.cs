using Microsoft.Extensions.Configuration;
using AutoMapper;
using System.Collections.Generic;
using CommandService.Models;
using System;
using Grpc.Net.Client;
using PlatformService;
namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public PlatformDataClient(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"---> Getting all platforms using Grpc. Url -> {_config["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();
            try{
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }catch(Exception ex)
            {
                Console.WriteLine("---> An error occured geting all platforms. "+ex.Message);
                return new List<Platform>();
            }
        }
    }
}