using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : Controller
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of trails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var trails = _trailRepo.GetTrails();
            var trailDtos = _mapper.Map<List<TrailDto>>(trails);
            return Ok(trailDtos);
        }

        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="id"> The id of the trail </param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int id)
        {
            var trail = _trailRepo.GetTrail(id);
            if(trail == null)
            {
                return NotFound();
            }
            var trailDto = _mapper.Map<TrailDto>(trail);
            return Ok(trailDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if(trailDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepo.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trail = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.CreateTrail(trail))
            {
                ModelState.AddModelError("", $"Something went wrong when adding the record {trail.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { id = trail.Id }, trail);
        }

        [HttpPatch("{id:int}", Name ="UpdateTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int id, [FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto == null || id != trailDto.Id)
            {
                return BadRequest(ModelState);
            }

            var trail = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.UpdateTrail(trail))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {trail.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{id:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int id)
        {
            if (!_trailRepo.TrailExists(id))
            {
                return NotFound();
            }

            var trail = _trailRepo.GetTrail(id);

            if (!_trailRepo.DeleteTrail(trail))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {trail.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
