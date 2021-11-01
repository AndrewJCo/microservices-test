using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext dbContext;

        public PlatformRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null) throw new ArgumentNullException(nameof(platform));

            dbContext.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return dbContext.Platforms.ToList();
        }

        public Platform GetPlatformById(int id)
        {
            return dbContext.Platforms.FirstOrDefault(x => x.Id == id);
        }

        public bool SaveChanges()
        {
            return (dbContext.SaveChanges() >= 0);
        }
    }
}
