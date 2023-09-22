using PlatformService.Dtos;
namespace PlatformService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishMessage(PlatformPublishDto platformPublishDto);
    }
}