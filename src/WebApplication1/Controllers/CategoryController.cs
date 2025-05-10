using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryModel>>> GetAllCategories()
    {
        var result = await _categoryService.GetCategoriesAsync();
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryModel>> GetCategoryById(Guid id)
    {
        var result = await _categoryService.GetCategoryAsync(id);
        if (result.IsFailure) return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<CategoryModel>> CreateCategory(CategoryRequestModel categoryRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _categoryService.CreateCategoryAsync(categoryRequest.Name);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<CategoryModel>> UpdateCategory(Guid id, CategoryRequestModel categoryRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _categoryService.UpdateCategoryNameAsync(id, categoryRequest.Name);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok();
    }
}