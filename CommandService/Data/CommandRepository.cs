using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandService.Models;

namespace CommandService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private AppDbContext context { get; }
        public CommandRepository(AppDbContext context)
        {
            this.context = context;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            command.PlatformId = platformId;

            context.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null) throw new ArgumentNullException(nameof(platform));

            context.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return context.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return context.Commands.FirstOrDefault(x => x.PlatformId == platformId && x.Id == commandId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return context.Commands.Where(x => x.PlatformId == platformId);
        }

        public bool PlatformExists(int platformId)
        {
            return context.Platforms.Any(x => x.Id == platformId);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        bool ICommandRepository.ExternalPlatformExists(int externalId)
        {
            return context.Platforms.Any(x => x.ExternalId == externalId);
        }
    }
}