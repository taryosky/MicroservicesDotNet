using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;
        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatforms(int platformId)
        {
            Console.WriteLine("---> Getting platform with Id "+platformId);
            if(!_repo.PlatformExist(platformId)){
                return NotFound();
            }
            var commands = _repo.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"-> Getting command with Id {commandId} for platform with Id {platformId}");

            if(!_repo.PlatformExist(platformId))
            {
                return NotFound();
            }

            var command = _repo.GetCommand(platformId, commandId);
            if(command == null)
                return NotFound();

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"-> Creating Command for platform with Id {platformId}");

            if(!_repo.PlatformExist(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandCreateDto);
            _repo.CreateCommand(platformId, command);
            _repo.SaveChanges();

            var commandReadDtoReturn = _mapper.Map<CommandReadDto>(command);
            return CreatedAtRoute(nameof(GetCommandForPlatform), new {platformId = platformId, commandId = command}, commandReadDtoReturn);
        }
    }
}