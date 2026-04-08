using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BreedsController(ManagementDbContext dbContext, ILogger<BreedsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var all = await dbContext.Breeds
                .ToListAsync();
            return Ok(all);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error has occurred while getting Breeds.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id}", Name = nameof(GetBreedById))]
    public async Task<IActionResult> GetBreedById (int id)
    {
        try
        {
            var breed = await dbContext.Breeds
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (breed == null)
                return NotFound();

            return Ok(breed);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error has occurred while getting a Breed.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewBreed newBreed)
    {
        var breed = newBreed.ToBreed();
        await dbContext.Breeds.AddAsync(breed);
        await dbContext.SaveChangesAsync();
        return CreatedAtRoute(nameof(GetBreedById), new { id = breed.Id }, newBreed);
    }
}

public record NewBreed([Required]string Name)
{
    public Breed ToBreed()
    {
        return new Breed(0, Name);
    }
}
