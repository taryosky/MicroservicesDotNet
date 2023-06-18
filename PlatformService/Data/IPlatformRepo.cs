using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        IEnumerable<Platform> GetAllPlatforms();
        Platform GetPlatformById(int id);
        void AddPlatform(Platform platform);
        bool SaveChanges();
    }
}