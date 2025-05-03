using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class FoodController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FoodModel>>> GetAllFoods()
    {
        var result = await _foodService.GetFoodsAsync();
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FoodModel>> GetFoodById(Guid id)
    {
        var result = await _foodService.GetFoodAsync(id);
        if (result.IsFailure) return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<FoodModel>> CreateFood(RequestFoodModel request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _foodService.CreateFoodAsync(request);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<FoodModel>> UpdateFood(Guid id, UpdateRequestFoodModel request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _foodService.UpdateFoodAsync(id, request);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteFood(Guid id)
    {
        var result = await _foodService.DeleteFoodAsync(id);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok();
    }
}