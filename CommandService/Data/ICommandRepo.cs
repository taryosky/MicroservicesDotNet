using System.Collections.Generic;
using CommandService.Models;
namespace CommandService.Data
{
    public interface ICommandRepo
    {
        bool SaveChanges();
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        bool PlatformExist(int platformId);
        bool PlatformExternaIdExist(int externalId);

        IEnumerable<Command> GetCommandsForPlatform(int platformid);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}