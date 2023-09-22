using System.Text.Json;
using System;
using CommandService.Dtos;
using CommandService.Data;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using CommandService.Models;
namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _mapper = mapper;
            _scopeFactory = scopeFactory;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEventType(message);
            switch(eventType)
            {
                case EventType.PlatformCreated:
                    //Todo
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private void AddPlatform(string platformPublishedMessage){
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var platform = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);
            try
            {
                var plat = _mapper.Map<Platform>(platform);
                if(!repo.PlatformExternaIdExist(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("---> Could not add platform to Db "+ex.Message);
            }
        }

        private EventType DetermineEventType(string notificationMessage)
        {
            Console.WriteLine("---> Determining Event type");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch(eventType.Event){
                case "Platform_Created":
                    Console.WriteLine("---> Platform Created event detected");
                    return EventType.PlatformCreated;
                default:
                    Console.WriteLine("---> Could not determine event type");
                    return EventType.Undetermined;
            }
        }
    }

    enum EventType
    {
        PlatformCreated,
        Undetermined
    }
}