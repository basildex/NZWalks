using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext _dbContext;
        public RegionsController(NZWalksDbContext dbContext) 
        {
            this._dbContext = dbContext;
        }

        // GET All Regions
        // https://localhost:<port>/api/regions
        [HttpGet]
        public IActionResult GetAll() 
        {
            // Get data from database - Domain models
            List<Region> value = [.. _dbContext.Regions];

            // Map Domain models to DTOs
            var regions = new List<RegionDto>();
            foreach (Region region in value)
            {
                regions.Add(new RegionDto
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl
                });
            }

            // Return the DTOs as response
            return Ok(regions);
        }

        // GET Region by Id
        // https://localhost:<port>/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetRegionById([FromRoute] Guid id)
        {
            // This .Find method only works for primary keys
            // Region region = _dbContext.Regions.Find(id);

            // This .FirstOrDefault LINQ query works for any property
            // Get the Domain Model from db
            Region regionDomainModel = _dbContext.Regions.FirstOrDefault(r => r.Id == id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Map/Convert to DTO and return to client
            var region = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            return Ok(region);
        }

        // POST create a new region
        // https://localhost:<port>/api/regions
        [HttpPost]
        public IActionResult CreateRegion([FromBody] CreateRegionDto region)
        {
            var newRegionDomainModel = new Region
            {
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            };

            // Use the DbContext to save the new region
            _dbContext.Regions.Add(newRegionDomainModel);
            _dbContext.SaveChanges();

            // Map newRegionDomainModel back to DTO
            var newRegionDto = new RegionDto
            {
                Id = newRegionDomainModel.Id,
                Code = newRegionDomainModel.Code,
                Name = newRegionDomainModel.Name,
                RegionImageUrl = newRegionDomainModel.RegionImageUrl
            };
            return CreatedAtAction(nameof(CreateRegion), new { id = newRegionDto.Id }, newRegionDto);
        }
    }
}
