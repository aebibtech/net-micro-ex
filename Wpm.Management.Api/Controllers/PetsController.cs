using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController(ManagementDbContext dbContext, ILogger<PetsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var all = await dbContext.Pets
                .Include(p => p.Breed)
                .ToListAsync();
            return Ok(all);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error has occurred while getting pets.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById (int id)
    {
        try
        {
            var pet = await dbContext.Pets
                .Include(p => p.Breed)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (pet == null)
                return NotFound();

            return Ok(pet);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error has occurred while getting a pet.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewPet newPet)
    {
        var pet = newPet.ToPet();
        await dbContext.Pets.AddAsync(pet);
        await dbContext.SaveChangesAsync();
        return CreatedAtRoute(nameof(GetById), new { id = pet.Id }, newPet);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PetUpdate petUpdate)
    {
        try
        {
            var pet = await dbContext.Pets.FindAsync(id);
            if (pet == null)
                return NotFound();

            pet.Name = petUpdate.Name;
            pet.Age = petUpdate.Age;
            pet.BreedId = petUpdate.BreedId;

            await dbContext.SaveChangesAsync();

            return Ok(pet);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error has occured while updating a pet.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}

public record NewPet([Required]string Name, int Age, int BreedId)
{
    public Pet ToPet()
    {
        return new Pet() { Name = Name, Age = Age, BreedId = BreedId };
    }
}

public record PetUpdate(
    string Name,
    int Age,
    int BreedId
);
