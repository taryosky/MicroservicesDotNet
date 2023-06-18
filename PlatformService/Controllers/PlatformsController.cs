using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController: ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandClient;

        public PlatformsController(IPlatformRepo repo, IMapper mapper, ICommandDataClient commandClient)
        {
            _repo = repo;
            _mapper = mapper;
            _commandClient = commandClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            //Get all platforms
            Console.WriteLine("--> getting all platforms");
            var platforms = _repo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = nameof(GetPlatformById))]
        public ActionResult<PlatformReadDto> GetPlatformById([FromRoute]int id)
        {
            var platform = _repo.GetPlatformById(id);
            if(platform == null)
                return NotFound("Platform not found");

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformToCreate)
        {
            var platform = _mapper.Map<Platform>(platformToCreate);
            _repo.AddPlatform(platform);
            _repo.SaveChanges();

            var platformToReturn = _mapper.Map<PlatformReadDto>(platform);

            try
            {
                await _commandClient.SendPlatformToCommand(platformToReturn);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"-> Could not send synchronouly: {ex.Message}");
            }
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platform.Id}, platformToReturn);
        }
    }
}